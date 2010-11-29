// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Test.VisualVerification;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using Xunit;
using Xunit.Extensions;

namespace Microsoft.Test.AcceptanceTests
{
    public class SnapshotTests
    {
        [Theory]
        [InlineData(@"Images\Black468x304.bmp", 255, 0, 0, 0, 468, 304)]
        [InlineData(@"Images\Black468x304.gif", 255, 0, 0, 0, 468, 304)]
        [InlineData(@"Images\Black468x304.jpg", 255, 0, 0, 0, 468, 304)]
        [InlineData(@"Images\Black468x304.png", 255, 0, 0, 0, 468, 304)]
        [InlineData(@"Images\White468x304.jpg", 255, 255, 255, 255, 468, 304)]
        [InlineData(@"Images\White468x304.png", 255, 255, 255, 255, 468, 304)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704")]
        public void CreationFromFile(
            string fileName,
            int a, int r, int g, int b,  // expected color
            int width, int height        // expected size
            )
        {
            Snapshot s = Snapshot.FromFile(fileName);
            Color c = Color.FromArgb(a, r, g, b);

            Assert.True(IsColorOfAllPixelsExpected(s, c));
            Assert.Equal<int>(width, s.Width);
            Assert.Equal<int>(height, s.Height);
        }

        [Fact]
        public void CreationFromTopLevelWindow()
        {
            // Top-level window
            Process p = Process.Start(new ProcessStartInfo("notepad.exe"));
            p.WaitForInputIdle();
            IntPtr hwnd = p.MainWindowHandle;
            Thread.Sleep(1000);

            NativeMethods.RECT wr = new NativeMethods.RECT();
            NativeMethods.RECT cr = new NativeMethods.RECT();
            NativeMethods.GetWindowRect(hwnd, out wr);
            NativeMethods.GetClientRect(hwnd, out cr);

            Snapshot s1 = Snapshot.FromWindow(hwnd, WindowSnapshotMode.IncludeWindowBorder);
            Snapshot s2 = Snapshot.FromWindow(hwnd, WindowSnapshotMode.ExcludeWindowBorder);

            p.CloseMainWindow();
            p.WaitForExit();

            s1.ToFile("TopIncludeBorder.png", ImageFormat.Png);
            s2.ToFile("TopExcludeBorder.png", ImageFormat.Png);

            Assert.Equal<int>(wr.Right - wr.X, s1.Width);
            Assert.Equal<int>(wr.Bottom - wr.Y, s1.Height); 
            Assert.True(s1.Width > s2.Width);
            Assert.True(s1.Height > s2.Height);
        }

        [Fact]
        public void CreationFromChildWindow()
        {
            // Top-level window
            Process p = Process.Start(new ProcessStartInfo("notepad.exe"));
            p.WaitForInputIdle();
            IntPtr hwnd = NativeMethods.GetWindow(p.MainWindowHandle, NativeMethods.GW_CHILD);
            Thread.Sleep(1000);

            NativeMethods.RECT wr = new NativeMethods.RECT();
            NativeMethods.RECT cr = new NativeMethods.RECT();
            NativeMethods.GetWindowRect(hwnd, out wr);
            NativeMethods.GetClientRect(hwnd, out cr);

            Snapshot s1 = Snapshot.FromWindow(hwnd, WindowSnapshotMode.IncludeWindowBorder);
            Snapshot s2 = Snapshot.FromWindow(hwnd, WindowSnapshotMode.ExcludeWindowBorder);

            p.CloseMainWindow();
            p.WaitForExit();

            s1.ToFile("ChildIncludeBorder.png", ImageFormat.Png);
            s2.ToFile("ChildExcludeBorder.png", ImageFormat.Png);

            Assert.Equal<int>(wr.Right - wr.X, s1.Width);
            Assert.Equal<int>(wr.Bottom - wr.Y, s1.Height);
            Assert.Equal<int>(cr.Right - cr.X, s2.Width);
            Assert.Equal<int>(cr.Bottom - cr.Y, s2.Height);

            //
            // Perhaps counter-intuitively, including the border of a child window
            // in the capture results in a larger capture than the one that does
            // not include the border for SOME child windows (those with slidebars)
            //
            Assert.True(s1.Width >= s2.Width);   
            Assert.True(s1.Height >= s2.Height);
        }

        [Theory]
        [InlineData(@"Images\Red468x304.png", 100, 100, 20, 50)]
        [InlineData(@"Images\Red468x304.png", 0, 0, 1, 1)]
        [InlineData(@"Images\Red468x304.png", 0, 0, 468, 304)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704")]
        public void Crop(
            string fileName, 
            int x, int y, int width, int height)
        {
            Snapshot s1 = Snapshot.FromFile(fileName);
            Color c1 = s1[s1.Height/2, s1.Width/2];

            Snapshot s2 = s1.Crop(new Rectangle(x, y, width, height));
            Color c2 = s2[s2.Height/2, s2.Width/2];

            Assert.Equal<int>(width, s2.Width);
            Assert.Equal<int>(height, s2.Height);
            Assert.Equal<Color>(c1, c2);
        }

        [Theory]
        [InlineData(@"Images\Red468x304.png", 10, 100)]
        [InlineData(@"Images\Red468x304.png", 10, 1000)]
        [InlineData(@"Images\Red468x304.png", 500, 1000)]
        [InlineData(@"Images\Red468x304.png", 1, 1)]
        [InlineData(@"Images\Red468x304.png", 0, 0)]
        public void Resize(string fileName, int width, int height)
        {
            Snapshot s1 = Snapshot.FromFile(fileName);
            int originalWidth = s1.Width;
            int originalHeight = s1.Height;

            Snapshot s2 = s1.Resize(new Size(width, height));

            Assert.Equal<int>(originalWidth, s1.Width);
            Assert.Equal<int>(originalHeight, s1.Height);
            Assert.Equal<int>(width, s2.Width);
            Assert.Equal<int>(height, s2.Height);
        }

        [Theory]
        [InlineData(@"Images\Red468x304.png", @"Images\Red468x304.png")]
        [InlineData(@"Images\Red468x304.png", @"Images\Black468x304.png")]
        [InlineData(@"Images\Red468x304.png", @"Images\White468x304.png")]
        // TODO: Add coverage for image and mask with different sizes
        public void Or(string fileName, string fileNameMask)
        {
            Snapshot s = Snapshot.FromFile(fileName);
            Snapshot sOriginal = s.Clone() as Snapshot;

            Snapshot mask = Snapshot.FromFile(fileNameMask);
            s.Or(mask);

            for (int row = 0; row < s.Height; row++)
            {
                for (int col = 0; col < s.Width; col++)
                {
                    Assert.True(s[row, col].A >= sOriginal[row, col].A);
                    Assert.True(s[row, col].R >= sOriginal[row, col].R);
                    Assert.True(s[row, col].G >= sOriginal[row, col].G);
                    Assert.True(s[row, col].B >= sOriginal[row, col].B);
                }
            }
        }

        [Theory]
        [InlineData(@"Images\Red468x304.png", @"Images\Red468x304.png")]
        [InlineData(@"Images\Red468x304.png", @"Images\Black468x304.png")]
        [InlineData(@"Images\Red468x304.png", @"Images\White468x304.png")]
        // TODO: Add coverage for image and mask with different sizes
        public void And(string fileName, string fileNameMask)
        {
            Snapshot s = Snapshot.FromFile(fileName);
            Snapshot mask = Snapshot.FromFile(fileNameMask);
            s.And(mask);

            // TODO: Implement verification
        }

        private void AssertPixelValues(Color expected, Color actual)
        {
            Assert.Equal<Color>(expected, actual);
        }

        private bool IsColorOfAllPixelsExpected(Snapshot snapshot, Color expectedColor)
        {
            for (int row = 0; row < snapshot.Height; row++)
            {
                for (int col = 0; col < snapshot.Width; col++)
                {
                    if (snapshot[row, col] != expectedColor) // bug! row and column are flipped
                    {
                        AssertPixelValues(expectedColor, snapshot[row, col]);
                        return false;
                    }
                }
            }

            return true;
        }
    }
}

