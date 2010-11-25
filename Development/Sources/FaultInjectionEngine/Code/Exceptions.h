// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

//
// Declarations of exceptions classes used in this project
//

#pragma once

BEGIN_DEFAULT_NAMESPACE

/// <summary>
/// Use CExceptionAsBreak::Throw() to break the normal execution path in case
/// CLR fails. Do NOT delete the thrown exception for it's the only static one.
/// </summary>
class CExceptionAsBreak
{
public:
    static void Throw(void)
    {
        throw &sharedExceptionAsBreak;
    };

private:
    static CExceptionAsBreak sharedExceptionAsBreak;
};

END_DEFAULT_NAMESPACE
