using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Automation;
using Microsoft.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SampleAppTest
{
    /// <summary>
    /// Contains all visual verification tests
    /// </summary>
    [TestClass]
    public class VisualVerificationTest
    {
        public VisualVerificationTest() { }

        public TestContext TestContext { get; set; }

        /// <summary>
        /// This sample demonstrates the usage of visual verification
        /// to test an application.  The test clicks the CheckBox which
        /// changes the background image.  The test then compares a snapshot
        /// of the running app to a master image.
        /// </summary>
        [TestMethod]
        [DeploymentItem("Master0.png")]
        public void VerifyWindowAppearance()
        {
            ApplicationDriver driver = new ApplicationDriver(typeof(SampleApp.App));
            driver.WaitForIdleUi();
            AutomationElement window = AutomationUtilities.FindElementsById(AutomationElement.RootElement, "sampleAppWindow")[0];
            AutomationElement styleBox = AutomationUtilities.FindElementsById(window, "styleBox")[0];
            AutomationElement captureRegion = AutomationUtilities.FindElementsById(window, "captureContainer")[0];

            AutomationHelpers.MoveToAndClick(styleBox);
            driver.WaitForIdleUi();

            try
            {
                // Capture the actual pixels from the bounds of the screen rectangle
                Snapshot actual = Snapshot.FromRectangle(AutomationHelpers.GetElementSize(captureRegion));
                LogFile(actual, "Actual.png");
                
                // Load the reference/master data from a previously saved file
                Snapshot master = Snapshot.FromFile(Path.Combine(TestContext.TestDeploymentDir, "Master0.png"));
                LogFile(master, "Master.png");

                // Log the outcome of the test, only on failure to save disk space.
                // For test stability in scenarios of varying window styles, consider:
                //  -cropping the capture region to eliminate the border rectangle
                //  -Testing on a well controlled test environment
                if (CompareImages(actual, master) == VerificationResult.Fail)
                {                    
                    Assert.Fail("Initial State test failed. Actual should look like Master image. Refer to logged images under:" + TestContext.TestLogsDir);
                }
            }
            finally
            {
                AutomationHelpers.CloseWindow(window);
                driver.Join();
            }
        }

        /// <summary>
        /// Compares an actual image against a master image
        /// If all pixels match, the operation will pass
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="master"></param>
        private VerificationResult CompareImages(Snapshot actual, Snapshot master)
        {
            // Compare the actual image with the master image
            Snapshot difference = actual.CompareTo(master);
            LogFile(difference, "Difference.png");

            // Configure the snapshot verifier - It expects a black image with ~7.5% color tolerance to detect human visible differences
            SnapshotColorVerifier colorVerifier = new SnapshotColorVerifier(Color.Black, new ColorDifference(255,18,18,18));

            // Evaluate the difference image and retun the result
            return colorVerifier.Verify(difference);
        }

        private void LogFile(Snapshot snapshot, string filePath)
        {
            string path = Path.Combine(TestContext.TestLogsDir, filePath);
            snapshot.ToFile(path, ImageFormat.Png);
            TestContext.WriteLine("Logged Image: " + path);
            TestContext.AddResultFile(path);
        }
    }
}
