using System;
using System.Globalization;

namespace Microsoft.Test.Text
{
    /// <summary>
    /// Provides factory methods for generation of text, interesting from testing point of view.
    /// </summary>
    ///
    /// <example>The following example demonstrates how to create a random string with of 10 characters:
    /// <code>
    /// // Generate a random string of 10 characters.
    ///
    /// StringProperties sp = new StringProperties();
    /// sp.MaxNumberOfCharacters = sp.MinNumberOfCharacters = 10;
    ///
    /// string s = StringFactory.GenerateRandomString(sp, 0);
    /// </code>
    /// </example>
    public static class StringFactory
    {
        /// <summary>
        /// Generates a random string, with the specified properties. 
        /// <font color="red">NOT IMPLEMENTED.</font>
        /// </summary>
        /// <param name="stringProperties">The properties of the generated string.</param>
        /// <param name="seed">A number used to calculate a starting value for the pseudo-random number sequence. If a negative number is specified, the absolute value of the number is used.</param>
        /// <returns>A random string with the specified properties.</returns>
        public static string GenerateRandomString(StringProperties stringProperties, int seed)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an array of predefined strings, interesting from a text testing point of view, with the specified properties.
        /// <font color="red">NOT IMPLEMENTED.</font>
        /// </summary>
        /// <param name="stringProperties">The properties of the generated string.</param>
        /// <returns>An array of predefined strings with the specified properties.</returns>
        public static string[] GetPredefinedStrings(StringProperties stringProperties)
        {
            throw new NotImplementedException();
        }
    }
}
