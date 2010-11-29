using System.Diagnostics;
using System.Windows.Automation;
using Microsoft.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SampleAppTest
{
    /// <summary>
    /// Contains all UIAutomation based input tests
    /// </summary>
    [TestClass]
    public class UIAutomationTest
    {
        public UIAutomationTest() { }

        public TestContext TestContext { get; set; }

        /// <summary>
        /// This sample demonstrates the usage of Input and UIAutomation
        /// to test an application.  The test selects the first TextBox,
        /// enters text, and click the "Append Text" button.  The test
        /// passes if the text appears in the second TextBox
        /// </summary>
        [TestMethod]
        public void VerifyInput()
        {
            AutomationElement rootElement;
            Process appProcess = AutomationHelpers.StartProcess(new ProcessStartInfo("SampleApp.exe"), out rootElement);

            AutomationElement inputTextBox = AutomationUtilities.FindElementsById(rootElement, "inputTextBox")[0];
            AutomationElement outputTextBox = AutomationUtilities.FindElementsById(rootElement, "outputTextBox")[0];
            AutomationElement button = AutomationUtilities.FindElementsById(rootElement, "appendButton")[0];

            string inputText = "TestTest";
            string expectedText = inputText+"\n";

            WindowPattern winPattern = (WindowPattern)rootElement.GetCurrentPattern(WindowPatternIdentifiers.Pattern);

            AutomationHelpers.MoveToAndClick(inputTextBox);
            winPattern.WaitForInputIdle(1000);
            Microsoft.Test.Keyboard.Type(inputText);
            winPattern.WaitForInputIdle(1000);
            AutomationHelpers.MoveToAndClick(button);
            winPattern.WaitForInputIdle(1000);

            object o;            
            outputTextBox.TryGetCurrentPattern(TextPatternIdentifiers.Pattern, out o);
            TextPattern pattern = (TextPattern)o;
            string actualText = pattern.DocumentRange.GetText(-1);

            try
            {
                Assert.AreEqual(expectedText,actualText, "The text did not match. Expected: {0}  Actual: {1}", expectedText, actualText);
            }
            finally
            {
                AutomationHelpers.CloseWindow(rootElement);
                appProcess.WaitForExit();
            }
        }
    }
}
