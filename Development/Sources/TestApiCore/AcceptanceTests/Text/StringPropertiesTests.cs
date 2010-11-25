// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Test.Text;
using Xunit;
using Xunit.Extensions;

namespace Microsoft.Test.AcceptanceTests.Text
{
    public class StringPropertiesTests
    {
        [Fact]
        public void AllPropertiesAreNullOrEmptyUponObjectInstantiation()
        {
            StringProperties sp = new StringProperties();

            Assert.True(sp.UnicodeRanges.Count == 0);
            Assert.Equal<int?>(null, sp.MinNumberOfCombiningMarks);
            Assert.Equal<bool?>(null, sp.HasNumbers);
            Assert.Equal<bool?>(null, sp.IsBidirectional);
            Assert.Equal<NormalizationForm?>(null, sp.NormalizationForm);
            Assert.Equal<int?>(null, sp.MinNumberOfCodePoints);
            Assert.Equal<int?>(null, sp.MaxNumberOfCodePoints);
            Assert.Equal<int?>(null, sp.MinNumberOfEndUserDefinedCodePoints);
            Assert.Equal<int?>(null, sp.MinNumberOfLineBreaks);
            Assert.Equal<int?>(null, sp.MinNumberOfSurrogatePairs);
            Assert.Equal<int?>(null, sp.MinNumberOfTextSegmentationCodePoints);
        }
    }
}
