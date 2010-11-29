using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Test.Text
{
    /// <summary>
    /// Contains types for the generation, manipulation and validation of strings and text, for testing purposes.  
    /// </summary>
    // Suppressed the warning that the class is never instantiated.
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812")]
    [System.Runtime.CompilerServices.CompilerGenerated()]
    class NamespaceDoc
    {
        // Empty class used only for generation of namespace comments.
    }

    /// <summary>
    /// Defines the desired properties of a character string. 
    /// For more information on character strings, see <a href="http://msdn.microsoft.com/en-us/library/dd317711(VS.85).aspx">this article</a>.
    /// </summary>
    /// <remarks>
    /// Note that this class is used as <i>"a filter"</i> when generating character strings with  
    /// <see cref="StringFactory"/>. Upon instantiation, all properties except CultureInfo of a <see cref="StringProperties"/>  
    /// object (which are all <a href="http://msdn.microsoft.com/en-us/library/system.nullable.aspx">Nullables</a>)   
    /// have null values, which means that the object does not impose any filtering limitations on  
    /// the generated strings. 
    /// <para>
    /// Setting properties to non-null values means that the value of the property should be taken  
    /// into account by <see cref="StringFactory"/> during  string generation. For example, setting  
    /// <see cref="MaxNumberOfCharacters"/> to 10 means <i>"generate strings with up to 10 characters"</i>.</para>
    /// </remarks>
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
    public class StringProperties
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public StringProperties()
        {
            CultureInfo = null;
        }

        /// <summary>
        /// Determines whether the string belongs to a specific culture by examining the name of the culture
        /// </summary>
        public CultureInfo CultureInfo {get; set;}

        /// <summary>
        /// Determines whether the string has combining marks. 
        /// <a href="http://en.wikipedia.org/wiki/Combining_diacritical_mark">Combining marks</a> (and combining 
        /// characters in general) are characters that are intended to modify other characters (e.g. accents, etc.)
        /// </summary>
        public bool? HasCombiningMarks { get; set; }

        /// <summary>
        /// Determines whether the string contains formatted numbers 
        /// </summary>
        public bool? HasNumbers { get; set; }

        /// <summary>
        /// Determines whether the string is <a href="http://en.wikipedia.org/wiki/Bi-directional_text">bi-directional</a>. 
        /// </summary>
        public bool? IsBidirectional { get; set; }

        /// <summary>
        /// Defines the desired type of normalization to perform on the string. 
        /// For more information, see <a href="http://www.unicode.org/reports/tr15">this article</a>.
        /// </summary>
        public NormalizationForm? NormalizationForm { get; set; }

        /// <summary>
        /// Determines the desired minimum number of characters in the string.
        /// </summary>
        public int? MinNumberOfCharacters { get; set; }

        /// <summary>
        /// Determines the desired maximum number of characters in the string.
        /// </summary>
        public int? MaxNumberOfCharacters { get; set; }

        /// <summary>
        /// Determines minimum number of <a href="http://msdn.microsoft.com/en-us/library/dd317802(VS.85).aspx">end-user-defined characters</a> (EUDC) in the string.
        /// </summary>
        public int? MinNumberOfEndUserDefinedCharacters { get; set; }

        /// <summary>
        /// Determines maximum number of <a href="http://msdn.microsoft.com/en-us/library/dd317802(VS.85).aspx">end-user-defined characters</a> (EUDC) in the string.
        /// </summary>
        public int? MaxNumberOfEndUserDefinedCharacters { get; set; }

        /// <summary>
        /// Determines the desired minimum number of line breaks in the string.
        /// </summary>
        public int? MinNumberOfLineBreaks { get; set; }

        /// <summary>
        /// Determines the desired maximum number of line breaks in the string.
        /// </summary>
        public int? MaxNumberOfLineBreaks { get; set; }

        /// <summary>
        /// Determines the starting range of the Unicode characters in the string.
        /// </summary>
        public int? StartOfUnicodeRange { get; set; }

        /// <summary>
        /// Determines the ending range of the Unicode characters in the string.
        /// </summary>
        public int? EndOfUnicodeRange { get; set; }

        /// <summary>
        /// Determines minimum number of <a href="http://en.wikipedia.org/wiki/Surrogate_pair">surrogate pairs</a> in the string. 
        /// </summary>
        public int? MinNumberOfSurrogatePairs { get; set; }

        /// <summary>
        /// Determines maximum number of <a href="http://en.wikipedia.org/wiki/Surrogate_pair">surrogate pairs</a> in the string. 
        /// </summary>
        public int? MaxNumberOfSurrogatePairs { get; set; }

        /// <summary>
        /// Determines minimum number of <a href="http://en.wikipedia.org/wiki/Text_segmentation">text segmentation characters</a> in the string.
        /// </summary>
        public int? MinNumberOfTextSegmentationCharacters { get; set; }

        /// <summary>
        /// Determines maximum number of <a href="http://en.wikipedia.org/wiki/Text_segmentation">text segmentation characters</a> in the string.
        /// </summary>
        public int? MaxNumberOfTextSegmentationCharacters { get; set; }
    }
}
