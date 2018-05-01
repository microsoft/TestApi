// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Test.FaultInjection.Constants
{
    internal static class EngineInfo
    {
        // FaultSession namespace
        public const string NameSpace = "Microsoft.Test.FaultInjection";

        public const string FaultDispatcherAssemblyPublicKeyToken = "3d18d97752fc57cc";
        public const int FaultDispatcherAssemblyRuntimeVersion = 4;

        // CLSID and registry key for profiling callback COM component
        public const string Engine_CLSID = "{2EB6DCDB-3250-4D7F-AA42-41B1B84113ED}";
        public const string Engine_RegistryKey = @"CLSID\" + Engine_CLSID;

        // File name for profiling callback COM dll
        public const string FaultEngineFileName = "FaultInjectionEngine.dll";
    }
}