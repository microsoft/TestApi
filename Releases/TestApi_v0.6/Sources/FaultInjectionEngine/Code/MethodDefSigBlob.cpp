// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#include "stdafx.h"
#include "MethodDefSigBlob.h"

USING_DEFAULT_NAMESPACE

PCCOR_SIGNATURE CMethodDefSigBlob::LocateReturnType() const
{
    PCCOR_SIGNATURE pSignature = RefSignature();
    GetParamCountFromMethodSig(pSignature);
    return pSignature;
}
