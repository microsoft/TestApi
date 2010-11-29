// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Microsoft.Test.FaultInjection;
using Xunit;

namespace Microsoft.Test.AcceptanceTests.FaultInjection
{
    /// <summary>
    /// Tests which verify the handling of errors produced by attempting to
    /// return incorrect types
    /// </summary>    
    public class ReturnTypeErrorTests
    {
        #region NullValueTypeTestInt

        /// <summary>
        /// Verifies the proper exception is thrown when attempting to return
        /// null from an int method
        /// </summary>
        [Fact]
        [FaultInjectionTest]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720")] //int in name
        public void NullValueTypeTestInt()
        {
            NullValueTypeInt();            
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720")] //int in name
        public int NullValueTypeInt()
        {
            Exception a;
            object b;
            bool exceptionCaught = false;
            try
            {
                FaultDispatcher.Trap(out a, out b);
            }
            catch (FaultInjectionException e)
            {
                Assert.True(e.Message.Contains("is Value Type"));
                exceptionCaught = true;
            }
            Assert.True(exceptionCaught);
            return 0;
        }

        #endregion

        #region NullValueTypeTestBool

        /// <summary>
        /// Verifies the proper exception is thrown when attempting to return
        /// null from a bool method
        /// </summary>
        [Fact]
        [FaultInjectionTest]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720")] //bool in name
        public void NullValueTypeTestBool()
        {
            NullValueTypeBool();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720")] //bool in name
        public bool NullValueTypeBool()
        {
            Exception a;
            object b;
            bool exceptionCaught = false;
            try
            {
                FaultDispatcher.Trap(out a, out b);
            }
            catch (FaultInjectionException e)
            {
                Assert.True(e.Message.Contains("is Value Type"));
                exceptionCaught = true;
            }
            Assert.True(exceptionCaught);
            return true;
        }

        #endregion

        #region ReturnTypeMismatchTestIntBool

        /// <summary>
        /// Verifies the proper exception is thrown when attempting to return
        /// a bool from an int method
        /// </summary>
        [Fact]
        [FaultInjectionTest]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720")] //bool in name
        public void ReturnTypeMismatchTestIntBool()
        {
            ReturnTypeMismatchIntBool();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720")] //bool in name
        public int ReturnTypeMismatchIntBool()
        {
            Exception a;
            object b;
            bool exceptionCaught = false;
            try
            {
                FaultDispatcher.Trap(out a, out b);
            }
            catch (FaultInjectionException e)
            {
                Assert.True(e.Message.Contains("mismatch"));
                exceptionCaught = true;
            }
            Assert.True(exceptionCaught);
            return 0;
        }

        #endregion

        #region ReturnTypeMismatchTestBoolInt

        /// <summary>
        /// Verifies the proper exception is thrown when attempting to return
        /// an int from a bool method
        /// </summary>
        [Fact]
        [FaultInjectionTest]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720")] //bool and int in name
        public void ReturnTypeMismatchTestBoolInt()
        {
            ReturnTypeMismatchBoolInt();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720")] //bool and int in name
        public bool ReturnTypeMismatchBoolInt()
        {
            Exception a;
            object b;
            bool exceptionCaught = false;
            try
            {
                FaultDispatcher.Trap(out a, out b) ;
            }
            catch (FaultInjectionException e)
            {
                Assert.True(e.Message.Contains("mismatch"));
                exceptionCaught = true;
            }
            Assert.True(exceptionCaught);
            return true;
        }

        #endregion
    }
}
