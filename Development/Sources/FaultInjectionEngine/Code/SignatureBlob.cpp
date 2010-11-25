// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#include "stdafx.h"
#include "SignatureBlob.h"
#include "TraceAndLog.h"
#include "Exceptions.h"

USING_DEFAULT_NAMESPACE

PCCOR_SIGNATURE CSignatureBlob::RefSignature(void) const
{
    return (PCCOR_SIGNATURE)(this->m_pBase);
}

void CSignatureBlob::EnsureWithin(PCCOR_SIGNATURE pTailOfSignature) const
{
    if(!HitTest(pTailOfSignature, FALSE))
    {
        EventReportError(IDS_REPORT_INVALID_SIGNATURE, this->GetBaseAddress(), this->GetTailAddress(), this->GetSize());
        CExceptionAsBreak::Throw();
    }
}

BOOL CSignatureBlob::ParseRetTypeSig(PCCOR_SIGNATURE &pSignature) const
{
    //-----------------------------------------------------------------------------------
    //  RetTypeSig ::= CustomMod* ( VOID | TYPEDBYREF | [BYREF] Type )
    
    CorElementType nElementType = this->BypassOptCustomMod(pSignature);
    return this->ParseRetTypeSig(nElementType, pSignature);
}

BOOL CSignatureBlob::ParseRetTypeSig(CorElementType nElementType, PCCOR_SIGNATURE &pSignature) const
{
    //-----------------------------------------------------------------------------------
    //  RetTypeSig ::= CustomMod* ( VOID | TYPEDBYREF | [BYREF] Type )
    switch(nElementType)
    {
    case ELEMENT_TYPE_CMOD_REQD      :
    case ELEMENT_TYPE_CMOD_OPT       :
        return this->ParseRetTypeSig(pSignature);

    case ELEMENT_TYPE_VOID           :
    case ELEMENT_TYPE_TYPEDBYREF     :
        return TRUE;

    case ELEMENT_TYPE_BYREF          :
        return this->ParseTypeSig(pSignature);

    default:
        break;
    }
    return this->ParseTypeSig(nElementType, pSignature);
}

BOOL CSignatureBlob::ParseTypeSig(PCCOR_SIGNATURE& pSignature) const
{
    CorElementType nElementType = ::CorSigUncompressElementType(pSignature);
    EnsureWithin(pSignature);
    return this->ParseTypeSig(nElementType, pSignature);
}

