using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Input;

namespace SampleAppTest
{
    static class AutomationHelpers
    {
        internal static void CloseWindow(AutomationElement windowElement)
        {
            WindowPattern window = (WindowPattern)windowElement.GetCurrentPattern(WindowPattern.Pattern);
            window.Close();
        }

        internal static System.Drawing.Rectangle GetElementSize(AutomationElement element)
        {
            Rect rr = (Rect)element.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
            return new Rectangle((int)rr.Left, (int)rr.Top, (int)rr.Width, (int)rr.Height);
        }

        internal static void MoveToAndClick(AutomationElement element)
        {
            System.Windows.Point winPoint = element.GetClickablePoint();
            System.Drawing.Point drawingPoint = new System.Drawing.Point((int)winPoint.X, (int)winPoint.Y);
            Microsoft.Test.Mouse.MoveTo(drawingPoint);
            Microsoft.Test.Mouse.Click(MouseButton.Left);
        }

        internal delegate AutomationElement AutomationDelegate(Process p);

        /// <summary>
        /// Starts a process and blocks until the Main Windows has loaded
        /// returns the MainWindow's AutomationElement as an out parameter
        /// </summary>
        public static Process StartProcess(ProcessStartInfo startInfo, out AutomationElement rootElement)
        {
            AutomationDelegate getRoot = p =>
            {
                return AutomationElement.FromHandle(p.MainWindowHandle);
            };

            return StartProcess(startInfo, getRoot, out rootElement);
        }

        internal static Process StartProcess(ProcessStartInfo startInfo, AutomationDelegate onProcessWindowOpened, out AutomationElement rootElement)
        {
            Process process = null;
            AutomationElement element = null;

            if (onProcessWindowOpened != null)
            {
                AutomationEventHandler onWindowOpenedHandler = null;

                onWindowOpenedHandler = (sender, e) =>
                {
                    if (process != null)
                    {
                        if (process.MainWindowHandle != IntPtr.Zero)
                        {
                            Automation.RemoveAutomationEventHandler
                            (
                                WindowPatternIdentifiers.WindowOpenedEvent,
                                AutomationElement.RootElement,
                                onWindowOpenedHandler
                            );

                            element = onProcessWindowOpened(process);
                        }
                        else
                        {
                            process.Refresh();
                        }
                    }
                };

                Automation.AddAutomationEventHandler
                (
                    WindowPatternIdentifiers.WindowOpenedEvent,
                    AutomationElement.RootElement,
                    TreeScope.Subtree,
                    onWindowOpenedHandler
                );
                process = Process.Start(startInfo);
                while (element == null)
                {
                    Thread.Sleep(10);
                }
            }
            rootElement = element;
            return process;
        }
    }
}
