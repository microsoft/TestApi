// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Microsoft.Test.FaultInjection.SignatureParsing;

namespace Microsoft.Test.FaultInjection.Conditions
{
    [Serializable()]
    internal sealed class TriggerOnNthCallBy : ICondition
    {
        public TriggerOnNthCallBy(int nth, String aTargetCaller)
        {
            calledTimes = 0;
            n = nth;
            if (nth <= 0)
            {
                throw new ArgumentException("The first parameter of TriggerOnNthCallBy(int, string) should be a postive number");
            }
            targetCaller = Signature.ConvertSignature(aTargetCaller);
        }

        public bool Trigger(IRuntimeContext context)
        {
            if (context.Caller == targetCaller)
            {
                if ((++calledTimes) == n)
                {
                    return true;
                }
            }
            return false;
        }
        private int calledTimes;
        private int n;
        private String targetCaller;
    }
}