BOOL CSignatureBlob::ParseTypeSig(CorElementType nElementType, PCCOR_SIGNATURE& pSignature) const
{
    //-----------------------------------------------------------------------------------
    //  Type ::= ( BOOLEAN | CHAR | I1 | U1 | U2 | U2 | I4 | U4 | I8 | U8 | R4 | R8 | I | U |
    //      | VALUETYPE TypeDefOrRefEncoded
    //      | CLASS TypeDefOrRefEncoded
    //      | STRING 
    //      | OBJECT
    //      | PTR CustomMod* VOID
    //      | PTR CustomMod* Type
    //      | FNPTR MethodDefSig
    //      | FNPTR MethodRefSig
    //      | ARRAY Type ArrayShape
    //      | SZARRAY CustomMod* Type
    //      | GENERICINST (CLASS | VALUETYPE) TypeDefOrRefEncoded GenArgCount Type*
    //      | VAR Number
    //      | MVAR Number
    switch(nElementType)
    {
    case ELEMENT_TYPE_BOOLEAN        :
    case ELEMENT_TYPE_CHAR           :
    case ELEMENT_TYPE_I1             :
    case ELEMENT_TYPE_U1             :
    case ELEMENT_TYPE_I2             :
    case ELEMENT_TYPE_U2             :
    case ELEMENT_TYPE_I4             :
    case ELEMENT_TYPE_U4             :
    case ELEMENT_TYPE_I8             :
    case ELEMENT_TYPE_U8             :
    case ELEMENT_TYPE_R4             :
    case ELEMENT_TYPE_R8             :
    case ELEMENT_TYPE_I              :
    case ELEMENT_TYPE_U              :
    case ELEMENT_TYPE_STRING         :
    case ELEMENT_TYPE_OBJECT         :
        return TRUE;

    case ELEMENT_TYPE_VALUETYPE      :
    case ELEMENT_TYPE_CLASS          :
        /* TypeDef_or_TypeRef = */ ::CorSigUncompressToken(pSignature);
        EnsureWithin(pSignature);
        return TRUE;

    case ELEMENT_TYPE_PTR            :
        nElementType = this->BypassOptCustomMod(pSignature);
        if(ELEMENT_TYPE_VOID == nElementType)
            return TRUE;
        return this->ParseTypeSig(nElementType, pSignature);

    case ELEMENT_TYPE_FNPTR          :
        return this->ParseMethodSig(pSignature);

    case ELEMENT_TYPE_ARRAY          :
        return this->ParseTypeSig(pSignature) && this->ParseArrayShapeSig(pSignature);

    case ELEMENT_TYPE_SZARRAY        :
        nElementType = this->BypassOptCustomMod(pSignature);
        return this->ParseTypeSig(nElementType, pSignature);

    case ELEMENT_TYPE_GENERICINST    :
        nElementType = ::CorSigUncompressElementType(pSignature);
        this->EnsureWithin(pSignature);
        if((ELEMENT_TYPE_CLASS != nElementType) && (ELEMENT_TYPE_VALUETYPE != nElementType))
            return FALSE;
        /* TypeDef_or_TypeRef = */ ::CorSigUncompressToken(pSignature);
        this->EnsureWithin(pSignature);
        {
            ULONG nGenericArgumentsCount = ::CorSigUncompressData(pSignature);
            this->EnsureWithin(pSignature);

            for(ULONG i = 0; i < nGenericArgumentsCount; i++)
            {
                if(!this->ParseTypeSig(pSignature))
                    return FALSE;
            }
        }
        return TRUE;

    case ELEMENT_TYPE_VAR            :
    case ELEMENT_TYPE_MVAR           :
        /* index = */ ::CorSigUncompressData(pSignature);
        this->EnsureWithin(pSignature);
        return TRUE;

    case ELEMENT_TYPE_END            :
    case ELEMENT_TYPE_VOID           :
    case ELEMENT_TYPE_CMOD_REQD      :
    case ELEMENT_TYPE_CMOD_OPT       :
    case ELEMENT_TYPE_INTERNAL       :
    case ELEMENT_TYPE_MAX            :
    case ELEMENT_TYPE_MODIFIER       :
    case ELEMENT_TYPE_SENTINEL       :
    case ELEMENT_TYPE_PINNED         :
    case ELEMENT_TYPE_R4_HFA         :
    case ELEMENT_TYPE_R8_HFA         :
    default:
        ASSERT(FALSE);
        return FALSE;
    }
}

BOOL CSignatureBlob::ParseArrayShapeSig(PCCOR_SIGNATURE &pSignature) const
{
    //-----------------------------------------------------------------------------------
    //  ArrayShape ::= Rank NumSizes Size* NumLoBounds LoBound*
    ULONG nRank = ::CorSigUncompressData(pSignature);
    this->EnsureWithin(pSignature);
    
    ULONG nNumberOfSize = ::CorSigUncompressData(pSignature);
    this->EnsureWithin(pSignature);
    
    for(ULONG i = 0; i < nNumberOfSize; i++)
    {
        ULONG nSize = ::CorSigUncompressData(pSignature);
        this->EnsureWithin(pSignature);
    }

    ULONG nNumberOfLowerbound = ::CorSigUncompressData(pSignature);
    this->EnsureWithin(pSignature);

    for(ULONG i = 0; i < nNumberOfLowerbound; i++)
    {
        ULONG nLowerbound = ::CorSigUncompressData(pSignature);
        this->EnsureWithin(pSignature);
    }

    return TRUE;
}

