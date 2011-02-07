// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#pragma once

BEGIN_DEFAULT_NAMESPACE

#pragma region Optimization Parameters

//  Adjust following parameters to optimize this component.
//      Increase those values always cause memory consuming, while decrease them
//  may cause some API functions being called twice. If consider only correctness
//  of the component, following parameters can be any positive integer.

#define PREFERRED_SLEEP_TIME_IN_MILLISECONDS        128
#define PREFERRED_DUPLICATED_ASSEMBLY_COUNT         8
#define PREFERRED_FILE_PATH_NAME_LENGTH             512
#define PREFERRED_QUALIFIED_TYPE_NAME_LENGTH        1024
#define PREFERRED_QUALIFIED_METHOD_NAME_LENGTH      1024
#define PREFERRED_NONQUALIFIED_METHOD_NAME_LENGTH   256

#pragma endregion

#pragma region Declaration of CSettings

class CSettings
{
public:
    static UINT GetEventLogLevel(void);
    static LPCTSTR GetEventLogFolder(void);
    static LPCTSTR GetMethodFilterFile(void);
    static LPCTSTR GetCLISystemAssemblyName(void);
    static LPCTSTR GetDispatcherAssemblyName(void);
    static LPCTSTR GetDispatcherFullQualifiedClassName(void);
    static LPCTSTR GetDispatcherNonQualifiedMethodName(void);
    static LPCTSTR GetQualifiedNameSeparatorBeforeMethod(void);
    static LPCTSTR GetQualifiedNameSeparatorBeforeNestedType(void);
    static const LPCTSTR* GetProtectedNamespaceList(void);
};

#pragma endregion

END_DEFAULT_NAMESPACE
