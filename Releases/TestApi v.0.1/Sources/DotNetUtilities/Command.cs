using System;

namespace Microsoft.Test
{
    /// <summary>
    /// An abstract class describing a command which has a name and an execute method.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// The name of the command. The base implementation is to strip off the last
        /// instance of "Command" from the end of the type name. So "DiscoverCommand"
        /// would become "Discover". If the type name does not have the string "Command" in it, 
        /// then the name of the command is the same as the type name. This behavior can be 
        /// overridden, but most derived classes are going to be of the form [Command Name] + Command.
        /// </summary>
        public virtual string Name
        {
            get
            {
                string typeName = this.GetType().Name;
                if (typeName.Contains("Command"))
                {
                    return typeName.Remove(typeName.LastIndexOf("Command", StringComparison.Ordinal));
                }
                else
                {
                    return typeName;
                }
            }
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        public abstract void Execute();
    }
}