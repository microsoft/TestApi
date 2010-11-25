

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0500 */
/* at Wed Nov 24 17:54:38 2010
 */
/* Compiler settings for .\FaultInjectionEngine.idl:
    Oicf, W1, Zp8, env=Win32 (32b run)
    protocol : dce , ms_ext, c_ext
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
//@@MIDL_FILE_HEADING(  )

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 440
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __FaultInjectionEngine_h__
#define __FaultInjectionEngine_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IEngine_FWD_DEFINED__
#define __IEngine_FWD_DEFINED__
typedef interface IEngine IEngine;
#endif 	/* __IEngine_FWD_DEFINED__ */


#ifndef __Engine_FWD_DEFINED__
#define __Engine_FWD_DEFINED__

#ifdef __cplusplus
typedef class Engine Engine;
#else
typedef struct Engine Engine;
#endif /* __cplusplus */

#endif 	/* __Engine_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "corprof.h"

#ifdef __cplusplus
extern "C"{
#endif 


#ifndef __IEngine_INTERFACE_DEFINED__
#define __IEngine_INTERFACE_DEFINED__

/* interface IEngine */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IEngine;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("68BB3A4D-0387-4F15-A8F6-4F9D347A1299")
    IEngine : public ICorProfilerCallback2
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IEngineVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEngine * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEngine * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEngine * This);
        
        HRESULT ( STDMETHODCALLTYPE *Initialize )( 
            IEngine * This,
            /* [in] */ IUnknown *pICorProfilerInfoUnk);
        
        HRESULT ( STDMETHODCALLTYPE *Shutdown )( 
            IEngine * This);
        
        HRESULT ( STDMETHODCALLTYPE *AppDomainCreationStarted )( 
            IEngine * This,
            /* [in] */ AppDomainID appDomainId);
        
        HRESULT ( STDMETHODCALLTYPE *AppDomainCreationFinished )( 
            IEngine * This,
            /* [in] */ AppDomainID appDomainId,
            /* [in] */ HRESULT hrStatus);
        
        HRESULT ( STDMETHODCALLTYPE *AppDomainShutdownStarted )( 
            IEngine * This,
            /* [in] */ AppDomainID appDomainId);
        
        HRESULT ( STDMETHODCALLTYPE *AppDomainShutdownFinished )( 
            IEngine * This,
            /* [in] */ AppDomainID appDomainId,
            /* [in] */ HRESULT hrStatus);
        
        HRESULT ( STDMETHODCALLTYPE *AssemblyLoadStarted )( 
            IEngine * This,
            /* [in] */ AssemblyID assemblyId);
        
        HRESULT ( STDMETHODCALLTYPE *AssemblyLoadFinished )( 
            IEngine * This,
            /* [in] */ AssemblyID assemblyId,
            /* [in] */ HRESULT hrStatus);
        
        HRESULT ( STDMETHODCALLTYPE *AssemblyUnloadStarted )( 
            IEngine * This,
            /* [in] */ AssemblyID assemblyId);
        
        HRESULT ( STDMETHODCALLTYPE *AssemblyUnloadFinished )( 
            IEngine * This,
            /* [in] */ AssemblyID assemblyId,
            /* [in] */ HRESULT hrStatus);
        
        HRESULT ( STDMETHODCALLTYPE *ModuleLoadStarted )( 
            IEngine * This,
            /* [in] */ ModuleID moduleId);
        
        HRESULT ( STDMETHODCALLTYPE *ModuleLoadFinished )( 
            IEngine * This,
            /* [in] */ ModuleID moduleId,
            /* [in] */ HRESULT hrStatus);
        
        HRESULT ( STDMETHODCALLTYPE *ModuleUnloadStarted )( 
            IEngine * This,
            /* [in] */ ModuleID moduleId);
        
        HRESULT ( STDMETHODCALLTYPE *ModuleUnloadFinished )( 
            IEngine * This,
            /* [in] */ ModuleID moduleId,
            /* [in] */ HRESULT hrStatus);
        
        HRESULT ( STDMETHODCALLTYPE *ModuleAttachedToAssembly )( 
            IEngine * This,
            /* [in] */ ModuleID moduleId,
            /* [in] */ AssemblyID AssemblyId);
        
        HRESULT ( STDMETHODCALLTYPE *ClassLoadStarted )( 
            IEngine * This,
            /* [in] */ ClassID classId);
        
        HRESULT ( STDMETHODCALLTYPE *ClassLoadFinished )( 
            IEngine * This,
            /* [in] */ ClassID classId,
            /* [in] */ HRESULT hrStatus);
        
        HRESULT ( STDMETHODCALLTYPE *ClassUnloadStarted )( 
            IEngine * This,
            /* [in] */ ClassID classId);
        
        HRESULT ( STDMETHODCALLTYPE *ClassUnloadFinished )( 
            IEngine * This,
            /* [in] */ ClassID classId,
            /* [in] */ HRESULT hrStatus);
        
        HRESULT ( STDMETHODCALLTYPE *FunctionUnloadStarted )( 
            IEngine * This,
            /* [in] */ FunctionID functionId);
        
        HRESULT ( STDMETHODCALLTYPE *JITCompilationStarted )( 
            IEngine * This,
            /* [in] */ FunctionID functionId,
            /* [in] */ BOOL fIsSafeToBlock);
        
        HRESULT ( STDMETHODCALLTYPE *JITCompilationFinished )( 
            IEngine * This,
            /* [in] */ FunctionID functionId,
            /* [in] */ HRESULT hrStatus,
            /* [in] */ BOOL fIsSafeToBlock);
        
        HRESULT ( STDMETHODCALLTYPE *JITCachedFunctionSearchStarted )( 
            IEngine * This,
            /* [in] */ FunctionID functionId,
            /* [out] */ BOOL *pbUseCachedFunction);
        
        HRESULT ( STDMETHODCALLTYPE *JITCachedFunctionSearchFinished )( 
            IEngine * This,
            /* [in] */ FunctionID functionId,
            /* [in] */ COR_PRF_JIT_CACHE result);
        
        HRESULT ( STDMETHODCALLTYPE *JITFunctionPitched )( 
            IEngine * This,
            /* [in] */ FunctionID functionId);
        
        HRESULT ( STDMETHODCALLTYPE *JITInlining )( 
            IEngine * This,
            /* [in] */ FunctionID callerId,
            /* [in] */ FunctionID calleeId,
            /* [out] */ BOOL *pfShouldInline);
        
        HRESULT ( STDMETHODCALLTYPE *ThreadCreated )( 
            IEngine * This,
            /* [in] */ ThreadID threadId);
        
        HRESULT ( STDMETHODCALLTYPE *ThreadDestroyed )( 
            IEngine * This,
            /* [in] */ ThreadID threadId);
        
        HRESULT ( STDMETHODCALLTYPE *ThreadAssignedToOSThread )( 
            IEngine * This,
            /* [in] */ ThreadID managedThreadId,
            /* [in] */ DWORD osThreadId);
        
        HRESULT ( STDMETHODCALLTYPE *RemotingClientInvocationStarted )( 
            IEngine * This);
        
        HRESULT ( STDMETHODCALLTYPE *RemotingClientSendingMessage )( 
            IEngine * This,
            /* [in] */ GUID *pCookie,
            /* [in] */ BOOL fIsAsync);
        
        HRESULT ( STDMETHODCALLTYPE *RemotingClientReceivingReply )( 
            IEngine * This,
            /* [in] */ GUID *pCookie,
            /* [in] */ BOOL fIsAsync);
        
        HRESULT ( STDMETHODCALLTYPE *RemotingClientInvocationFinished )( 
            IEngine * This);
        
        HRESULT ( STDMETHODCALLTYPE *RemotingServerReceivingMessage )( 
            IEngine * This,
            /* [in] */ GUID *pCookie,
            /* [in] */ BOOL fIsAsync);
        
        HRESULT ( STDMETHODCALLTYPE *RemotingServerInvocationStarted )( 
            IEngine * This);
        
        HRESULT ( STDMETHODCALLTYPE *RemotingServerInvocationReturned )( 
            IEngine * This);
        
        HRESULT ( STDMETHODCALLTYPE *RemotingServerSendingReply )( 
            IEngine * This,
            /* [in] */ GUID *pCookie,
            /* [in] */ BOOL fIsAsync);
        
        HRESULT ( STDMETHODCALLTYPE *UnmanagedToManagedTransition )( 
            IEngine * This,
            /* [in] */ FunctionID functionId,
            /* [in] */ COR_PRF_TRANSITION_REASON reason);
        
        HRESULT ( STDMETHODCALLTYPE *ManagedToUnmanagedTransition )( 
            IEngine * This,
            /* [in] */ FunctionID functionId,
            /* [in] */ COR_PRF_TRANSITION_REASON reason);
        
        HRESULT ( STDMETHODCALLTYPE *RuntimeSuspendStarted )( 
            IEngine * This,
            /* [in] */ COR_PRF_SUSPEND_REASON suspendReason);
        
        HRESULT ( STDMETHODCALLTYPE *RuntimeSuspendFinished )( 
            IEngine * This);
        
        HRESULT ( STDMETHODCALLTYPE *RuntimeSuspendAborted )( 
            IEngine * This);
        
        HRESULT ( STDMETHODCALLTYPE *RuntimeResumeStarted )( 
            IEngine * This);
        
        HRESULT ( STDMETHODCALLTYPE *RuntimeResumeFinished )( 
            IEngine * This);
        
        HRESULT ( STDMETHODCALLTYPE *RuntimeThreadSuspended )( 
            IEngine * This,
            /* [in] */ ThreadID threadId);
        
        HRESULT ( STDMETHODCALLTYPE *RuntimeThreadResumed )( 
            IEngine * This,
            /* [in] */ ThreadID threadId);
        
        HRESULT ( STDMETHODCALLTYPE *MovedReferences )( 
            IEngine * This,
            /* [in] */ ULONG cMovedObjectIDRanges,
            /* [size_is][in] */ ObjectID oldObjectIDRangeStart[  ],
            /* [size_is][in] */ ObjectID newObjectIDRangeStart[  ],
            /* [size_is][in] */ ULONG cObjectIDRangeLength[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *ObjectAllocated )( 
            IEngine * This,
            /* [in] */ ObjectID objectId,
            /* [in] */ ClassID classId);
        
        HRESULT ( STDMETHODCALLTYPE *ObjectsAllocatedByClass )( 
            IEngine * This,
            /* [in] */ ULONG cClassCount,
            /* [size_is][in] */ ClassID classIds[  ],
            /* [size_is][in] */ ULONG cObjects[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *ObjectReferences )( 
            IEngine * This,
            /* [in] */ ObjectID objectId,
            /* [in] */ ClassID classId,
            /* [in] */ ULONG cObjectRefs,
            /* [size_is][in] */ ObjectID objectRefIds[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *RootReferences )( 
            IEngine * This,
            /* [in] */ ULONG cRootRefs,
            /* [size_is][in] */ ObjectID rootRefIds[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *ExceptionThrown )( 
            IEngine * This,
            /* [in] */ ObjectID thrownObjectId);
        
        HRESULT ( STDMETHODCALLTYPE *ExceptionSearchFunctionEnter )( 
            IEngine * This,
            /* [in] */ FunctionID functionId);
        
        HRESULT ( STDMETHODCALLTYPE *ExceptionSearchFunctionLeave )( 
            IEngine * This);
        
        HRESULT ( STDMETHODCALLTYPE *ExceptionSearchFilterEnter )( 
            IEngine * This,
            /* [in] */ FunctionID functionId);
        
        HRESULT ( STDMETHODCALLTYPE *ExceptionSearchFilterLeave )( 
            IEngine * This);
        
        HRESULT ( STDMETHODCALLTYPE *ExceptionSearchCatcherFound )( 
            IEngine * This,
            /* [in] */ FunctionID functionId);
        
        HRESULT ( STDMETHODCALLTYPE *ExceptionOSHandlerEnter )( 
            IEngine * This,
            /* [in] */ UINT_PTR __unused);
        
        HRESULT ( STDMETHODCALLTYPE *ExceptionOSHandlerLeave )( 
            IEngine * This,
            /* [in] */ UINT_PTR __unused);
        
        HRESULT ( STDMETHODCALLTYPE *ExceptionUnwindFunctionEnter )( 
            IEngine * This,
            /* [in] */ FunctionID functionId);
        
        HRESULT ( STDMETHODCALLTYPE *ExceptionUnwindFunctionLeave )( 
            IEngine * This);
        
        HRESULT ( STDMETHODCALLTYPE *ExceptionUnwindFinallyEnter )( 
            IEngine * This,
            /* [in] */ FunctionID functionId);
        
        HRESULT ( STDMETHODCALLTYPE *ExceptionUnwindFinallyLeave )( 
            IEngine * This);
        
        HRESULT ( STDMETHODCALLTYPE *ExceptionCatcherEnter )( 
            IEngine * This,
            /* [in] */ FunctionID functionId,
            /* [in] */ ObjectID objectId);
        
        HRESULT ( STDMETHODCALLTYPE *ExceptionCatcherLeave )( 
            IEngine * This);
        
        HRESULT ( STDMETHODCALLTYPE *COMClassicVTableCreated )( 
            IEngine * This,
            /* [in] */ ClassID wrappedClassId,
            /* [in] */ REFGUID implementedIID,
            /* [in] */ void *pVTable,
            /* [in] */ ULONG cSlots);
        
        HRESULT ( STDMETHODCALLTYPE *COMClassicVTableDestroyed )( 
            IEngine * This,
            /* [in] */ ClassID wrappedClassId,
            /* [in] */ REFGUID implementedIID,
            /* [in] */ void *pVTable);
        
        HRESULT ( STDMETHODCALLTYPE *ExceptionCLRCatcherFound )( 
            IEngine * This);
        
        HRESULT ( STDMETHODCALLTYPE *ExceptionCLRCatcherExecute )( 
            IEngine * This);
        
        HRESULT ( STDMETHODCALLTYPE *ThreadNameChanged )( 
            IEngine * This,
            /* [in] */ ThreadID threadId,
            /* [in] */ ULONG cchName,
            /* [in] */ WCHAR name[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *GarbageCollectionStarted )( 
            IEngine * This,
            /* [in] */ int cGenerations,
            /* [length_is][size_is][in] */ BOOL generationCollected[  ],
            /* [in] */ COR_PRF_GC_REASON reason);
        
        HRESULT ( STDMETHODCALLTYPE *SurvivingReferences )( 
            IEngine * This,
            /* [in] */ ULONG cSurvivingObjectIDRanges,
            /* [size_is][in] */ ObjectID objectIDRangeStart[  ],
            /* [size_is][in] */ ULONG cObjectIDRangeLength[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *GarbageCollectionFinished )( 
            IEngine * This);
        
        HRESULT ( STDMETHODCALLTYPE *FinalizeableObjectQueued )( 
            IEngine * This,
            /* [in] */ DWORD finalizerFlags,
            /* [in] */ ObjectID objectID);
        
        HRESULT ( STDMETHODCALLTYPE *RootReferences2 )( 
            IEngine * This,
            /* [in] */ ULONG cRootRefs,
            /* [size_is][in] */ ObjectID rootRefIds[  ],
            /* [size_is][in] */ COR_PRF_GC_ROOT_KIND rootKinds[  ],
            /* [size_is][in] */ COR_PRF_GC_ROOT_FLAGS rootFlags[  ],
            /* [size_is][in] */ UINT_PTR rootIds[  ]);
        
        HRESULT ( STDMETHODCALLTYPE *HandleCreated )( 
            IEngine * This,
            /* [in] */ GCHandleID handleId,
            /* [in] */ ObjectID initialObjectId);
        
        HRESULT ( STDMETHODCALLTYPE *HandleDestroyed )( 
            IEngine * This,
            /* [in] */ GCHandleID handleId);
        
        END_INTERFACE
    } IEngineVtbl;

    interface IEngine
    {
        CONST_VTBL struct IEngineVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEngine_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEngine_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEngine_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEngine_Initialize(This,pICorProfilerInfoUnk)	\
    ( (This)->lpVtbl -> Initialize(This,pICorProfilerInfoUnk) ) 

#define IEngine_Shutdown(This)	\
    ( (This)->lpVtbl -> Shutdown(This) ) 

#define IEngine_AppDomainCreationStarted(This,appDomainId)	\
    ( (This)->lpVtbl -> AppDomainCreationStarted(This,appDomainId) ) 

#define IEngine_AppDomainCreationFinished(This,appDomainId,hrStatus)	\
    ( (This)->lpVtbl -> AppDomainCreationFinished(This,appDomainId,hrStatus) ) 

#define IEngine_AppDomainShutdownStarted(This,appDomainId)	\
    ( (This)->lpVtbl -> AppDomainShutdownStarted(This,appDomainId) ) 

#define IEngine_AppDomainShutdownFinished(This,appDomainId,hrStatus)	\
    ( (This)->lpVtbl -> AppDomainShutdownFinished(This,appDomainId,hrStatus) ) 

#define IEngine_AssemblyLoadStarted(This,assemblyId)	\
    ( (This)->lpVtbl -> AssemblyLoadStarted(This,assemblyId) ) 

#define IEngine_AssemblyLoadFinished(This,assemblyId,hrStatus)	\
    ( (This)->lpVtbl -> AssemblyLoadFinished(This,assemblyId,hrStatus) ) 

#define IEngine_AssemblyUnloadStarted(This,assemblyId)	\
    ( (This)->lpVtbl -> AssemblyUnloadStarted(This,assemblyId) ) 

#define IEngine_AssemblyUnloadFinished(This,assemblyId,hrStatus)	\
    ( (This)->lpVtbl -> AssemblyUnloadFinished(This,assemblyId,hrStatus) ) 

#define IEngine_ModuleLoadStarted(This,moduleId)	\
    ( (This)->lpVtbl -> ModuleLoadStarted(This,moduleId) ) 

#define IEngine_ModuleLoadFinished(This,moduleId,hrStatus)	\
    ( (This)->lpVtbl -> ModuleLoadFinished(This,moduleId,hrStatus) ) 

#define IEngine_ModuleUnloadStarted(This,moduleId)	\
    ( (This)->lpVtbl -> ModuleUnloadStarted(This,moduleId) ) 

#define IEngine_ModuleUnloadFinished(This,moduleId,hrStatus)	\
    ( (This)->lpVtbl -> ModuleUnloadFinished(This,moduleId,hrStatus) ) 

#define IEngine_ModuleAttachedToAssembly(This,moduleId,AssemblyId)	\
    ( (This)->lpVtbl -> ModuleAttachedToAssembly(This,moduleId,AssemblyId) ) 

#define IEngine_ClassLoadStarted(This,classId)	\
    ( (This)->lpVtbl -> ClassLoadStarted(This,classId) ) 

#define IEngine_ClassLoadFinished(This,classId,hrStatus)	\
    ( (This)->lpVtbl -> ClassLoadFinished(This,classId,hrStatus) ) 

#define IEngine_ClassUnloadStarted(This,classId)	\
    ( (This)->lpVtbl -> ClassUnloadStarted(This,classId) ) 

#define IEngine_ClassUnloadFinished(This,classId,hrStatus)	\
    ( (This)->lpVtbl -> ClassUnloadFinished(This,classId,hrStatus) ) 

#define IEngine_FunctionUnloadStarted(This,functionId)	\
    ( (This)->lpVtbl -> FunctionUnloadStarted(This,functionId) ) 

#define IEngine_JITCompilationStarted(This,functionId,fIsSafeToBlock)	\
    ( (This)->lpVtbl -> JITCompilationStarted(This,functionId,fIsSafeToBlock) ) 

#define IEngine_JITCompilationFinished(This,functionId,hrStatus,fIsSafeToBlock)	\
    ( (This)->lpVtbl -> JITCompilationFinished(This,functionId,hrStatus,fIsSafeToBlock) ) 

#define IEngine_JITCachedFunctionSearchStarted(This,functionId,pbUseCachedFunction)	\
    ( (This)->lpVtbl -> JITCachedFunctionSearchStarted(This,functionId,pbUseCachedFunction) ) 

#define IEngine_JITCachedFunctionSearchFinished(This,functionId,result)	\
    ( (This)->lpVtbl -> JITCachedFunctionSearchFinished(This,functionId,result) ) 

#define IEngine_JITFunctionPitched(This,functionId)	\
    ( (This)->lpVtbl -> JITFunctionPitched(This,functionId) ) 

#define IEngine_JITInlining(This,callerId,calleeId,pfShouldInline)	\
    ( (This)->lpVtbl -> JITInlining(This,callerId,calleeId,pfShouldInline) ) 

#define IEngine_ThreadCreated(This,threadId)	\
    ( (This)->lpVtbl -> ThreadCreated(This,threadId) ) 

#define IEngine_ThreadDestroyed(This,threadId)	\
    ( (This)->lpVtbl -> ThreadDestroyed(This,threadId) ) 

#define IEngine_ThreadAssignedToOSThread(This,managedThreadId,osThreadId)	\
    ( (This)->lpVtbl -> ThreadAssignedToOSThread(This,managedThreadId,osThreadId) ) 

#define IEngine_RemotingClientInvocationStarted(This)	\
    ( (This)->lpVtbl -> RemotingClientInvocationStarted(This) ) 

#define IEngine_RemotingClientSendingMessage(This,pCookie,fIsAsync)	\
    ( (This)->lpVtbl -> RemotingClientSendingMessage(This,pCookie,fIsAsync) ) 

#define IEngine_RemotingClientReceivingReply(This,pCookie,fIsAsync)	\
    ( (This)->lpVtbl -> RemotingClientReceivingReply(This,pCookie,fIsAsync) ) 

#define IEngine_RemotingClientInvocationFinished(This)	\
    ( (This)->lpVtbl -> RemotingClientInvocationFinished(This) ) 

#define IEngine_RemotingServerReceivingMessage(This,pCookie,fIsAsync)	\
    ( (This)->lpVtbl -> RemotingServerReceivingMessage(This,pCookie,fIsAsync) ) 

#define IEngine_RemotingServerInvocationStarted(This)	\
    ( (This)->lpVtbl -> RemotingServerInvocationStarted(This) ) 

#define IEngine_RemotingServerInvocationReturned(This)	\
    ( (This)->lpVtbl -> RemotingServerInvocationReturned(This) ) 

#define IEngine_RemotingServerSendingReply(This,pCookie,fIsAsync)	\
    ( (This)->lpVtbl -> RemotingServerSendingReply(This,pCookie,fIsAsync) ) 

#define IEngine_UnmanagedToManagedTransition(This,functionId,reason)	\
    ( (This)->lpVtbl -> UnmanagedToManagedTransition(This,functionId,reason) ) 

#define IEngine_ManagedToUnmanagedTransition(This,functionId,reason)	\
    ( (This)->lpVtbl -> ManagedToUnmanagedTransition(This,functionId,reason) ) 

#define IEngine_RuntimeSuspendStarted(This,suspendReason)	\
    ( (This)->lpVtbl -> RuntimeSuspendStarted(This,suspendReason) ) 

#define IEngine_RuntimeSuspendFinished(This)	\
    ( (This)->lpVtbl -> RuntimeSuspendFinished(This) ) 

#define IEngine_RuntimeSuspendAborted(This)	\
    ( (This)->lpVtbl -> RuntimeSuspendAborted(This) ) 

#define IEngine_RuntimeResumeStarted(This)	\
    ( (This)->lpVtbl -> RuntimeResumeStarted(This) ) 

#define IEngine_RuntimeResumeFinished(This)	\
    ( (This)->lpVtbl -> RuntimeResumeFinished(This) ) 

#define IEngine_RuntimeThreadSuspended(This,threadId)	\
    ( (This)->lpVtbl -> RuntimeThreadSuspended(This,threadId) ) 

#define IEngine_RuntimeThreadResumed(This,threadId)	\
    ( (This)->lpVtbl -> RuntimeThreadResumed(This,threadId) ) 

#define IEngine_MovedReferences(This,cMovedObjectIDRanges,oldObjectIDRangeStart,newObjectIDRangeStart,cObjectIDRangeLength)	\
    ( (This)->lpVtbl -> MovedReferences(This,cMovedObjectIDRanges,oldObjectIDRangeStart,newObjectIDRangeStart,cObjectIDRangeLength) ) 

#define IEngine_ObjectAllocated(This,objectId,classId)	\
    ( (This)->lpVtbl -> ObjectAllocated(This,objectId,classId) ) 

#define IEngine_ObjectsAllocatedByClass(This,cClassCount,classIds,cObjects)	\
    ( (This)->lpVtbl -> ObjectsAllocatedByClass(This,cClassCount,classIds,cObjects) ) 

#define IEngine_ObjectReferences(This,objectId,classId,cObjectRefs,objectRefIds)	\
    ( (This)->lpVtbl -> ObjectReferences(This,objectId,classId,cObjectRefs,objectRefIds) ) 

#define IEngine_RootReferences(This,cRootRefs,rootRefIds)	\
    ( (This)->lpVtbl -> RootReferences(This,cRootRefs,rootRefIds) ) 

#define IEngine_ExceptionThrown(This,thrownObjectId)	\
    ( (This)->lpVtbl -> ExceptionThrown(This,thrownObjectId) ) 

#define IEngine_ExceptionSearchFunctionEnter(This,functionId)	\
    ( (This)->lpVtbl -> ExceptionSearchFunctionEnter(This,functionId) ) 

#define IEngine_ExceptionSearchFunctionLeave(This)	\
    ( (This)->lpVtbl -> ExceptionSearchFunctionLeave(This) ) 

#define IEngine_ExceptionSearchFilterEnter(This,functionId)	\
    ( (This)->lpVtbl -> ExceptionSearchFilterEnter(This,functionId) ) 

#define IEngine_ExceptionSearchFilterLeave(This)	\
    ( (This)->lpVtbl -> ExceptionSearchFilterLeave(This) ) 

#define IEngine_ExceptionSearchCatcherFound(This,functionId)	\
    ( (This)->lpVtbl -> ExceptionSearchCatcherFound(This,functionId) ) 

#define IEngine_ExceptionOSHandlerEnter(This,__unused)	\
    ( (This)->lpVtbl -> ExceptionOSHandlerEnter(This,__unused) ) 

#define IEngine_ExceptionOSHandlerLeave(This,__unused)	\
    ( (This)->lpVtbl -> ExceptionOSHandlerLeave(This,__unused) ) 

#define IEngine_ExceptionUnwindFunctionEnter(This,functionId)	\
    ( (This)->lpVtbl -> ExceptionUnwindFunctionEnter(This,functionId) ) 

#define IEngine_ExceptionUnwindFunctionLeave(This)	\
    ( (This)->lpVtbl -> ExceptionUnwindFunctionLeave(This) ) 

#define IEngine_ExceptionUnwindFinallyEnter(This,functionId)	\
    ( (This)->lpVtbl -> ExceptionUnwindFinallyEnter(This,functionId) ) 

#define IEngine_ExceptionUnwindFinallyLeave(This)	\
    ( (This)->lpVtbl -> ExceptionUnwindFinallyLeave(This) ) 

#define IEngine_ExceptionCatcherEnter(This,functionId,objectId)	\
    ( (This)->lpVtbl -> ExceptionCatcherEnter(This,functionId,objectId) ) 

#define IEngine_ExceptionCatcherLeave(This)	\
    ( (This)->lpVtbl -> ExceptionCatcherLeave(This) ) 

#define IEngine_COMClassicVTableCreated(This,wrappedClassId,implementedIID,pVTable,cSlots)	\
    ( (This)->lpVtbl -> COMClassicVTableCreated(This,wrappedClassId,implementedIID,pVTable,cSlots) ) 

#define IEngine_COMClassicVTableDestroyed(This,wrappedClassId,implementedIID,pVTable)	\
    ( (This)->lpVtbl -> COMClassicVTableDestroyed(This,wrappedClassId,implementedIID,pVTable) ) 

#define IEngine_ExceptionCLRCatcherFound(This)	\
    ( (This)->lpVtbl -> ExceptionCLRCatcherFound(This) ) 

#define IEngine_ExceptionCLRCatcherExecute(This)	\
    ( (This)->lpVtbl -> ExceptionCLRCatcherExecute(This) ) 


#define IEngine_ThreadNameChanged(This,threadId,cchName,name)	\
    ( (This)->lpVtbl -> ThreadNameChanged(This,threadId,cchName,name) ) 

#define IEngine_GarbageCollectionStarted(This,cGenerations,generationCollected,reason)	\
    ( (This)->lpVtbl -> GarbageCollectionStarted(This,cGenerations,generationCollected,reason) ) 

#define IEngine_SurvivingReferences(This,cSurvivingObjectIDRanges,objectIDRangeStart,cObjectIDRangeLength)	\
    ( (This)->lpVtbl -> SurvivingReferences(This,cSurvivingObjectIDRanges,objectIDRangeStart,cObjectIDRangeLength) ) 

#define IEngine_GarbageCollectionFinished(This)	\
    ( (This)->lpVtbl -> GarbageCollectionFinished(This) ) 

#define IEngine_FinalizeableObjectQueued(This,finalizerFlags,objectID)	\
    ( (This)->lpVtbl -> FinalizeableObjectQueued(This,finalizerFlags,objectID) ) 

#define IEngine_RootReferences2(This,cRootRefs,rootRefIds,rootKinds,rootFlags,rootIds)	\
    ( (This)->lpVtbl -> RootReferences2(This,cRootRefs,rootRefIds,rootKinds,rootFlags,rootIds) ) 

#define IEngine_HandleCreated(This,handleId,initialObjectId)	\
    ( (This)->lpVtbl -> HandleCreated(This,handleId,initialObjectId) ) 

#define IEngine_HandleDestroyed(This,handleId)	\
    ( (This)->lpVtbl -> HandleDestroyed(This,handleId) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEngine_INTERFACE_DEFINED__ */



#ifndef __FaultInjectionEngineLib_LIBRARY_DEFINED__
#define __FaultInjectionEngineLib_LIBRARY_DEFINED__

/* library FaultInjectionEngineLib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_FaultInjectionEngineLib;

EXTERN_C const CLSID CLSID_Engine;

#ifdef __cplusplus

class DECLSPEC_UUID("2EB6DCDB-3250-4D7F-AA42-41B1B84113ED")
Engine;
#endif
#endif /* __FaultInjectionEngineLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


