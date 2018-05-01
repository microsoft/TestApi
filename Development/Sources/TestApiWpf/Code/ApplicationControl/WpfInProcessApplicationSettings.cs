// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Test.ApplicationControl
{
    /// <summary>
    /// Configures a WPF in-process test application.
    /// </summary>
    [Serializable]
    public class WpfInProcessApplicationSettings : InProcessApplicationSettings
    {
        /// <summary>
        /// The window class to start.
        /// </summary>
        /// <remarks>
        /// This must be the full class name.
        /// </remarks>
        public string WindowClassName
        {
            get;
            set;
        }
    }
}
