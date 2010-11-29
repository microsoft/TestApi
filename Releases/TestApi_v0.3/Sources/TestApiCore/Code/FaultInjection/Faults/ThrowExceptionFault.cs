// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Microsoft.Test.FaultInjection.Constants;

namespace Microsoft.Test.FaultInjection.Faults
{
    [Serializable()]
    internal sealed class ThrowExceptionFault : IFault
    {
        public ThrowExceptionFault(Exception exceptionValue)
        {
            if (exceptionValue == null)
            {
                throw new FaultInjectionException(ApiErrorMessages.ExceptionNull);
            }
            this.exceptionValue = exceptionValue;
        }
        public void Retrieve(IRuntimeContext rtx, out Exception exceptionValue, out object returnValue)
        {
            returnValue = null;
            exceptionValue = this.exceptionValue;
        }
        private readonly Exception exceptionValue;
    }
}