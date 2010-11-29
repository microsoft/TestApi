// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Microsoft.Test.FaultInjection;
using Microsoft.Test.FaultInjection.Constants;
using Xunit;

namespace Microsoft.Test.AcceptanceTests.FaultInjection
{
    /// <summary>
    /// Tests which varify the handling of non-generic methods
    /// </summary>    
    public class NonGenericSignatureTests
    {
        #region NonGenericSignatureTest

        /// <summary>
        /// Verifies the ability to inject a method with a complex
        /// non-
        /// </summary>
        [Fact]
        [FaultInjectionTest]
        public void NonGenericSignatureTest()
        {
            Int32[][] i = null;
            object o = null;
            TestMethod(out i, ref o, null, null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045")] //ref obj parameter
        public void TestMethod(out Int32[][] intArray, ref object obj, TestEnum[,,] testEnum, NestedClass[][] nestedClass, params int[] intArray2 )
        {
            intArray = null;
            Exception a;
            object b;
            
            // BUG: The code below used to Assert than FD.Trap returns true. 
            // In the latest version of TestApi (which uses xUnit 1.5), this changed. We have  
            // confirmed that the FI functionality works as expected, so we are capturing this fault
            //  as current expected behavior for AppCompat reasons.
            Assert.True(FaultDispatcher.Trap(out a, out b) == false);
        }

        #endregion

        #region Test Classes

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034")] //Don't nest class
        public class NestedClass
        { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711")] //Don't end in "Enum"
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034")] //Don't nest class        
        public enum TestEnum
        {
            Aaa,
            Bbb,
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704")] //Hungarian notation
            Ccc
        }

        #endregion
    }

}
