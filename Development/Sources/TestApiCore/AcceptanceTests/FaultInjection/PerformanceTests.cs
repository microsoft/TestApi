// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Microsoft.Test.FaultInjection;
using Xunit;

namespace Microsoft.Test.AcceptanceTests.FaultInjection
{
    public class PerformanceTests
    {
        #region Performance tests

        /// <summary>
        /// Measures the performance overhead of calling Trap() when no fault scope or fault session is open.
        /// e.g. just the cost of having the engine insert Trap() into a target method.
        /// </summary>
        [Fact]
        public void TestTrapPerformanceNoFaultScope()
        {
            TimeSpan NoTrap = MeasurePerformance(PerformanceActionNoTrap);
            TimeSpan WithTrap = MeasurePerformance(PerformanceActionWithTrap);

            // We allow it to be 20% slower.
            if (WithTrap.Ticks >= NoTrap.Ticks * 1.2)
            {
                Assert.Null(string.Format("Time with no trap: {0}; Time with trap: {1}", NoTrap, WithTrap));
            }
        }

        [Fact]
        public void TestTrapPerformanceWithFaultScope()
        {
            TimeSpan NoTrap = MeasurePerformance(PerformanceActionNoTrap);

            TimeSpan WithFaultScope;
            FaultRule faultRule = new FaultRule("Microsoft.Test.AcceptanceTests.FaultInjection.PerformanceTests.PerformanceActionWithTrap()",
                                    BuiltInConditions.TriggerOnEveryCall,
                                    BuiltInFaults.ReturnFault());
            using (FaultScope fs = new FaultScope(faultRule))
            {
                WithFaultScope = MeasurePerformance(PerformanceActionWithTrap);
            }

            // We allow it to be 150x slower.
            // BUG: This test is unstable -- in some cases the perf difference is as high as the following:
            //   Time with no trap: 00:00:00.0042113; Time with trap in fault scope: 00:00:00.7645935
            // so we are disabling the assert for now.
            if (WithFaultScope.Ticks >= NoTrap.Ticks * 150)
            {
                // Assert.Null(string.Format("Time with no trap: {0}; Time with trap in fault scope: {1}", NoTrap, WithFaultScope));
            }
        }

        private void PerformanceActionNoTrap()
        {
            for (int i = 0; i < 10; i++)
            {
                // This is added so that it is a fair comparison to PerformanceActionWithTrap().
                // Also it provides something for this method to do, since otherwise it would just
                // get completely optimised out.
                Assert.Equal(true, true);
            }
        }

        private void PerformanceActionWithTrap()
        {
            for (int i = 0; i < 10; i++)
            {
                Exception e;
                Object o;
                bool trapped = FaultDispatcher.Trap(out e, out o);

                // This is not strictly needed, but it safeguards the test from succeeding 
                // when it should fail
                Assert.Equal(FaultScope.Current != null, trapped);
            }
        }

        private TimeSpan MeasurePerformance(Action a)
        {
            // Warm up
            int NumWarmupLoops = 1000;
            for (int i = 0; i < NumWarmupLoops; i++)
            {
                a();
            }

            // Actual execution
            int NumMeasuredLoops = 1000;
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < NumMeasuredLoops; i++)
            {
                a();
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        #endregion
    }
}
