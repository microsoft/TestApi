// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

//
//  CILMethodBody represent a block of memory that contains IL Method Body.
//

#pragma once
#include "MemoryRef.h"
#include "ILMethodHeader.h"
#include "ILMethodSect.h"

BEGIN_DEFAULT_NAMESPACE

class CILMethodBody : public CMemoryRef
{
#pragma region Constructors and Destructors
public:
    CILMethodBody(void) : CMemoryRef() {};
    CILMethodBody(LPVOID pMemory, ULONG nSize) : CMemoryRef(pMemory, nSize) {};
#pragma endregion

#pragma region Public Methods
public:
    /// <Summary>
    /// Get the IL Method Code from body. It returns exactly the memory block
    /// where code locates. It does NOT copy the memory, just locates and gets
    /// pointer and size of the memory.
    /// </Summary>
    CMemoryRef GetCode(void) const;

    /// <Summary>
    /// Get the IL Method (Structured Exception Handler) Section from body. It
    /// returns exactly the memory block where SEH locates. It does NOT copy
    /// the memory, just locates and gets pointer and size of the memory.
    /// </Summary>
    CILMethodSect GetSect(void) const;

    /// <Summary>
    /// Get the IL Method Header from body. It returns exactly the memory block
    /// where SEH locates. It does NOT copy the memory, just locates and gets
    /// pointer and size of the memory.
    /// </Summary>
    CILMethodHeader GetHeader(void) const;
#pragma endregion
};

END_DEFAULT_NAMESPACE
