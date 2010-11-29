// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

//
//  Declarations of classes CTraceAndLog, CEventLog, and CDebugTrace
//  Being base class of CEventLog and CDebugTrace, CTraceAndLog provides common
//  implementation of log and trace. CEventLog is designed for write events to
//  log files. CDebugTrace use ATLTRACE to output debug information.
//

#pragma once
#include "resource.h"
#include "TextFile.h"
#include "MemoryRef.h"

BEGIN_DEFAULT_NAMESPACE

#define EventReportInfo     CEventLog(IDS_EVENT_LEVEL_INFO, __FILE__, __LINE__)
#define EventReportWarning  CEventLog(IDS_EVENT_LEVEL_WARNING, __FILE__, __LINE__)
#define EventReportError    CEventLog(IDS_EVENT_LEVEL_ERROR, __FILE__, __LINE__)

#if defined(_DEBUG)
#define DebugTrace  CDebugTrace(IDS_EVENT_LEVEL_TRACE, __FILE__, __LINE__)
#define DebugDump   CDebugTrace(IDS_EVENT_LEVEL_DUMP, __FILE__, __LINE__)
#else
#define DebugTrace
#define DebugDump
#endif

#pragma region Declaration of CTraceAndLog

class CTraceAndLog
{
protected:
    CTraceAndLog() {};
    CTraceAndLog(UINT nEventLevelId, const char *pszSourceFileName, int nSourceLineNumber);

protected:
    CString m_szPrefix;
    UINT m_nEventLevelId;
};

#pragma endregion

#pragma region Declaration of CDebugTrace

class CDebugTrace : public CTraceAndLog
{
public:
    CDebugTrace(UINT nEventLevel, const char *pstrSourceFile, int nSourceLine)
        : CTraceAndLog(nEventLevel, pstrSourceFile, nSourceLine){};

public:
    CDebugTrace& _cdecl operator ()(LPCTSTR pstrMessage, ...);
    CDebugTrace& _cdecl operator ()(CMemoryRef xMemoryDump, LPCTSTR pstrMessage, ...);
};

#pragma endregion

#pragma region Declaration of CEventLog

class CEventLog : public CTraceAndLog
{
public:
    CEventLog(UINT nEventLevel, const char *pstrSourceFile, int nSourceLine)
        : CTraceAndLog(nEventLevel, pstrSourceFile, nSourceLine){};

public:
    CEventLog& _cdecl operator ()(UINT nMessageId, ...);

public:
    static BOOL Initialize(void);

private:
    static void LockEventLogFile(void);
    static void UnlockEventLogFile(void);
    static BOOL CarefullyWriteLogFile(LPCTSTR pstrText);

private:
    static LONG m_nEventLogFileLock;  // Help to make log-file writing critical section.
    static CWriteTextFile m_xEventLogFile;
};

#pragma endregion

END_DEFAULT_NAMESPACE
