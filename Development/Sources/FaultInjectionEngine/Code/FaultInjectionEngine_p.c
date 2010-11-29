

/* this ALWAYS GENERATED file contains the proxy stub code */


 /* File created by MIDL compiler version 7.00.0555 */
/* at Sun Nov 28 23:15:08 2010
 */
/* Compiler settings for FaultInjectionEngine.idl:
    Oicf, W1, Zp8, env=Win32 (32b run), target_arch=X86 7.00.0555 
    protocol : dce , ms_ext, c_ext
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
/* @@MIDL_FILE_HEADING(  ) */

#if !defined(_M_IA64) && !defined(_M_AMD64)


#pragma warning( disable: 4049 )  /* more than 64k source lines */
#if _MSC_VER >= 1200
#pragma warning(push)
#endif

#pragma warning( disable: 4211 )  /* redefine extern to static */
#pragma warning( disable: 4232 )  /* dllimport identity*/
#pragma warning( disable: 4024 )  /* array to pointer mapping*/
#pragma warning( disable: 4152 )  /* function/data pointer conversion in expression */
#pragma warning( disable: 4100 ) /* unreferenced arguments in x86 call */

#pragma optimize("", off ) 

#define USE_STUBLESS_PROXY


/* verify that the <rpcproxy.h> version is high enough to compile this file*/
#ifndef __REDQ_RPCPROXY_H_VERSION__
#define __REQUIRED_RPCPROXY_H_VERSION__ 440
#endif


#include "rpcproxy.h"
#ifndef __RPCPROXY_H_VERSION__
#error this stub requires an updated version of <rpcproxy.h>
#endif /* __RPCPROXY_H_VERSION__ */


#include "FaultInjectionEngine.h"

#define TYPE_FORMAT_STRING_SIZE   3                                 
#define PROC_FORMAT_STRING_SIZE   1                                 
#define EXPR_FORMAT_STRING_SIZE   1                                 
#define TRANSMIT_AS_TABLE_SIZE    0            
#define WIRE_MARSHAL_TABLE_SIZE   0            

typedef struct _FaultInjectionEngine_MIDL_TYPE_FORMAT_STRING
    {
    short          Pad;
    unsigned char  Format[ TYPE_FORMAT_STRING_SIZE ];
    } FaultInjectionEngine_MIDL_TYPE_FORMAT_STRING;

typedef struct _FaultInjectionEngine_MIDL_PROC_FORMAT_STRING
    {
    short          Pad;
    unsigned char  Format[ PROC_FORMAT_STRING_SIZE ];
    } FaultInjectionEngine_MIDL_PROC_FORMAT_STRING;

typedef struct _FaultInjectionEngine_MIDL_EXPR_FORMAT_STRING
    {
    long          Pad;
    unsigned char  Format[ EXPR_FORMAT_STRING_SIZE ];
    } FaultInjectionEngine_MIDL_EXPR_FORMAT_STRING;


static const RPC_SYNTAX_IDENTIFIER  _RpcTransferSyntax = 
{{0x8A885D04,0x1CEB,0x11C9,{0x9F,0xE8,0x08,0x00,0x2B,0x10,0x48,0x60}},{2,0}};


extern const FaultInjectionEngine_MIDL_TYPE_FORMAT_STRING FaultInjectionEngine__MIDL_TypeFormatString;
extern const FaultInjectionEngine_MIDL_PROC_FORMAT_STRING FaultInjectionEngine__MIDL_ProcFormatString;
extern const FaultInjectionEngine_MIDL_EXPR_FORMAT_STRING FaultInjectionEngine__MIDL_ExprFormatString;


extern const MIDL_STUB_DESC Object_StubDesc;


extern const MIDL_SERVER_INFO IEngine_ServerInfo;
extern const MIDL_STUBLESS_PROXY_INFO IEngine_ProxyInfo;



#if !defined(__RPC_WIN32__)
#error  Invalid build platform for this stub.
#endif

