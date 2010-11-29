// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Microsoft.Test.FaultInjection.SignatureParsing;

namespace Microsoft.Test.FaultInjection.Conditions
{
    [Serializable()] 
    internal sealed class TriggerIfCalledBy : ICondition
    {
        public TriggerIfCalledBy(String aTargetCaller)
        {
            targetCaller = Signature.ConvertSignature(aTargetCaller);
        }

        public bool Trigger(IRuntimeContext context)
        {
            if (context.Caller == targetCaller)
            {
                return true;
            }
            return false;
        }
        private String targetCaller;
    }   
}