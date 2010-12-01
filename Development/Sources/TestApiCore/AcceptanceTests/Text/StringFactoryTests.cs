// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Test.Text;
using Xunit;
using Xunit.Extensions;

namespace Microsoft.Test.AcceptanceTests.Text
{
    public class StringFactoryTests
    {
        [Fact]
        public void GeneratePureRandomStrings()
        {
            StringProperties properties = new StringProperties();
            properties.MinNumberOfCodePoints = 10;
            properties.MaxNumberOfCodePoints = 30;
            string s1 = StringFactory.GenerateRandomString(properties, 1234);
            
            Assert.NotNull(s1);
        }

        [Fact]
        public void VerifyStringLength()
        {
            StringProperties properties = new StringProperties();
            properties.MinNumberOfCodePoints = 20;
            properties.MaxNumberOfCodePoints = 30;
            string s1 = StringFactory.GenerateRandomString(properties, 1234);

            properties.MinNumberOfCodePoints = properties.MaxNumberOfCodePoints = 5;
            string s2 = StringFactory.GenerateRandomString(properties, 1234);
            
            Assert.True(s1.Length >= 20 && s1.Length <= 60);
            Assert.True(s2.Length >= 5 && s2.Length <= 10);
        }

        [Fact]
        public void VerifySameSeedGeneratesSameString()
        {
            StringProperties properties = new StringProperties();
            properties.MinNumberOfCodePoints = 10;
            properties.MaxNumberOfCodePoints = 30;

            string s1 = StringFactory.GenerateRandomString(properties, 1234);
            string s2 = StringFactory.GenerateRandomString(properties, 1234);
            string s3 = StringFactory.GenerateRandomString(properties, 4321);

            Assert.NotNull(s1);
            Assert.NotNull(s2);
            Assert.NotNull(s2);

            Assert.Equal<string>(s1, s2);
            Assert.NotEqual<string>(s1, s3);
        }
        
        [Fact]
        public void GenerateRandomStringsWithProvidedRangeAsArabicSupplement()
        {
            StringProperties properties1 = new StringProperties();
            properties1.MinNumberOfCodePoints = 5;
            properties1.MaxNumberOfCodePoints = 10;
            properties1.UnicodeRanges.Add(new UnicodeRange(0x0750, 0x077F));
            string s1 = StringFactory.GenerateRandomString(properties1, 1234);

            bool isInTheRange = true;
            foreach (char c in s1)
            {
                if (Convert.ToInt32(c) < 0x0750 || Convert.ToInt32(c) > 0x077F)
                {
                    isInTheRange = false;
                }
            }
            
            Assert.NotNull(s1);
            Assert.True(s1.Length >= 5 && s1.Length <= 20);
            Assert.True(isInTheRange);
        }

        [Fact]
        public void GenerateRandomStringsWithProvidedChartOfArabicSupplement()
        {
            StringProperties properties1 = new StringProperties();
            properties1.MinNumberOfCodePoints = 5;
            properties1.MaxNumberOfCodePoints = 10;
            properties1.UnicodeRanges.Add(new UnicodeRange(UnicodeChart.ArabicSupplement));
            string s1 = StringFactory.GenerateRandomString(properties1, 1234);

            bool isInTheRange = true;
            foreach (char c in s1)
            {
                if (Convert.ToInt32(c) < 0x0750 || Convert.ToInt32(c) > 0x077F)
                {
                    isInTheRange = false;
                }
            }

            Assert.NotNull(s1);
            Assert.True(s1.Length >= 5 && s1.Length <= 20);
            Assert.True(isInTheRange);
        }

        [Fact]
        public void GenerateRandomStringsWithBidi()
        {
            StringProperties properties = new StringProperties();
            properties.MinNumberOfCodePoints = 10;
            properties.MaxNumberOfCodePoints = 20;
            properties.UnicodeRanges.Add(new UnicodeRange(0x0000, 0xFFFF));
            properties.IsBidirectional = true;
            String s1 = StringFactory.GenerateRandomString(properties, 1234);
            
            bool isInTheRange = false;
            foreach (char c in s1)
            {
                if (Convert.ToInt32(c) <= 0xFFFF)
                {
                    isInTheRange = true;
                }
            }

            Assert.NotNull(s1);
            Assert.True(s1.Length >= 10 && s1.Length <= 20);
            Assert.True(isInTheRange);
        }