#if !(TARGET_IS_NT40_OR_LATER)
#error You need Windows NT 4.0 or later to run this stub because it uses these features:
#error   -Oif or -Oicf, more than 64 delegated procs.
#error However, your C/C++ compilation flags indicate you intend to run this app on earlier systems.
#error This app will fail with the RPC_X_WRONG_STUB_VERSION error.
#endif


static const FaultInjectionEngine_MIDL_PROC_FORMAT_STRING FaultInjectionEngine__MIDL_ProcFormatString =
    {
        0,
        {

			0x0
        }
    };

static const FaultInjectionEngine_MIDL_TYPE_FORMAT_STRING FaultInjectionEngine__MIDL_TypeFormatString =
    {
        0,
        {
			NdrFcShort( 0x0 ),	/* 0 */

			0x0
        }
    };


/* Object interface: IUnknown, ver. 0.0,
   GUID={0x00000000,0x0000,0x0000,{0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x46}} */


/* Object interface: ICorProfilerCallback, ver. 0.0,
   GUID={0x176FBED1,0xA55C,0x4796,{0x98,0xCA,0xA9,0xDA,0x0E,0xF8,0x83,0xE7}} */


/* Object interface: ICorProfilerCallback2, ver. 0.0,
   GUID={0x8A8CC829,0xCCF2,0x49fe,{0xBB,0xAE,0x0F,0x02,0x22,0x28,0x07,0x1A}} */


/* Object interface: IEngine, ver. 0.0,
   GUID={0x68BB3A4D,0x0387,0x4F15,{0xA8,0xF6,0x4F,0x9D,0x34,0x7A,0x12,0x99}} */

#pragma code_seg(".orpc")
static const unsigned short IEngine_FormatStringOffsetTable[] =
    {
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    (unsigned short) -1,
    0
    };

static const MIDL_STUBLESS_PROXY_INFO IEngine_ProxyInfo =
    {
    &Object_StubDesc,
    FaultInjectionEngine__MIDL_ProcFormatString.Format,
    &IEngine_FormatStringOffsetTable[-3],
    0,
    0,
    0
    };


static const MIDL_SERVER_INFO IEngine_ServerInfo = 
    {
    &Object_StubDesc,
    0,
    FaultInjectionEngine__MIDL_ProcFormatString.Format,
    &IEngine_FormatStringOffsetTable[-3],
    0,
    0,
    0,
    0};
