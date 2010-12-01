// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;

namespace Microsoft.Test.ApplicationControl
{
    /// <summary>
    /// Configures an out-of-process automated application.
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
