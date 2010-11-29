using System;
using Microsoft.Test.FaultInjection;
using Xunit;

namespace Microsoft.Test.AcceptanceTests
{
    /// <summary>
    /// Tests which verify the handling of nested classes
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1053")] //remove constructors
    public class NestedClassTests
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034")] //Don't nest class
        public class NestedClass
        {
            #region NestedClassTest

            /// <summary>
            /// Verifies the ability to inject a method inside a nested class
            /// </summary>
            [Fact]
            [FaultInjectionTest]
            public void NestedClassTest()
            {
                Exception a;
                object b;
                Assert.True(FaultDispatcher.Trap(out a, out b));
            }

            #endregion
            
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034")] //Don't nest class
            public class DoublyNestedClass
            {
                #region DoublyNestedClassTest

                /// <summary>
                /// Verifies the ability to inject a method inside a doubly nested class
                /// </summary>
                [Fact]
                [FaultInjectionTest]
                public void DoublyNestedClassTest()
                {
                    Exception a;
                    object b;
                    Assert.True(FaultDispatcher.Trap(out a, out b));
                }

                #endregion
            }
        }

    }
}
