using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Reflection;

namespace SampleAppTest
{
    class ApplicationDriver
    {
        public ApplicationDriver(Type applicationType)
        {
            this.applicationType = applicationType;
            thread = new Thread(StartAppThread);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Name = "WPF Application Thread";
            thread.Start(this);
            
        }

        /// <summary>
        /// Explicitly Resets dispatcherIdle Event and waits for next event
        /// </summary>
        public void WaitForIdleUi()
        {
            // We are not interested in a valid signal which has occurred prior to blocking. We will wait for the next
            dispatcherIdle.Reset();
            dispatcherIdle.WaitOne();
            Thread.Sleep(2000); // Allows rendering operations and animations to complete
        }

        /// <summary>
        /// Launches the Application code - This is equivalent to the Main Method.
        /// </summary>
        static private void StartAppThread(object threadArgument)
        {
            ApplicationDriver driver = (ApplicationDriver)threadArgument;
            Dispatcher.CurrentDispatcher.Invoke(
                DispatcherPriority.ApplicationIdle,
                (ThreadStart)delegate
                {
                    Application app = (Application)Activator.CreateInstance(driver.applicationType);

                    // Stock VS built WPF applications have an initializeComponent step which is performed as part of the Main method.
                    MethodInfo methodInfo = driver.applicationType.GetMethod("InitializeComponent");
                    if (methodInfo != null)
                    {
                        methodInfo.Invoke(app, null);
                    }
                    // Note: This is non-default step happens before Run. It is needed because the resourceAssembly we are testing is in the external WPF App bits.
                    Application.ResourceAssembly = driver.applicationType.Assembly;
                    app.Activated += (s, e) =>
                    {
                        Dispatcher.CurrentDispatcher.Invoke(
                DispatcherPriority.ApplicationIdle,
                (ThreadStart)delegate
                {
                    driver.dispatcherIdle.Set();
                    MonitorDispatcherReadiness(driver, TimeSpan.FromMilliseconds(100), DispatcherPriority.ApplicationIdle);
                });
                    };
                    app.Run();
                });
        }

        /// <summary>
        /// Uses a timer to signal every interval over the EventWaitHandle when the Dispatcher is Idle
        /// </summary>
        /// <param name="evt">The EventWaitHandle to signal out with</param>
        /// <param name="interval">Time spent between polling</param>
        /// <param name="priority">Priority of Dispatcher to Test against</param>
        static private void MonitorDispatcherReadiness(ApplicationDriver driver, TimeSpan interval, DispatcherPriority priority)
        {
            DispatcherTimer timer = new DispatcherTimer(priority);
            timer.Interval = interval;

            // With each tick, we signal the idleness of the dispatcher on UI thread
            timer.Tick += (sender, args) =>
            {
                driver.dispatcherIdle.Set();
            };
            timer.Start();
        }

        internal void Join()
        {
            thread.Join(200);
        }

        private AutoResetEvent dispatcherIdle = new AutoResetEvent(false);
        private Type applicationType;        
        private Thread thread;        
    }
}
