// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;

namespace Microsoft.Test.FaultInjection
{
    /// <summary>
    /// Stores information about a faulted method.
    /// </summary>
    public class RuntimeContext : IRuntimeContext
    {
        #region Private Data

        private int calledTimes = 0;
        StackTrace callStackTrace = null;
        private CallStack callStack = null;

        #endregion

        #region Contructors
        /// <summary>
        /// Initializes a new instance of the RuntimeContext class.
        /// </summary>
        public RuntimeContext()
        {
        }

        #endregion

        #region Public Members

        /// <summary>
        /// The number of times the method has been called.
        /// </summary>
        public int CalledTimes
        {
            get
            {
                int times = calledTimes;
                return times;
            }
            set
            {
                calledTimes = value;
                
            }
        }

        /// <summary>
        /// The method's stack trace.
        /// </summary>
        public StackTrace CallStackTrace
        {
            get
            {
                return callStackTrace;
            }
            set
            {
                callStackTrace = value;          
            }
        }

        /// <summary>
        /// An array of C#-style method signatures for each method on the call stack.
        /// </summary>
        public CallStack CallStack
        {
            get
            {
                return callStack;
            }
            set
            {              
                callStack = value;              
            }
        }

        /// <summary>
        /// The C#-style method signature of the caller of the faulted method.
        /// </summary>
        public string Caller
        {
            get
            {
                if (callStack != null)
                {
                    return callStack[1];
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion    
    }
}
