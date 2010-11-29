using System;
using System.Runtime.InteropServices;

namespace Microsoft.Test
{
    /// <summary>
    /// Native methods
    /// </summary>
    internal static class NativeMethods
    {
        //GDI wrappers cover API's for used for Visual Verification/Screen Capture
        #region GDI APIs
        private const string GdiBinaryFilename = "GDI32.dll";

        /// <summary>
        /// The RasterOperation used by BitBlt and StretchBlt
        /// </summary>
        [FlagsAttribute]
        public enum RasterOperationCodeEnum
        {
            /// <summary/>
            SRCCOPY = 0x00CC0020,
            /// <summary/>
            SRCPAINT = 0x00EE0086,
            /// <summary/>
            SRCAND = 0x008800C6,
            /// <summary/>
            SRCINVERT = 0x00660046,
            /// <summary/>
            SRCERASE = 0x00440328,
            /// <summary/>
            NOTSRCCOPY = 0x00330008,
            /// <summary/>
            NOTSRCERASE = 0x001100A6,
            /// <summary/>
            MERGECOPY = 0x00C000CA,
            /// <summary/>
            MERGEPAINT = 0x00BB0226,
            /// <summary/>
            PATCOPY = 0x00F00021,
            /// <summary/>
            PATPAINT = 0x00FB0A09,
            /// <summary/>
            PATINVERT = 0x005A0049,
            /// <summary/>
            DSTINVERT = 0x00550009,
            /// <summary/>
            BLACKNESS = 0x00000042,
            /// <summary/>
            WHITENESS = 0x00FF0062,
            /// <summary/>
            CAPTUREBLT = 0x40000000
        }

        [DllImportAttribute(GdiBinaryFilename, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static public extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, Int32 RasterOpCode);

        [DllImport(GdiBinaryFilename, SetLastError = true, CharSet = CharSet.Unicode)]
        static public extern IntPtr CreateDC(string driverName, string deviceName, string reserved, IntPtr initData);

        [DllImportAttribute(GdiBinaryFilename, SetLastError = true)]
        static public extern IntPtr CreateCompatibleBitmap(IntPtr hdcSrc, int width, int height);

        [DllImportAttribute(GdiBinaryFilename, SetLastError = true)]
        static public extern IntPtr CreateCompatibleDC(IntPtr HDCSource);

        [DllImportAttribute(GdiBinaryFilename, SetLastError = true)]
        static public extern IntPtr SelectObject(IntPtr HDC, IntPtr hgdiobj);

        [DllImportAttribute(GdiBinaryFilename, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static public extern bool DeleteDC(IntPtr HDC);

        [DllImportAttribute(GdiBinaryFilename, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static public extern bool DeleteObject(IntPtr hBMP);
        #endregion
    }
}
