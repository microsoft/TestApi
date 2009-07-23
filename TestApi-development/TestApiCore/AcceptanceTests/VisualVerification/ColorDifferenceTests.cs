// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Test.VisualVerification;
using Xunit;
using Xunit.Extensions;

namespace Microsoft.Test.AcceptanceTests
{
    public class ColorDifferenceTests
    {
        [Theory]
        [InlineData((byte)0,   (byte)0,   (byte)0,   (byte)0)]
        [InlineData((byte)1,   (byte)1,   (byte)1,   (byte)1)]
        [InlineData((byte)20,  (byte)100, (byte)150, (byte)23)]
        [InlineData((byte)255, (byte)255, (byte)255, (byte)255)]
        public void ParameterizedConstructor(byte alpha, byte red, byte green, byte blue)
        {
            ColorDifference cd = new ColorDifference(alpha, red, green, blue);
            Assert.Equal<byte>(alpha, cd.A);
            Assert.Equal<byte>(red, cd.R);
            Assert.Equal<byte>(green, cd.G);
            Assert.Equal<byte>(blue, cd.B);
        }

        [Fact]
        public void DefaultConstructor()
        {
            ColorDifference cd = new ColorDifference();
            Assert.Equal<byte>(0, cd.A);
            Assert.Equal<byte>(0, cd.R);
            Assert.Equal<byte>(0, cd.G);
            Assert.Equal<byte>(0, cd.B);
        }

        [Fact]
        public void Properties()
        {
            ColorDifference cd = new ColorDifference(1, 2, 3, 4);
            cd.A = 15;
            cd.R = 45;
            cd.G = 230;
            cd.B = 112;
            Assert.Equal<byte>(15, cd.A);
            Assert.Equal<byte>(45, cd.R);
            Assert.Equal<byte>(230, cd.G);
            Assert.Equal<byte>(112, cd.B);
        }
    }
}
