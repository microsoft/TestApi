// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Test.Input;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Xunit;
using Xunit.Extensions;

namespace Microsoft.Test.AcceptanceTests
{
    public class MouseTests
    {
        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(1, 1, 1, 1)]
        [InlineData(100, 1, 100, 1)]
        [InlineData(-12345, -12345, 0, 0)]
        [InlineData(Int16.MinValue, Int16.MinValue, 0, 0)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704")]
        public void Move(int x, int y, int expectedX, int expectedY)
        {
            Mouse.MoveTo(new Point(x, y));

            POINT pActual = new POINT();
            pActual.x = -1000;
            pActual.y = -1000;

            Assert.True(GetCursorPos(ref pActual));
            Assert.Equal<int>(expectedX, pActual.x);
            Assert.Equal<int>(expectedY, pActual.y);
        }

        [Fact]
        public void Click()
        {
            // TBD
        }


        //
        //  Win32 APIs
        //
        [DllImport("user32.dll")]
        internal static extern bool GetCursorPos(ref POINT point);

        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            internal int x;
            internal int y;
        };
    }
}

