// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.IO;
using Microsoft.Test.CommandLineParsing;
using Xunit;
using Xunit.Extensions;

namespace Microsoft.Test.AcceptanceTests.CommandLineParsing
{
    public class DirectoryInfoConverterTests
    {
        [Theory]
        [InlineData(@"c:\foo.text")]
        [InlineData(@"\\somemachine\somedir\somefile.text")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720")]
        public void BasicUsage(string directoryString)
        {
            DirectoryInfoConverter c = new DirectoryInfoConverter();
            DirectoryInfo di = c.ConvertFromString(directoryString) as DirectoryInfo;

            Assert.Equal<string>(directoryString, di.FullName);
        }
    }
}

