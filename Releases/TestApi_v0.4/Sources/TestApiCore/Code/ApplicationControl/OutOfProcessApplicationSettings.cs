using System;
using System.Diagnostics;

namespace Microsoft.Test.ApplicationControl
{
    /// <summary>
    /// Configures an out-of-process test application.
    /// </summary>
    [Serializable]
    public class OutOfProcessApplicationSettings : ApplicationSettings
    {
        /// <summary>
        /// The ProcessStartInfo to start a process.
        /// </summary>
        public ProcessStartInfo ProcessStartInfo
        {
            get;
            set;
        }
    }
}
