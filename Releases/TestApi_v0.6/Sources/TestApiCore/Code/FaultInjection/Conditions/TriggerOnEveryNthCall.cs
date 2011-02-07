// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Test.FaultInjection.Conditions
{
    [Serializable()]
    internal sealed class TriggerOnEveryNthCall : ICondition
    {
        public TriggerOnEveryNthCall(int nth)
        {
            if (nth <= 0)
            {
                throw new ArgumentException("The parameter of TriggerEveryNthCall(int) should be a positive number");
            }
            this.n = nth;
        }

        public bool Trigger(IRuntimeContext context)
        {
            if (context.CalledTimes % n == 0)
            {
                return true;
            }
            return false;
        }
        private int n;
    }  
}