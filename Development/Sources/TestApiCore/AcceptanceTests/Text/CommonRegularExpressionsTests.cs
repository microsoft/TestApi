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
        /// <summary>
        /// InlineData requires all inline data to be constants. 
        /// With PropertyData we can get around that limitation, which allows us to supply  
        /// static readonly property values (CommonRegularExpressions.XXX) to the data-driven test.
        /// </summary>
        [Theory]
        [PropertyData("CommonRegularExpressionsTestData")]
        public void GenerateRandomStringFromCommonRegularExpression(Regex regex, string expectedMatch)
        {
            string s = StringFactory.GenerateRandomString(regex, 1234);

            Assert.True(regex.IsMatch(s));
            Assert.True(regex.IsMatch(expectedMatch));
        }

        public static IEnumerable<object[]> CommonRegularExpressionsTestData
        {
            get
            {
                yield return new object[] { CommonRegularExpressions.CalendarDate, @"12-01-2010" };
                yield return new object[] { CommonRegularExpressions.EmailAddress, @"somebody@microsoft.com" };
                yield return new object[] { CommonRegularExpressions.IpAddress, @"128.0.0.1" };
                yield return new object[] { CommonRegularExpressions.Time, @"13:01" };
                yield return new object[] { CommonRegularExpressions.UsaPhoneNumber, @"123-456-7890" };
                yield return new object[] { CommonRegularExpressions.UsaSocialSecurityNumber, @"555-22-9999" };
                yield return new object[] { CommonRegularExpressions.UsaZipCode, @"12345" };
                yield return new object[] { CommonRegularExpressions.UsaZipCodeExtended, @"12345-6789" };
            }
        }
    }
}
