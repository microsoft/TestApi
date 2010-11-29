// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Test
{
    /// <summary>
    /// An InProcessSettings object can be passed to the
    /// AutomatedApplication.Start method to indicate an AutomatedApplication
    /// should be created inside the current process.  The settings describe 
    /// how to create the process through the path to the executable and the 
    /// application type name.  Optionally, the settings can also specify 
    /// delegates to be called when the AutomatedApplication's main window 
    /// opens or when the process exits.
    /// </summary>
    public class InProcessSettings : MarshalByRefObject
    {
        /// <summary>
        /// The constructor to set the StartInfo and applicable 
        /// AutomationApplicationCallback delegates.
        /// </summary>
        /// <param name="path">
        /// The full path to the application including the file name and 
        /// extension.
        /// </param>
        /// <param name="applicationType">
        /// The name of the application type.
        /// </param>
        /// <param name="mainWindowOpened">
        /// The AutomatedApplicationCallback to be called when the application's 
        /// main window opens.
        /// </param>
        /// <param name="exited">
        /// The AutomatedApplicationCallback to be called when the application 
        /// exits.
        /// </param>
        public InProcessSettings(
            string path,
            string applicationType,
            AutomatedApplicationCallback mainWindowOpened,
            AutomatedApplicationCallback exited)
        {
            Path = path;
            ApplicationType = applicationType;
            MainWindowOpened = mainWindowOpened;
            Exited = exited;
        }

        /// <summary>
        /// Information about the test application to launch.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Information about the test application to launch.
        /// </summary>
        public string ApplicationType { get; set; }

        /// <summary>
        /// The AutomatedApplicationCallback to be called when the application's 
        /// main window opens.
        /// </summary>
        public AutomatedApplicationCallback MainWindowOpened { get; set; }

        /// <summary>
        /// The AutomatedApplicationCallback to be called when the application 
        /// exits.
        /// </summary>
        public AutomatedApplicationCallback Exited { get; set; }
    }
}
