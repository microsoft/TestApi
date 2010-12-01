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
    public class FileInfoConverterTests
    {
        [Theory]
        [InlineData(@"c:\foo.text")]
        [InlineData(@"\\somemachine\somedir\somefile.text")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720")]
        public void BasicUsage(string fileString)
        {
            FileInfoConverter c = new FileInfoConverter();
            FileInfo fi = c.ConvertFromString(fileString) as FileInfo;

            Assert.Equal<string>(fileString, fi.FullName);
        }
    }
}

