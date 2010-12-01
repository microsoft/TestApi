// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Text.RegularExpressions;

namespace Microsoft.Test.Text
{
    /// <summary>
    /// Provides a set of commonly-used regular expressions.
    /// </summary>
    public static class CommonRegularExpressions
    {
        /// <summary>
        /// Calendar date (MM-DD-YYYY). Example: 12-01-2010.
        /// </summary>
        public static readonly Regex CalendarDate = new Regex(@"(\d|1[12])/[1-2]\d/((\d{2})|(\d{4}))");

        /// <summary>
        /// IP address. Example: 128.0.0.1
        /// </summary>
        public static readonly Regex IpAddress = new Regex(@"((?<num>(1?\d?\d)|(2[0-4]\d)|(25[0-4]))\.){3}\k<num>");

        /// <summary>
        /// Time. Example: 13:01.
        /// </summary>
        public static readonly Regex Time = new Regex(@"(((0?\d)|(1[012])):[0-6]\d ?([ap]m)|((2[0-3])|([01] ?\d)):[0-6]\d)");

        /// <summary>
        /// USA phone number. Example: 123-456-7890.
        /// </summary>
        public static readonly Regex UsaPhoneNumber = new Regex(@"^\d{3}-\d{3}-\d{4}$");

        /// <summary>
        /// USA social security number. Example: 555-22-9999.
        /// </summary>
        public static readonly Regex UsaSocialSecurityNumber = new Regex(@"^\d{3}-\d{2}-\d{4}$");

        /// <summary>
        /// USA ZIP code. Example: 12345.
        /// </summary>
        public static readonly Regex UsaZipCode = new Regex(@"^\d{5}$");

        /// <summary>
        /// USA extended ZIP code. Example: 12345-6789.
        /// </summary>
        public static readonly Regex UsaZipCodeExtended = new Regex(@"^\d{5}-\d{4}$");

        /// <summary>
        /// Email address. Example: william.rawls@gmail.com.
        /// </summary>
        public static readonly Regex EmailAddress = new Regex(@"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
    }
}
