// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.Test.Input;
using Xunit;
using Xunit.Extensions;
using System.Diagnostics;

namespace Microsoft.Test.AcceptanceTests.Input
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

/*
        [Theory]
        [InlineData(0, 0)]
        [InlineData(200, 100)]
        [InlineData(-200, -100)]
        public void DragTo(int xDelta, int yDelta)
        {
            // Bring up Notepad. Record its location and size
            // Calculate the position of the center of its title bar
            // Move the mouse to the center of its title bar

            Process p = Process.Start("notepad.exe");
            p.WaitForInputIdle();

            RECT rect = new RECT();
            GetWindowRect(p.MainWindowHandle, ref rect);

            POINT startOfDrag = new POINT();
            startOfDrag.x = rect.left + (rect.right - rect.left) / 2;
            startOfDrag.y = rect.top + 15; // the size of the title bar is about 30 pixels. We want to grab the window in the middle of the title bar


            // Calculate expected end location
            // DragTo the specified location

            POINT endOfDrag = new POINT();
            endOfDrag.x = startOfDrag.x + xDelta;
            endOfDrag.y = startOfDrag.y + yDelta;

            RECT newExpectedLocation = new RECT();
            newExpectedLocation.left = rect.left + (endOfDrag.x - startOfDrag.x);
            newExpectedLocation.top = rect.top + (endOfDrag.y - startOfDrag.y);


            Mouse.MoveTo(new Point(startOfDrag.x, startOfDrag.y));
            Mouse.DragTo(MouseButton.Left, new Point(endOfDrag.x, endOfDrag.y));

            // Confirm the new location is what we expected.
            RECT newLocation = new RECT();
            GetWindowRect(p.MainWindowHandle, ref newLocation);
            Assert.Equal<int>(newExpectedLocation.left, newLocation.left);
            Assert.Equal<int>(newExpectedLocation.right, newLocation.right);
            Assert.Equal<int>(newExpectedLocation.top, newLocation.top);
            Assert.Equal<int>(newExpectedLocation.bottom, newLocation.bottom);

            p.CloseMainWindow();
            p.Close();
            p.WaitForExit();
        }
*/

        //
        //  Win32 APIs
        //
        [DllImport("user32.dll")]
        internal static extern bool GetCursorPos(ref POINT point);

        [DllImport("user32.dll")]
        internal static extern bool GetWindowRect(IntPtr hwnd, ref RECT rect);

        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            internal int x;
            internal int y;
        };
        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            internal int left;
            internal int top;
            internal int right;
            internal int bottom;
        };
    }
}

