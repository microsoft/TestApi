// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using Microsoft.Test.LeakDetection;
using Xunit;

namespace Microsoft.Test.AcceptanceTests.LeakDetection
{
    public class MemorySnapshotTests : IDisposable
    {
        private Process notepad;

        public MemorySnapshotTests()
        {
            //Launch Notepad application            
            notepad = new Process();
            notepad.StartInfo.FileName = "Notepad.exe";
            notepad.Start();

            notepad.WaitForInputIdle();
        }

        public void Dispose()
        {
            notepad.CloseMainWindow();
            notepad.Close();

            GC.SuppressFinalize(this);
        }

        #region Tests

        [Fact]
        public void TestFromProcess()
        {
            // Generate MemorySnapshot for Notepad.
            MemorySnapshot memorySnapshot = MemorySnapshot.FromProcess(notepad.Id);

            // Generate the oracle for comparison.
            Dictionary<string, long> oracle = GenerateOracle(notepad.Id);

            // Do a comparison of all the properties.
            VerifyProperties(memorySnapshot, oracle);
        }


        [Fact]
        public void TestToFile()
        {
            string filePath = @"C:\testSnap.xml";

            // Store call ToFile to log memorySnapshot to file.
            MemorySnapshot memorySnapshot = MemorySnapshot.FromProcess(notepad.Id);
            memorySnapshot.ToFile(filePath);

            // Generate the oracle for comparison.
            Dictionary<string, long> oracle = GenerateOracle(notepad.Id);

            // Go through xml nodes and compare to data in oracle.
            XmlDocument xmlDoc = new XmlDocument();
            using (Stream s = new FileInfo(filePath).OpenRead())
            {
                try
                {
                    xmlDoc.Load(s);
                }
                catch (XmlException)
                {
                    throw new XmlException("MemorySnapshot file \"" + filePath + "\" could not be loaded.");
                }
            }

            // Grab memory stats.
            Assert.Equal(oracle["GdiObjectCount"], DeserializeNode(xmlDoc, "GdiObjectCount"));
            Assert.Equal(oracle["HandleCount"], DeserializeNode(xmlDoc, "HandleCount"));
            Assert.Equal(oracle["PageFileBytes"], DeserializeNode(xmlDoc, "PageFileBytes"));
            Assert.Equal(oracle["PageFilePeakBytes"], DeserializeNode(xmlDoc, "PageFilePeakBytes"));
            Assert.Equal(oracle["PoolNonpagedBytes"], DeserializeNode(xmlDoc, "PoolNonpagedBytes"));
            Assert.Equal(oracle["PoolPagedBytes"], DeserializeNode(xmlDoc, "PoolPagedBytes"));
            Assert.Equal(oracle["ThreadCount"], DeserializeNode(xmlDoc, "ThreadCount"));
            Assert.Equal(oracle["UserObjectCount"], DeserializeNode(xmlDoc, "UserObjectCount"));
            Assert.Equal(oracle["VirtualMemoryBytes"], DeserializeNode(xmlDoc, "VirtualMemoryBytes"));
            Assert.Equal(oracle["VirtualMemoryPrivateBytes"], DeserializeNode(xmlDoc, "VirtualMemoryPrivateBytes"));
            Assert.Equal(oracle["WorkingSetBytes"], DeserializeNode(xmlDoc, "WorkingSetBytes"));
            Assert.Equal(oracle["WorkingSetPeakBytes"], DeserializeNode(xmlDoc, "WorkingSetPeakBytes"));
            Assert.Equal(oracle["WorkingSetPrivateBytes"], DeserializeNode(xmlDoc, "WorkingSetPrivateBytes"));
        }

        [Fact]
        public void TestFromFile()
        {
            string filePath = @"C:\testSnap.xml";

            // Serialize MemorySnapshot to file.
            MemorySnapshot memorySnapshot = MemorySnapshot.FromProcess(notepad.Id);
            memorySnapshot.ToFile(filePath);

            // Generate the oracle for comparison.
            Dictionary<string, long> oracle = GenerateOracle(notepad.Id);

            // Call from file to load data from file.
            MemorySnapshot fileSnapshot = MemorySnapshot.FromFile(filePath);

            // Compare to data in oracle.
            VerifyProperties(fileSnapshot, oracle);
        }

        [Fact]
        public void TestCompareTo()
        {
            // Generate a memory snapshot.
            MemorySnapshot memorySnapshot = MemorySnapshot.FromProcess(notepad.Id);

            // Generate a second MemorySnapshot and get a diff.
            MemorySnapshot latestSnap = MemorySnapshot.FromProcess(notepad.Id);
            MemorySnapshot diff = latestSnap.CompareTo(memorySnapshot);

            // Manipulate the oracle to contain the same diff.
            Dictionary<string, long> diffOracle = GenerateDiffOracle(memorySnapshot, latestSnap);

            // Compare to verify data.
            VerifyProperties(diff, diffOracle);
        }

