using System;
using Microsoft.Test.FaultInjection;
using Xunit;

namespace Microsoft.Test.AcceptanceTests
{
    /// <summary>
    /// Tests which verify the proper value is returned by a
    /// ReturnValueFault
    /// </summary>    
    public class ReturnValueTests
    {
        #region ReturnNullTest

        /// <summary>
        /// Verifies that null is retuned by a ReturnFault
        /// </summary>
        [Fact]
        [FaultInjectionTest]
        public void ReturnNullTest()
        {
            Exception a;
            object b;
            Assert.True(FaultDispatcher.Trap(out a, out b));
            if (a != null)
            {
                throw a;
            }
            else
            {
                return;
            }
        }

        #endregion

        #region ReturnIntTest

        /// <summary>
        /// Verifies the proper int return value is injected
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720")] //int in name
        [Fact]
        [FaultInjectionTest]
        public void ReturnIntTest()
        {
            Assert.Equal<int>(ReturnIntTargetMethod(null, 0), 232);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704")] //Hungarian notation
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720")] //int in name
        public int ReturnIntTargetMethod(string str, Int32 number)
        {
            Exception a;
            object b;
            Assert.True(FaultDispatcher.Trap(out a, out b));
            Assert.Null(a);
            return (int)b;
        }

        #endregion

        #region ReturnBoolTest

        /// <summary>
        /// Verifies the proper bool return value is injected
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720")] //Bool in name
        [Fact]
        [FaultInjectionTest]
        public void ReturnBoolTest()
        {
            Assert.False(ReturnBoolTargetMethod(null, 0));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704")] //Hungarian notation
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720")] //Bool in name
        public bool ReturnBoolTargetMethod(string str, Int32 number)
        {
            Exception a;
            object b;
            Assert.True(FaultDispatcher.Trap(out a, out b));
            Assert.Null(a);
            return (bool)b;
        }

        #endregion
    }
}
