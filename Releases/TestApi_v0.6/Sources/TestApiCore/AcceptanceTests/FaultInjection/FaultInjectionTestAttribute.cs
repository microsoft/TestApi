// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Reflection;
using Microsoft.Test.FaultInjection;
using Xunit;

namespace Microsoft.Test.AcceptanceTests.FaultInjection
{
    /// <summary>
    /// Enable setup and teardown of the Fault Injection test environment 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class FaultInjectionTestAttribute : BeforeAfterTestAttribute
    {
        /// <summary>
        /// Post test cleanup.
        /// </summary>
        public override void After(MethodInfo methodUnderTest)
        {
            FaultScope.Current.Dispose();
        }

        /// <summary>
        /// Pre-test setup:
        ///     Create and initialize FaultSession
        ///     Set environment variables
        /// </summary>
        public override void Before(MethodInfo methodUnderTest)
        {
            new FaultScope(FaultInjectionTestData.FaultRules);
        }
    }
}
