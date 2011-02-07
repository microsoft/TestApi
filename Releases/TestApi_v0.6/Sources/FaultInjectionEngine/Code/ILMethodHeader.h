// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

//
//  CILMethodHeader represent a block of memory that contains IL Method Header.
//

#pragma once
#include "MemoryRef.h"

BEGIN_DEFAULT_NAMESPACE

class CILMethodHeader : public CMemoryRef
{
public:
    CILMethodHeader(void) : CMemoryRef() {};
    CILMethodHeader(LPCVOID pMemory, size_t nSize) : CMemoryRef(pMemory, nSize) {};

public:
    BOOL IsFat(void) const;
    BOOL IsTiny(void) const;
    size_t GetSize(void) const;  // size of the header, in bytes
    ULONG GetCodeSize(void) const;
    void SetCodeSize(ULONG nSize);
    ULONG GetMaxStack(void) const;
    void SetMaxStack(ULONG nMaxStack);
    mdSignature GetLocalVarToken(void) const;
    void SetLocalVarToken(mdSignature tkLocalVarSig);

protected:
    COR_ILMETHOD_FAT* PtrFatHeader(void) const;
    COR_ILMETHOD_TINY* PtrTinyHeader(void) const;
};

END_DEFAULT_NAMESPACE
