// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Test.CommandLineParsing;
using System;
using System.IO;
using Xunit;
using Xunit.Extensions;

namespace Microsoft.Test.AcceptanceTests
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

