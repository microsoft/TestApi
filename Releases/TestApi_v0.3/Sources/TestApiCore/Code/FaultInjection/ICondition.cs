// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Test.FaultInjection
{

    /// <summary>
    /// Defines the contract for specifying when a fault will be triggered on a method.
    /// </summary>
    /// <remarks>
    /// If the fault condition is not triggered, the faulted method will execute its original code.
    /// For more information on how to use a condition, see the <see cref="FaultSession"/> class.
    /// </remarks>
    public interface ICondition
    {
        /// <summary>
        /// Determines whether a fault should be triggered.
        /// </summary>
        /// <param name="context">The runtime context information for this call and the faulted method.</param>
        /// <returns>Returns true if a fault should be triggered, otherwise returns false.</returns>
        bool Trigger(IRuntimeContext context);
    }
}
