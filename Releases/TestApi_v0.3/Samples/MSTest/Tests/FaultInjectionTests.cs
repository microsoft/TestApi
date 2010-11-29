using System;
using System.Diagnostics;
using System.Windows.Automation;
using Microsoft.Test;
using Microsoft.Test.ApplicationControl;
using Microsoft.Test.FaultInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{    
    [TestClass]
    public class FaultInjectionTests
    {
        private const int inputWaitTime = 1000; //time to wait for input in milliseconds

        /// <summary>
        /// This test brings up the sample application, simulates input and verifies that
        /// the input is ignored.  In this case we have injected a ReturnValueFault to the Append method. 
        /// The append method will now always return an empty string.
        /// 
        /// Note that the test assumes that SampleApp.exe is present in the same directory as the test.
        /// </summary>
        [TestMethod]
        [DeploymentItem("FaultInjectionEngine\\x64\\FaultInjectionEngine.dll", "FaultInjectionEngine\\x64")]
        [DeploymentItem("FaultInjectionEngine\\x64\\FaultInjectionEngine.pdb", "FaultInjectionEngine\\x64")]
        [DeploymentItem("FaultInjectionEngine\\x86\\FaultInjectionEngine.dll", "FaultInjectionEngine\\x86")]
        [DeploymentItem("FaultInjectionEngine\\x86\\FaultInjectionEngine.pdb", "FaultInjectionEngine\\x86")]
        public void FaultAppendText()
        {
            string sampleAppPath = "SampleApp.exe";

            //Create the FaultRule and FaultSession
            FaultRule rule = new FaultRule(
                "SampleApp.Window1.Append(string, string)",
                BuiltInConditions.TriggerOnEveryCall,
                BuiltInFaults.ReturnValueFault(""));

            FaultSession session = new FaultSession(rule);

            //Start the app
            ProcessStartInfo psi = session.GetProcessStartInfo(sampleAppPath);
            OutOfProcessApplication testApp = new OutOfProcessApplication(
                new OutOfProcessApplicationSettings
                { 
                    ProcessStartInfo = psi,
                    ApplicationImplementationFactory = new UIAutomationOutOfProcessApplicationFactory() 
                });

            testApp.Start();


            try
            {
                testApp.WaitForMainWindow(TimeSpan.FromSeconds(15));

                // Discover various elements in the UI
                AutomationElement mainWindowElement = (AutomationElement)testApp.MainWindow;
                AutomationElement inputTextBox = AutomationUtilities.FindElementsById(mainWindowElement, "inputTextBox")[0];
                AutomationElement outputTextBox = AutomationUtilities.FindElementsById(mainWindowElement, "outputTextBox")[0];
                AutomationElement appendButton = AutomationUtilities.FindElementsById(mainWindowElement, "appendButton")[0];

                // Click on the input text box and simulate typing
                string inputText = "TestTest";
                string expectedText = ""; //expected text should be nothing since we intercept the Append method

                WindowPattern winPattern = mainWindowElement.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;
                Helpers.MoveToAndClick(inputTextBox);
                winPattern.WaitForInputIdle(inputWaitTime);
                Microsoft.Test.Input.Keyboard.Type(inputText);
                winPattern.WaitForInputIdle(inputWaitTime);

                // Now click the button
                Helpers.MoveToAndClick(appendButton);
                winPattern.WaitForInputIdle(inputWaitTime);

                // Now, get the text of the outputTextBox and compare it against what's expected
                // Fail the test if expected != actual
                TextPattern textPattern = outputTextBox.GetCurrentPattern(TextPattern.Pattern) as TextPattern;
                string actualText = textPattern.DocumentRange.GetText(-1);

                // Report the test result
                Assert.AreEqual(expectedText, actualText);
            }
            finally
            {
                // Close the tested application
                testApp.Close();
            }
        }
    }
}
