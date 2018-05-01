// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Test.FaultInjection.Constants;
using Microsoft.Test.FaultInjection.SignatureParsing;

namespace Microsoft.Test.FaultInjection
{
    /// <summary>
    ///
    /// </summary>
    public class FaultScope : IDisposable
    {
        private static object syncRoot = new object();

        /// <summary>
        ///
        /// </summary>
        /// <param name="rules"></param>
        public FaultScope(params FaultRule[] rules)
        {
            if (rules == null || rules.Length == 0)
            {
                throw new FaultInjectionException(ApiErrorMessages.FaultRulesNullOrEmpty);
            }
            lock (syncRoot)
            {
                if (Current != null)
                {
                    throw new FaultInjectionException(ApiErrorMessages.FaultScopeExists);
                }

                Current = this;
                FaultRules = rules;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            lock (syncRoot)
            {
                if (Current == this)
                {
                    Current = null;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static FaultScope Current
        {
            get;
            private set;
        }

        /// <summary>
        ///
        /// </summary>
        internal FaultRule[] FaultRules
        {
            get;
            private set;
        }

        /// <summary>
        /// Checks that
        /// </summary>
        public void AssertFaultInjectionEnabled()
        {
            StringBuilder errors = new StringBuilder();

            // Verify registry
            if (!ComRegistrar.IsRegistered())
            {
                errors.AppendLine(ApiErrorMessages.EngineNotRegistered);
            }

            // Verify profiler environment variables.
            // Is "COMPLUS_ProfAPI_ProfilerCompatibilitySetting=EnableV2Profiler" still needed?
            if (Environment.GetEnvironmentVariable(EnvironmentVariable.EnableProfiling) != "1")
            {
                errors.AppendLine(ApiErrorMessages.ProfilerNotEnabled);
            }

            string profiler = Environment.GetEnvironmentVariable(EnvironmentVariable.Profiler);
            if (profiler != EngineInfo.Engine_CLSID)
            {
                errors.AppendLine(string.Format(ApiErrorMessages.ProfilerNotSpecified, profiler));
            }

            if (Environment.GetEnvironmentVariable(EnvironmentVariable.ProfilerCompatibilityForCLR4) !=
                EnvironmentVariable.EnableV2Profiler)
            {
                errors.AppendLine(ApiErrorMessages.ProfilerV2CompatibilityNotSpecified);
            }

            // Verify log directory
            string logDirectory = Environment.GetEnvironmentVariable(EnvironmentVariable.LogDirectory);
            if (String.IsNullOrEmpty(logDirectory) || logDirectory.Trim().Length == 0)
            {
                errors.AppendLine(ApiErrorMessages.LogDirectoryNotSet);
            }
            else if (!Directory.Exists(logDirectory))
            {
                errors.AppendLine(string.Format(ApiErrorMessages.LogDirectoryAccessDenied, logDirectory));
            }

            // Verify that fault rules can be loaded
            FaultRule[] rules = FaultRuleLoader.Load();
            if (rules == null)
            {
                errors.AppendLine(ApiErrorMessages.NoFaultRulesLoaded);
            }

            // Verify that the method filter file exists
            string methodFilterFileName = Environment.GetEnvironmentVariable(EnvironmentVariable.MethodFilter);
            if (string.IsNullOrEmpty(methodFilterFileName))
            {
                errors.AppendLine(ApiErrorMessages.MethodFilterFileNotSet);
            }
            else
            {
                StreamReader methodFilterFile = null;
                try
                {
                    // Verify that the method filter file contains an entry for every method that has a fault rule
                    methodFilterFile = File.OpenText(methodFilterFileName);
                    List<string> methodFilterContents = new List<string>();
                    while (!methodFilterFile.EndOfStream)
                    {
                        methodFilterContents.Add(methodFilterFile.ReadLine().Trim());
                    }

                    bool allEntriesFound = true;
                    foreach (FaultRule r in rules)
                    {
                        string comSignature = Signature.ConvertSignature(r.MethodSignature, SignatureStyle.Com);
                        if (!methodFilterContents.Any(s => s == comSignature))
                        {
                            allEntriesFound = false;
                            errors.AppendLine(
                                string.Format(ApiErrorMessages.MethodFilterFileHasMissingEntry,
                                              Environment.GetEnvironmentVariable(EnvironmentVariable.MethodFilter),
                                              r.MethodSignature,
                                              comSignature));
                        }
                    }
                    if (!allEntriesFound)
                    {
                        errors.AppendLine(string.Format(ApiErrorMessages.MethodFilterFileContentsDump, methodFilterFileName));
                        foreach (string s in methodFilterContents)
                        {
                            errors.AppendLine(s);
                        }
                    }
                }
                catch (IOException e)
                {
                    if (methodFilterFile != null)
                    {
                        methodFilterFile.Dispose();
                    }
                    errors.AppendLine(string.Format(ApiErrorMessages.MethodFilterFileAccessDenied, methodFilterFileName));
                    errors.Append("\t");
                    errors.AppendLine(e.Message);
                }
            }

            // Verify FaultDispatcher.Trap() method exists and can be loaded by the current runtime
            FaultDispatcherDelegate t = FaultDispatcher.Trap;

            // Verify FaultDispatcher assembly has the public key token that the engine expects
            string faultDispatcherAssemblyName = typeof(FaultDispatcher).Assembly.FullName;
            if (!faultDispatcherAssemblyName.Contains(EngineInfo.FaultDispatcherAssemblyPublicKeyToken))
            {
                errors.AppendLine(string.Format(ApiErrorMessages.FaultDispatcherCannotBeLoaded, faultDispatcherAssemblyName));
            }

            // Now throw the errors if they exist
            if (errors.Length != 0)
            {
                throw new FaultInjectionException(errors.ToString());
            }
        }

        private delegate bool FaultDispatcherDelegate(out Exception e, out object o);
    }
}
