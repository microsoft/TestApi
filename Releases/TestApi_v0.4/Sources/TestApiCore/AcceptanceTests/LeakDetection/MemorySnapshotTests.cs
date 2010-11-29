// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Test.LeakDetection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml;
using Xunit;
using Xunit.Extensions;

namespace Microsoft.Test.AcceptanceTests
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

        private const int GR_GDIOBJECTS = 0;
        private const int GR_USEROBJECTS = 1;

        private static PROCESS_MEMORY_COUNTERS_EX GetCounters(IntPtr hProcess)
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
        private static extern int GetGuiResources(IntPtr hProcess, int flags);

        // Interop call the get performance memory counters
        [DllImport("psapi.dll", SetLastError = true)]
        private static extern int GetProcessMemoryInfo(IntPtr hProcess, out PROCESS_MEMORY_COUNTERS_EX counters, int size);

        // Struct to hold performace memory counters.
        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_MEMORY_COUNTERS_EX
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

            oracle.Add("GdiObjectCount", GetGuiResources(testProcess.Handle, GR_GDIOBJECTS));
            oracle.Add("HandleCount", testProcess.HandleCount);
            oracle.Add("PageFileBytes", counters.PagefileUsage);
            oracle.Add("PageFilePeakBytes", counters.PeakPagefileUsage);
            oracle.Add("PoolNonpagedBytes", counters.QuotaNonPagedPoolUsage);
            oracle.Add("PoolPagedBytes", counters.QuotaPagedPoolUsage);
            oracle.Add("ThreadCount", testProcess.Threads.Count);
            oracle.Add("UserObjectCount", GetGuiResources(testProcess.Handle, GR_USEROBJECTS));
            oracle.Add("VirtualMemoryBytes", testProcess.VirtualMemorySize64);            
            oracle.Add("VirtualMemoryPrivateBytes", counters.PrivateUsage);
            oracle.Add("WorkingSetBytes", testProcess.WorkingSet64);
            oracle.Add("WorkingSetPeakBytes", testProcess.PeakWorkingSet64);
            oracle.Add("WorkingSetPrivateBytes", testProcess.PrivateMemorySize64);            

            return oracle;
        }

        private Dictionary<string, long> GenerateDiffOracle(MemorySnapshot originalSnapshot, MemorySnapshot latestSnapshot)
        {       
            Dictionary<string, long> diffOracle = new Dictionary<string, long>();            

            // Create oracle with correct data to verify against.            
            diffOracle.Add("GdiObjectCount", latestSnapshot.GdiObjectCount - originalSnapshot.GdiObjectCount);
            diffOracle.Add("HandleCount", latestSnapshot.HandleCount - originalSnapshot.HandleCount);
            diffOracle.Add("PageFileBytes", latestSnapshot.PageFileBytes - originalSnapshot.PageFileBytes);
            diffOracle.Add("PageFilePeakBytes", latestSnapshot.PageFilePeakBytes - originalSnapshot.PageFilePeakBytes);
            diffOracle.Add("PoolNonpagedBytes", latestSnapshot.PoolNonpagedBytes - originalSnapshot.PoolNonpagedBytes);
            diffOracle.Add("PoolPagedBytes", latestSnapshot.PoolPagedBytes - originalSnapshot.PoolPagedBytes);
            diffOracle.Add("ThreadCount", latestSnapshot.ThreadCount - originalSnapshot.ThreadCount);
            diffOracle.Add("UserObjectCount", latestSnapshot.UserObjectCount - originalSnapshot.UserObjectCount);
            diffOracle.Add("VirtualMemoryBytes", latestSnapshot.VirtualMemoryBytes - originalSnapshot.VirtualMemoryBytes);            
            diffOracle.Add("VirtualMemoryPrivateBytes", latestSnapshot.VirtualMemoryPrivateBytes - originalSnapshot.VirtualMemoryPrivateBytes);
            diffOracle.Add("WorkingSetBytes", latestSnapshot.WorkingSetBytes - originalSnapshot.WorkingSetBytes);
            diffOracle.Add("WorkingSetPeakBytes", latestSnapshot.WorkingSetPeakBytes - originalSnapshot.WorkingSetPeakBytes);
            diffOracle.Add("WorkingSetPrivateBytes", latestSnapshot.WorkingSetPrivateBytes - originalSnapshot.WorkingSetPrivateBytes);            

            return diffOracle;
        }

        private void VerifyProperties(MemorySnapshot memorySnapshot, Dictionary<string, long> oracle)
        {
            Assert.Equal(memorySnapshot.GdiObjectCount, oracle["GdiObjectCount"]);
            Assert.Equal(memorySnapshot.HandleCount, oracle["HandleCount"]);
            Assert.Equal(memorySnapshot.PageFileBytes, oracle["PageFileBytes"]);
            Assert.Equal(memorySnapshot.PageFilePeakBytes, oracle["PageFilePeakBytes"]);
            Assert.Equal(memorySnapshot.PoolNonpagedBytes, oracle["PoolNonpagedBytes"]);
            Assert.Equal(memorySnapshot.PoolPagedBytes, oracle["PoolPagedBytes"]);
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