// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#include "stdafx.h"
#include "ILMethodBody.h"

USING_DEFAULT_NAMESPACE

CILMethodHeader CILMethodBody::GetHeader() const
{
    ASSERT(!this->IsNull());
    ASSERT(IsEnough<IMAGE_COR_ILMETHOD_TINY>());

    CILMethodHeader rv(this->m_pBase, this->m_nSize);
    if(rv.IsTiny())
    {
        // Correct the header size.
        rv.Attach(this->m_pBase, sizeof(IMAGE_COR_ILMETHOD_TINY));
    }
    else
    {
        ASSERT(rv.IsFat());
        // Correct the header size.
        rv.Attach(this->m_pBase, rv.GetSize());
    }
    return rv;
}

CMemoryRef CILMethodBody::GetCode(void) const
{
    return CMemoryRef(((LPCBYTE)(this->m_pBase)) + this->GetHeader().GetSize(),
        this->GetHeader().GetCodeSize());
}

CILMethodSect CILMethodBody::GetSect(void) const
{
    ASSERT(!this->IsNull());
    ASSERT(sizeof(IMAGE_COR_ILMETHOD_TINY) <= this->m_nSize);

    if(this->GetHeader().IsTiny())
    {
        return CILMethodSect();  // NULL
    }
    else
    {
        ASSERT(this->GetHeader().IsFat());

        const COR_ILMETHOD_SECT* pSect = ((const COR_ILMETHOD_FAT*)(this->m_pBase))->GetSect();
        if(NULL == pSect)
            return CILMethodSect();  // NULL
        
        ASSERT(LPCBYTE(this->m_pBase) + this->m_nSize > LPCBYTE(pSect));
        
        return CILMethodSect(pSect, ULONG(LPCBYTE(this->m_pBase) + this->m_nSize - LPCBYTE(pSect)));
    }
}
