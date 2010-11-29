// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Runtime.Serialization;

namespace Microsoft.Test.FaultInjection
{
    /// <summary>
    /// An exception that is thrown when and error in the FaultInjection API occurs.
    /// </summary>
    [Serializable]
    public class FaultInjectionException : Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the FaultInjectionException class.
        /// </summary>
        public FaultInjectionException() { }
        
        /// <summary>
        /// Initializes a new instance of the FaultInjectionException class using the specified message.
        /// </summary>
        public FaultInjectionException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the FaultInjectionException class using the specified message and inner
        /// exception.
        /// </summary>
        public FaultInjectionException(string message, Exception innerException) : base(message, innerException) { }
        
        /// <summary>
        /// Constructor used for serialization purposes.
        /// </summary>
        protected FaultInjectionException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion
    }
}
