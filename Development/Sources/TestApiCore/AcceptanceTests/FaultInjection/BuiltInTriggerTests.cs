// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Test.FaultInjection;
using Xunit;

namespace Microsoft.Test.AcceptanceTests.FaultInjection
{
    /// <summary>
    /// Tests which verify the built in conditions
    /// </summary>    
    public class BuiltInTriggerTests
    {
        #region TriggerIfCalledByTest

        /// <summary>
        /// Verifies the condition is triggered when the specified method is 
        /// on the top of the call stack. Created using the factory method that
        /// takes a string.
        /// </summary>
        [Fact]
        public void TriggerIfCalledByTestString()
        {
            string method = "Microsoft.Test.AcceptanceTests.BuiltInTriggerTests.TriggerIfCalledByTestString()";
            ICondition condition = BuiltInConditions.TriggerIfCalledBy(method);
            DoTriggerIfCalledByTest(condition);
        }

        /// <summary>
        /// Verifies the condition is triggered when the specified method is 
        /// on the top of the call stack. Created using the factory method that
        /// takes a MethodBase.
        /// </summary>
        [Fact]
        public void TriggerIfCalledByTestMethodBase()
        {
            MethodBase method = ((Action)TriggerIfCalledByTestMethodBase).Method;
            ICondition condition = BuiltInConditions.TriggerIfCalledBy(method);
            DoTriggerIfCalledByTest(condition);
        }

        private void DoTriggerIfCalledByTest(ICondition condition)
        {
            RuntimeContext ctx = new RuntimeContext();
            ctx.CallStack = new CallStack(new StackTrace(0));
            int loopTimes = 10;
            for (int i = 0; i < loopTimes; ++i)
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
        /// contained withing the call stack. Created using the factory method that
        /// takes a string.
        /// </summary>
        [Fact]
        public void TriggerIfStackContainsTestString()
        {
            string method = "Microsoft.Test.AcceptanceTests.BuiltInTriggerTests.TriggerIfStackContainsTestString()";
            ICondition condition = BuiltInConditions.TriggerIfStackContains(method);
            DoTriggerIfStackContainsTests(condition);
        }

        /// <summary>
        /// Verifies the condition is triggered when the specified method is
        /// contained withing the call stack. Created using the factory method that
        /// takes a MethodBase.
        /// </summary>
        [Fact]
        public void TriggerIfStackContainsTestMethodBase()
        {
            MethodInfo method = ((Action)TriggerIfStackContainsTestMethodBase).Method;
            ICondition condition = BuiltInConditions.TriggerIfStackContains(method);
            DoTriggerIfStackContainsTests(condition);
        }

        private void DoTriggerIfStackContainsTests(ICondition condition)
        {
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
            ICondition condition = BuiltInConditions.TriggerOnEveryNthCall(n);
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
