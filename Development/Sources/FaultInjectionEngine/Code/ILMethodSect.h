// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

//
//  CILMethodSect represent a block of memory that contains IL Method (Structured
//  Exception Handler) Section.
//

#pragma once
#include "MemoryRef.h"

BEGIN_DEFAULT_NAMESPACE

class CILMethodSect : public CMemoryRef
{
public:
    CILMethodSect(void) : CMemoryRef() {};
    CILMethodSect(LPCVOID pMemory, size_t nSize) : CMemoryRef(pMemory, nSize) {};

public:
    void Attach(LPCVOID pMemory, size_t nSize) { CMemoryRef::Attach(pMemory, nSize); };

public:
    BOOL IsFat(void) const;
    BOOL IsSmall(void) const;
    BOOL IsExceptionHandler(void) const;
    BOOL AreThereMoreSections(void) const;
    void SetAsHasMoreSections(BOOL bMore = TRUE);
    // section data-size is the size (in bytes) of whole section!
    size_t GetSectionDataSize(void) const;
    void SetSectionDataSize(size_t nSize);
    int GetExceptionHandlerClauseCount(void) const;
    CILMethodSect GetNextSection(void) const;
    IMAGE_COR_ILMETHOD_SECT_EH_CLAUSE_FAT &GetFatExceptionHandlerClause(int nIndex) const;
    IMAGE_COR_ILMETHOD_SECT_EH_CLAUSE_SMALL &GetSmallExceptionHandlerClause(int nIndex) const;

protected:
    CorILMethodSect GetSectionKind(void) const;
    void SetSectionKind(CorILMethodSect nKind);
    COR_ILMETHOD_SECT_EH* PtrSection(void) const;
    COR_ILMETHOD_SECT_FAT* PtrFatSection(void) const;
    COR_ILMETHOD_SECT_SMALL* PtrSmallSection(void) const;
};

END_DEFAULT_NAMESPACE
