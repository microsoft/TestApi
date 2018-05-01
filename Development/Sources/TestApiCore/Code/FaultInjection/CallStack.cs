// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Test.FaultInjection.SignatureParsing;

namespace Microsoft.Test.FaultInjection
{
    /// <summary>
    /// Extracts frame information from a StackTrace object.
    /// </summary>
    /// <remarks>
    /// Calling CallStack[n] (where n is zero-indexed) will return a C#-style method signature for the nth frame.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711")]
    public class CallStack
    {
        #region Private Data

        private StackTrace stackTrace;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the CallStack class.
        /// </summary>
        /// <param name="stackTrace"> A stack trace from which to create the CallStack.</param>
        public CallStack(StackTrace stackTrace)
        {
            this.stackTrace = stackTrace;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Number of Frames in the CallStack.
        /// </summary>
        public int FrameCount
        {
            get
            {
                return stackTrace.FrameCount;
            }
        }

        /// <summary>
        /// C#-style method signature for the specified frame.
        /// </summary>
        /// <param name="index">frame to evaluate</param>
        public String this[int index]
        {
            get
            {
                return GetCallStackFunction(index);
            }
        }

        #endregion

        #region Private Members

        private String GetCallStackFunction(int index)
        {
            StackFrame stackFrame = stackTrace.GetFrame(index);

            if (stackFrame == null)
            {
                return null;
            }

            return MethodSignatureTranslator.GetFormalMethodString(stackFrame.GetMethod());
        }

        #endregion
    }
}