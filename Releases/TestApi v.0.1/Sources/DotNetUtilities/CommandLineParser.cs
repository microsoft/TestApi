using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Microsoft.Test
{
    /// <summary>
    /// Provides utilities for parsing command-line values.
    /// </summary>
    /// <example>
    /// The following example shows how to parse to a strongly typed data structure and to an executable command structure.
    /// <code>
    /**
        // SAMPLE USAGE #1:
        // Sample for parsing the following command-line:
        // Test.exe /verbose /runId=10
        // This sample declares a class in which the strongly typed arguments are populated
        public class CommandLineArguments
        {
           bool? Verbose { get; set; }
           int? RunId { get; set; }
        }
    
        CommandLineArguments a = new CommandLineArguments();
        CommandLineParser.ParseArguments(args, a);
    
        // SAMPLE USAGE #2:
        // Sample for parsing the following command-line:
        // Test.exe run /verbose /id=10
        // In this particular case we have an actual command on the command-line (“run”),
        // which we want to effectively de-serialize and execute.
        public class RunCommand : Command
        {
            bool? Verbose { get; set; }
            int? RunId { get; set; }
 
            public override void Execute()
            {
            }
        }

        Command c = CommandLineParser.ParseCommand(args, new Command[] { new RunCommand() });
        c.Execute();
    */
    /// </code>
    /// </example>
    public static class CommandLineParser
    {
        #region Constructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static CommandLineParser()
        {
            // The parser will want to convert from value line string arguments into various
            // data types on a value. Any type that doesn't have a default TypeConverter that
            // can convert from string to it's type needs to have a custom TypeConverter written
            // for it, and have it added here.
            TypeDescriptor.AddAttributes(typeof(FileInfo), new TypeConverterAttribute(typeof(FileInfoConverter)));
        }

        #endregion

        #region Public and Protected Members

        /// <summary>
        /// Iterates through a collection of key/value pairs, where the key is a property name of 
        /// the object to populate and the value is the value to set the property to. 
        /// </summary>
        /// <param name="arguments">Collection of key/value pairs using a /key=value syntax.</param>
        /// <param name="valueToPopulate">The object to set properties for.</param>
        /// <returns>true if parsing is successful; otherwise, false.</returns>
        public static bool ParseArguments(IEnumerable<string> arguments, Object valueToPopulate)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(valueToPopulate);

            CommandLineDictionary commandLineDictionary = new CommandLineDictionary(arguments);

            foreach (string key in commandLineDictionary.Keys)
            {
                PropertyDescriptor property = properties[key];
                if (property == null)
                {
                    return false;
                }

                TypeConverter typeConverter = TypeDescriptor.GetConverter(property.PropertyType);
                if (typeConverter == null)
                {
                    return false;
                }

                object convertedValue = typeConverter.ConvertFromInvariantString(commandLineDictionary[key]);
                properties[key].SetValue(valueToPopulate, convertedValue);
            }

            return true;
        }

        /// <summary>
        /// Parse a command line into a Command. For these purposes a command line is defined as a
        /// command name followed by arguments.
        /// </summary>
        /// <param name="arguments">Command-line arguments, starting with command name.</param>
        /// <param name="commands">Collection of possible commands.</param>
        /// <returns>New Command instance populated with values from the command line.</returns>
        public static Command ParseCommand(IEnumerable<string> arguments, IEnumerable<Command> commands)
        {
            foreach (Command command in commands)
            {
                if (String.Equals(command.Name, arguments.ElementAt(0), StringComparison.OrdinalIgnoreCase))
                {
                    Command commandToReturn = (Command)Activator.CreateInstance(command.GetType());
                    // Strip off the first element (the command name) from the arguments collection
                    // before passing it onto ParseArguments.
                    if (ParseArguments(arguments.Except(new string[] { arguments.ElementAt(0) }), commandToReturn))
                    {
                        return commandToReturn;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Prints names and descriptions for properties on the specified object.
        /// </summary>
        /// <param name="value">Command to print usage for.</param>
        public static void PrintArgumentsUsage(object value)
        {
            // We only want to display properties defined locally on the type. We might consider
            // generalizing this behavior in the future.
            IEnumerable<PropertyDescriptor> properties = TypeDescriptor.GetProperties(value).Cast<PropertyDescriptor>();
            IEnumerable<PropertyDescriptor> propertiesOnParent = TypeDescriptor.GetProperties(value.GetType().BaseType).Cast<PropertyDescriptor>();
            properties = properties.Except(propertiesOnParent);

            IEnumerable<string> propertyNames = properties.Select(property => property.Name);
            IEnumerable<string> propertyDescriptions = properties.Select(property => property.Description);
            IEnumerable<string> lines = FormatNamesAndDescriptions(propertyNames, propertyDescriptions, Console.WindowWidth);

            Console.WriteLine("Possible arguments:");
            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }
        }

        /// <summary>
        /// Prints detailed usage for a specific command if specified in the
        /// arguments, or a general summary of all commands otherwise.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <param name="commands">Collection of possible commands.</param>
        public static void PrintCommandUsage(IEnumerable<string> args, IEnumerable<Command> commands)
        {
            // Prune out any help request so we can get at the potential command name
            List<string> arguments = new List<string>(args);
            arguments.RemoveAll(str => String.Equals(str, "help", StringComparison.OrdinalIgnoreCase) ||
                                       String.Equals(str, "/?", StringComparison.OrdinalIgnoreCase));

            // See if the first argument matches one of the command names, and if so, print
            // usage for that specific command.
            if (arguments[0] != null)
            {
                foreach (Command command in commands)
                {
                    if (String.Equals(arguments[0], command.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("For the '" + command.Name + "' command:");
                        PrintArgumentsUsage(command);
                        return;
                    }
                }
            }

            // Otherwise we print out general descriptions for every command.
            IEnumerable<string> commandNames = commands.Select(command => command.Name);
            IEnumerable<string> commandDescriptions = commands.Select(command => command.GetAttribute<DescriptionAttribute>().Description);
            IEnumerable<string> lines = FormatNamesAndDescriptions(commandNames, commandDescriptions, Console.WindowWidth);

            Console.WriteLine("Possible commands:");
            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }
        }

        /// <summary>
        /// Creates a string that represents key/value arguments for the properties of the 
        /// specified object. For example, an object with a name (string) of "example" and a 
        /// priority value (integer) of 1 translates to '/name=example  /priority=1'. This 
        /// can be used to send data structures through the command line.
        /// </summary>
        /// <param name="valueToConvert">Value to create key/value arguments from.</param>
        /// <returns>Space-delimited key/value arguments.</returns>
        public static string ToString(object valueToConvert)
        {
            IEnumerable<PropertyDescriptor> properties = TypeDescriptor.GetProperties(valueToConvert).Cast<PropertyDescriptor>();
            IEnumerable<PropertyDescriptor> propertiesOnParent = TypeDescriptor.GetProperties(valueToConvert.GetType().BaseType).Cast<PropertyDescriptor>();
            properties = properties.Except(propertiesOnParent);
            CommandLineDictionary commandLineDictionary = new CommandLineDictionary(new List<string>());

            foreach (PropertyDescriptor property in properties)
            {
                commandLineDictionary[property.Name] = property.GetValue(valueToConvert).ToString();
            }

            return commandLineDictionary.ToString();
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Gets an attribute on the specified object instance.
        /// </summary>
        /// <typeparam name="T">Type of attribute to get.</typeparam>
        /// <param name="value">Object instance to look for attribute on.</param>
        /// <returns>First instance of the specified attribute.</returns>
        private static T GetAttribute<T>(this object value) where T : Attribute
        {
            IEnumerable<Attribute> attributes = TypeDescriptor.GetAttributes(value).Cast<Attribute>();
            return (T)attributes.First(attribute => attribute is T);
        }

        /// <summary>
        /// Given collections of names and descriptions, returns a set of lines
        /// where the description text is wrapped and left aligned. eg:
        /// First Name   this is a string that wraps around
        ///              and is left aligned.
        /// Second Name  this is another string.
        /// </summary>
        /// <param name="names">Collection of name strings.</param>
        /// <param name="descriptions">Collection of description strings.</param>
        /// /// <param name="maxLineLength">Maximum length of formatted lines</param>
        /// <returns>Formatted lines of text.</returns>
        private static IEnumerable<string> FormatNamesAndDescriptions(IEnumerable<string> names, IEnumerable<string> descriptions, int maxLineLength)
        {
            if (names.Count() != descriptions.Count())
            {
                throw new ArgumentException("Collection sizes are not equal", "names");
            }

            int namesMaxLength = names.Max(commandName => commandName.Length);

            List<string> lines = new List<string>();

            for (int i = 0; i < names.Count(); i++)
            {
                string line = names.ElementAt(i);
                line = line.PadRight(namesMaxLength + 2);

                foreach (string wrappedLine in WordWrap(descriptions.ElementAt(i), maxLineLength - namesMaxLength - 3))
                {
                    line += wrappedLine;
                    lines.Add(line);
                    line = new string(' ', namesMaxLength + 2);
                }
            }

            return lines;
        }

        /// <summary>
        /// Word wrap text for a specified maximum line length.
        /// </summary>
        /// <param name="text">Text to word wrap.</param>
        /// <param name="maxLineLength">Maximum length of a line.</param>
        /// <returns>Collection of lines for the word wrapped text.</returns>
        private static IEnumerable<string> WordWrap(string text, int maxLineLength)
        {
            List<string> lines = new List<string>();
            string currentLine = String.Empty;

            foreach (string word in text.Split(' '))
            {
                // Whenever adding the word would push us over the maximum
                // width, add the current line to the lines collection and
                // begin a new line. The new line starts with space padding
                // it to be left aligned with the previous line of text from
                // this column.
                if (currentLine.Length + word.Length > maxLineLength)
                {
                    lines.Add(currentLine);
                    currentLine = String.Empty;
                }

                currentLine += word;

                // Add spaces between words except for when we are at exactly the
                // maximum width.
                if (currentLine.Length != maxLineLength)
                {
                    currentLine += " ";
                }
            }

            // Add the remainder of the current line except for when it is
            // empty, which is true in the case when we had just started a
            // new line.
            if (currentLine.Trim() != String.Empty)
            {
                lines.Add(currentLine);
            }

            return lines;
        }

        #endregion
    }
}