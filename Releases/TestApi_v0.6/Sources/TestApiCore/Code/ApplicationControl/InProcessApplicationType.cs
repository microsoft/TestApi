// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Test.ApplicationControl
{
    /// <summary>
    /// Defines the Thread and AppDomain properties of an InProcessApplication.
    /// </summary>
    [Serializable]
    public enum InProcessApplicationType
    {
        /// <summary>
        /// The application runs in-process and on a separate thread.
        /// </summary>
        InProcessSeparateThread = 0,

        /// <summary>
        /// The application runs in-process, on a separate thread and 
        /// in a separate AppDomain.
        /// </summary>
        InProcessSeparateThreadAndAppDomain,

        /// <summary>
        /// The application runs in-process and on the same thread.
        /// </summary>
        InProcessSameThread
    }
}
