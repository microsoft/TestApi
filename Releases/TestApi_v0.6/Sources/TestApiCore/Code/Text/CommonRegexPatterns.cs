// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Text.RegularExpressions;

namespace Microsoft.Test.Text
{
    /// <summary>
    /// Provides a set of commonly-used regular expression patterns.
    /// </summary>
    public static class CommonRegexPatterns
    {
        /// <summary>
        /// Calendar date in formats MM/DD/YYYY, MM/DD/YY, MM-DD-YYYY, MM-DD-YY, etc. Example: 12/1/2010.
        /// </summary>
        public static readonly Regex Date = new Regex(@"^([0]?[1-9]|[1][0-2])[./-]([0]?[1-9]|[1|2][0-9]|[3][0|1])[./-]([0-9]{4}|[0-9]{2})$");

        /// <summary>
        /// Email address. Examples: some.body@microsoft.com, somebody@microsoft.com.
        /// </summary>
        public static readonly Regex EmailAddress = new Regex(@"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");

        /// <summary>
        /// GUID (Globally Unique Identifier). See <a href="http://msdn.microsoft.com/en-us/library/system.guid.aspx">this article</a> for more information.
        /// </summary>
        public static readonly Regex Guid = new Regex(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$");

        /// <summary>
        /// IP address. Example: 127.0.0.1
        /// </summary>
        public static readonly Regex IpAddress = new Regex(@"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$");

        /// <summary>
        /// Time in HH:MM format. Examples: 13:01, 23:59, 00:45.
        /// </summary>
        public static readonly Regex Time = new Regex(@"^([0-1][0-9]|[2][0-3]):([0-5][0-9])$");

        /// <summary>
        /// USA phone number. Example: 123-456-7890.
        /// </summary>
        public static readonly Regex UsaPhoneNumber = new Regex(@"^\d{3}-\d{3}-\d{4}$");

        /// <summary>
        /// USA social security number. Example: 123-45-6789.
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
    }
}