        [Fact]
        public void GenerateRandomStringsWithCombiningMarks()
        {
            StringProperties properties = new StringProperties();
            properties.MinNumberOfCodePoints = 10;
            properties.MaxNumberOfCodePoints = 30;
            properties.UnicodeRanges.Add(new UnicodeRange(0x0000, 0x03FF));
            properties.MinNumberOfCombiningMarks = 5;
            String s1 = StringFactory.GenerateRandomString(properties, 1234);
            
            bool isInTheRange = false;
            foreach (char c in s1)
            {
                if (Convert.ToInt32(c) <= 0x03FF)
                {
                    isInTheRange = true;
                }
            }

            Assert.NotNull(s1);
            Assert.True(s1.Length >= 10 && s1.Length <=30);
            Assert.True(isInTheRange);
        }

        [Fact]
        public void GenerateRandomStringsWithEndUserDefinedCodePoints()
        {
            StringProperties properties = new StringProperties();
            properties.MinNumberOfCodePoints = 20;
            properties.MaxNumberOfCodePoints = 30;
            properties.UnicodeRanges.Add(new UnicodeRange(0x0000, 0xFFFF));
            properties.MinNumberOfEndUserDefinedCodePoints = 5;
            String s1 = StringFactory.GenerateRandomString(properties, 1234);

            int numOfEUDC = 0;
            foreach (char c in s1)
            {
                if (Convert.ToInt32(c) >= 0xE000 && Convert.ToInt32(c) <= 0xF8FF)
                {
                    numOfEUDC++;
                }
            }

            Assert.NotNull(s1);
            Assert.True(s1.Length >= 20 && s1.Length <= 30);
            Assert.True(numOfEUDC >= 5);
        }

        [Fact]
        public void GenerateRandomStringsWithLineBreaks()
        {
            StringProperties properties = new StringProperties();
            properties.MinNumberOfCodePoints = 10;
            properties.MaxNumberOfCodePoints = 40;
            properties.UnicodeRanges.Add(new UnicodeRange(0x0000, 0xFFFF));
            properties.MinNumberOfLineBreaks = 5;
            String s1 = StringFactory.GenerateRandomString(properties, 1234);
            
            bool isInTheRange = false;
            foreach (char c in s1)
            {
                if (Convert.ToInt32(c) <= 0x1FFFF)
                {
                    isInTheRange = true;
                }
            }

            Assert.NotNull(s1);
            Assert.True(s1.Length >= 10 && s1.Length <= 40);
            Assert.True(isInTheRange);
        }

        [Fact]
        public void GenerateRandomStringsWithNumbers()
        {
            StringProperties properties = new StringProperties();
            properties.MinNumberOfCodePoints = 20;
            properties.MaxNumberOfCodePoints = 30;
            properties.UnicodeRanges.Add(new UnicodeRange(0x0000, 0x1FFFF));
            properties.HasNumbers = true;
            String s1 = StringFactory.GenerateRandomString(properties, 1234);

            bool hasNumber = false;
            foreach (char c in s1)
            {
                if (Convert.ToInt32(c) >= 0x0030 && Convert.ToInt32(c) <= 0x0039)
                {
                    hasNumber = true;
                }
            }

            Assert.NotNull(s1);
            Assert.True(s1.Length >= 20 && s1.Length <= 60);
            Assert.True(hasNumber);
        }

        [Fact]
        public void GenerateRandomStringsWithSurrogates()
        {
            StringProperties properties = new StringProperties();
            properties.MinNumberOfCodePoints = 20;
            properties.MaxNumberOfCodePoints = 30;
            properties.UnicodeRanges.Add(new UnicodeRange(0x0000, 0x1FFFF));
            properties.MinNumberOfSurrogatePairs = 5;
            String s1 = StringFactory.GenerateRandomString(properties, 1234);

            int numOfSurrogates = 0;
            foreach (char c in s1)
            {
                if (Convert.ToInt32(c) >= 0xD800 && Convert.ToInt32(c) <= 0xDBFF)
                {
                    numOfSurrogates++;
                }
            }

            Assert.NotNull(s1);
            Assert.True(s1.Length >= 20 && s1.Length <= 60);
            Assert.True(numOfSurrogates >= 5);
        }