        #endregion

        #region Interop Helpers

        internal static PROCESS_MEMORY_COUNTERS_EX GetCounters(IntPtr hProcess)
        {
            PROCESS_MEMORY_COUNTERS_EX counters = new PROCESS_MEMORY_COUNTERS_EX();
            counters.cb = Marshal.SizeOf(counters);
            if (NativeMemoryMethods.GetProcessMemoryInfo(hProcess, out counters, Marshal.SizeOf(counters)) == 0)
            {
                throw new Win32Exception();
            }

            return counters;
        }

        internal static long GetPrivateWorkingSet(Process process)
        {
            SYSTEM_INFO sysinfo = new SYSTEM_INFO();
            NativeMemoryMethods.GetSystemInfo(ref sysinfo);

            int wsInfoLength = (int)(Marshal.SizeOf(new PSAPI_WORKING_SET_INFORMATION()) + 
                                     Marshal.SizeOf(new PSAPI_WORKING_SET_BLOCK()) * (process.WorkingSet64 / (sysinfo.dwPageSize)));
            IntPtr workingSetPointer = Marshal.AllocHGlobal(wsInfoLength);

            if (NativeMemoryMethods.QueryWorkingSet(process.Handle, workingSetPointer, wsInfoLength) == 0)
            {
                throw new Win32Exception();
            }

            PSAPI_WORKING_SET_INFORMATION workingSet = GenerateWorkingSetArray(workingSetPointer);
            Marshal.FreeHGlobal(workingSetPointer);

            return CalculatePrivatePages(workingSet) * sysinfo.dwPageSize;
        }

        // Generates an array containing working set information based on a pointer in memory.
        private static PSAPI_WORKING_SET_INFORMATION GenerateWorkingSetArray(IntPtr workingSetPointer)
        {
            int entries = Marshal.ReadInt32(workingSetPointer);
                        
            PSAPI_WORKING_SET_INFORMATION workingSet = new PSAPI_WORKING_SET_INFORMATION();
            workingSet.NumberOfEntries = entries;
            workingSet.WorkingSetInfo = new PSAPI_WORKING_SET_BLOCK[entries];            


            for (int i = 0; i < entries; i++)
            {
                workingSet.WorkingSetInfo[i].Flags = (uint)Marshal.ReadInt32(workingSetPointer, 4 + i * 4);
            }

            return workingSet;
        }

        // Calculates the number of private pages in memory based on working set information.
        private static int CalculatePrivatePages(PSAPI_WORKING_SET_INFORMATION workingSet)
        {
            int totalPages = workingSet.NumberOfEntries;
            int privatePages = 0;

            for (int i = 0; i < totalPages; i++)
            {
                if (workingSet.WorkingSetInfo[i].Block1.Shared == 0)
                {
                    privatePages++;
                }
            }

            return privatePages;
        }

        #endregion

        #region Private Helpers

        private static long DeserializeNode(XmlDocument xmlDoc, string nodeName)
        {
            XmlNode memoryStatNode = xmlDoc.DocumentElement.SelectSingleNode(nodeName);
            if (memoryStatNode == null)
            {
                throw new XmlException("MemorySnapshot file is missing value: " + nodeName);
            }

            XmlAttribute attribute = memoryStatNode.Attributes["Value"];
            return (long)Convert.ToInt64(attribute.InnerText, NumberFormatInfo.InvariantInfo);
        }

        private Dictionary<string, long> GenerateOracle(int processId)
        {
            Process testProcess = Process.GetProcessById(processId);
            PROCESS_MEMORY_COUNTERS_EX counters = GetCounters(testProcess.Handle);

            // Create oracle with correct data to verify against.
            Dictionary<string, long> oracle = new Dictionary<string, long>();

            oracle.Add("GdiObjectCount", NativeMemoryMethods.GetGuiResources(testProcess.Handle, NativeMemoryMethods.GR_GDIOBJECTS));
            oracle.Add("HandleCount", testProcess.HandleCount);
            oracle.Add("PageFileBytes", counters.PagefileUsage.ToInt64());
            oracle.Add("PageFilePeakBytes", counters.PeakPagefileUsage.ToInt64());
            oracle.Add("PoolNonpagedBytes", counters.QuotaNonPagedPoolUsage.ToInt64());
            oracle.Add("PoolPagedBytes", counters.QuotaPagedPoolUsage.ToInt64());
            oracle.Add("ThreadCount", testProcess.Threads.Count);
            oracle.Add("UserObjectCount", NativeMemoryMethods.GetGuiResources(testProcess.Handle, NativeMemoryMethods.GR_USEROBJECTS));
            oracle.Add("VirtualMemoryBytes", testProcess.VirtualMemorySize64);
            oracle.Add("VirtualMemoryPrivateBytes", counters.PrivateUsage.ToInt64());
            oracle.Add("WorkingSetBytes", testProcess.WorkingSet64);
            oracle.Add("WorkingSetPeakBytes", testProcess.PeakWorkingSet64);
            oracle.Add("WorkingSetPrivateBytes", GetPrivateWorkingSet(testProcess));

            return oracle;
        }

