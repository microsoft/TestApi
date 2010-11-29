using System.ComponentModel;
using System.IO;

namespace Microsoft.Test
{
    /// <summary>
    /// Converter that can convert from a string to a FileInfo.
    /// </summary>
    public class FileInfoConverter : TypeConverter
    {
        /// <summary>
        /// Converts from a string to a FileInfo.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="culture">Culture.</param>
        /// <param name="value">Value to convert.</param>
        /// <returns>FileInfo, or null if value was null or non-string.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string && value != null)
            {
                return new FileInfo((string)value);
            }
            else
            {
                return null;
            }          
        }
    }
}