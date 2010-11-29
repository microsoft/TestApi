// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Test.Text;
using System;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Extensions;
using System.Globalization;

namespace Microsoft.Test.AcceptanceTests
{
    public class StringPropertiesTests
    {
        [Fact]
        public void AllPropertiesAreNullUponObjectInstantiation()
        {
            StringProperties sp = new StringProperties();

            Assert.Equal<CultureInfo>(null, sp.CultureInfo);
            Assert.Equal<bool?>(null, sp.HasCombiningMarks);
            Assert.Equal<bool?>(null, sp.HasNumbers);
            Assert.Equal<bool?>(null, sp.IsBidirectional);
            Assert.Equal<NormalizationForm?>(null, sp.NormalizationForm);
            Assert.Equal<int?>(null, sp.MinNumberOfCharacters);
            Assert.Equal<int?>(null, sp.MaxNumberOfCharacters);
            Assert.Equal<int?>(null, sp.MinNumberOfEndUserDefinedCharacters);
            Assert.Equal<int?>(null, sp.MaxNumberOfEndUserDefinedCharacters);
            Assert.Equal<int?>(null, sp.MinNumberOfLineBreaks);
            Assert.Equal<int?>(null, sp.MaxNumberOfLineBreaks);
            Assert.Equal<int?>(null, sp.StartOfUnicodeRange);
            Assert.Equal<int?>(null, sp.EndOfUnicodeRange);
            Assert.Equal<int?>(null, sp.MinNumberOfSurrogatePairs);
            Assert.Equal<int?>(null, sp.MaxNumberOfSurrogatePairs);
            Assert.Equal<int?>(null, sp.MinNumberOfTextSegmentationCharacters);
            Assert.Equal<int?>(null, sp.MaxNumberOfTextSegmentationCharacters);
        }
    }
}
