// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#include "stdafx.h"

#include <shellapi.h>

#include "Settings.h"
#include "TraceAndLog.h"

USING_DEFAULT_NAMESPACE

#pragma region Implementation of CTraceAndLog

CTraceAndLog::CTraceAndLog(UINT nEventLevel, const char *pstrSourceFile, int nSourceLine)
{
    ASSERT(IDS_EVENT_LEVEL_ERROR == nEventLevel
        || IDS_EVENT_LEVEL_WARNING == nEventLevel
        || IDS_EVENT_LEVEL_INFO == nEventLevel
        || IDS_EVENT_LEVEL_TRACE == nEventLevel
        || IDS_EVENT_LEVEL_DUMP == nEventLevel);

    this->m_nEventLevelId = nEventLevel;
    this->m_szPrefix.FormatMessage(nEventLevel, ::GetCurrentProcessId(), ::GetCurrentThreadId());

    if(IDS_EVENT_LEVEL_WARNING >= nEventLevel)
    {
        // Trace file/line location when debug.
        ATLTRACE("\n@Ln %d of '%s':\n", nSourceLine, pstrSourceFile);
    }
};

#pragma endregion

#pragma region Implementation of CEventLog

#pragma region Static Variables (Log File and its Lock)

// The two values must, and only must, be different.
#define EVENT_LOG_FILE_LOCKED     1
#define EVENT_LOG_FILE_UNLOCKED   0
LONG CEventLog::m_nEventLogFileLock = EVENT_LOG_FILE_UNLOCKED;
CWriteTextFile CEventLog::m_xEventLogFile;

#pragma endregion

#pragma region Static Methods (Operate Log File)

void CEventLog::LockEventLogFile(void)
{
    // Lock the log-file before write to it.
    while(EVENT_LOG_FILE_UNLOCKED != ::InterlockedCompareExchange(&m_nEventLogFileLock,
        EVENT_LOG_FILE_LOCKED, EVENT_LOG_FILE_UNLOCKED))
    {
        ::Sleep(PREFERRED_SLEEP_TIME_IN_MILLISECONDS);
    }
}

void CEventLog::UnlockEventLogFile(void)
{
    m_nEventLogFileLock = EVENT_LOG_FILE_UNLOCKED;
}

BOOL CEventLog::CarefullyWriteLogFile(LPCTSTR pstrText)
{
    ASSERT(NULL != pstrText);

    if(!m_xEventLogFile.IsOpened())
        return FALSE;

    LockEventLogFile();
    BOOL rv = FALSE;
    try
    {
        rv = m_xEventLogFile.WriteText(pstrText);
    }
    catch(...)
    {
        UnlockEventLogFile();
        throw;
    }
    UnlockEventLogFile();
    return rv;
}

BOOL CEventLog::Initialize(void)
{
    // Get name of the process
    int nCmdLineArgNumber = 0;
    LPWSTR *ppstrCmdLineArgList = ::CommandLineToArgvW(::GetCommandLineW(), &nCmdLineArgNumber);
    CString szProcessName;
    if(NULL == ppstrCmdLineArgList)
    {
        // Command line not found. Use process id as name.
        szProcessName.FormatMessage(IDS_PROCESS_ID_AS_NAME, ::GetCurrentProcessId());
    }
    else
    {
        ASSERT(0 < nCmdLineArgNumber);

        // Remove directory pathname to get the application name.
        CString szExecutionCmd = ppstrCmdLineArgList[0];
        int nOffset = szExecutionCmd.ReverseFind(_T('\\')) + 1;
        if(nOffset > 0)
            szProcessName = szExecutionCmd.Right(szExecutionCmd.GetLength() - nOffset);
        else
            szProcessName = szExecutionCmd;
    }

    // Get current date-time
    ::SYSTEMTIME xDateTime;
    ::GetLocalTime(&xDateTime);

    // Form event log file's pathname
    CString szLogFilePathname;
    szLogFilePathname.FormatMessage(IDS_EVENT_LOG_FILE_PATHNAME,
        CSettings::GetEventLogFolder(), szProcessName,
        xDateTime.wYear, xDateTime.wMonth, xDateTime.wDay,
        xDateTime.wHour, xDateTime.wMinute, xDateTime.wSecond);

    // Open event log file for write.
    BOOL rv = m_xEventLogFile.Open(szLogFilePathname);
    if(rv)
    {
        DebugTrace(_T("EventLog will write to file '%s'."), szLogFilePathname);
    }

    // Check preprocessor definitions at debug mode only.
    ASSERT(IDS_EVENT_LEVEL_ERROR < IDS_EVENT_LEVEL_WARNING
        && IDS_EVENT_LEVEL_WARNING < IDS_EVENT_LEVEL_INFO
        && IDS_EVENT_LEVEL_INFO < IDS_EVENT_LEVEL_TRACE
        && IDS_EVENT_LEVEL_TRACE < IDS_EVENT_LEVEL_DUMP);

    return rv;
}