        [Fact]
        public void GenerateRandomNormalizedString()
        {
            StringProperties properties = new StringProperties();
            properties.MinNumberOfCodePoints = 10;
            properties.MaxNumberOfCodePoints = 30;
            properties.UnicodeRanges.Add(new UnicodeRange(0x0000, 0x1FFFF));
            properties.NormalizationForm = NormalizationForm.FormC;
            String s1 = StringFactory.GenerateRandomString(properties, 1234);
            
            bool isFormC = s1.IsNormalized(NormalizationForm.FormC);

            Assert.NotNull(s1);
            Assert.True(s1.Length >= 10 && s1.Length <= 60);
            Assert.True(isFormC);
        }

        [Fact]
        public void GenerateRandomStringsWithSegmentation()
        {
            StringProperties properties = new StringProperties();
            properties.MinNumberOfCodePoints = 10;
            properties.MaxNumberOfCodePoints = 40;
            properties.UnicodeRanges.Add(new UnicodeRange(0x0000, 0xFFFF));
            properties.MinNumberOfTextSegmentationCodePoints = 5;
            String s1 = StringFactory.GenerateRandomString(properties, 1234);
            
            bool isInTheRange = false;
            foreach (char c in s1)
            {
                if (Convert.ToInt32(c) <= 0xFFFF)
                {
                    isInTheRange = true;
                }
            }
            
            Assert.NotNull(s1);
            Assert.True(s1.Length >= 10 && s1.Length <= 40);
            Assert.True(isInTheRange);
        }

        [Fact]
        public void GenerateRandomStringsWithEndUserDefinedCharactersAndHasNumber()
        {
            StringProperties properties = new StringProperties();
            properties.MinNumberOfCodePoints = 10;
            properties.MaxNumberOfCodePoints = 40;
            properties.UnicodeRanges.Add(new UnicodeRange(0x0000, 0xFFFF));
            properties.MinNumberOfEndUserDefinedCodePoints = 6;
            properties.HasNumbers = true;
            String s1 = StringFactory.GenerateRandomString(properties, 1234);

            bool hasNumbers = false;
            foreach (char c in s1)
            {
                if (Convert.ToInt32(c) >= 0x0030 && Convert.ToInt32(c) <= 0x0039)
                {
                    hasNumbers = true;
                }
            }
            Assert.True(hasNumbers);

            int numOfEUDCs = 0;
            foreach (char c in s1)
            {
                if (Convert.ToInt32(c) >= 0xE000 && Convert.ToInt32(c) <= 0xF8FF)
                {
                    numOfEUDCs++;
                }
            }
            
            Assert.NotNull(s1);
            Assert.True(s1.Length >= 10 && s1.Length <= 40);
            Assert.True(numOfEUDCs >= 6);
        }

        [Fact]
        public void VerifyOnlyArabicChartGivenForBidiProperty()
        {
            StringProperties sp = new StringProperties();
            sp.MinNumberOfCodePoints = 5;
            sp.MaxNumberOfCodePoints = 10;
            sp.UnicodeRanges.Add(new UnicodeRange(UnicodeChart.Arabic));
            sp.IsBidirectional = true;
            Random rand = new Random();

            bool exceptionCaught = false;
            try
            {
                string s = StringFactory.GenerateRandomString(sp, rand.Next());
            }
            catch (ArgumentOutOfRangeException e)
            {
                Assert.True(e.Message.Contains("0x007A ranges are needed to construct Bidi string"));
                exceptionCaught = true;
            }

            Assert.True(exceptionCaught);
        }

        [Fact]
        public void VerifyOnlyHebrewChartGivenForBidiProperty()
        {
            StringProperties sp = new StringProperties();
            sp.MinNumberOfCodePoints = 5;
            sp.MaxNumberOfCodePoints = 10;
            sp.UnicodeRanges.Add(new UnicodeRange(UnicodeChart.Hebrew));
            sp.IsBidirectional = true;
            Random rand = new Random();

            bool exceptionCaught = false;
            try
            {
                string s = StringFactory.GenerateRandomString(sp, rand.Next());
            }
            catch (ArgumentOutOfRangeException e)
            {
                Assert.True(e.Message.Contains("0x007A ranges are needed to construct Bidi string"));
                exceptionCaught = true;
            }

            Assert.True(exceptionCaught);
        }

