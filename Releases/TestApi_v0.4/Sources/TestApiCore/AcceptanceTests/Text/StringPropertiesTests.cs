// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Test.Text;
using System;
using System.Collections.Generic;
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

            Assert.True(sp.UnicodeRange == null);
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
