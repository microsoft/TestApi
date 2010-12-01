// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Test.Text;
using Xunit;
using Xunit.Extensions;
using System.Collections.Generic;

namespace Microsoft.Test.AcceptanceTests.Text
{
    public class CommonRegularExpressionsTests
    {
        [Theory]
        [PropertyData("CommonRegularExpressionsFactory")]
        public void VerifyCommonRegularExpression(Regex regex, string matchCandiate, bool isMatchExpected)
        {
            bool isMatchActual = regex.IsMatch(matchCandiate);

            Assert.Equal<bool>(isMatchExpected, isMatchActual);
        }

        public static IEnumerable<object[]> CommonRegularExpressionsFactory
        {
            get
            {
                yield return new object[] { CommonRegularExpressions.CalendarDate, @"12/31/2010", true };
                yield return new object[] { CommonRegularExpressions.CalendarDate, @"1/1/2010", true };
                yield return new object[] { CommonRegularExpressions.CalendarDate, @"12/01/2010", true };
                yield return new object[] { CommonRegularExpressions.CalendarDate, @"01/01/2010", true };
                yield return new object[] { CommonRegularExpressions.CalendarDate, @"02/30/2010", true };
                yield return new object[] { CommonRegularExpressions.CalendarDate, @"13/01/2010", true };  // bug in the regex
                yield return new object[] { CommonRegularExpressions.CalendarDate, @"12-32-2010", false };
                yield return new object[] { CommonRegularExpressions.CalendarDate, @"12/32/2010", false };

                yield return new object[] { CommonRegularExpressions.EmailAddress, @"somebody@microsoft.com" , true };
                yield return new object[] { CommonRegularExpressions.EmailAddress, @"Some.Name@microsoft.com", true };
                yield return new object[] { CommonRegularExpressions.EmailAddress, @"s@microsoft.com", true };
                yield return new object[] { CommonRegularExpressions.EmailAddress, @"somebody@.com", false };
                yield return new object[] { CommonRegularExpressions.EmailAddress, @"somebody@microsoft.", false };
                yield return new object[] { CommonRegularExpressions.EmailAddress, @"somebody@microsoft.c", false };

                yield return new object[] { CommonRegularExpressions.IpAddress, @"128.0.0.1", false }; // bug in the regex
                yield return new object[] { CommonRegularExpressions.IpAddress, @"128.0.0.0", true };
                yield return new object[] { CommonRegularExpressions.IpAddress, @"255.255.255.255", false };
                yield return new object[] { CommonRegularExpressions.IpAddress, @"255.255.255.256", false };
                yield return new object[] { CommonRegularExpressions.IpAddress, @"0.0.0.1", false };

                yield return new object[] { CommonRegularExpressions.Time, @"13:01", true };
                yield return new object[] { CommonRegularExpressions.Time, @"01:01", true };
                yield return new object[] { CommonRegularExpressions.Time, @"00:00", true };
                yield return new object[] { CommonRegularExpressions.Time, @"1:01", false };
                yield return new object[] { CommonRegularExpressions.Time, @"13:0", false };
                yield return new object[] { CommonRegularExpressions.Time, @"25:01", false };
                yield return new object[] { CommonRegularExpressions.Time, @"10:60", true };  // bug in the regex

                yield return new object[] { CommonRegularExpressions.UsaPhoneNumber, @"123-456-7890" ,true };
                yield return new object[] { CommonRegularExpressions.UsaPhoneNumber, @"023-456-7890", true };
                yield return new object[] { CommonRegularExpressions.UsaPhoneNumber, @"555-000-0001", true };
                yield return new object[] { CommonRegularExpressions.UsaPhoneNumber, @"555-000-0000", true };
                yield return new object[] { CommonRegularExpressions.UsaPhoneNumber, @"123-456-789", false };
                yield return new object[] { CommonRegularExpressions.UsaPhoneNumber, @"123-456-78901", false };
                yield return new object[] { CommonRegularExpressions.UsaPhoneNumber, @"12-456-7890", false };
                yield return new object[] { CommonRegularExpressions.UsaPhoneNumber, @"123-45-7890", false };

                yield return new object[] { CommonRegularExpressions.UsaSocialSecurityNumber, @"123-45-6789", true };
                yield return new object[] { CommonRegularExpressions.UsaSocialSecurityNumber, @"333-22-4444", true };
                yield return new object[] { CommonRegularExpressions.UsaSocialSecurityNumber, @"3333-22-4444", false };
                yield return new object[] { CommonRegularExpressions.UsaSocialSecurityNumber, @"33-22-4444", false };
                yield return new object[] { CommonRegularExpressions.UsaSocialSecurityNumber, @"333-222-4444", false };
                yield return new object[] { CommonRegularExpressions.UsaSocialSecurityNumber, @"333-2-4444", false };
                yield return new object[] { CommonRegularExpressions.UsaSocialSecurityNumber, @"333-22-44444", false };
                yield return new object[] { CommonRegularExpressions.UsaSocialSecurityNumber, @"333-22-444", false };
                yield return new object[] { CommonRegularExpressions.UsaSocialSecurityNumber, @"3a3-22-4444", false };

                yield return new object[] { CommonRegularExpressions.UsaZipCode, @"01234", true };
                yield return new object[] { CommonRegularExpressions.UsaZipCode, @"12345", true };
                yield return new object[] { CommonRegularExpressions.UsaZipCode, @"123456", false };
                yield return new object[] { CommonRegularExpressions.UsaZipCode, @"1234", false };
                yield return new object[] { CommonRegularExpressions.UsaZipCode, @"123a5", false };
                yield return new object[] { CommonRegularExpressions.UsaZipCode, @"123-5", false };

                yield return new object[] { CommonRegularExpressions.UsaZipCodeExtended, @"12345-6789", true };
                yield return new object[] { CommonRegularExpressions.UsaZipCodeExtended, @"12345-678", false };
                yield return new object[] { CommonRegularExpressions.UsaZipCodeExtended, @"12345-67890", false };
            }
        }
    }
}
