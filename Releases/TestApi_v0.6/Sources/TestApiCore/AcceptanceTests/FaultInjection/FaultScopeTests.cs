// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Microsoft.Test.FaultInjection;
using Xunit;

namespace Microsoft.Test.AcceptanceTests.FaultInjection
{
    public class FaultScopeTests
    {
        #region TrapOutsideFaultScope

        /// <summary>
        /// Verifies that target methods are still callable outside of a FaultScope
        /// </summary>
        [Fact]
        public void TrapOutsideFaultScope()
        {
            Exception e;
            object o;
            Assert.False(FaultDispatcher.Trap(out e, out o));
        }

        #endregion
    }
}
