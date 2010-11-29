// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Test.FaultInjection.Faults
{
    [Serializable()]
    internal sealed class ReturnFault : IFault
    {
        public void Retrieve(IRuntimeContext rtx, out Exception exceptionValue, out object returnValue)
        {
            exceptionValue = null;
            returnValue = null;
        }
    }
}