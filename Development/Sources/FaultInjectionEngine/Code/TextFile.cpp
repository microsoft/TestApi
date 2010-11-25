// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#include "stdafx.h"
#include "TextFile.h"
#include "TraceAndLog.h"

USING_DEFAULT_NAMESPACE

#pragma region Implementation of CTextFile

const LPCTSTR CTextFile::modeRead = _T("rt");
const LPCTSTR CTextFile::modeWrite = _T("wt");

CTextFile::CTextFile(void)
{
    this->m_pFile = NULL;
}

CTextFile::~CTextFile(void)
{
    if(NULL != this->m_pFile)
        ::fclose(this->m_pFile);
}

BOOL CTextFile::Open(LPCTSTR pstrPathName, LPCTSTR pstrOpenMode)
{
    ASSERT(NULL != pstrPathName);
    ASSERT(NULL != pstrOpenMode);

    ASSERT(!this->IsOpened());  // Do NOT reuse one instance to open multiple files.

#if defined(_DEBUG)
    this->m_szFilePathName = pstrPathName;
#endif

    this->m_pFile = ::_tfopen(pstrPathName, pstrOpenMode);
    if(NULL == this->m_pFile)
    {
        DebugTrace(_T("Failed to open file '%s'. Error : %08X"), pstrPathName, ::GetLastError());
        return FALSE;
    }
    
    // It seems the _tfopen_s is not supported under Windows XP/2003.

    //errno_t nErrorCode = ::_tfopen_s(&(this->m_pFile), pstrPathName, pstrOpenMode);
    //if(0 != nErrorCode)
    //{
    //    // Do NOT use EventReport here, the log file may be not ready.
    //    DebugTrace(_T("Failed to open file '%s'. Error : %08X"), pstrPathName, ::GetLastError());

    //    this->m_pFile = NULL;
    //    return FALSE;
    //}

    ASSERT(NULL != this->m_pFile);
    return TRUE;
}

BOOL CTextFile::IsOpened() const
{
    return NULL != this->m_pFile;
}

BOOL CTextFile::IsEndOfFile(void)
{
    return (NULL == this->m_pFile) || ::feof(this->m_pFile);
}

CString CTextFile::ReadLine(int nPreferredLineLength)
{
    ASSERT(0 < nPreferredLineLength);

    if(!this->IsOpened())
        return _T("");

    CString szResultLine;
    while(!::feof(this->m_pFile))
    {
        CString szBuffer;
        LPTSTR rv = ::_fgetts(szBuffer.GetBufferSetLength(nPreferredLineLength), nPreferredLineLength, this->m_pFile);
        if(NULL == rv) // error or eof
        {
            if(::ferror(this->m_pFile))
            {
#if defined(_DEBUG)
                // Do NOT use EventReport here, the log file may be not ready.
                DebugTrace(_T("Failed to read file '%s'. Error : %08X"),
                    this->m_szFilePathName, ::ferror(this->m_pFile));
#endif
                ::fclose(this->m_pFile);
                this->m_pFile = NULL;
                return _T("");
            }

            ASSERT(::feof(this->m_pFile));
            return szResultLine;
        }
        szBuffer.ReleaseBuffer();

        if(szBuffer.Right(1).FindOneOf(_T("\x0A\x0D\x0D0A\x0A0D")) >= 0) // End Of Line
        {
            szResultLine += szBuffer.Left(szBuffer.GetLength() - 1);
            break;
        }
        szResultLine += szBuffer;
    }
    return szResultLine;
}

BOOL CTextFile::WriteText(LPCTSTR pstrText)
{
    ASSERT(NULL != pstrText);

    if(!IsOpened())
        return FALSE;

    int rv = ::_fputts(pstrText, this->m_pFile);
    if(_TEOF == rv) // error
    {
#if defined(_DEBUG)
        // Do NOT use EventReport here, the log file may be not ready.
        DebugTrace(_T("Failed to write string '%s' to file '%s'."), pstrText, this->m_szFilePathName);
#endif
        return FALSE;
    }

    return TRUE;
}

#pragma endregion
