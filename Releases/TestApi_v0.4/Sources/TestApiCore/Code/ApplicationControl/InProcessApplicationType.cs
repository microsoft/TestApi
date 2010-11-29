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
        /// A test application that runs in-process and on a separate thread.
        /// </summary>
        InProcessSeparateThread = 0,

        /// <summary>
        /// A test application that runs in-process, on a separate thread and 
        /// in a separate AppDomain.
        /// </summary>
        InProcessSeparateThreadAndAppDomain,

        /// <summary>
        /// A test application that runs in-process and on the same thread.
        /// </summary>
        InProcessSameThread
    }
}