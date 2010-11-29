// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

namespace Microsoft.Test.LeakDetection
{   
    internal static class MemoryInterop
    {
        internal const int GR_GDIOBJECTS = 0;
        internal const int GR_USEROBJECTS = 1;

        internal static PROCESS_MEMORY_COUNTERS_EX GetCounters(IntPtr hProcess)
        {
            PROCESS_MEMORY_COUNTERS_EX counters = new PROCESS_MEMORY_COUNTERS_EX();
            counters.cb = Marshal.SizeOf(counters);
            if (GetProcessMemoryInfo(hProcess, out counters, Marshal.SizeOf(counters)) == 0)
            {
                throw new Win32Exception();
            }

            return counters;
        }

        // flags: 0 - Count of GDI objects
        // flags: 1 - Count of USER objects                
        [DllImport("User32.dll")]
        internal static extern int GetGuiResources(IntPtr hProcess, int flags);

        // Interop call the get performance memory counters
        [DllImport("psapi.dll", SetLastError = true)]
        static extern int GetProcessMemoryInfo(IntPtr hProcess, out PROCESS_MEMORY_COUNTERS_EX counters, int size);

        // Struct to hold performace memory counters.
        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_MEMORY_COUNTERS_EX
        {
            public int cb;
            public int PageFaultCount;
            public int PeakWorkingSetSize;
            public int WorkingSetSize;
            public int QuotaPeakPagedPoolUsage;
            public int QuotaPagedPoolUsage;
            public int QuotaPeakNonPagedPoolUsage;
            public int QuotaNonPagedPoolUsage;
            public int PagefileUsage;
            public int PeakPagefileUsage;
            public int PrivateUsage;
        }        
    }
}