#pragma endregion

#pragma region Override Operators for Event Report

#pragma warning(push)
#pragma warning(disable : 4793)
CEventLog& _cdecl CEventLog::operator ()(UINT nFormatId, ...)
{
    // Format message
    CString szFormat;
    szFormat.LoadString(nFormatId);

    // Format message
    CString szMessage;
    va_list pVariableArgumentsList;
    va_start(pVariableArgumentsList, nFormatId);
    szMessage.FormatMessageV(szFormat, &pVariableArgumentsList);
    va_end(pVariableArgumentsList);

    // Write log and trace out
    szMessage = this->m_szPrefix + szMessage + _T("\n");
    CarefullyWriteLogFile(szMessage);
    ATLTRACE(szMessage);
    return *this;
};
#pragma warning(pop)

#pragma endregion

#pragma endregion

#pragma region Implementation of CDebugTrace

#pragma warning(push)
#pragma warning(disable : 4793)
CDebugTrace& _cdecl CDebugTrace::operator ()(LPCTSTR pstrFormat, ...)
{
    // Format message
    CString szMessage;
    va_list pVariableArgumentsList;
    va_start(pVariableArgumentsList, pstrFormat);
    szMessage.FormatV(pstrFormat, pVariableArgumentsList);
    va_end(pVariableArgumentsList);

    // Trace out
    ATLTRACE(this->m_szPrefix + szMessage + _T("\n"));
    return *this;
};

CDebugTrace& _cdecl CDebugTrace::operator ()(CMemoryRef xMemoryDump, LPCTSTR pstrFormat, ...)
{
    // Format message
    CString szMessage;
    va_list pVariableArgumentsList;
    va_start(pVariableArgumentsList, pstrFormat);
    szMessage.FormatV(pstrFormat, pVariableArgumentsList);
    va_end(pVariableArgumentsList);

    // Attach memory address and trace out
    CString szAddressInfo;
    szAddressInfo.FormatMessage(IDS_MEMORY_DUMP_ADDRESS_INFO,
        xMemoryDump.GetBaseAddress(), xMemoryDump.GetTailAddress(), xMemoryDump.GetSize());
    ATLTRACE(this->m_szPrefix + szMessage + szAddressInfo + _T("\n"));

    // Format memory dump and trace out
    CString szLine;
    for(size_t i = 0; i < xMemoryDump.GetSize(); i++)
    {
        if(0 == i % 8)
        {
            if(0 != i)  // end of line, trace out
            {
                ATLTRACE(this->m_szPrefix + szLine + _T("\n"));
                szLine.Empty();
            }
            szLine.Format(_T("#%03X:  "), i);  // head of line, offset
        }
        else if(0 == i % 4)
        {
            szLine += _T("  ");
        }

        CString szByte;
        szByte.Format(_T("%02X "), xMemoryDump[i]);
        szLine += szByte;
    }
    if(!szLine.IsEmpty())
    {
        ATLTRACE(this->m_szPrefix + szLine + _T("\n"));
    }
    return *this;
};
#pragma warning(pop)

#pragma endregion
