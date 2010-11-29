using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Automation;
using NUnit.Framework;
using Microsoft.Test;


[TestFixture]
public class UIAutomationTests
{
    /// <summary>
    /// This test brings up the sample application, simulates input and verifies that
    /// the application processes the input correctly.
    /// 
    /// Note that the test assumes that SampleApp.exe is present in the same directory as the test.
    /// </summary>
    [Test]
    public void VerifyInput()
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
            // Discover various elements in the UI
            //
            AutomationElement inputTextBox = AutomationUtilities.FindElementsById(a.MainWindow, "inputTextBox")[0];
            AutomationElement outputTextBox = AutomationUtilities.FindElementsById(a.MainWindow, "outputTextBox")[0];
            AutomationElement appendButton = AutomationUtilities.FindElementsById(a.MainWindow, "appendButton")[0];

            //
            // Click on the input text box and simulate typing
            //
            string inputText = "TestTest";
            string expectedText = inputText + "\n";

            WindowPattern winPattern = a.MainWindow.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;
            Helpers.MoveToAndClick(inputTextBox);
            winPattern.WaitForInputIdle(1000);
            Microsoft.Test.Keyboard.Type(inputText);
            winPattern.WaitForInputIdle(1000);

            //
            // Now click the button
            //
            Helpers.MoveToAndClick(appendButton);
            winPattern.WaitForInputIdle(1000);

            //
            // Now, get the text of the outputTextBox and compare it against what's expected
            // Fail the test if expected != actual
            //
            TextPattern textPattern = outputTextBox.GetCurrentPattern(TextPattern.Pattern) as TextPattern;
            string actualText = textPattern.DocumentRange.GetText(-1);

            //
            // Report the test result
            //
            Assert.AreEqual(expectedText, actualText);
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