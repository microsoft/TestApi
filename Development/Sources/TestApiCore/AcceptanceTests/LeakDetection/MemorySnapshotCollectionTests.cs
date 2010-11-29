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
using System.Threading;
using System.Xml;
using Microsoft.Test.LeakDetection;
using Xunit;
using Xunit.Extensions;

namespace Microsoft.Test.AcceptanceTests.LeakDetection
{
    public class MemorySnapshotCollectionTests : IDisposable
    {
        private Process notepad;

        public MemorySnapshotCollectionTests()
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
        public void TestToFromFile()
        {
            string filePath = @".\TestSnapCollection.xml";
            
            MemorySnapshotCollection collection = new MemorySnapshotCollection();           

            // The following is for testing purposes and not representative use of the MemorySnapshotCollection class.
            MemorySnapshot ms1 = MemorySnapshot.FromProcess(notepad.Id);
            collection.Add(ms1);
            MemorySnapshot ms2 = MemorySnapshot.FromProcess(notepad.Id);
            collection.Add(ms2);
            MemorySnapshot ms3 = MemorySnapshot.FromProcess(notepad.Id);
            collection.Add(ms3);            

            // Serialize MemorySnapshot to file.
            collection.ToFile(filePath);

            // Call from file to load data from file.
            MemorySnapshotCollection fileCollection = MemorySnapshotCollection.FromFile(filePath);
                        
            // Verify Count.
            Assert.Equal(fileCollection.Count, collection.Count);

            // Generate Diffs for comparison.
            MemorySnapshot diff1 = ms1.CompareTo(fileCollection[0]);
            MemorySnapshot diff2 = ms2.CompareTo(fileCollection[1]);
            MemorySnapshot diff3 = ms3.CompareTo(fileCollection[2]);

            // Generate expected Diff results.
            Dictionary<string, long> diffOracle = GenerateDiffOracle();

            // Verify Diffs are as expected.
            VerifyDiff(diff1, diffOracle);
            VerifyDiff(diff2, diffOracle);
            VerifyDiff(diff3, diffOracle);
        }        

        #endregion

        #region Private Helpers

        private Dictionary<string, long> GenerateDiffOracle()
        {
            Dictionary<string, long> diffOracle = new Dictionary<string, long>();

            // Create oracle with correct data to verify against.            
            diffOracle.Add("GdiObjectCount", 0);
            diffOracle.Add("HandleCount", 0);
            diffOracle.Add("PageFileBytes", 0);
            diffOracle.Add("PageFilePeakBytes", 0);
            diffOracle.Add("PoolNonpagedBytes", 0);
            diffOracle.Add("PoolPagedBytes", 0);
            diffOracle.Add("ThreadCount", 0);
            diffOracle.Add("UserObjectCount", 0);
            diffOracle.Add("VirtualMemoryBytes", 0);            
            diffOracle.Add("VirtualMemoryPrivateBytes", 0);
            diffOracle.Add("WorkingSetBytes", 0);
            diffOracle.Add("WorkingSetPeakBytes", 0);
            diffOracle.Add("WorkingSetPrivateBytes", 0);
            
            return diffOracle;
        }

        private void VerifyDiff(MemorySnapshot memorySnapshot, Dictionary<string, long> oracle)
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