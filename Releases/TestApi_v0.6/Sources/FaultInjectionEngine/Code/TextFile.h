// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#pragma once

BEGIN_DEFAULT_NAMESPACE

#pragma region Declaration of CTextFile

class CTextFile
{
public:
    CTextFile(void);
    ~CTextFile(void);

public:
    BOOL IsOpened() const;
    BOOL IsEndOfFile();

private:
    FILE* m_pFile;
#if defined(_DEBUG)
    CString m_szFilePathName;  // remember file's pathname (only) for debug trace
#endif

protected:
    BOOL Open(LPCTSTR pstrPathName, LPCTSTR pstrOpenMode);
    CString ReadLine(int nPreferredLineLength);
    BOOL WriteText(LPCTSTR pstrText);

protected:
    static const LPCTSTR modeRead;
    static const LPCTSTR modeWrite;
};

#pragma endregion


#pragma region Declaration of CReadTextFile

class CReadTextFile : public CTextFile
{
public:
    BOOL Open(LPCTSTR pstrPathName)
    {
        return CTextFile::Open(pstrPathName, CTextFile::modeRead);
    };

    CString ReadLine(int nPreferredLineLength)
    {
        return CTextFile::ReadLine(nPreferredLineLength);
    };
};

#pragma endregion


#pragma region Declaration of CWriteTextFile

class CWriteTextFile : public CTextFile
{
public:
    BOOL Open(LPCTSTR pstrPathName)
    {
        return CTextFile::Open(pstrPathName, CTextFile::modeWrite);
    };

    BOOL WriteText(LPCTSTR pstrText)
    {
        return CTextFile::WriteText(pstrText);
    };
};

#pragma endregion

END_DEFAULT_NAMESPACE
