// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#pragma once
#include "SignatureBlob.h"

BEGIN_DEFAULT_NAMESPACE

struct CRetTypeSigBlob : public CSignatureBlob
{
public:
    CRetTypeSigBlob(void) : CSignatureBlob() {};
    CRetTypeSigBlob(LPCVOID pBase, size_t nSize) : CSignatureBlob(pBase, nSize) {};

public:
    void AdjustSize(void);
};

END_DEFAULT_NAMESPACE
