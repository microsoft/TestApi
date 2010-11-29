// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Microsoft.Test.FaultInjection.SignatureParsing;

namespace Microsoft.Test.FaultInjection.Faults
{
    [Serializable(), RuntimeFault]
    internal sealed class ReturnValueRuntimeFault : IFault
    {
        public ReturnValueRuntimeFault(string returnValueExpression)
        {
            this.returnValueExpression = returnValueExpression;
        }
        public void Retrieve(IRuntimeContext rtx, out Exception exceptionValue, out object returnValue)
        {
            exceptionValue = null;
            returnValue = Expression.GeneralExpression(returnValueExpression);
        }
        private readonly string returnValueExpression;
    }
}