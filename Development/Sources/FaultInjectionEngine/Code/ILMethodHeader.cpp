// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#include "stdafx.h"
#include "ILMethodHeader.h"

USING_DEFAULT_NAMESPACE

COR_ILMETHOD_TINY* CILMethodHeader::PtrTinyHeader(void) const
{
    return (COR_ILMETHOD_TINY*)(this->m_pBase);
}

COR_ILMETHOD_FAT* CILMethodHeader::PtrFatHeader(void) const
{
    return (COR_ILMETHOD_FAT*)(this->m_pBase);
}

BOOL CILMethodHeader::IsTiny(void) const
{
    ASSERT(NULL != this->m_pBase);
    ASSERT(this->IsEnough<IMAGE_COR_ILMETHOD_TINY>());

    return this->PtrTinyHeader()->IsTiny();
}

BOOL CILMethodHeader::IsFat(void) const
{
    ASSERT(NULL != this->m_pBase);
    ASSERT(this->IsEnough<IMAGE_COR_ILMETHOD_TINY>());  // Do NOT compare with FAT header size

    return this->PtrFatHeader()->IsFat();
}

size_t CILMethodHeader::GetSize(void) const
{
    if(this->IsTiny())
    {
        return sizeof(IMAGE_COR_ILMETHOD_TINY);
    }
    else
    {
        ASSERT(this->IsFat());
        // Do NOT forget convert size-in-dwords to size-in-bytes.
        return this->PtrFatHeader()->GetSize() * sizeof(DWORD);
    }
}

ULONG CILMethodHeader::GetCodeSize(void) const
{
    if(this->IsTiny())
        return this->PtrTinyHeader()->GetCodeSize();
    else
        return this->PtrFatHeader()->GetCodeSize();
}

void CILMethodHeader::SetCodeSize(ULONG nSize)
{
    ASSERT(this->IsFat());  // Currently not support tiny header.
    this->PtrFatHeader()->SetCodeSize(nSize);
}

ULONG CILMethodHeader::GetMaxStack(void) const
{
    if(this->IsTiny())
        return 8;
    else
        return this->PtrFatHeader()->GetMaxStack();
}

void CILMethodHeader::SetMaxStack(ULONG nMaxStack)
{
    ASSERT(this->IsFat());
    this->PtrFatHeader()->SetMaxStack(nMaxStack);
}

mdSignature CILMethodHeader::GetLocalVarToken(void) const
{
    if(this->IsTiny())
        return mdSignatureNil;
    else
        return this->PtrFatHeader()->GetLocalVarSigTok();
}

void CILMethodHeader::SetLocalVarToken(mdSignature tkLocalVarSig)
{
    ASSERT(this->IsFat());
    this->PtrFatHeader()->SetLocalVarSigTok(tkLocalVarSig);
}