using System;
using Microsoft.Test.FaultInjection;
using Microsoft.Test.FaultInjection.Constants;
using Xunit;

namespace Microsoft.Test.AcceptanceTests
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
            Assert.True(FaultDispatcher.Trap(out a, out b) == true);
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
