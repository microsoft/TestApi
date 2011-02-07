// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#pragma once

#include "MemoryRef.h"

BEGIN_DEFAULT_NAMESPACE

struct CSignatureBlob : public CMemoryRef
{
public:
    CSignatureBlob(void) : CMemoryRef() {};
    CSignatureBlob(LPCVOID pBase, size_t nSize) : CMemoryRef(pBase, nSize) {};

public:
    void EnsureWithin(PCCOR_SIGNATURE pTailOfSignature) const;
    BOOL ParseRetTypeSig(PCCOR_SIGNATURE &pSignature) const;
    BOOL ParseRetTypeSig(CorElementType nElementType, PCCOR_SIGNATURE &pSignature) const;
    BOOL ParseTypeSig(PCCOR_SIGNATURE &pSignature) const;
    BOOL ParseTypeSig(CorElementType nElementType, PCCOR_SIGNATURE &pSignature) const;
    BOOL ParseMethodSig(PCCOR_SIGNATURE &pSignature) const;
    BOOL ParseParameterSig(PCCOR_SIGNATURE &pSignature) const;
    BOOL ParseParameterSig(CorElementType nElementType, PCCOR_SIGNATURE &pSignature) const;
    BOOL ParseArrayShapeSig(PCCOR_SIGNATURE &pSignature) const;

protected:
    PCCOR_SIGNATURE RefSignature(void) const;
    CorElementType BypassOptCustomMod(PCCOR_SIGNATURE& pSignature) const;
    ULONG GetParamCountFromMethodSig(PCCOR_SIGNATURE &pSignature) const;
};

END_DEFAULT_NAMESPACE
