// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#include "stdafx.h"
#include "MetadataMethod.h"

USING_DEFAULT_NAMESPACE

void CMetadataMethod::SetMethodDefToken(mdMethodDef tkMethodDef)
{
    this->m_tkMethodDef = tkMethodDef;
    this->SetMetadataLoaded(METADATA_METHOD_DEF_TOKEN);
};

mdMethodDef CMetadataMethod::GetMethodDefToken(void) const
{
    ASSERT(this->IsMetadataLoaded(METADATA_METHOD_DEF_TOKEN));
    return this->m_tkMethodDef;
};

void CMetadataMethod::SetFullQualifiedMethodName(CString szMethodName)
{
    this->m_szFullQualifiedMethodName = szMethodName;
    this->SetMetadataLoaded(METADATA_FULL_QUALIFIED_METHOD_NAME);
};

CString CMetadataMethod::GetFullQualifiedMethodName(void) const
{
    ASSERT(this->IsMetadataLoaded(METADATA_FULL_QUALIFIED_METHOD_NAME));
    return this->m_szFullQualifiedMethodName;
};

void CMetadataMethod::SetILMethodBody(LPVOID pMemory, ULONG nSize)
{
    this->m_xILMethodBody.Attach(pMemory, nSize);
    this->SetMetadataLoaded(METADATA_IL_METHOD_BODY);
};

const CILMethodBody& CMetadataMethod::GetILMethodBody(void) const
{
    ASSERT(this->IsMetadataLoaded(METADATA_IL_METHOD_BODY));
    return this->m_xILMethodBody;
};

void CMetadataMethod::SetMethodSignature(PCCOR_SIGNATURE pMethodSignature, ULONG nMethodSignatureSize)
{
    this->m_xMethodSignature.Attach(pMethodSignature, nMethodSignatureSize);
    this->SetMetadataLoaded(METADATA_METHOD_SIGNATURE);
};

const CMethodDefSigBlob& CMetadataMethod::GetMethodSignature(void) const
{
    ASSERT(this->IsMetadataLoaded(METADATA_METHOD_SIGNATURE));
    return this->m_xMethodSignature;
};