        [Fact]
        public void VerifyOnlyLatinChartGivenForBidiProperty()
        {
            StringProperties sp = new StringProperties();
            sp.MinNumberOfCodePoints = 5;
            sp.MaxNumberOfCodePoints = 10;
            sp.UnicodeRanges.Add(new UnicodeRange(UnicodeChart.Latin));
            sp.IsBidirectional = true;
            Random rand = new Random();

            bool exceptionCaught = false;
            try
            {
                string s = StringFactory.GenerateRandomString(sp, rand.Next());
            }
            catch (ArgumentOutOfRangeException e)
            {
                Assert.True(e.Message.Contains("Refer to Arabic and Hebrew ranges"));
                exceptionCaught = true;
            }

            Assert.True(exceptionCaught);
        }

        [Fact]
        public void GenerateRandomStringsWithCodePointsFromTaiLeAndNewTaiLueOnly()
        {
            StringProperties properties = new StringProperties();
            properties.MinNumberOfCodePoints = 10;
            properties.MaxNumberOfCodePoints = 20;
            properties.UnicodeRanges.Add(new UnicodeRange(UnicodeChart.TaiLe));
            properties.UnicodeRanges.Add(new UnicodeRange(UnicodeChart.NewTaiLue));
            String s1 = StringFactory.GenerateRandomString(properties, 1234);
            
            bool isInTheRange = false;
            foreach (char c in s1)
            {
                if ((Convert.ToInt32(c) >= 0x1950 && Convert.ToInt32(c) <= 0x19DF))
                {
                    isInTheRange = true;
                }
            }

            Assert.NotNull(s1);
            Assert.True(s1.Length >= 10 && s1.Length <= 20);
            Assert.True(isInTheRange);

            isInTheRange = false;
            foreach (char c in s1)
            {
                // Make sure Tai Le code point is in the string
                if (Convert.ToInt32(c) >= 0x1950 && Convert.ToInt32(c) <= 0x197F)
                {
                    isInTheRange = true;
                }
            }
            Assert.True(isInTheRange);

            isInTheRange = false;
            foreach (char c in s1)
            {
                // Make sure New Tai Lue code point is in the string
                if (Convert.ToInt32(c) >= 0x1980 && Convert.ToInt32(c) <= 0x19DF)
                {
                    isInTheRange = true;
                }
            }
            Assert.True(isInTheRange);
        }

        [Fact]
        public void GenerateRandomBidiStringsWithCodePointsFromLatinAndHebrewOnly()
        {
            StringProperties properties = new StringProperties();
            properties.MinNumberOfCodePoints = 10;
            properties.MaxNumberOfCodePoints = 20;
            
            properties.UnicodeRanges.Add(new UnicodeRange(UnicodeChart.Arabic));
            properties.UnicodeRanges.Clear();
            
            properties.UnicodeRanges.Add(new UnicodeRange(0x0030, 0x007A));
            properties.UnicodeRanges.Add(new UnicodeRange(UnicodeChart.Hebrew));
            properties.IsBidirectional = true;
            String s1 = StringFactory.GenerateRandomString(properties, 1234);
            
            bool isInTheRange = false;
            foreach (char c in s1)
            {
                if ((Convert.ToInt32(c) >= 0x0030 && Convert.ToInt32(c) <= 0x007A) ||
                    (Convert.ToInt32(c) >= 0x0590 && Convert.ToInt32(c) <= 0x05FF))
                {
                    isInTheRange = true;
                }
            }

            Assert.NotNull(s1);
            Assert.True(s1.Length >= 10 && s1.Length <= 20);
            Assert.True(isInTheRange);

            isInTheRange = false;
            foreach (char c in s1)
            {
                // Make sure Latin code point is in the string
                if (Convert.ToInt32(c) >= 0x0030 && Convert.ToInt32(c) <= 0x007A)
                {
                    isInTheRange = true;
                }
            }
            Assert.True(isInTheRange);

            isInTheRange = false;
            foreach (char c in s1)
            {
                // Make sure Hebrew code point is in the string
                if (Convert.ToInt32(c) >= 0x0590 && Convert.ToInt32(c) <= 0x05FF)
                {
                    isInTheRange = true;
                }
            }
            Assert.True(isInTheRange);
        }

