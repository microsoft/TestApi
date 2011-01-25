// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Test.FaultInjection.Constants
{
    internal static class ApiErrorMessages
    {
        // Error messages for class FaultRule
        public const string ConditionNull = "Condition object of a fault rule can't be null.";
        public const string FaultNull = "Fault object of a fault rule can't be null.";

        // Error messages for class FaultSession
        public const string FaultRulesNullOrEmpty = "Fault rules used to initialize FaultSession instance can't be null or empty.";
        public const string FaultRulesConflict = "Each method can attach only one FaultRule in one FaultSession.";
        public const string LauncherNull = "Launch delegate can't be null";
        public const string UnableToCreateFileReadWriteMutex = "Unable to create Mutex for file I/O with name \"{0}\".";
        public const string RegisterEngineFailed = "Register fault injection engine file \"{1}\" failed. Regsvr32 return error code 0x{0:X} ";
        public const string RegisterEngineAccessDenied = "Register fault injection engine file \"{1}\" failed. You should run fault injection tool as Administrator(e.g. Run cmd or Visual Studio as Administrator), error code 0x{0:X}.";
        public const string RegisterEngineFileNotFound = "Cannot find fault injection engine file \"{1}\". Please check if the file exists, error code 0x{0:X}.";
        public const string LogDirectoryNullOrEmpty = "Directory for log files can't be null or empty.";

        // Error messages for FaultScope
        public const string FaultScopeExists = "A FaultScope is already open in this process. Dispose that FaultScope before creating a new one.";
        public const string EngineNotRegistered = "Fault injection engine is not registered. Execute 'regsvr32 " + EngineInfo.FaultEngineFileName + "' to register it.";
        public const string ProfilerNotEnabled = "Environment variable " + EnvironmentVariable.EnableProfiling + " must be set it to 1 in order to enable the fault injection engine.";
        public const string ProfilerNotSpecified = "Environment variable " + EnvironmentVariable.Proflier + " is set to {0}. Please set it to {" + EngineInfo.Engine_CLSID + "} in order to enable the fault injection engine.";
        public const string ProfilerV2CompatibilityNotSpecified = "Environment variable " + EnvironmentVariable.Proflier + " must be set to " + EnvironmentVariable.EnableV2Profiler + " in order to enable the fault injection engine.";
        public const string LogDirectoryNotSet = "Environment variable " + EnvironmentVariable.LogDirectory + " was not found." + LogDirectoryExample;
        public const string LogDirectoryAccessDenied = "Environment variable " + EnvironmentVariable.LogDirectory + " has value {0} but that directory does not exist or cannot be opened." + LogDirectoryExample;
        public const string LogDirectoryExample = "Please set it to the name of a directory where log files should be placed.";
        public const string MethodFilterFileNotSet = "Environment variable " + EnvironmentVariable.MethodFilter + " was not found. " + MethodFilterFileExample;
        public const string MethodFilterFileAccessDenied = "Environment variable " + EnvironmentVariable.MethodFilter + " has value {0} but that file does not exist or cannot be opened." + MethodFilterFileExample;
        public const string MethodFilterFileExample = "Please set it to the full absolute path of a file that contains the name of the methods to inject faults into.";
        public const string MethodFilterFileHasMissingEntry = "Method filter file {0} does not contain an entry for the method {1}, so the fault injection engine will ignore it.\nPlease add this text on a new line to that file:\n{2}";
        public const string NoFaultRulesLoaded = "No fault rules were loaded";
        public const string FaultDispatcherCannotBeLoaded = "Fault dispatcher assembly {0} does not have the public key token " + EngineInfo.FaultDispatcherAssemblyPublicKeyToken + " so the fault injection engine will fail to load it.";
        public const string MethodFilterFileContentsDump = "Contents of method filter file {0} are as follows:";

        // Error messages for FaultRuleLoader
        public const string UnableToFindEnvironmentVariable = "Can't find environment variable \"{0}\"";

        // Error messages for faults
        public const string ExceptionNull = "The exception object used to initialize ThrowExceptionFault can't be null.";
        public const string ExpressionNullOrEmpty = "Expression to specify exception want to be thrown can't be null or empty.";

        // Error messages for Helpers
        public const string MethodSignatureNullOrEmpty = "Method signature can't be null or empty.";
        public const string InvalidMethodSignature = "Invalid method signature: {0}";
        public const string FaultRulesNullInSerialization = "Can't serialize null FaultRule[] instance";
        public const string GenericConstructorNotSupported = "Fault injection into the constructor of a generic type is currently unsupported";
    }
}