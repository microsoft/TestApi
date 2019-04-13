// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Test.Text;
using Xunit;
using Xunit.Extensions;
using System;

namespace Microsoft.Test.AcceptanceTests.Text
{
    public class CommonRegexPatternsTests
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
                yield return new object[] { CommonRegexPatterns.Date, @"12/31/2010", true };
                yield return new object[] { CommonRegexPatterns.Date, @"1/1/2010", true };
                yield return new object[] { CommonRegexPatterns.Date, @"12/01/2010", true };
                yield return new object[] { CommonRegexPatterns.Date, @"12/1/2010", true };
                yield return new object[] { CommonRegexPatterns.Date, @"12-1-2010", true };
                yield return new object[] { CommonRegexPatterns.Date, @"12-01-2010", true };
                yield return new object[] { CommonRegexPatterns.Date, @"12/1/10", true };
                yield return new object[] { CommonRegexPatterns.Date, @"12/01/10", true };
                yield return new object[] { CommonRegexPatterns.Date, @"01/01/2010", true };
                yield return new object[] { CommonRegexPatterns.Date, @"02/30/2010", true };
                yield return new object[] { CommonRegexPatterns.Date, @"13/01/2010", false };
                yield return new object[] { CommonRegexPatterns.Date, @"12/0/2010", false };
                yield return new object[] { CommonRegexPatterns.Date, @"12-32-2010", false };
                yield return new object[] { CommonRegexPatterns.Date, @"12/32/2010", false };

                yield return new object[] { CommonRegexPatterns.EmailAddress, @"somebody@microsoft.com" , true };
                yield return new object[] { CommonRegexPatterns.EmailAddress, @"Some.Body@microsoft.com", true };
                yield return new object[] { CommonRegexPatterns.EmailAddress, @"s@microsoft.com", true };
                yield return new object[] { CommonRegexPatterns.EmailAddress, @"somebody@.com", false };
                yield return new object[] { CommonRegexPatterns.EmailAddress, @"somebody@microsoft.", false };
                yield return new object[] { CommonRegexPatterns.EmailAddress, @"somebody@microsoft.c", false };

                yield return new object[] { CommonRegexPatterns.Guid, @"0f8fad5b-d9cb-469f-a165-70867728950e", true };
                yield return new object[] { CommonRegexPatterns.Guid, @"7c9e6679-7425-40de-944b-e07fc1f90ae7", true };
                yield return new object[] { CommonRegexPatterns.Guid, Guid.NewGuid().ToString(), true };
                yield return new object[] { CommonRegexPatterns.Guid, Guid.NewGuid().ToString(), true };
                yield return new object[] { CommonRegexPatterns.Guid, Guid.NewGuid().ToString(), true };
                yield return new object[] { CommonRegexPatterns.Guid, Guid.NewGuid().ToString(), true };
                yield return new object[] { CommonRegexPatterns.Guid, Guid.NewGuid().ToString(), true };
                yield return new object[] { CommonRegexPatterns.Guid, Guid.NewGuid().ToString(), true };
                yield return new object[] { CommonRegexPatterns.Guid, @"0f8fad5b-d9cb-469f-a165-70867728950", false };
                yield return new object[] { CommonRegexPatterns.Guid, @"0f8fad5b-d9cb-469f-a165-70867728950ee", false };

                yield return new object[] { CommonRegexPatterns.IpAddress, @"127.0.0.1", true };
                yield return new object[] { CommonRegexPatterns.IpAddress, @"127.0.0.0", true };
                yield return new object[] { CommonRegexPatterns.IpAddress, @"255.255.255.255", true };
                yield return new object[] { CommonRegexPatterns.IpAddress, @"255.255.255.256", false };
                yield return new object[] { CommonRegexPatterns.IpAddress, @"0.0.0.1", false };
                yield return new object[] { CommonRegexPatterns.IpAddress, @"256.0.0.1", false };
                yield return new object[] { CommonRegexPatterns.IpAddress, @"127.0.0.1.", false };
                yield return new object[] { CommonRegexPatterns.IpAddress, @"127.0.0.", false };
                yield return new object[] { CommonRegexPatterns.IpAddress, @"127.0.0", false };

