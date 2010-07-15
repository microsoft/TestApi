// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Test;
using Microsoft.Test.ApplicationControl;
using Microsoft.Test.VisualVerification;
using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Automation;
using Xunit;

public class VisualVerificationTests
{
    /// <summary>
    /// This test demonstrates the use of the histogram graphing API.
    /// It compares two images from disc, generates the graph image of the histogram,
    /// then compares that graph to a saved, known good version. 
    /// </summary>
    [Fact]
    public void VerifyHistogram()
    {
        //
        // Load up a pair of high-information content images and compare them
        //
        Snapshot master = Snapshot.FromFile(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Mandelbrot01.png"));
        Snapshot compare = Snapshot.FromFile(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Mandelbrot02.png"));
        Snapshot difference = compare.CompareTo(master);

        //
        // Generate a histogram from the diff image
        //
        Histogram h = Histogram.FromSnapshot(difference);

        //
        // Save out the line graph of the histogram as a lossless png
        //
        h.ToGraph(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Histogram-Graph-Actual.png"), ImageFormat.Png);

        //
        // Load the graph image into memory so that we can compare it to a known good version
        //
        Snapshot graphImage = Snapshot.FromFile(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Histogram-Graph-Actual.png"));

        //
        // Compare the rendered graph to a known good render
        //
        Snapshot expectedGraph = Snapshot.FromFile(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Histogram-Graph-Expected.png"));
        Snapshot graphDifference = expectedGraph.CompareTo(graphImage);

        //
        // There should be absolutely no differences between the graph and the expected graph
        //
        Microsoft.Test.VisualVerification.ColorDifference colorDiff = new ColorDifference(0, 0, 0, 0);
        SnapshotColorVerifier verifier = new SnapshotColorVerifier(System.Drawing.Color.Black, colorDiff);

        VerificationResult result = verifier.Verify(graphDifference);

        Assert.Equal<VerificationResult>(VerificationResult.Pass, result);
    }


    /// <summary>
    /// This test demonstrates the use of the visual verification APIs.
    /// The test compares the actual visual appearance of the main window of the sample 
    /// application to the expected visual appearance (captured in a master image that is 
    /// loaded from disk) and fails if the difference between the two exceeds a certain 
    /// tolerance, defined by a tolerance map.
    /// 
    /// Note that the test assumes that SampleApp.exe is present in the same directory as the test.
    /// </summary>
    [Fact]
    public void VerifyWindowAppearance()
    {
        //
        // Start the application we are testing
        //
        string sampleAppPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), "SampleApp.exe");
        AutomatedApplication a = new OutOfProcessApplication(new OutOfProcessApplicationSettings
            {
                ProcessStartInfo = new ProcessStartInfo(sampleAppPath),
                ApplicationImplementationFactory = new UIAutomationOutOfProcessApplicationFactory()
            });
        a.Start();

        try
        {
            a.WaitForMainWindow(TimeSpan.FromSeconds(10));
            Thread.Sleep(1000);  // Ensure that the Vista/Win7 window creation animation is complete

            var mainWindow = a.MainWindow as AutomationElement;

            //
            // Discover the checkbox in the UI, then click it
            //
            AutomationElement styleBox = AutomationUtilities.FindElementsById(mainWindow, "styleBox")[0];
            Helpers.MoveToAndClick(styleBox);

            //
            // Capture the window image and compare to the master image by generating a
            // diff image and processing the diff image with a tolerance map verifier
            //
            Snapshot toleranceMap = Snapshot.FromFile(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ToleranceMap.png"));
            Snapshot master = Snapshot.FromFile(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Master.png"));
            Snapshot actual = Snapshot.FromWindow((IntPtr)mainWindow.Current.NativeWindowHandle, WindowSnapshotMode.ExcludeWindowBorder);
            Snapshot difference = actual.CompareTo(master);

            master.ToFile(@"Master-expected.png", ImageFormat.Png);
            actual.ToFile(@"Master-actual.png", ImageFormat.Png);
            difference.ToFile(@"Master-difference.png", ImageFormat.Png);

            //
            // Report the test result
            //
            SnapshotVerifier verifier = new SnapshotToleranceMapVerifier(toleranceMap);
            Assert.Equal<VerificationResult>(VerificationResult.Pass, verifier.Verify(difference));
        }
        finally
        {
            //
            // Close the tested application
            //
            a.Close();
        }
    }
}


