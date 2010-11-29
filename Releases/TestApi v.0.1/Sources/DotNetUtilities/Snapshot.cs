using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Microsoft.Test
{
    /// <summary>
    /// Represents image pixels in a two-dimensional array for use in visual verification.
    /// Every element of the array represents a pixel at the given [row, column] of the image.
    /// A Snapshot object can be instantiated from a file or captured from the screen.
    /// </summary>
    ///
    /// <example>Takes a snapshot and verifies it is an absolute match to an expected image.
    /// <code>
    /**
        // Take a snapshot, compare to master image, validate match and save the diff
        // in case of a poor match.
        Snapshot actual = Snapshot.FromRectangle(new Rectangle(0, 0, 800, 600));
        Snapshot expected = Snapshot.FromFile("Expected.bmp");
        Snapshot diff = actual.CompareTo(expected);
     
        // The SnapshotColorVerifier.Verify() method compares every pixel of a diff bitmap 
        // against the threshold defined by the ColorDifference tolerance. If all pixels
        // fall within the tolerance, then the method returns VerificationResult.Pass
        SnapshotVerifier v = new SnapshotColorVerifier(Color.Black, new ColorDifference(0, 0, 0, 0));
        if (v.Verify(diff) == VerificationResult.Fail)
        {
            diff.ToFile("Actual.bmp", ImageFormat.Bmp);
        }
    */
    /// </code>
    /// </example>
    public class Snapshot
    {
        #region Constructor

        /// <summary>
        /// Snapshot Constructor - Creates buffer of black, opaque pixels 
        /// </summary>
        private Snapshot(int height, int width)
        {
            buffer = new Color[height, width];
        }

        #endregion

        #region Public Static Initializers

        /// <summary>
        /// Creates a Snapshot instance from data in the specified image file.
        /// </summary>
        /// <param name="filePath">Path to the image file.</param>
        /// <returns>A Snapshot instance containing the pixels in the loaded file.</returns>
        public static Snapshot FromFile(string filePath)
        {
            // Note: open the stream directly because the underlying Bitmap class does not consistently throw on access failures.
            using (Stream s = new FileInfo(filePath).OpenRead())
            {
                // Load the bitmap from disk. NOTE: imageFormat argument is not used in Bitmap.
                // Then convert to Snapshot format

                Bitmap bmp = new System.Drawing.Bitmap(s);
                return CreateSnapshotFromBitmap(bmp);
            }
        }

        /// <summary>
        /// Creates a Snapshot instance populated with pixels sampled from the specified screen rectangle.
        /// </summary>
        /// <param name="rectangle">Rectangle of the screen region to be sampled from.</param>
        /// <returns>A Snapshot instance of the pixels from the bounds of the screen rectangle.</returns>
        public static Snapshot FromRectangle(Rectangle rectangle)
        {
            return CreateSnapshotFromBitmap(CaptureBitmap(rectangle));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Compares the current Snapshot instance to the specified Snapshot to produce a difference image. 
        /// Note: This does not compare alpha channels.
        /// </summary>
        /// <param name="snapshot">The Snapshot to be compared to.</param>
        /// <returns>A new Snapshot object representing the difference image (i.e. the result of the comparison).</returns>
        public Snapshot CompareTo(Snapshot snapshot)
        {
            return CompareTo(snapshot, false);
        }

        /// <summary>
        /// Compares the current Snapshot instance to the specified Snapshot to produce a difference image. 
        /// </summary>
        /// <param name="snapshot">The target Snapshot to be compared to.</param>
        /// <param name="compareAlphaChannel">If true, compares alpha channels. If false, the alpha channel difference values are fixed to 255.</param>
        /// <returns>A new Snapshot object representing the difference image (i.e. the result of the comparison).</returns>
        public Snapshot CompareTo(Snapshot snapshot, bool compareAlphaChannel)
        {
            if (this.Width != snapshot.Width || this.Height != snapshot.Height)
            {
                throw new InvalidOperationException("Snapshots must be of identical size to be compared.");
            }

            Snapshot result = new Snapshot(snapshot.Height, snapshot.Width);
            for (int column = 0; column < result.Width; column++)
            {
                for (int row = 0; row < result.Height; row++)
                {
                    result[row, column] = this[row, column].SubtractColors(snapshot[row, column], compareAlphaChannel);
                }
            }
            return result;
        }

        /// <summary>
        /// Writes the current Snapshot (at 32 bits per pixel) to a file.
        /// </summary>
        /// <param name="filePath">The path to the output file.</param>
        /// <param name="imageFormat">The file storage format to be used.</param>
        public void ToFile(string filePath, ImageFormat imageFormat)
        {
            Bitmap temp = CreateBitmapFromSnapshot();

            using (Stream s = new FileInfo(filePath).OpenWrite())
            {
                temp.Save(s, imageFormat);
            }
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Returns a Color instance for the pixel at the specified row and column.
        /// </summary>
        /// <param name="row">Zero-based row position of the pixel.</param>
        /// <param name="column">Zero-based column position of the pixel.</param>
        /// <returns>A Color instance for the pixel at the specified row and column.</returns>
        // Suppressed the warning as our usage is more obvious than single argument indexer for a 2D array
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1023")]
        public Color this[int row, int column]
        {
            get
            {
                IsValidPixel(row, column);
                return buffer[row, column];
            }
            set
            {
                IsValidPixel(row, column);
                buffer[row, column] = value;
            }
        }

        /// <summary>
        /// Returns the width of the pixel buffer.
        /// </summary>
        public int Width
        {
            get
            {
                return buffer.GetLength(1);
            }
        }

        /// <summary>
        /// Returns the height of the pixel buffer.
        /// </summary>
        public int Height
        {
            get
            {
                return buffer.GetLength(0);
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Instantiates a Bitmap with contents of TestBitmap.
        /// </summary>
        /// <returns>A Bitmap containing the a copy of the Snapshot buffer data.</returns>
        unsafe private System.Drawing.Bitmap CreateBitmapFromSnapshot()
        {
            System.Drawing.Bitmap temp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

            Rectangle bounds = new Rectangle(Point.Empty, temp.Size);
            BitmapData bitmapData = temp.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            Byte* pBase = (Byte*)bitmapData.Scan0.ToPointer();

            for (int row = 0; row < Height; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    // The active implementation is the faster, but unsafe alternative to setpixel API:
                    //    temp.SetPixel(column,row,buffer[row, column]);

                    PixelData* pixelDataAddress = (PixelData*)(pBase + row * Width * sizeof(PixelData) + column * sizeof(PixelData));
                    pixelDataAddress->A = buffer[row, column].A;
                    pixelDataAddress->R = buffer[row, column].R;
                    pixelDataAddress->G = buffer[row, column].G;
                    pixelDataAddress->B = buffer[row, column].B;
                }
            }
            temp.UnlockBits(bitmapData);
            return temp;
        }

        /// <summary>
        /// Captures a Bitmap from a rectangle region.
        /// </summary>
        /// <param name="rectangle">The rectangular bounds to be captured.</param>
        /// <returns>A Bitmap representation of the region specified.</returns>
        static private Bitmap CaptureBitmap(Rectangle rectangle)
        {
            //Empty rectangle semantic is undefined.
            if (rectangle.IsEmpty)
            {
                throw new ArgumentOutOfRangeException("rectangle");
            }

            System.Drawing.Bitmap capturedBitmap = null;
            IntPtr handleToSourceDeviceContext = IntPtr.Zero;
            try
            {
                handleToSourceDeviceContext = NativeMethods.CreateDC("DISPLAY", string.Empty, string.Empty, IntPtr.Zero);
                if (handleToSourceDeviceContext == IntPtr.Zero) { throw new System.ComponentModel.Win32Exception(); }

                capturedBitmap = CreateBitmapFromDeviceContext(handleToSourceDeviceContext, rectangle);
            }
            finally
            {
                if (handleToSourceDeviceContext != IntPtr.Zero)
                {
                    NativeMethods.DeleteDC(handleToSourceDeviceContext);
                    handleToSourceDeviceContext = IntPtr.Zero;
                }
            }
            return capturedBitmap;
        }

        /// <summary>
        /// Instantiates a Snapshot representation from a Windows Bitmap.
        /// </summary>
        /// <param name="source">Source bitmap to be converted.</param>
        /// <returns>A snapshot based on the source buffer.</returns>
        static private Snapshot CreateSnapshotFromBitmap(System.Drawing.Bitmap source)
        {
            Snapshot bmp = new Snapshot(source.Height, source.Width);
            for (int row = 0; row < source.Height; row++)
            {
                for (int column = 0; column < source.Width; column++)
                {
                    //TODO: Replace GetPixel usage with more performant direct buffer access.
                    bmp[row, column] = source.GetPixel(column, row);
                }
            }
            return bmp;
        }

        /// <summary>
        /// Captures a Bitmap from the deviceContext on the specified areaToCopy.
        /// </summary>
        /// <param name="handleDeviceContext">The device context to capture from</param>
        /// <param name="areaToCopy">The Rectangle area to copy.</param>
        /// <returns>A bitmap based on the specified inputs.</returns>
        static private System.Drawing.Bitmap CreateBitmapFromDeviceContext(IntPtr handleDeviceContext, Rectangle areaToCopy)
        {
            IntPtr handleDeviceContextSrc = IntPtr.Zero;
            IntPtr handleDeviceContextDestination = IntPtr.Zero;
            IntPtr handleBmp = IntPtr.Zero;
            IntPtr handlePreviousObj = IntPtr.Zero;
            System.Drawing.Bitmap bmp = null;

            try
            {
                handleDeviceContextSrc = handleDeviceContext;

                // Allocate memory for the bitmap
                handleBmp = NativeMethods.CreateCompatibleBitmap(handleDeviceContextSrc, areaToCopy.Width, areaToCopy.Height);
                if (handleBmp == IntPtr.Zero) { throw new System.ComponentModel.Win32Exception(); }

                // Create destination DC
                handleDeviceContextDestination = NativeMethods.CreateCompatibleDC(handleDeviceContextSrc);
                if (handleDeviceContextDestination == IntPtr.Zero) { throw new System.ComponentModel.Win32Exception(); }

                // copy screen to bitmap
                handlePreviousObj = NativeMethods.SelectObject(handleDeviceContextDestination, handleBmp);
                if (handlePreviousObj == IntPtr.Zero) { throw new System.ComponentModel.Win32Exception(); }

                // Note : CAPTUREBLT is needed to capture layered windows
                bool result = NativeMethods.BitBlt(handleDeviceContextDestination, 0, 0, areaToCopy.Width, areaToCopy.Height, handleDeviceContextSrc, areaToCopy.Left, areaToCopy.Top, (Int32)(NativeMethods.RasterOperationCodeEnum.SRCCOPY | NativeMethods.RasterOperationCodeEnum.CAPTUREBLT));
                if (result == false) { throw new System.ComponentModel.Win32Exception(); }

                //Convert Win32 Handle to Bitmap to a Winforms Bitmap
                bmp = Bitmap.FromHbitmap(handleBmp);
            }

            // Do Unmanaged cleanup
            finally
            {
                if (handlePreviousObj != IntPtr.Zero)
                {
                    NativeMethods.SelectObject(handleDeviceContextDestination, handlePreviousObj);
                    handlePreviousObj = IntPtr.Zero;
                }

                if (handleDeviceContextSrc != IntPtr.Zero)
                {
                    NativeMethods.DeleteDC(handleDeviceContextSrc);
                    handleDeviceContextSrc = IntPtr.Zero;
                }

                if (handleDeviceContextDestination != IntPtr.Zero)
                {
                    NativeMethods.DeleteDC(handleDeviceContextDestination);
                    handleDeviceContextDestination = IntPtr.Zero;
                }

                if (handleBmp != IntPtr.Zero)
                {
                    NativeMethods.DeleteObject(handleBmp);
                    handleBmp = IntPtr.Zero;
                }
            }
            return bmp;
        }

        /// <summary>
        /// Tests if the specified pixel coordinate is contained within the bounds of the buffer.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        private void IsValidPixel(int row, int column)
        {
            if (row < 0 || row >= Height)
            {
                throw new ArgumentOutOfRangeException("row");
            }

            if (column < 0 || column >= Width)
            {
                throw new ArgumentOutOfRangeException("column");
            }
        }
        #endregion

        #region Private Fields and Structures
        /// <summary>
        /// A BGRA pixel data structure - This is only used for data conversion and export purposes for conversion with Bitmap buffer. 
        /// NOTE: This order aligns with 32 bpp ARGB pixel Format.
        /// </summary>
        private struct PixelData
        {
            public byte B;
            public byte G;
            public byte R;
            public byte A;
        }

        /// <summary>
        /// The color buffer is organized in row-Major form i.e. [row, column] => [y,x]
        /// </summary>
        private Color[,] buffer;

        #endregion
    }
}
