// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Microsoft.Test.FaultInjection.SignatureParsing;

namespace Microsoft.Test.FaultInjection.Conditions
{
    [Serializable()] 
    internal sealed class TriggerIfStackContains : ICondition
    {
        public TriggerIfStackContains(String aTargetFunction)
        {
            targetFunction = Signature.ConvertSignature(aTargetFunction);
        }

        public bool Trigger(IRuntimeContext context)
        {
            for (int i = 0; i < context.CallStack.FrameCount; ++i)
            {
                if (context.CallStack[i] == null)
                {
                    return false;
                }
                else if (context.CallStack[i] == targetFunction)
                {
                    return true;
                }
            }
            return false;
        }
        private String targetFunction;
    }  
}