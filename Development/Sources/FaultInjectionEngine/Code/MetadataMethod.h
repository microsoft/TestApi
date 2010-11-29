// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#pragma once
#include "ILMethodBody.h"
#include "MethodDefSigBlob.h"

BEGIN_DEFAULT_NAMESPACE

class CMetadataMethod
{
public:
    CMetadataMethod(mdMethodDef tkMethodDef)
    {
#if defined(_DEBUG)
        this->m_nLoadedMetadata = METADATA_NULL;
#endif
        this->SetMethodDefToken(tkMethodDef);
    };
    ~CMetadataMethod(void) {};

public:
    mdMethodDef GetMethodDefToken(void) const;
    void SetMethodDefToken(mdMethodDef tkMethodDef);
    CString GetFullQualifiedMethodName(void) const;
    void SetFullQualifiedMethodName(CString szMethodName);
    const CILMethodBody& GetILMethodBody(void) const;
    void SetILMethodBody(LPVOID pMemory, ULONG nSize);
    const CMethodDefSigBlob& GetMethodSignature(void) const;
    void SetMethodSignature(PCCOR_SIGNATURE pMethodSignature, ULONG nMethodSignatureSize);

private:
    mdMethodDef m_tkMethodDef;
    CString m_szFullQualifiedMethodName;
    CILMethodBody m_xILMethodBody;
    CMethodDefSigBlob m_xMethodSignature;

private:
    // Following methods are used to ensure every metadata should be loaded (set) before
    // get it. It depends on the program execution path, so only check them in debug mode.
#if defined(_DEBUG)
    void SetMetadataLoaded(ULONG nMetadataMask)
    {
        this->m_nLoadedMetadata |= nMetadataMask;
    };
    BOOL IsMetadataLoaded(ULONG nMetadataMask) const
    {
        return ((this->m_nLoadedMetadata & nMetadataMask) == nMetadataMask);
    };
    ULONG m_nLoadedMetadata;  // record which parts of the metadata have been loaded
#else
    void SetMetadataLoaded(ULONG nMetadataMask) {};
    BOOL IsMetadataLoaded(ULONG nMetadataMask) const { return TRUE; };
#endif

    enum
    {
        METADATA_METHOD_DEF_TOKEN             = 0x0001,
        METADATA_FULL_QUALIFIED_METHOD_NAME   = 0x0002,
        METADATA_IL_METHOD_BODY               = 0x0004,
        METADATA_METHOD_SIGNATURE             = 0x0005,
        METADATA_NULL         = 0x0000
    };
};

END_DEFAULT_NAMESPACE