CINTERFACE_PROXY_VTABLE(80) _IEngineProxyVtbl = 
{
    0,
    &IID_IEngine,
    IUnknown_QueryInterface_Proxy,
    IUnknown_AddRef_Proxy,
    IUnknown_Release_Proxy ,
    0 /* ICorProfilerCallback::Initialize */ ,
    0 /* ICorProfilerCallback::Shutdown */ ,
    0 /* ICorProfilerCallback::AppDomainCreationStarted */ ,
    0 /* ICorProfilerCallback::AppDomainCreationFinished */ ,
    0 /* ICorProfilerCallback::AppDomainShutdownStarted */ ,
    0 /* ICorProfilerCallback::AppDomainShutdownFinished */ ,
    0 /* ICorProfilerCallback::AssemblyLoadStarted */ ,
    0 /* ICorProfilerCallback::AssemblyLoadFinished */ ,
    0 /* ICorProfilerCallback::AssemblyUnloadStarted */ ,
    0 /* ICorProfilerCallback::AssemblyUnloadFinished */ ,
    0 /* ICorProfilerCallback::ModuleLoadStarted */ ,
    0 /* ICorProfilerCallback::ModuleLoadFinished */ ,
    0 /* ICorProfilerCallback::ModuleUnloadStarted */ ,
    0 /* ICorProfilerCallback::ModuleUnloadFinished */ ,
    0 /* ICorProfilerCallback::ModuleAttachedToAssembly */ ,
    0 /* ICorProfilerCallback::ClassLoadStarted */ ,
    0 /* ICorProfilerCallback::ClassLoadFinished */ ,
    0 /* ICorProfilerCallback::ClassUnloadStarted */ ,
    0 /* ICorProfilerCallback::ClassUnloadFinished */ ,
    0 /* ICorProfilerCallback::FunctionUnloadStarted */ ,
    0 /* ICorProfilerCallback::JITCompilationStarted */ ,
    0 /* ICorProfilerCallback::JITCompilationFinished */ ,
    0 /* ICorProfilerCallback::JITCachedFunctionSearchStarted */ ,
    0 /* ICorProfilerCallback::JITCachedFunctionSearchFinished */ ,
    0 /* ICorProfilerCallback::JITFunctionPitched */ ,
    0 /* ICorProfilerCallback::JITInlining */ ,
    0 /* ICorProfilerCallback::ThreadCreated */ ,
    0 /* ICorProfilerCallback::ThreadDestroyed */ ,
    0 /* ICorProfilerCallback::ThreadAssignedToOSThread */ ,
    0 /* ICorProfilerCallback::RemotingClientInvocationStarted */ ,
    0 /* ICorProfilerCallback::RemotingClientSendingMessage */ ,
    0 /* ICorProfilerCallback::RemotingClientReceivingReply */ ,
    0 /* ICorProfilerCallback::RemotingClientInvocationFinished */ ,
    0 /* ICorProfilerCallback::RemotingServerReceivingMessage */ ,
    0 /* ICorProfilerCallback::RemotingServerInvocationStarted */ ,
    0 /* ICorProfilerCallback::RemotingServerInvocationReturned */ ,
    0 /* ICorProfilerCallback::RemotingServerSendingReply */ ,
    0 /* ICorProfilerCallback::UnmanagedToManagedTransition */ ,
    0 /* ICorProfilerCallback::ManagedToUnmanagedTransition */ ,
    0 /* ICorProfilerCallback::RuntimeSuspendStarted */ ,
    0 /* ICorProfilerCallback::RuntimeSuspendFinished */ ,
    0 /* ICorProfilerCallback::RuntimeSuspendAborted */ ,
    0 /* ICorProfilerCallback::RuntimeResumeStarted */ ,
    0 /* ICorProfilerCallback::RuntimeResumeFinished */ ,
    0 /* ICorProfilerCallback::RuntimeThreadSuspended */ ,
    0 /* ICorProfilerCallback::RuntimeThreadResumed */ ,
    0 /* ICorProfilerCallback::MovedReferences */ ,
    0 /* ICorProfilerCallback::ObjectAllocated */ ,
    0 /* ICorProfilerCallback::ObjectsAllocatedByClass */ ,
    0 /* ICorProfilerCallback::ObjectReferences */ ,
    0 /* ICorProfilerCallback::RootReferences */ ,
    0 /* ICorProfilerCallback::ExceptionThrown */ ,
    0 /* ICorProfilerCallback::ExceptionSearchFunctionEnter */ ,
    0 /* ICorProfilerCallback::ExceptionSearchFunctionLeave */ ,
    0 /* ICorProfilerCallback::ExceptionSearchFilterEnter */ ,
    0 /* ICorProfilerCallback::ExceptionSearchFilterLeave */ ,
    0 /* ICorProfilerCallback::ExceptionSearchCatcherFound */ ,
    0 /* ICorProfilerCallback::ExceptionOSHandlerEnter */ ,
    0 /* ICorProfilerCallback::ExceptionOSHandlerLeave */ ,
    0 /* ICorProfilerCallback::ExceptionUnwindFunctionEnter */ ,
    0 /* ICorProfilerCallback::ExceptionUnwindFunctionLeave */ ,
    0 /* ICorProfilerCallback::ExceptionUnwindFinallyEnter */ ,
    0 /* ICorProfilerCallback::ExceptionUnwindFinallyLeave */ ,
    0 /* ICorProfilerCallback::ExceptionCatcherEnter */ ,
    0 /* ICorProfilerCallback::ExceptionCatcherLeave */ ,
    0 /* ICorProfilerCallback::COMClassicVTableCreated */ ,
    0 /* ICorProfilerCallback::COMClassicVTableDestroyed */ ,
    0 /* ICorProfilerCallback::ExceptionCLRCatcherFound */ ,
    0 /* ICorProfilerCallback::ExceptionCLRCatcherExecute */ ,
    0 /* ICorProfilerCallback2::ThreadNameChanged */ ,
    0 /* ICorProfilerCallback2::GarbageCollectionStarted */ ,
    0 /* ICorProfilerCallback2::SurvivingReferences */ ,
    0 /* ICorProfilerCallback2::GarbageCollectionFinished */ ,
    0 /* ICorProfilerCallback2::FinalizeableObjectQueued */ ,
    0 /* ICorProfilerCallback2::RootReferences2 */ ,
    0 /* ICorProfilerCallback2::HandleCreated */ ,
    0 /* ICorProfilerCallback2::HandleDestroyed */
};


