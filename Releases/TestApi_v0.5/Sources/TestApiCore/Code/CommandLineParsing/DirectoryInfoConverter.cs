// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.IO;

namespace Microsoft.Test.CommandLineParsing
{
    /// <summary>
    /// Converter that can convert from a string to a DirectoryInfo.
    /// </summary>
    public class DirectoryInfoConverter : TypeConverter
    {
        /// <summary>
        /// Converts from a string to a DirectoryInfo.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="culture">Culture.</param>
        /// <param name="value">Value to convert.</param>
        /// <returns>DirectoryInfo, or null if value was null or non-string.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string && value != null)
            {
                return new DirectoryInfo((string)value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
        /// <param name="sourceType">A Type that represents the type you want to convert from.</param>
        /// <returns>True if this converter can perform the conversion; otherwise, False.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }
    }
}