BOOL CSignatureBlob::ParseParameterSig(PCCOR_SIGNATURE &pSignature) const
{
    ASSERT(HitTest(pSignature));
    //-----------------------------------------------------------------------------------
    //  Param ::= CustomMod* ( TYPEDBYREF | [BYREF] Type )
    CorElementType nElementType = this->BypassOptCustomMod(pSignature);
    EnsureWithin(pSignature);
    return this->ParseParameterSig(nElementType, pSignature);
}

BOOL CSignatureBlob::ParseParameterSig(CorElementType nElementType, PCCOR_SIGNATURE &pSignature) const
{
    //-----------------------------------------------------------------------------------
    //  Param ::= CustomMod* ( TYPEDBYREF | [BYREF] Type )
    switch(nElementType)
    {
    case ELEMENT_TYPE_CMOD_REQD      :
    case ELEMENT_TYPE_CMOD_OPT       :
        return this->ParseParameterSig(pSignature);

    case ELEMENT_TYPE_TYPEDBYREF     :
        return TRUE;

    case ELEMENT_TYPE_BYREF          :
        return this->ParseTypeSig(pSignature);

    default:
        return FALSE;
    }
}

BOOL CSignatureBlob::ParseMethodSig(PCCOR_SIGNATURE &pSignature) const
{
    ASSERT(HitTest(pSignature));
    //-----------------------------------------------------------------------------------
    //  MethodDefSig ::=
    //      [HASTHIS [EXPLICTTHIS]] (DEFAULT | VARARG | GENERIC GenParamCount) ParamCount
    //      RetTypeSig (ParamSig*)
    //  MethodRefSig ::=
    //      [HASTHIS [EXPLICITTHIS]] VARARG ParamCount
    //      RetTypeSig ([SENTINEL] ParamSig)*

    ULONG nParamCount = GetParamCountFromMethodSig(pSignature);

    if(!this->ParseRetTypeSig(pSignature))
        return FALSE;

    for(ULONG i = 0; i < nParamCount; i++)
    {
        CorElementType nElementType = ::CorSigUncompressElementType(pSignature);
        this->EnsureWithin(pSignature);

        if( (ELEMENT_TYPE_SENTINEL == nElementType)
            ? this->ParseParameterSig(pSignature) : this->ParseParameterSig(nElementType, pSignature) )
        {
            return FALSE;
        }
    }

    return TRUE;
}

ULONG CSignatureBlob::GetParamCountFromMethodSig(PCCOR_SIGNATURE &pSignature) const
{
    ASSERT(HitTest(pSignature));
    //-----------------------------------------------------------------------------------
    //  MethodDefSig ::= [HASTHIS [EXPLICTTHIS]] (DEFAULT | VARARG | GENERIC GenParamCount)
    //      ParamCount RetTypeSig (ParamSig*)
    //  MethodRefSig ::= [HASTHIS [EXPLICITTHIS]] VARARG ParamCount RetTypeSig ParamSig*
    //      [SENTINEL Param+]

    // HASTHIS, EXPLICTTHIS, DEFAULT, VARARG, GENERIC are composited in CorCallingConvention
    ULONG nCallingConvention = ::CorSigUncompressCallingConv(pSignature);
    EnsureWithin(pSignature);

    if(IMAGE_CEE_CS_CALLCONV_GENERIC & nCallingConvention) // GENERIC
    {
        // GenParamCount (that following GENERIC)
        ULONG nGenParamCount = ::CorSigUncompressData(pSignature);
        EnsureWithin(pSignature);
    }

    // ParamCount
    ULONG nParamCount = ::CorSigUncompressData(pSignature);
    EnsureWithin(pSignature);
    return nParamCount;
}

CorElementType CSignatureBlob::BypassOptCustomMod(PCCOR_SIGNATURE& pSignature) const
{
    ASSERT(HitTest(pSignature));
    //-----------------------------------------------------------------------------------
    //  CustMod* ::= (CMOD_REQD | CMOD_OPT)*

    CorElementType nElementType;
    do
    {
        nElementType = ::CorSigUncompressElementType(pSignature);
        this->EnsureWithin(pSignature);
    }
    while((ELEMENT_TYPE_CMOD_REQD == nElementType) || (ELEMENT_TYPE_CMOD_OPT == nElementType));

    return nElementType;
}
