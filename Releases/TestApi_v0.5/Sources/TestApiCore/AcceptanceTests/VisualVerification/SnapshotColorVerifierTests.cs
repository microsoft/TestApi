// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Drawing;
using System.Globalization;
using Microsoft.Test.VisualVerification;
using Xunit;
using Xunit.Extensions;

namespace Microsoft.Test.AcceptanceTests.VisualVerification
{
    public class SnapshotColorVerifierTests
    {
        [Fact]
        public void DefaultConstructor()
        {
            SnapshotColorVerifier v = new SnapshotColorVerifier();

            Assert.Equal<byte>(Color.Black.A, v.ExpectedColor.A);
            Assert.Equal<byte>(Color.Black.R, v.ExpectedColor.R);
            Assert.Equal<byte>(Color.Black.G, v.ExpectedColor.G);
            Assert.Equal<byte>(Color.Black.B, v.ExpectedColor.B);
            Assert.Equal<byte>(0, v.Tolerance.A);
            Assert.Equal<byte>(0, v.Tolerance.R);
            Assert.Equal<byte>(0, v.Tolerance.G);
            Assert.Equal<byte>(0, v.Tolerance.B);
        }

        [Theory]
        [InlineData("00000000", "00000000")]
        [InlineData("FFFFFFFF", "FFFFFFFF")]
        [InlineData("33333333", "BBBBBBBB")]
        // TODO: Add more
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720")]
        public void ParameterizedConstructor(string expectedColorString, string toleranceString)
        {
            int expectedColor = Int32.Parse(expectedColorString, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            int tolerance = Int32.Parse(toleranceString, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);

            Color c = Color.FromArgb((int)expectedColor);
            byte ta = (byte)(tolerance & 0xFF000000 >> 128);
            byte tr = (byte)(tolerance & 0x00FF0000 >> 32);
            byte tg = (byte)(tolerance & 0x0000FF00 >> 16);
            byte tb = (byte)(tolerance & 0x000000FF >> 0);
            ColorDifference t = new ColorDifference(ta, tr, tg, tb);

            SnapshotColorVerifier v = new SnapshotColorVerifier(c, t);

            Assert.Equal<byte>(c.A, v.ExpectedColor.A);
            Assert.Equal<byte>(c.R, v.ExpectedColor.R);
            Assert.Equal<byte>(c.G, v.ExpectedColor.G);
            Assert.Equal<byte>(c.B, v.ExpectedColor.B);
            Assert.Equal<byte>(t.A, v.Tolerance.A);
            Assert.Equal<byte>(t.R, v.Tolerance.R);
            Assert.Equal<byte>(t.G, v.Tolerance.G);
            Assert.Equal<byte>(t.B, v.Tolerance.B);
        }
    }
}

