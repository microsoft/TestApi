// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

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
