// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Test.FaultInjection.Constants
{
    internal static class EnvironmentVariable
    {
        // Environment variables we used to pass informations
        public const string EnableProfiling = "COR_ENABLE_PROFILING";
        public const string Proflier = "COR_PROFILER";
        public const string RuleRepository = "FAULT_INJECTION_RULE_REPOSITORY";
        public const string MethodFilter = "FAULT_INJECTION_METHOD_FILTER";
        public const string LogDirectory = "FAULT_INJECTION_LOG_DIR";
        public const string LogVerboseLevel = "FAULT_INJECTION_LOG_LEVEL";
    }
}