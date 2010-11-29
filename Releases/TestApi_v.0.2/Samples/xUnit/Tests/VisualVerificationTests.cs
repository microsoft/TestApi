using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Automation;
using Xunit;
using Microsoft.Test;


public class VisualVerificationTests
{
    /// <summary>
    /// This test demonstrates the use of the visual verification APIs.
    /// The test compares the actual visual appearance of the main window of the sample 
    /// application to the expected visual appearance (captured in a master image that is 
    /// loaded from disk) and fails if the difference between the two exceeds a certain 
    /// tolerance.
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
        OutOfProcessSettings oops = new OutOfProcessSettings(new ProcessStartInfo(sampleAppPath), null, null);
        AutomatedApplication a = AutomatedApplication.Start(oops);

        try
        {
            a.WaitForMainWindow(TimeSpan.FromSeconds(10));

            //
            // Discover the checkbox in the UI, then click it
            //
            AutomationElement styleBox = AutomationUtilities.FindElementsById(a.MainWindow, "styleBox")[0];
            Helpers.MoveToAndClick(styleBox);
            Thread.Sleep(1000);  // Ensure that the Vista/Win7 window creation animation is complete

            //
            // Capture the window image and compare to the master image by generating a
            // diff image and processing the diff image with a tolerance color verifier
            //
            Snapshot master = Snapshot.FromFile(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Master0.png"));
            Snapshot actual = Snapshot.FromWindow((IntPtr)a.MainWindow.Current.NativeWindowHandle, WindowSnapshotMode.ExcludeWindowBorder);
            Snapshot difference = actual.CompareTo(master);
            master.ToFile(@"Master0-expected.png", ImageFormat.Png);
            actual.ToFile(@"Master0-actual.png", ImageFormat.Png);
            difference.ToFile(@"Master0-difference.png", ImageFormat.Png);

            //
            // Report the test result
            //
            SnapshotVerifier verifier = new SnapshotColorVerifier(Color.Black, new ColorDifference(255, 18, 18, 18));
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


