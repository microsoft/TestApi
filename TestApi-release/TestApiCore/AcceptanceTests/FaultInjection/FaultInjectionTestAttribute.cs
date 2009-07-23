using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Test.FaultInjection;
using Xunit;
using System.Runtime.InteropServices;

namespace Microsoft.Test.AcceptanceTests
{
    /// <summary>
    /// Enable setup and teardown of the Fault Injection test environment 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class FaultInjectionTestAttribute : BeforeAfterTestAttribute
    {
        #region Public Members

        /// <summary>
        /// Post test cleanup
        /// </summary>
        public override void After(MethodInfo methodUnderTest)
        {
            ClearEnvironmentVariables();
        }

        /// <summary>
        /// Pre-test setup:
        ///     Create and initialize FaultSession
        ///     Set environment variables
        /// </summary>
        public override void Before(MethodInfo methodUnderTest)
        {
            string processorArch = DetectProccessorArchitecture();
            ComRegistrar.Register(@".\FaultInjectionEngine\" + processorArch + @"\FaultInjectionEngine.dll");
            FaultSession session = new FaultSession(FaultInjectionTestData.FaultRules);
            ProcessStartInfo startInfo = session.GetProcessStartInfo("notepad.exe");
            SetLocalEnvironmentVariables(startInfo);
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Sets environment variables that are normally set in the target process
        /// locally to enable testing
        /// </summary>
        private void SetLocalEnvironmentVariables(ProcessStartInfo startInfo)
        {
            Environment.SetEnvironmentVariable("COR_ENABLE_PROFILING", startInfo.EnvironmentVariables["COR_ENABLE_PROFILING"], EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("COR_PROFILER", startInfo.EnvironmentVariables["COR_PROFILER"], EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("FAULT_INJECTION_RULE_REPOSITORY", startInfo.EnvironmentVariables["FAULT_INJECTION_RULE_REPOSITORY"], EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("FAULT_INJECTION_METHOD_FILTER", startInfo.EnvironmentVariables["FAULT_INJECTION_METHOD_FILTER"], EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("FAULT_INJECTION_LOG_DIR", startInfo.EnvironmentVariables["FAULT_INJECTION_LOG_DIR"], EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("FAULT_INJECTION_LOG_LEVEL", startInfo.EnvironmentVariables["FAULT_INJECTION_LOG_LEVEL"], EnvironmentVariableTarget.Process);
        }

        /// <summary>
        /// Clears locally set environment variables
        /// </summary>
        private void ClearEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("COR_ENABLE_PROFILING", string.Empty, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("COR_PROFILER", string.Empty, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("FAULT_INJECTION_RULE_REPOSITORY", string.Empty, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("FAULT_INJECTION_METHOD_FILTER", string.Empty, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("FAULT_INJECTION_LOG_DIR", string.Empty, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("FAULT_INJECTION_LOG_LEVEL", string.Empty, EnvironmentVariableTarget.Process);
        }

        private string DetectProccessorArchitecture()
        {
            if (Marshal.SizeOf(new IntPtr()) == 8)
            {
                return "x64";
            }
            else
            {
                return "x86";
            }
        }

        #endregion
    }
}
