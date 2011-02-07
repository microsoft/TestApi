// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

//
//  If the environment variables and default values changed, modify the settings
//  in this file. This file also contains the settings of predefied strings that
//  should be align to other components of Fault Injection Tool.
//

#include "stdafx.h"
#include "resource.h"
#include "Settings.h"
#include "TraceAndLog.h"

BEGIN_DEFAULT_NAMESPACE

#pragma region Predefined Strings

//  Following strings should ONLY be modified to align to the settings of
//  other components of Fault Injection.

#define CLI_SYSTEM_ASSEMBLY_NAME    _T("mscorlib")
#define DISPATCHER_ASSEMBLY_NAME    _T("TestApiCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=3d18d97752fc57cc, processorArchitecture=MSIL")
#define DISPATCHER_CLASS_NAME       _T("Microsoft.Test.FaultInjection.FaultDispatcher")
#define DISPATCHER_METHOD_NAME      _T("Trap")
#define QUALIFIED_NAME_SEPARATOR    _T(".")
const LPCTSTR PROTECTED_NAMESPACE_LIST[] = {
    _T("Microsoft.Test.FaultInjection."),
    NULL    // Must be NULL terminated!
};

#pragma endregion

#pragma region Environment Variables & Values

#define ENV_VAR_METHOD_FILTER_FILE  _T("FAULT_INJECTION_METHOD_FILTER")
#define ENV_VAR_EVENT_LOG_FOLDER    _T("FAULT_INJECTION_LOG_DIR")
#define ENV_VAR_EVENT_LOG_LEVEL     _T("FAULT_INJECTION_LOG_LEVEL")

#define ENV_VAL_EVENT_LOG_LEVEL_ERROR   _T("ERROR")
#define ENV_VAL_EVENT_LOG_LEVEL_WARNING _T("WARNING")
#define ENV_VAL_EVENT_LOG_LEVEL_INFO    _T("INFO")

#pragma endregion

#pragma region Helper Functions

static CString GetEnvironment(LPCTSTR pstrEnvironmentVariable, DWORD nPreferredValueLength)
{
    ASSERT(0 < nPreferredValueLength);

    CString szEnvironmentValue;
    DWORD nLength = ::GetEnvironmentVariable(pstrEnvironmentVariable,
        szEnvironmentValue.GetBufferSetLength(nPreferredValueLength), nPreferredValueLength);
    if(nLength > nPreferredValueLength)
    {
        // nPreferredValueLength is not large enough. nLength is the required buffer size.
        // Let's do it again.
        nLength = ::GetEnvironmentVariable(pstrEnvironmentVariable, szEnvironmentValue.GetBufferSetLength(nLength), nLength);
    }

    if(0 == nLength)
    {
        // Specified environment variable is not found.
        // Do NOT use EventReport here, the log file may be not ready.
        DebugTrace(_T("EnvVar[%s] is NOT found!"), pstrEnvironmentVariable);

        return _T("");
    }

    // If success, nLength holds the number of characters stored, excluding the terminating null.
    szEnvironmentValue.ReleaseBufferSetLength(nLength);

    // Do NOT use EventReport here, the log file may be not ready.
    DebugTrace(_T("EnvVar[%s]=%s"), pstrEnvironmentVariable, (LPCTSTR)szEnvironmentValue);

    return szEnvironmentValue;
}

static UINT GetEventLogLevel(void)
{
    CString szEventLogLevel = GetEnvironment(ENV_VAR_EVENT_LOG_LEVEL, 8);
    
    // Using event-log specified by environment-variable. If no match, log ERROR only.
    if(szEventLogLevel == ENV_VAL_EVENT_LOG_LEVEL_INFO)
        return IDS_EVENT_LEVEL_INFO;
    else if(szEventLogLevel == ENV_VAL_EVENT_LOG_LEVEL_WARNING)
        return IDS_EVENT_LEVEL_WARNING;
    else
        return IDS_EVENT_LEVEL_ERROR;
}

#pragma endregion

#pragma region Settings Loaded from Environment

UINT _nEventLogLevel = GetEventLogLevel();

CString _szEventLogFolder = GetEnvironment(
    ENV_VAR_EVENT_LOG_FOLDER, PREFERRED_FILE_PATH_NAME_LENGTH);

CString _szMethodFilterFile = GetEnvironment(
    ENV_VAR_METHOD_FILTER_FILE, PREFERRED_FILE_PATH_NAME_LENGTH);

#pragma endregion

#pragma region Implementation of CSettings

UINT CSettings::GetEventLogLevel(void)
{
    return _nEventLogLevel;
}

LPCTSTR CSettings::GetEventLogFolder(void)
{
    return _szEventLogFolder;
}

LPCTSTR CSettings::GetMethodFilterFile(void)
{
    return _szMethodFilterFile;
}

LPCTSTR CSettings::GetCLISystemAssemblyName(void)
{
    return CLI_SYSTEM_ASSEMBLY_NAME;
}

LPCTSTR CSettings::GetDispatcherAssemblyName(void)
{
    return DISPATCHER_ASSEMBLY_NAME;
}

LPCTSTR CSettings::GetDispatcherFullQualifiedClassName(void)
{
    return DISPATCHER_CLASS_NAME;
}

LPCTSTR CSettings::GetDispatcherNonQualifiedMethodName(void)
{
    return DISPATCHER_METHOD_NAME;
}

LPCTSTR CSettings::GetQualifiedNameSeparatorBeforeMethod(void)
{
    return QUALIFIED_NAME_SEPARATOR;
}

LPCTSTR CSettings::GetQualifiedNameSeparatorBeforeNestedType(void)
{
    return QUALIFIED_NAME_SEPARATOR;
}

const LPCTSTR* CSettings::GetProtectedNamespaceList(void)
{
    return PROTECTED_NAMESPACE_LIST;
}

#pragma endregion

END_DEFAULT_NAMESPACE