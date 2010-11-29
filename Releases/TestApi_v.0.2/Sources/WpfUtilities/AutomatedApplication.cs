// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Microsoft.Test
{
    /// <summary>
    /// A  delegate that will be called when the appropriate event is raised by
    /// an AutomatedApplication.  
    /// </summary>
    /// <param name="application">
    /// The delegate will be passed a handle to the associated 
    /// AutomatedApplication.
    /// </param>
    public delegate void AutomatedApplicationCallback(
        AutomatedApplication application);

    /// <summary>
    /// The AutomatedApplication class allows the user to load and start a test
    /// application either in the current process or in a new, separate process.  
    /// The application is described using either InProcessSettings or
    /// OutOfProcessSettings which enable setting delegates and describe the 
    /// application to launch.  These delegates are called when the 
    /// applcation's main window opens and/or when the application exits.
    /// </summary>
    /// <example>
    /// The following example shows in-process usage. The code runs the target
    /// application in a separate thread within the current process.
    /// <code>
    /**
    public void MyTest()
    {
        InProcessSettings settings = new InProcessSettings(
            "c:\\myTestApp.exe",
            "MyTestApp",
            OnMainWindowCreated,
            OnExit);
        
        AutomatedApplication a = AutomatedApplication.Start(settings);
        a.WaitForMainWindow(TimeSpan.FromSeconds(5));
        
        // Perform various tests...

        a.Close();
    }
     
    private static void OnMainWindowCreated(AutomatedApplication ap)
    {
        Console.WriteLine("The main window of the application was created.");
    }

    private static void OnExit(AutomatedApplication a)
    {
        Console.WriteLine("The application has exited.");
    }
    */
    /// </code>
    /// </example>
    /// <example>
    /// The following example demonstrates out-of-process usage:
    /// <code>
    /**
    public void MyTest()
    {
        ProcessStartInfo psi = new ProcessStartInfo("c:\\myTestApp.exe");
        OutOfProcessSettings settings = new OutOfProcessSettings(
            psi,
            OnMainWindowOpened,
            OnExit);
     
        AutomatedApplication a = AutomatedApplication.Start(settings);
        a.WaitForMainWindow(TimeSpan.FromSeconds(5));
     
        // Perform various tests...
      
        a.Close();
    }

    private static void OnMainWindowCreated(AutomatedApplication ap)
    {
        Console.WriteLine("The main window of the application was created.");
    }

    private static void OnExit(AutomatedApplication ap)
    {
        Console.WriteLine("The application has exited.");
    }
    */
    /// </code>
    /// </example>
    public class AutomatedApplication : MarshalByRefObject
    {
        #region Public Methods

        /// <summary>
        /// Starts a new application using the settings provided and returns an 
        /// AutomatedApplication to wrap it.
        /// </summary>
        /// <param name="settings">
        /// The settings specify that the application should be started inside 
        /// the current process.  They specify the application to create, 
        /// which is described by the application path and type name. If 
        /// applicable the settings also describe the delegates to set.
        /// </param>
        /// <returns>
        /// Returns an AutomatedApplication handle which gives the user access 
        /// to the main window and the ability to close the application.
        /// </returns>
        public static AutomatedApplication Start(InProcessSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (settings.Path == null)
            {
                throw new ArgumentException("settings.Path should not be null");
            }

            if (settings.ApplicationType == null)
            {
                throw new ArgumentException(
                    "settings.ApplicationType should not be null");
            }

            AutomatedApplication automatedApplication;

            if (Assembly.GetEntryAssembly() != null)
            {
                AppDomain appDomain = AppDomain.CreateDomain("Automated Application Domain");
                
                automatedApplication = (AutomatedApplication)appDomain.CreateInstanceAndUnwrap(
                    Assembly.GetExecutingAssembly().GetName().Name,
                    typeof(AutomatedApplication).FullName);

                automatedApplication.InitializeApplication(settings, appDomain);
            }
            else
            {
                automatedApplication = new AutomatedApplication();
                automatedApplication.InitializeApplication(settings, null);
            }

            return automatedApplication;
        }

        /// <summary>
        /// Starts a new application using the settings provided and returns an 
        /// AutomatedApplication to wrap it.
        /// </summary>
        /// <param name="settings">
        /// The settings specify that the application should be started in a 
        /// new process.  They specify the application to create, as described
        /// by a ProcessStartInfo object. If applicable the settings also 
        /// describe the delegates to set.
        /// </param>
        /// <returns>
        /// Returns an AutomatedApplication handle which gives the user access 
        /// to the main window and the ability to close the application.
        /// </returns>
        public static AutomatedApplication Start(OutOfProcessSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (settings.StartInfo == null)
            {
                throw new ArgumentException(
                    "settings.StartInfo should not be null");
            }

            AutomatedApplication automatedApplication = 
                new AutomatedApplication();
            
            automatedApplication.outOfProcessSettings = settings;
            automatedApplication.inProcess = false;
            automatedApplication.process = new Process();
            
            automatedApplication.process.StartInfo = 
                automatedApplication.outOfProcessSettings.StartInfo;

            SetMainWindowOpenedHandler(automatedApplication);
            SetExitedHandler(automatedApplication);

            automatedApplication.process.Start();

            return automatedApplication;
        }

        /// <summary>
        /// Closes the automated application gracefully.
        /// </summary>
        public void Close()
        {
            if (inProcess)
            {
                CloseInProcess();
            }
            else
            {
                CloseOutOfProcess();
            }
        }

        /// <summary>
        /// AutomatedApplication objects should be created using the appropriate
        /// Start method rather than the public constructor.
        /// </summary>
        public AutomatedApplication() { }

        /// <summary>
        /// Blocks execution of the current thread until the main window of the 
        /// application is displayed or until the specified timeout interval elapses.
        /// </summary>
        /// <param name="timeout">The timeout interval.</param>
        public void WaitForMainWindow(TimeSpan timeout)
        {
            TimeSpan zero = TimeSpan.FromMilliseconds(0);
            TimeSpan delta = TimeSpan.FromMilliseconds(10);

            while (this.MainWindow == null && timeout > zero)
            {
                Thread.Sleep(10);
                timeout -= delta;
            }
        }

        #endregion

        #region Private Methods

        private void CloseInProcess()
        {
            Debug.Assert(inProcess);

            application.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                (ThreadStart)delegate
                {
                    if (application != null && application.MainWindow != null)
                    {
                        application.MainWindow.Close();
                    }
                });
        }

        private void CloseOutOfProcess()
        {
            Debug.Assert(!inProcess);

            if (process != null && !process.HasExited)
            {
                process.CloseMainWindow();
                process.Close();

                if (outOfProcessSettings.Exited != null)
                {
                    outOfProcessSettings.Exited(this);
                }
            }
        }

        private void InitializeApplication(
            InProcessSettings settings, 
            AppDomain createdDomain)
        {
            inProcess = true;
            inProcessSettings = settings;
            appDomain = createdDomain;

            mainApplicationThread = new Thread(InitializeApplicationWorker);
            mainApplicationThread.SetApartmentState(ApartmentState.STA);

            mainApplicationThread.Start(this);
        }

        private static void InitializeApplicationWorker(object application)
        {
            AutomatedApplication automatedApplication =
                (AutomatedApplication)application;

            Debug.Assert(automatedApplication.inProcess);

            AssemblyName assemblyName = AssemblyName.GetAssemblyName(automatedApplication.inProcessSettings.Path);
            Assembly assembly = Assembly.Load(assemblyName);

            Type applicationType = assembly.GetType(
                automatedApplication.inProcessSettings.ApplicationType);

            automatedApplication.application =
                (Application)Activator.CreateInstance(applicationType);

            MethodInfo methodInfo = applicationType.GetMethod(
                "InitializeComponent");

            if (methodInfo != null)
            {
                methodInfo.Invoke(automatedApplication.application, null);
            }

            Application.ResourceAssembly = assembly;

            SetMainWindowOpenedHandler(automatedApplication);
            SetExitedHandler(automatedApplication);

            automatedApplication.application.Run();
        }

        private static void SetInProcessWindowHandler(
            AutomatedApplication automatedApplication)
        {
            Debug.Assert(automatedApplication.inProcess);

            EventHandler applicationActivatedHandler;
            applicationActivatedHandler = (sender, e) =>
            {
                if (automatedApplication.mainWindowOpened) return;

                automatedApplication.MainWindow = AutomationElement.FromHandle(
                    new WindowInteropHelper(automatedApplication.application.MainWindow).Handle);

                if (automatedApplication.inProcessSettings.MainWindowOpened != null)
                {
                    automatedApplication.inProcessSettings.MainWindowOpened(
                        automatedApplication);
                }

                automatedApplication.mainWindowOpened = true;
            };

            automatedApplication.application.Activated += applicationActivatedHandler;
        }

        private static void SetMainWindowOpenedHandler(
            AutomatedApplication automatedApplication)
        {
            automatedApplication.mainWindowOpened = false;

            if (automatedApplication.inProcess)
            {
                SetInProcessWindowHandler(automatedApplication);
            }
            else
            {
                SetOutOfProcessWindowHandler(automatedApplication);
            }
        }

        private static void SetOutOfProcessWindowHandler(
            AutomatedApplication automatedApplication)
        {
            Debug.Assert(!automatedApplication.inProcess);

            AutomationEventHandler onWindowOpenedHandler;
            onWindowOpenedHandler = (sender, e) =>
            {
                if (automatedApplication.mainWindowOpened) return;

                automatedApplication.MainWindow = AutomationElement.FromHandle(
                    automatedApplication.process.MainWindowHandle);

                if (automatedApplication.outOfProcessSettings.MainWindowOpened != null)
                {
                    automatedApplication.outOfProcessSettings.MainWindowOpened(automatedApplication);
                }

                automatedApplication.mainWindowOpened = true;
            };

            Automation.AddAutomationEventHandler(
                WindowPatternIdentifiers.WindowOpenedEvent,
                AutomationElement.RootElement,
                TreeScope.Subtree,
                onWindowOpenedHandler);
        }

        private static void SetExitedHandler(
            AutomatedApplication automatedApplication)
        {
            if (automatedApplication.inProcess)
            {
                if (automatedApplication.inProcessSettings.Exited != null)
                {
                    automatedApplication.application.Exit += (sender, e) =>
                    {
                        automatedApplication.inProcessSettings.Exited(
                            automatedApplication);
                    };
                }
            }
            else
            {
                if (automatedApplication.outOfProcessSettings.Exited != null)
                {
                    automatedApplication.process.EnableRaisingEvents = true;
                    automatedApplication.process.Exited += (sender, e) =>
                    {
                        automatedApplication.outOfProcessSettings.Exited(
                            automatedApplication);
                    };
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// A read-only property that returns a handle to the main window
        /// of the test application.
        /// </summary>
        public AutomationElement MainWindow { get; private set; }

        #endregion

        #region Private Fields

        private bool inProcess;
        private InProcessSettings inProcessSettings;
        private OutOfProcessSettings outOfProcessSettings;
        private Process process;
        private Application application;
        private Thread mainApplicationThread;
        private AppDomain appDomain;
        private bool mainWindowOpened;

        #endregion
    }
}