        [Theory]
        [InlineData(@"((\(\d{3}\)?)|(\d{3}-))\d{3}-\d{4}",                                  1234, @"555-898-7332")]          // North American phone number
        [InlineData(@"[A-Za-z0-9]+@(([A-Za-z0-9\-])+\.)+([A-Za-z\-])+",                     3456, @"testapi@microsoft.com")] // email address
        [InlineData(@"(\d|1[12])/[1-2]\d/((\d{2})|(\d{4}))",                                78, @"12-01-2010")]              // calendar date
        [InlineData(@"([^/:\*\?<>\|\x00-\x1F\x7F\\])",                                      9, @"W")]                        // valid filename character
        [InlineData(@"([^\xFFFE\xFFFF\xFDD0-\xFDEF\xD800-\xDBFF\xDC00-\xDFFF]|([\xD800-\xDBFF][\xDC00-\xDFFF]))", -3, @"Ж")] // valid Unicode character
        [InlineData(@"((?<num>(1?\d?\d)|(2[0-4]\d)|(25[0-4]))\.){3}\k<num>",                121, @"128.0.0.1")]              // ip address
        [InlineData(@"(((0?\d)|(1[012])):[0-6]\d ?([ap]m)|((2[0-3])|([01] ?\d)):[0-6]\d)",  0, @"13:05")]                    // time
        public void GenerateRandomStringFromRegularExpression(string regexStr, int seed, string expectedMatch)
        {
            Regex regex = new Regex(regexStr);

            string s = StringFactory.GenerateRandomString(regex, seed);

            Assert.True(regex.IsMatch(s));
            Assert.True(regex.IsMatch(expectedMatch));
        }

        [Theory]
        [InlineData(@"((\(\d{3}\)?)|(\d{3}-))\d{3}-\d{4}",                                  1234)]  // North American phone number
        [InlineData(@"[A-Za-z0-9]+@(([A-Za-z0-9\-])+\.)+([A-Za-z\-])+",                     3456)]  // email address
        [InlineData(@"(\d|1[12])/[1-2]\d/((\d{2})|(\d{4}))",                                78)]    // calendar date
        [InlineData(@"([^/:\*\?<>\|\x00-\x1F\x7F\\])",                                      9)]     // valid filename character
        [InlineData(@"([^\xFFFE\xFFFF\xFDD0-\xFDEF\xD800-\xDBFF\xDC00-\xDFFF]|([\xD800-\xDBFF][\xDC00-\xDFFF]))", -3)] // valid Unicode character
        [InlineData(@"((?<num>(1?\d?\d)|(2[0-4]\d)|(25[0-4]))\.){3}\k<num>",                121)]   // ip address
        [InlineData(@"(((0?\d)|(1[012])):[0-6]\d ?([ap]m)|((2[0-3])|([01] ?\d)):[0-6]\d)",  0)]     // time
        public void VerifyReproducibilityOfRandomStringsFromRegularExpression(string regexStr, int seed)
        {
            Regex regex = new Regex(regexStr);

            string s1 = StringFactory.GenerateRandomString(regex, seed);
            string s2 = StringFactory.GenerateRandomString(regex, seed);

            Assert.Equal<string>(s1, s2);
        }

        [Theory]
        [PropertyData("CommonRegularExpressionsFactory1")]
        public void GenerateRandomStringFromCommonRegularExpression(Regex regex)
        {
            string s = StringFactory.GenerateRandomString(regex, 1234);

            Assert.True(regex.IsMatch(s));
        }

        public static IEnumerable<object[]> CommonRegularExpressionsFactory1
        {
            get
            {
                yield return new object[] { CommonRegularExpressions.Date };
                yield return new object[] { CommonRegularExpressions.EmailAddress };
                yield return new object[] { CommonRegularExpressions.IpAddress };
                yield return new object[] { CommonRegularExpressions.Time };
                yield return new object[] { CommonRegularExpressions.UsaPhoneNumber };
                yield return new object[] { CommonRegularExpressions.UsaSocialSecurityNumber };
                yield return new object[] { CommonRegularExpressions.UsaZipCode };
                yield return new object[] { CommonRegularExpressions.UsaZipCodeExtended };
            }
        }
    }
}
