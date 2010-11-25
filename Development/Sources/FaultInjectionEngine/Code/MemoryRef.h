// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

//
//  CMemoryRef represent a block of memory within [m_pBase, m_pBase+m_nSize).
//  We provide this structure to protected the memory access, to ensure every
//  operation does not exceed the bound of the memory.
//

#pragma once

BEGIN_DEFAULT_NAMESPACE

struct CMemoryRef
{
public:
    CMemoryRef(void) { Attach(NULL, 0); };
    CMemoryRef(LPCVOID pBase, size_t nSize) { Attach(pBase, nSize); };

public:
    void Attach(LPCVOID pBase, size_t nSize)
    {
        ASSERT((0 == nSize && NULL == pBase) || (0 < nSize && NULL != pBase));

        m_nSize = nSize;
        m_pBase = pBase;
    };

public:
    BOOL IsNull() const
    {
        return (NULL == m_pBase);
    };
    LPCVOID GetBaseAddress() const
    {
        return m_pBase;
    };
    LPCVOID GetTailAddress() const
    {
        return ((LPCBYTE)m_pBase) + m_nSize;
    };
    size_t GetSize() const
    {
        return m_nSize;
    };
    BYTE operator [] (size_t i) const
    {
#pragma warning(push)
#pragma warning(disable: 4296)
        ASSERT(0 <= i && i < m_nSize);
#pragma warning(pop)
        return ((LPCBYTE)(m_pBase))[i];
    };

public:
    void MemoryCopy(const CMemoryRef &xSource)
    {
        MemoryCopyAt(0, xSource, xSource.m_nSize);
    };
    void MemoryCopy(const CMemoryRef &xSource, size_t nSizeToCopy)
    {
        MemoryCopyAt(0, xSource, nSizeToCopy);
    };
    void MemoryCopyAt(size_t nOffset, const CMemoryRef &xSource)
    {
        MemoryCopyAt(nOffset, xSource, xSource.m_nSize);
    }
    void MemoryCopyAt(size_t nOffset, const CMemoryRef &xSource, size_t nSizeToCopy)
    {
        ASSERT(NULL != m_pBase);
        ASSERT(NULL != xSource.m_pBase);
        ASSERT(nSizeToCopy <= xSource.m_nSize);
        ASSERT(nOffset + nSizeToCopy <= m_nSize);

        ::memmove_s(((LPBYTE)(m_pBase)) + nOffset, m_nSize - nOffset,
            xSource.m_pBase, nSizeToCopy);
    };

protected:
    BOOL HitTest(LPCVOID pTest, BOOL bExclusive = TRUE) const
    {
        if(bExclusive)
            return m_pBase <= pTest && pTest < (LPCBYTE)m_pBase + m_nSize;
        else
            return m_pBase <= pTest && pTest <= (LPCBYTE)m_pBase + m_nSize;
    };

protected:
    template<typename T> BOOL IsEnough(size_t nOffset = 0) const
    {
        return nOffset + sizeof(T) <= m_nSize;
    };

protected:
    friend class CTraceAndLog;
    LPCVOID m_pBase;
    size_t  m_nSize;  // in bytes
};

END_DEFAULT_NAMESPACE
