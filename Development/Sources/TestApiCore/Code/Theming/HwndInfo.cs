using System;

namespace Microsoft.Test.Theming
{
    internal struct HwndInfo
    {
        public HwndInfo(IntPtr hWnd)
        {
            this.hWnd = hWnd;
            NativeMethods.GetWindowThreadProcessId(hWnd, out ProcessId);
        }

        /// <summary>
        /// Hwnd
        /// </summary>
        public IntPtr hWnd;

        /// <summary>
        /// Process ID of Hwnd
        /// </summary>
        public int ProcessId;

        
    }
}
