using System;
using Microsoft.Test.FaultInjection;
using Xunit;

namespace Microsoft.Test.AcceptanceTests
{
    /// <summary>
    /// Tests which verify handling of constructor methods
    /// </summary>    
    public class ConstructorTests
    {
        #region StaticConstructorTest

        /// <summary>
        /// Verifies the ability to inject a static constructor
        /// </summary>
        [Fact]
        [FaultInjectionTest]
        public void StaticConstructorTest()
        {
            Microsoft.Test.AcceptanceTests.ConstructorTestOuterClass.InnerClass.num = 2;
        }

        #endregion

        #region ConstructorTest

        /// <summary>
        /// Verifies the ability to inject an instance constructor
        /// </summary>
        [Fact]
        [FaultInjectionTest]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806")] //unused instantiation
        public void ConstructorTest()
        {
            new ConstructorTestOuterClass.InnerClass();
        }

        #endregion
    }

    #region Test Classes

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1053")] //remove constructors
    public class ConstructorTestOuterClass
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1053")] //remove constructors
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034")] //Don't nest class
        public class InnerClass
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704")] //Hungarian notation
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211")]  //Make num constant
            public static int num = 0;

            static InnerClass()
            {
                Exception a;
                object b;
                Assert.True(FaultDispatcher.Trap(out a, out b) == true);
            }

            public InnerClass()
            {
                Exception a;
                object b;
                Assert.True(FaultDispatcher.Trap(out a, out b) == true);            
            }
        }
    }

    #endregion
}
