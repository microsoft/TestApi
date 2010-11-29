// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#pragma once
#include "SignatureBlob.h"
#include "RetTypeSigBlob.h"

BEGIN_DEFAULT_NAMESPACE

struct CMethodDefSigBlob : public CSignatureBlob
{
public:
    CMethodDefSigBlob(void) : CSignatureBlob() {};
    CMethodDefSigBlob(LPCVOID pMemory, size_t nSize) : CSignatureBlob(pMemory, nSize) {};

public:
    PCCOR_SIGNATURE LocateReturnType() const;
};

END_DEFAULT_NAMESPACE
