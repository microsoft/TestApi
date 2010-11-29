// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#include "stdafx.h"
#include "ILMethodSect.h"
#include "TraceAndLog.h"

USING_DEFAULT_NAMESPACE

COR_ILMETHOD_SECT_EH* CILMethodSect::PtrSection(void) const
{
#if defined(_DEBUG)
    if(NULL != this->m_pBase)
    {
        // no less than small section without clauses
        ASSERT(IsEnough<IMAGE_COR_ILMETHOD_SECT_SMALL>());
    }
#endif

    return (COR_ILMETHOD_SECT_EH*)(this->m_pBase);
}

COR_ILMETHOD_SECT_FAT* CILMethodSect::PtrFatSection(void) const
{
#if defined(_DEBUG)
    if(NULL != this->m_pBase)
    {
        ASSERT(this->IsFat());
        ASSERT(sizeof(IMAGE_COR_ILMETHOD_SECT_FAT) <= this->m_nSize);  // no less than fat section without clauses
    }
#endif
    return (COR_ILMETHOD_SECT_FAT*)(this->m_pBase);
}

COR_ILMETHOD_SECT_SMALL* CILMethodSect::PtrSmallSection(void) const
{
#if defined(_DEBUG)
    if(NULL != this->m_pBase)
    {
        ASSERT(this->IsSmall());
        ASSERT(sizeof(IMAGE_COR_ILMETHOD_SECT_SMALL) <= this->m_nSize);  // no less than small section without clauses
    }
#endif
    return (COR_ILMETHOD_SECT_SMALL*)(this->m_pBase);
}

BOOL CILMethodSect::IsSmall(void) const
{
    ASSERT(!this->IsNull());

    return !this->IsFat();
}

BOOL CILMethodSect::IsFat(void) const
{
    ASSERT(!this->IsNull());

    return this->PtrSection()->IsFat();
}

BOOL CILMethodSect::IsExceptionHandler(void) const
{
    ASSERT(!this->IsNull());

    return CorILMethod_Sect_EHTable == (this->PtrSection()->Kind() & CorILMethod_Sect_KindMask);
}

BOOL CILMethodSect::AreThereMoreSections(void) const
{
    ASSERT(!this->IsNull());

    return this->PtrSection()->More();
}

void CILMethodSect::SetAsHasMoreSections(BOOL bMore)
{
    ASSERT(!this->IsNull());
    UINT nKind = this->GetSectionKind();
    if(bMore)
        nKind |= CorILMethod_Sect_MoreSects;
    else
        nKind &= ~CorILMethod_Sect_MoreSects;
    this->SetSectionKind((CorILMethodSect)(nKind));
}

CorILMethodSect CILMethodSect::GetSectionKind(void) const
{
    ASSERT(!this->IsNull());

    // MUST NOT use COR_ILMETHOD_SECT::Kind(). It's masked by CorILMethod_Sect_KindMask

    if(this->IsFat())
        return (CorILMethodSect)(this->PtrFatSection()->GetKind());
    else
        return (CorILMethodSect)(this->PtrSmallSection()->Kind);
}

void CILMethodSect::SetSectionKind(CorILMethodSect nKind)
{
    ASSERT(!this->IsNull());
    if(this->IsFat())
        this->PtrFatSection()->SetKind(nKind);
    else
        this->PtrSmallSection()->Kind = (BYTE)(nKind);
}

size_t CILMethodSect::GetSectionDataSize(void) const
{
    if(this->IsNull())
        return 0;

    return this->PtrSection()->DataSize();
}

void CILMethodSect::SetSectionDataSize(size_t nSize)
{
    ASSERT(!this->IsNull());

    if(this->IsFat())
    {
        ASSERT(nSize < (1 << 24));  // IMAGE_COR_ILMETHOD_SECT_FAT { ...; unsigned DataSize : 24; ... }
        this->PtrFatSection()->SetDataSize((unsigned)(nSize));
    }
    else
    {
        ASSERT(nSize < (1 << (8 * sizeof(BYTE))));  // IMAGE_COR_ILMETHOD_SECT_SMALL { ...; BYTE DataSize; ... }
        this->PtrSmallSection()->DataSize = (BYTE)(nSize);
    }
}

//size_t CILMethodSect::GetSectionTotalSize(void) const
//{
//    if(this->IsNull())
//        return 0;
//
//    if(this->IsFat())
//    {
//        // IMAGE_COR_ILMETHOD_SECT_EH_FAT include header, padding, and ONE clause
//        return sizeof(IMAGE_COR_ILMETHOD_SECT_EH_FAT) - sizeof(IMAGE_COR_ILMETHOD_SECT_EH_CLAUSE_FAT)
//            + this->GetSectionDataSize();
//    }
//    else // this->IsSmall()
//    {
//        // IMAGE_COR_ILMETHOD_SECT_EH_SMALL include header, padding, and ONE clause
//        return sizeof(IMAGE_COR_ILMETHOD_SECT_EH_SMALL) - sizeof(IMAGE_COR_ILMETHOD_SECT_EH_CLAUSE_SMALL)
//            + this->GetSectionDataSize();
//    }
//}

int CILMethodSect::GetExceptionHandlerClauseCount(void) const
{
    ASSERT(!this->IsNull());
    ASSERT(this->IsExceptionHandler());

    return this->PtrSection()->EHCount();
}

CILMethodSect CILMethodSect::GetNextSection(void) const
{
    ASSERT(!this->IsNull());

    const COR_ILMETHOD_SECT *pNext = this->PtrSection()->Next();
    if(NULL == pNext)
        return CILMethodSect();  // NULL one
    else
    {
        ASSERT((LPCBYTE)(pNext) < ((LPCBYTE)(this->m_pBase) + this->m_nSize));
        return CILMethodSect((LPVOID)(pNext),
            (ULONG)(((LPCBYTE)(this->m_pBase)) + this->m_nSize - ((LPCBYTE)(pNext))));
    }
}

IMAGE_COR_ILMETHOD_SECT_EH_CLAUSE_SMALL& CILMethodSect::GetSmallExceptionHandlerClause(int nIndex) const
{
    ASSERT(!this->IsNull());
    ASSERT(this->IsSmall() && this->IsExceptionHandler());

    return ((IMAGE_COR_ILMETHOD_SECT_EH_CLAUSE_SMALL*)(this->PtrSection()->Small.Clauses))[nIndex];
}

IMAGE_COR_ILMETHOD_SECT_EH_CLAUSE_FAT& CILMethodSect::GetFatExceptionHandlerClause(int nIndex) const
{
    ASSERT(!this->IsNull());
    ASSERT(this->IsFat() && this->IsExceptionHandler());

    return ((IMAGE_COR_ILMETHOD_SECT_EH_CLAUSE_FAT*)(this->PtrSection()->Fat.Clauses))[nIndex];
}
