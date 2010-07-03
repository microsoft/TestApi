// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Microsoft.Test.FaultInjection.SignatureParsing;

namespace Microsoft.Test.FaultInjection.Faults
{
    [Serializable(), RuntimeFault]
    internal sealed class ThrowExceptionRuntimeFault : IFault
    {
        public ThrowExceptionRuntimeFault(string exceptionExpression)
        {
            this.exceptionExpression = exceptionExpression;
        }
        public void Retrieve(IRuntimeContext rtx, out Exception exceptionValue, out object returnValue)
        {
            returnValue = null;
            exceptionValue = (Exception)Expression.GeneralExpression(exceptionExpression);
        }
        private readonly string exceptionExpression;
    }
}