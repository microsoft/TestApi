// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Test.FaultInjection;
using Xunit;

namespace Microsoft.Test.AcceptanceTests.FaultInjection
{ 
    /// <summary>
    /// Tests which verify the ability to inject exceptions
    /// </summary>
    public class ThrowExceptionTests
    {
        #region ThrowBuiltInExceptionTest

        /// <summary>
        /// Verifies the ability to inject a built-in exception
        /// </summary>
        [Fact]
        [FaultInjectionTest]
        public void ThrowBuiltInExceptionTest()
        {
            Exception a;
            object b;
            FaultDispatcher.Trap(out a, out b);
            Assert.IsType(typeof(ApplicationException), a);
        }

        #endregion

        #region ThrowCustomExceptionTest

        /// <summary>
        /// Verifies the ability to inject a custom exception
        /// </summary>
        [Fact]
        [FaultInjectionTest]
        public void ThrowCustomExceptionTest()
        {
            Exception a;
            object b;
            FaultDispatcher.Trap(out a, out b);
            Assert.IsType(typeof(CustomizedException), a);
        }

        #endregion
    }

    #region CustomizedException

    [Serializable()]
    public class CustomizedException : Exception
    {
        public CustomizedException()
        { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032")] //Make protected
        public CustomizedException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704")] //Hungarian notation
        public CustomizedException(string str)
            : base(str)
        { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704")] //Hungarian notation
        public CustomizedException(string str, Exception ex)
            : base (str, ex)
        { }
    }

    #endregion
}
