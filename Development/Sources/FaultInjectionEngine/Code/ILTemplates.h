// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

//
// The sterotypes of IL Method Header, IL Method SEH, and the prologue code
//

#pragma once

BEGIN_DEFAULT_NAMESPACE

//---------------------------------------------------------
// The default IL-Method FAT Header (sterotype for those convert from tiny ones).
const IMAGE_COR_ILMETHOD_FAT imageDefaultILMethodFatHeader =
{
    /* Flags */             CorILMethod_FatFormat | CorILMethod_InitLocals,
    /* Size (in dwords) */  sizeof(IMAGE_COR_ILMETHOD_FAT) / sizeof(DWORD),
    /* MaxStack */          0,  // should be corrected dynamically
    /* CodeSize */          0,  // should be corrected dynamically
    /* LocalVarSigTok */    0   // should be corrected dynamically
};

//---------------------------------------------------------
// The default IL-Method FAT (EH)Section (sterotype for those convert from small ones).
const IMAGE_COR_ILMETHOD_SECT_FAT imageDefaultILMethodFatSection =
{
    /* Kind */      CorILMethod_Sect_EHTable | CorILMethod_Sect_FatFormat,   // MORE_SECTS flag should be corrected dynamically
    /* DataSize */  0   // should be corrected dynamically
};

//---------------------------------------------------------
// Prologue IL code template

const BYTE IL_CODE__PROLOGUE[] = {
    0xFE,0x0D,  0,0,        // IL__0 (4):  ldloca  "throwException"
    0xFE,0x0D,  0,0,        // IL__4 (4):  ldloca  "returnValue"
    0x28,       0,0,0,0,    // IL__8 (5):  call  "static bool Trap(Exception&, Object&)"
    0x2C,       21,         // IL_13 (2):  brfalse.s  ORIGINAL_CODE
    0xFE,0x0C,  0,0,        // IL_15 (4):  ldloc  "throwException"
    0x2C,       5,          // IL_19 (2):  brfalse.s  RETURN_SECTION
    0xFE,0x0C,  0,0,        // IL_21 (4):  ldloc  "throwException"
    0x7A,                   // IL_25 (1):  throw
    // RETURN_SECTION:
    0xFE,0x0C,  0,0,        // IL_26 (4):  ldloc  "returnValue"
    0xA5,       0,0,0,0,    // IL_30 (5):  unbox.any  "token of return-type"
    0x2A,					// IL_35 (1):  ret
    // ORIGINAL_CODE:
    0
};

const ULONG IL_OFFSET__LOCALVAR_1[] = {2, 17, 23, 0};    // replace as 2-bytes local-var index of "throwException"
const ULONG IL_OFFSET__LOCALVAR_2[] = {6, 28, 0};    // replace as 2-bytes local-var index of "returnValue"
const ULONG IL_OFFSET__CALL_TRAP    = 9;    // replace as 4-bytes method token of "Trap"

const ULONG IL_SIZE__PROLOGUE       = 36;
const ULONG IL_SIZE__RETURN_VOID    = 10;
const ULONG IL_SIZE__RETURN_OBJECT  = 6;
const ULONG IL_OFFSET__RETURN       = 26;
const ULONG IL_OFFSET__UNBOX        = 30;
const ULONG IL_OFFSET__RETURN_TYPE  = 31;

const BYTE IL_CODE__JUST_RETURN[] = {
    0x2A,					// IL_26 (1):  ret
    0,0,0,0,0,0,0,0,0,      // IL_27 (9):  nop, ..., nop
    // ORIGINAL_CODE:
    0
};

const ULONG IL_NUMBER__MIN_STACK    = 2;

END_DEFAULT_NAMESPACE
