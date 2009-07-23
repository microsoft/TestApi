using System;
using System.Diagnostics;
using Microsoft.Test.FaultInjection;
using Microsoft.Test.FaultInjection.Conditions;
using Xunit;
using System.Runtime.CompilerServices;

namespace Microsoft.Test.AcceptanceTests
{
    /// <summary>
    /// Tests which verify the built in conditions
    /// </summary>    
    public class BuiltInTriggerTests
    {
        #region TriggerIfCalledByTest

        /// <summary>
        /// Verifies the condition is triggered when the specified method is 
        /// on the top of the call stack
        /// </summary>
        [Fact]
        public void TriggerIfCalledByTest()
        {
            string method = "System.RuntimeMethodHandle._InvokeMethodFast(System.Object,System.Object[],ref System.SignatureStruct,System.Reflection.MethodAttributes,System.RuntimeTypeHandle)";
            ICondition condition = BuiltInConditions.TriggerIfCalledBy(method);
            RuntimeContext ctx = new RuntimeContext();
            ctx.CallStack = new CallStack(new StackTrace(0));
            int loopTimes = 10;
            for(int i = 0; i < loopTimes; ++i)
            {
                Assert.True(condition.Trigger(ctx));
            }
            ctx.CallStack = null;
            for (int i = 0; i < loopTimes; ++i)
            {
                Assert.False(condition.Trigger(ctx));
            }
        }

        #endregion

        #region TriggerIfStackContainsTest

        /// <summary>
        /// Verifies the condition is triggered when the specified method is
        /// contained  withing the call stack
        /// </summary>
        [Fact]
        public void TriggerIfStackContainsTest()
        {
            string method = "System.RuntimeMethodHandle._InvokeMethodFast(System.Object,System.Object[],ref System.SignatureStruct,System.Reflection.MethodAttributes,System.RuntimeTypeHandle)";
            ICondition condition = BuiltInConditions.TriggerIfStackContains(method);
            RuntimeContext ctx = new RuntimeContext();
            ctx.CallStack = new CallStack(new StackTrace(0));
            
            int loopTimes = 10;
            for (int i = 0; i < loopTimes; ++i)
            {
                Assert.True(condition.Trigger(ctx));
            }
            ctx.CallStack = new CallStack(new StackTrace(6));
            for (int i = 0; i < loopTimes; ++i)
            {
                Assert.False(condition.Trigger(ctx));
            }
        }

        #endregion

        #region TriggerOnEveryNthCallTest

        /// <summary>
        /// Verifies the condition is triggered every Nth time
        /// the target method is called
        /// </summary>
        [Fact]
        public void TriggerOnEveryNthCallTest()
        {
            int n = 2;
            ICondition condition = BuiltInConditions.TriggerEveryOnNthCall(n);
            RuntimeContext ctx = new RuntimeContext();
            int loopTimes = 20;
            for (int i = 0; i < loopTimes; ++i)
            {
                ctx.CalledTimes++;
                bool shouldTrigger = ((i + 1) % 2 == 0)?true:false;
                Assert.Equal<bool>(shouldTrigger, condition.Trigger(ctx));
            }
        }

        #endregion

        #region TriggerOnNthCallTest

        /// <summary>
        /// Verifies the condition is triggered the Nth time
        /// the target method is called
        /// </summary>
        [Fact]
        public void TriggerOnNthCallTest()
        {
            int n = 3;
            ICondition condition = BuiltInConditions.TriggerOnNthCall(n);
            RuntimeContext ctx = new RuntimeContext();
            int loopTimes = 20;
            for (int i = 0; i < loopTimes; ++i)
            {
                ctx.CalledTimes++;
                if (ctx.CalledTimes != n)
                {
                    Assert.False(condition.Trigger(ctx));
                }
                else
                {
                    Assert.True(condition.Trigger(ctx));
                }
            }
        }

        #endregion

        #region TriggerOnNthCallByTest

        /// <summary>
        /// Verifies the condition is triggered the Nth time
        /// the target method is called by the specified caller
        /// </summary>
        [Fact]
        [FaultInjectionTest]
        public void TriggerOnNthCallByTest()
        {
            int loopTimes = 5;
            for (int i = 0; i < loopTimes; ++i)
            {
                Assert.False(TesteeTriggerOnNthCallBy());   
            }
            for (int i = 0; i < loopTimes; ++i)
            {
                Assert.False(TesterTriggerOnNthCallBy());   
            }
            for (int i = 0; i < loopTimes; ++i)
            {
                if (i < 4)
                {
                    Assert.False(TesterTriggerOnNthCallBy());        
                }
                if (i == 4)
                {
                    Assert.True(TesterTriggerOnNthCallBy());
                }
            }
        }

        //Need to prevent JIT iniling or this method will not appear in the callstack
        [MethodImpl(MethodImplOptions.NoInlining)]
        private bool TesterTriggerOnNthCallBy()
        {
            return TesteeTriggerOnNthCallBy();
        }

        private bool TesteeTriggerOnNthCallBy()
        {
            Exception a;
            object b;

            return FaultDispatcher.Trap(out a, out b);
        }

        #endregion
    }
}