        private Dictionary<string, long> GenerateDiffOracle(MemorySnapshot originalSnapshot, MemorySnapshot latestSnapshot)
        {
            Dictionary<string, long> diffOracle = new Dictionary<string, long>();

            // Create oracle with correct data to verify against.            
            diffOracle.Add("GdiObjectCount", latestSnapshot.GdiObjectCount - originalSnapshot.GdiObjectCount);
            diffOracle.Add("HandleCount", latestSnapshot.HandleCount - originalSnapshot.HandleCount);
            diffOracle.Add("PageFileBytes", latestSnapshot.PageFileBytes.ToInt64() - originalSnapshot.PageFileBytes.ToInt64());
            diffOracle.Add("PageFilePeakBytes", latestSnapshot.PageFilePeakBytes.ToInt64() - originalSnapshot.PageFilePeakBytes.ToInt64());
            diffOracle.Add("PoolNonpagedBytes", latestSnapshot.PoolNonpagedBytes.ToInt64() - originalSnapshot.PoolNonpagedBytes.ToInt64());
            diffOracle.Add("PoolPagedBytes", latestSnapshot.PoolPagedBytes.ToInt64() - originalSnapshot.PoolPagedBytes.ToInt64());
            diffOracle.Add("ThreadCount", latestSnapshot.ThreadCount - originalSnapshot.ThreadCount);
            diffOracle.Add("UserObjectCount", latestSnapshot.UserObjectCount - originalSnapshot.UserObjectCount);
            diffOracle.Add("VirtualMemoryBytes", latestSnapshot.VirtualMemoryBytes - originalSnapshot.VirtualMemoryBytes);
            diffOracle.Add("VirtualMemoryPrivateBytes", latestSnapshot.VirtualMemoryPrivateBytes.ToInt64() - originalSnapshot.VirtualMemoryPrivateBytes.ToInt64());
            diffOracle.Add("WorkingSetBytes", latestSnapshot.WorkingSetBytes - originalSnapshot.WorkingSetBytes);
            diffOracle.Add("WorkingSetPeakBytes", latestSnapshot.WorkingSetPeakBytes - originalSnapshot.WorkingSetPeakBytes);
            diffOracle.Add("WorkingSetPrivateBytes", latestSnapshot.WorkingSetPrivateBytes - originalSnapshot.WorkingSetPrivateBytes);

            return diffOracle;
        }

        private void VerifyProperties(MemorySnapshot memorySnapshot, Dictionary<string, long> oracle)
        {
            Assert.Equal(memorySnapshot.GdiObjectCount, oracle["GdiObjectCount"]);
            Assert.Equal(memorySnapshot.HandleCount, oracle["HandleCount"]);
            Assert.Equal(memorySnapshot.PageFileBytes, new IntPtr(oracle["PageFileBytes"]));
            Assert.Equal(memorySnapshot.PageFilePeakBytes, new IntPtr(oracle["PageFilePeakBytes"]));
            Assert.Equal(memorySnapshot.PoolNonpagedBytes, new IntPtr(oracle["PoolNonpagedBytes"]));
            Assert.Equal(memorySnapshot.PoolPagedBytes, new IntPtr(oracle["PoolPagedBytes"]));
            Assert.Equal(memorySnapshot.ThreadCount, oracle["ThreadCount"]);
            Assert.Equal(memorySnapshot.UserObjectCount, oracle["UserObjectCount"]);
            Assert.Equal(memorySnapshot.VirtualMemoryBytes, oracle["VirtualMemoryBytes"]);
            Assert.Equal(memorySnapshot.WorkingSetBytes, oracle["WorkingSetBytes"]);
            Assert.Equal(memorySnapshot.WorkingSetPeakBytes, oracle["WorkingSetPeakBytes"]);
            Assert.Equal(memorySnapshot.WorkingSetPrivateBytes, oracle["WorkingSetPrivateBytes"]);
        }

        #endregion
    }
}