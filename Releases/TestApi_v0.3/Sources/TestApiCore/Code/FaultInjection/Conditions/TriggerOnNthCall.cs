// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Test.FaultInjection.Conditions
{
    [Serializable()]
    internal sealed class TriggerOnNthCall : ICondition
    {
        public TriggerOnNthCall(int nth)
        {
            if (nth <= 0)
            {
                throw new ArgumentException("The parameter of TriggerOnNthCall(int) should be a postive number");
            }
            this.n = nth;
        }

        public bool Trigger(IRuntimeContext context)
        {
            if (context.CalledTimes == n)
            {
                return true;
            }
            return false;
        }
        private int n;
    } 
}