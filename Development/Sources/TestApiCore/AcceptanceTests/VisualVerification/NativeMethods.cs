// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Test.AcceptanceTests.VisualVerification
{
    internal static class NativeMethods
    {
        /// <summary>
        /// WIN32 RECT structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            internal int X;
            internal int Y;
            internal int Right;
            internal int Bottom;
        }
        
        [DllImport("user32.dll", SetLastError = true)]
        static internal extern bool GetClientRect(IntPtr hwnd, out RECT rect);

        [DllImport("user32.dll", SetLastError = true)]
        static internal extern bool GetWindowRect(IntPtr hwnd, out RECT rect);

        [DllImport("user32.dll", SetLastError = true)]
        static internal extern IntPtr GetWindow(IntPtr hwnd, uint option);

        internal const uint GW_CHILD = 5;
    }
}