                yield return new object[] { CommonRegexPatterns.Time, @"13:01", true };
                yield return new object[] { CommonRegexPatterns.Time, @"01:01", true };
                yield return new object[] { CommonRegexPatterns.Time, @"00:00", true };
                yield return new object[] { CommonRegexPatterns.Time, @"00:45", true };
                yield return new object[] { CommonRegexPatterns.Time, @"23:59", true };
                yield return new object[] { CommonRegexPatterns.Time, @"23:60", false };
                yield return new object[] { CommonRegexPatterns.Time, @"10:60", false };
                yield return new object[] { CommonRegexPatterns.Time, @"24:00", false };
                yield return new object[] { CommonRegexPatterns.Time, @"25:01", false };
                yield return new object[] { CommonRegexPatterns.Time, @"1:01", false };
                yield return new object[] { CommonRegexPatterns.Time, @"13:0", false };

                yield return new object[] { CommonRegexPatterns.UsaPhoneNumber, @"123-456-7890" ,true };
                yield return new object[] { CommonRegexPatterns.UsaPhoneNumber, @"023-456-7890", true };
                yield return new object[] { CommonRegexPatterns.UsaPhoneNumber, @"555-000-0001", true };
                yield return new object[] { CommonRegexPatterns.UsaPhoneNumber, @"555-000-0000", true };
                yield return new object[] { CommonRegexPatterns.UsaPhoneNumber, @"123-456-789", false };
                yield return new object[] { CommonRegexPatterns.UsaPhoneNumber, @"123-456-78901", false };
                yield return new object[] { CommonRegexPatterns.UsaPhoneNumber, @"12-456-7890", false };
                yield return new object[] { CommonRegexPatterns.UsaPhoneNumber, @"123-45-7890", false };

                yield return new object[] { CommonRegexPatterns.UsaSocialSecurityNumber, @"123-45-6789", true };
                yield return new object[] { CommonRegexPatterns.UsaSocialSecurityNumber, @"333-22-4444", true };
                yield return new object[] { CommonRegexPatterns.UsaSocialSecurityNumber, @"3333-22-4444", false };
                yield return new object[] { CommonRegexPatterns.UsaSocialSecurityNumber, @"33-22-4444", false };
                yield return new object[] { CommonRegexPatterns.UsaSocialSecurityNumber, @"333-222-4444", false };
                yield return new object[] { CommonRegexPatterns.UsaSocialSecurityNumber, @"333-2-4444", false };
                yield return new object[] { CommonRegexPatterns.UsaSocialSecurityNumber, @"333-22-44444", false };
                yield return new object[] { CommonRegexPatterns.UsaSocialSecurityNumber, @"333-22-444", false };
                yield return new object[] { CommonRegexPatterns.UsaSocialSecurityNumber, @"3a3-22-4444", false };

                yield return new object[] { CommonRegexPatterns.UsaZipCode, @"01234", true };
                yield return new object[] { CommonRegexPatterns.UsaZipCode, @"12345", true };
                yield return new object[] { CommonRegexPatterns.UsaZipCode, @"123456", false };
                yield return new object[] { CommonRegexPatterns.UsaZipCode, @"1234", false };
                yield return new object[] { CommonRegexPatterns.UsaZipCode, @"123a5", false };
                yield return new object[] { CommonRegexPatterns.UsaZipCode, @"123-5", false };

                yield return new object[] { CommonRegexPatterns.UsaZipCodeExtended, @"12345-6789", true };
                yield return new object[] { CommonRegexPatterns.UsaZipCodeExtended, @"12345-678", false };
                yield return new object[] { CommonRegexPatterns.UsaZipCodeExtended, @"12345-67890", false };
            }
        }
    }
}
