// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;

namespace Microsoft.Test
{
    /// <summary>
    /// An OutOfProcessSettings object can be passed to the
    /// AutomatedApplication.Start method to indicate an AutomatedApplication
    /// should be created in its own process.  The settings describe how to 
    /// create the process through a ProcessStartInfo object.  Optionally, the
    /// settings can also specify delegates to be called when the 
    /// AutomatedApplication's main window opens or when the process exits.
    /// </summary>
    public class OutOfProcessSettings
    {
        /// <summary>
        /// The constructor to set the StartInfo and applicable 
        /// AutomatedApplicationCallback delegates.
        /// </summary>
        /// <param name="startInfo">
        /// Information about the test application to launch.
        /// </param>
        /// <param name="mainWindowOpened">
        /// The AutomatedApplicationCallback to be called when the application's 
        /// main window opens.
        /// </param>
        /// <param name="exited">
        /// The AutomatedApplicationCallback to be called when the application 
        /// exits.
        /// </param>
        public OutOfProcessSettings(
            ProcessStartInfo startInfo,
            AutomatedApplicationCallback mainWindowOpened,
            AutomatedApplicationCallback exited)
        {
            StartInfo = startInfo;
            MainWindowOpened = mainWindowOpened;
            Exited = exited;
        }

        /// <summary>
        /// Information about the test application to launch.
        /// </summary>
        public ProcessStartInfo StartInfo { get; set; }

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
