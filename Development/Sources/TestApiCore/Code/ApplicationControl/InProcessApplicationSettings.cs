// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Test.ApplicationControl
{
    /// <summary>
    /// Configures an in-process automated application.
    /// </summary>
    [Serializable]
    public class InProcessApplicationSettings : ApplicationSettings
    {
        /// <summary>
        /// Path to the application.
        /// </summary>
        public string Path
        {
            get;
            set;
        }

        /// <summary>
        /// The type of application to create.
        /// </summary>
        public InProcessApplicationType InProcessApplicationType
        {
            get;
            set;
        }
    }
}