static const PRPC_STUB_FUNCTION IEngine_table[] =
{
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION,
    STUB_FORWARDING_FUNCTION
};

CInterfaceStubVtbl _IEngineStubVtbl =
{
    &IID_IEngine,
    &IEngine_ServerInfo,
    80,
    &IEngine_table[-3],
    CStdStubBuffer_DELEGATING_METHODS
};

static const MIDL_STUB_DESC Object_StubDesc = 
    {
    0,
    NdrOleAllocate,
    NdrOleFree,
    0,
    0,
    0,
    0,
    0,
    FaultInjectionEngine__MIDL_TypeFormatString.Format,
    1, /* -error bounds_check flag */
    0x20000, /* Ndr library version */
    0,
    0x700022b, /* MIDL Version 7.0.555 */
    0,
    0,
    0,  /* notify & notify_flag routine table */
    0x1, /* MIDL flag */
    0, /* cs routines */
    0,   /* proxy/server info */
    0
    };

const CInterfaceProxyVtbl * const _FaultInjectionEngine_ProxyVtblList[] = 
{
    ( CInterfaceProxyVtbl *) &_IEngineProxyVtbl,
    0
};

const CInterfaceStubVtbl * const _FaultInjectionEngine_StubVtblList[] = 
{
    ( CInterfaceStubVtbl *) &_IEngineStubVtbl,
    0
};

PCInterfaceName const _FaultInjectionEngine_InterfaceNamesList[] = 
{
    "IEngine",
    0
};

const IID *  const _FaultInjectionEngine_BaseIIDList[] = 
{
    &IID_ICorProfilerCallback2,
    0
};


#define _FaultInjectionEngine_CHECK_IID(n)	IID_GENERIC_CHECK_IID( _FaultInjectionEngine, pIID, n)

int __stdcall _FaultInjectionEngine_IID_Lookup( const IID * pIID, int * pIndex )
{
    
    if(!_FaultInjectionEngine_CHECK_IID(0))
        {
        *pIndex = 0;
        return 1;
        }

    return 0;
}

const ExtendedProxyFileInfo FaultInjectionEngine_ProxyFileInfo = 
{
    (PCInterfaceProxyVtblList *) & _FaultInjectionEngine_ProxyVtblList,
    (PCInterfaceStubVtblList *) & _FaultInjectionEngine_StubVtblList,
    (const PCInterfaceName * ) & _FaultInjectionEngine_InterfaceNamesList,
    (const IID ** ) & _FaultInjectionEngine_BaseIIDList,
    & _FaultInjectionEngine_IID_Lookup, 
    1,
    2,
    0, /* table of [async_uuid] interfaces */
    0, /* Filler1 */
    0, /* Filler2 */
    0  /* Filler3 */
};
#pragma optimize("", on )
#if _MSC_VER >= 1200
#pragma warning(pop)
#endif


#endif /* !defined(_M_IA64) && !defined(_M_AMD64)*/

