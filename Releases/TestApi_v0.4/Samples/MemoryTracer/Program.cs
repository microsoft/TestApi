// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using Microsoft.Test.CommandLineParsing;
using Microsoft.Test.LeakDetection;

namespace MemoryTracer
{
    class Program
    {
        static bool exitLoop = false;

        public static void Main(string[] args)
        {
            CommandLineArguments a = Init(args);

            if (a == null)
            {
                PrintUsage();
                return;
            }

            if (a.ProcessName != null)
            {
                Process[] processlist = Process.GetProcesses();
                int processFound = 0;

                foreach(Process p in processlist)
                {
                    if (string.Equals(a.ProcessName, p.ProcessName, StringComparison.OrdinalIgnoreCase))
                    {
                        processFound++;
                        a.Pid = p.Id;                        
                    }                    
                }

                if (processFound == 0)
                {
                    Console.WriteLine("ProcessName not found. Starting new instance...");
                    Process p = new Process();
                    p.StartInfo.FileName = a.ProcessName;
                    p.Start();
                    a.Pid = p.Id;
                    p.WaitForInputIdle();
                }

                if (processFound > 1)
                {
                    Console.WriteLine("Found multiple processes with the same name. Please specify a /pid.");
                    return;                    
                }
            }
                        
            Console.WriteLine("Hit \"Ctrl^C\" to exit.");
            Console.WriteLine("Attaching to Process ID: " + a.Pid);
            Console.WriteLine("Generating memory snapshots...");

            Console.CancelKeyPress += new ConsoleCancelEventHandler(ConsoleOnCancelKeyPress);
            MemorySnapshotCollection msc = new MemorySnapshotCollection();

            if (a.InitialDelay != null)
            {
                Thread.Sleep(a.InitialDelay.Value);
            }

            PrintHeader();
            MemorySnapshot ms;

            while (true)
            {
                if (exitLoop == true)
                {
                    break;
                }

                try
                {
                    ms = MemorySnapshot.FromProcess(a.Pid.Value);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("Process no longer avilable. Terminating MemoryTracer...");
                    return;
                }


                PrintToConsole(ms);
                if (a.SaveTo != null)
                {
                    msc.Add(ms);
                }

                Thread.Sleep(a.Interval.Value);
            }

            if (a.SaveTo != null)
            {
                msc.ToFile(a.SaveTo);
                Console.WriteLine("MemorySnapshots saved to: " + a.SaveTo);
            }
        }

        public static CommandLineArguments Init(string[] args)
        {
            CommandLineDictionary d = CommandLineDictionary.FromArguments(args);
            if (d == null || d.ContainsKey("?") || d.ContainsKey("help"))
            {
                return null;
            }

            CommandLineArguments a = new CommandLineArguments();
            a.ParseArguments(args);

            if (a.Interval == null)
            {
                return null;
            }

            if (a.Pid == null && a.ProcessName == null)
            {                
                return null;
            }            

            return a;
        }

        private const string format = "{0,22} {1,12} {2,15} {3,16} {4,8} {5,14} {6,18} {7,18} {8,15} {9,19} {10,26} {11,16} {12,20} {13,23}";

        public static void PrintHeader()
        {
            Console.WriteLine(
                String.Format(
                    format,
                    "Timestamp\t",
                    "HandleCount\t", "GdiObjectCount\t", "UserObjectCount\t", "Threads\t",
                    "PageFileBytes\t", "PageFilePeakBytes\t", "PoolNonpagedBytes\t", "PoolPagedBytes\t",
                    "VirtualMemoryBytes\t", "VirtualMemoryPrivateBytes\t",
                    "WorkingSetBytes\t", "WorkingSetPeakBytes\t", "WorkingSetPrivateBytes\t"
                )
            );
        }

        public static void PrintToConsole(MemorySnapshot ms)
        {
            Console.WriteLine(
                String.Format(
                    format,
                    ms.Timestamp.ToString() + "\t",
                    ms.HandleCount + "\t", ms.GdiObjectCount + "\t", ms.UserObjectCount + "\t", ms.ThreadCount + "\t",
                    ms.PageFileBytes + "\t", ms.PageFilePeakBytes + "\t", ms.PoolNonpagedBytes + "\t", ms.PoolPagedBytes + "\t",
                    ms.VirtualMemoryBytes + "\t", ms.VirtualMemoryPrivateBytes + "\t",
                    ms.WorkingSetBytes + "\t", ms.WorkingSetPeakBytes + "\t", ms.WorkingSetPrivateBytes + "\t"
                )
            );
        }

        public static void PrintUsage()
        {
            Console.WriteLine(
                "Track the memory for a given process name or process id.\n" +
                "To save data to xml specify path to file in /saveTo.\n" + 
                "To wait for a time period before tracking specify /initialDelay.\n\n" +
                "MemoryTracer.exe /pid|processName /interval [/initialDelay] [/saveTo]");
        }

        static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            // Lets us terminate the loop on Ctrl^C.
            exitLoop = true;
            e.Cancel = true;
        }
    }

    public class CommandLineArguments
    {
        [Description("The initial delay before taking the first memory snapshot.")]
        public int? InitialDelay { get; set; }

        [Description("The amount of time in milliseconds to wait before taking a memory snapshot. Must be specified.")]
        public int? Interval { get; set; }

        [Description("The Process ID of the process to be monitored.")]
        public int? Pid { get; set; }

        [Description("The Process Name of the process to be monitored.")]
        public string ProcessName { get; set; }

        [Description("The path of the file where to save the memorysnapshots.")]
        public string SaveTo { get; set; }
    }
}