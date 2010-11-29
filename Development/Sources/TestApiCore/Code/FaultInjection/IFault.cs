// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Test.FaultInjection
{
    /// <summary>
    /// Defines the contract for a fault.
    /// </summary>
    /// <remarks>
    /// For more information on how to use a fault, see the <see cref="FaultSession"/> class.
    /// </remarks>
    /// 
    /// <example>
    /// Define a custom fault, which returns a random int when triggered.
    /// <code>
    /// public class ReturnRandomIntFault : IFault
    /// {
    ///     private Random rand;
    ///
    ///     public ReturnRandomIntFault(int seed)
    ///     {
    ///         rand = new Random(seed);
    ///     }
    ///
    ///     public void Retrieve(IRuntimeContext context, out Exception exceptionValue, out object returnValue)
    ///     {
    ///         exceptionValue = null;
    ///         returnValue = rand.Next();
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IFault
    {
        /// <summary>
        /// Defines the behavior of the fault when triggered.
        /// </summary>
        /// <param name="context">The runtime context information for this call.</param>
        /// <param name="exceptionValue">An output paramter for the exception to be thrown by the faulted method.</param>
        /// <param name="returnValue">An output paramter for the value to be returned by the faulted method.</param>
        /// <remarks>
        /// Parameter <paramref name="returnValue"/>
        /// is only checked when <paramref name="exceptionValue"/> returns null.
        /// </remarks>
        void Retrieve(IRuntimeContext context, out Exception exceptionValue, out object returnValue);
    }  

}
