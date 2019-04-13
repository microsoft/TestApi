// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;

namespace Microsoft.Test.FaultInjection
{
    /// <summary>
    /// Defines the contract for information provided by the faulted method.
    /// </summary>
    public interface IRuntimeContext
    {
        /// <summary>
        /// The number of times the method is called.
        /// </summary>
        int CalledTimes
        {
            get;
        }

        /// <summary>
        /// The method's stack trace.
        /// </summary>
        StackTrace CallStackTrace
        {
            get;
        }

        /// <summary>
        /// An array of C#-style method signatures for each method on the call stack.
        /// </summary>
        CallStack CallStack
        {
            get;
        }

        /// <summary>
        /// The C#-style method signature of the caller of the faulted method.
        /// </summary>
        String Caller
        {
            get;
        }
    }
}