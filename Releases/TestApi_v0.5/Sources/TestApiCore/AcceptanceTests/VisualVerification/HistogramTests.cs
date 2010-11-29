// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.IO;
using Microsoft.Test.VisualVerification;
using Xunit;
using Xunit.Extensions;

namespace Microsoft.Test.AcceptanceTests.VisualVerification
{
    // TODO: Need more Histogram tests
    public class HistogramTests
    {
        [Theory]
        [InlineData(@".\Images\Black468x304.bmp")]
        [InlineData(@".\Images\Black468x304.png")]
        [InlineData(@".\Images\White468x304.png")]
        [InlineData(@".\Images\Black468x304WithDefect1Point.png")]
        [InlineData(@".\Images\Black468x304WithDefect2Points.png")]
        public void CreationFromSnapshot(string filePath)
        {
            Snapshot s = Snapshot.FromFile(filePath);
            Histogram h = Histogram.FromSnapshot(s);
            h.ToFile(filePath + ".xml");
            
            
            // TODO: read the resulting XML and confirm that 
            // the saved histogram properly represents the files.
        }
    }
}

