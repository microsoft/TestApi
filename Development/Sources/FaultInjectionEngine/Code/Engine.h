// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#pragma once
#include "resource.h"       // main symbols
#include "FaultInjectionEngine.h"


#if defined(_WIN32_WCE) && !defined(_CE_DCOM) && !defined(_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA)
#error "Single-threaded COM objects are not properly supported on Windows CE platform, such as the Windows Mobile platforms that do not include full DCOM support. Define _CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA to force ATL to support creating single-thread COM object's and allow use of it's single-threaded COM object implementations. The threading model in your rgs file was set to 'Free' as that is the only threading model supported in non DCOM Windows CE platforms."
#endif


// CEngine

class ATL_NO_VTABLE CEngine :
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CEngine, &CLSID_Engine>,
	public IDispatchImpl<IEngine, &IID_IEngine, &LIBID_FaultInjectionEngineLib, /*wMajor =*/ 1, /*wMinor =*/ 0>
{
public:
	CEngine()
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_ENGINE)


BEGIN_COM_MAP(CEngine)
	COM_INTERFACE_ENTRY(IEngine)
	COM_INTERFACE_ENTRY(ICorProfilerCallback2)
END_COM_MAP()



	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}

	void FinalRelease()
	{
	}

public:

#pragma region Private Member Methods
private:
    /// <summary>
    /// Load method filter (list of methods to be trapped) from file into memory.
    /// The method filter file is created by API, and the pathname of the file is
    /// passed from API to Engine through environment variables.
    /// </summary>
    BOOL LoadMethodFilter(void);

    /// <summary>
    /// See if the method should be trapped (prologue insearted). Based on the full
    /// qualified name of the method.
    /// </summary>
    BOOL ShouldMethodBeTrapped(CString szFullQualifiedMethodName) const;
#pragma endregion

#pragma region Private Member Variables
private:
    CComQIPtr<ICorProfilerInfo> m_pCorProfilerInfo;  // pointer of CLR
    CAtlArray<CString> m_vszMethodsToBeTrapped;  // name list of methods to be trapped
#pragma endregion

#pragma region Virtual Methods Derived from ICorProfilerCallback2
public:
    STDMETHOD(ThreadNameChanged)( 
        /* [in] */ ThreadID threadId,
        /* [in] */ ULONG cchName,
        /* [in] */ WCHAR name[  ]);

    STDMETHOD(GarbageCollectionStarted)( 
        /* [in] */ int cGenerations,
        /* [length_is][size_is][in] */ BOOL generationCollected[  ],
        /* [in] */ COR_PRF_GC_REASON reason);

    STDMETHOD(SurvivingReferences)( 
        /* [in] */ ULONG cSurvivingObjectIDRanges,
        /* [size_is][in] */ ObjectID objectIDRangeStart[  ],
        /* [size_is][in] */ ULONG cObjectIDRangeLength[  ]);

    STDMETHOD(GarbageCollectionFinished)(void);

    STDMETHOD(FinalizeableObjectQueued)( 
        /* [in] */ DWORD finalizerFlags,
        /* [in] */ ObjectID objectID);

    STDMETHOD(RootReferences2)( 
        /* [in] */ ULONG cRootRefs,
        /* [size_is][in] */ ObjectID rootRefIds[  ],
        /* [size_is][in] */ COR_PRF_GC_ROOT_KIND rootKinds[  ],
        /* [size_is][in] */ COR_PRF_GC_ROOT_FLAGS rootFlags[  ],
        /* [size_is][in] */ UINT_PTR rootIds[  ]);

    STDMETHOD(HandleCreated)( 
        /* [in] */ GCHandleID handleId,
        /* [in] */ ObjectID initialObjectId);

    STDMETHOD(HandleDestroyed)( 
        /* [in] */ GCHandleID handleId);

    // ICorProfilerCallback
    STDMETHOD(Initialize)( 
        /* [in] */ IUnknown *pICorProfilerInfoUnk);

    STDMETHOD(Shutdown)(void);

    STDMETHOD(AppDomainCreationStarted)( 
        /* [in] */ AppDomainID appDomainId);

    STDMETHOD(AppDomainCreationFinished)( 
        /* [in] */ AppDomainID appDomainId,
        /* [in] */ HRESULT hrStatus);

    STDMETHOD(AppDomainShutdownStarted)( 
        /* [in] */ AppDomainID appDomainId);

    STDMETHOD(AppDomainShutdownFinished)( 
        /* [in] */ AppDomainID appDomainId,
        /* [in] */ HRESULT hrStatus);

    STDMETHOD(AssemblyLoadStarted)( 
        /* [in] */ AssemblyID assemblyId);

    STDMETHOD(AssemblyLoadFinished)( 
        /* [in] */ AssemblyID assemblyId,
        /* [in] */ HRESULT hrStatus);

    STDMETHOD(AssemblyUnloadStarted)( 
        /* [in] */ AssemblyID assemblyId);

    STDMETHOD(AssemblyUnloadFinished)( 
        /* [in] */ AssemblyID assemblyId,
        /* [in] */ HRESULT hrStatus);

    STDMETHOD(ModuleLoadStarted)( 
        /* [in] */ ModuleID moduleId);

    STDMETHOD(ModuleLoadFinished)( 
        /* [in] */ ModuleID moduleId,
        /* [in] */ HRESULT hrStatus);

    STDMETHOD(ModuleUnloadStarted)( 
        /* [in] */ ModuleID moduleId);

    STDMETHOD(ModuleUnloadFinished)( 
        /* [in] */ ModuleID moduleId,
        /* [in] */ HRESULT hrStatus);

    STDMETHOD(ModuleAttachedToAssembly)( 
        /* [in] */ ModuleID moduleId,
        /* [in] */ AssemblyID AssemblyId);

    STDMETHOD(ClassLoadStarted)( 
        /* [in] */ ClassID classId);

    STDMETHOD(ClassLoadFinished)( 
        /* [in] */ ClassID classId,
        /* [in] */ HRESULT hrStatus);

    STDMETHOD(ClassUnloadStarted)( 
        /* [in] */ ClassID classId);

    STDMETHOD(ClassUnloadFinished)( 
        /* [in] */ ClassID classId,
        /* [in] */ HRESULT hrStatus);

    STDMETHOD(FunctionUnloadStarted)( 
        /* [in] */ FunctionID functionId);

    STDMETHOD(JITCompilationStarted)( 
        /* [in] */ FunctionID functionId,
        /* [in] */ BOOL fIsSafeToBlock);

    STDMETHOD(JITCompilationFinished)( 
        /* [in] */ FunctionID functionId,
        /* [in] */ HRESULT hrStatus,
        /* [in] */ BOOL fIsSafeToBlock);

    STDMETHOD(JITCachedFunctionSearchStarted)( 
        /* [in] */ FunctionID functionId,
        /* [out] */ BOOL *pbUseCachedFunction);

    STDMETHOD(JITCachedFunctionSearchFinished)( 
        /* [in] */ FunctionID functionId,
        /* [in] */ COR_PRF_JIT_CACHE result);

    STDMETHOD(JITFunctionPitched)( 
        /* [in] */ FunctionID functionId);

    STDMETHOD(JITInlining)( 
        /* [in] */ FunctionID callerId,
        /* [in] */ FunctionID calleeId,
        /* [out] */ BOOL *pfShouldInline);

    STDMETHOD(ThreadCreated)( 
        /* [in] */ ThreadID threadId);

    STDMETHOD(ThreadDestroyed)( 
        /* [in] */ ThreadID threadId);

    STDMETHOD(ThreadAssignedToOSThread)( 
        /* [in] */ ThreadID managedThreadId,
        /* [in] */ DWORD osThreadId);

    STDMETHOD(RemotingClientInvocationStarted)(void);

    STDMETHOD(RemotingClientSendingMessage)( 
        /* [in] */ GUID *pCookie,
        /* [in] */ BOOL fIsAsync);

    STDMETHOD(RemotingClientReceivingReply)( 
        /* [in] */ GUID *pCookie,
        /* [in] */ BOOL fIsAsync);

    STDMETHOD(RemotingClientInvocationFinished)(void);

    STDMETHOD(RemotingServerReceivingMessage)( 
        /* [in] */ GUID *pCookie,
        /* [in] */ BOOL fIsAsync);

    STDMETHOD(RemotingServerInvocationStarted)(void);

    STDMETHOD(RemotingServerInvocationReturned)(void);

    STDMETHOD(RemotingServerSendingReply)( 
        /* [in] */ GUID *pCookie,
        /* [in] */ BOOL fIsAsync);

    STDMETHOD(UnmanagedToManagedTransition)( 
        /* [in] */ FunctionID functionId,
        /* [in] */ COR_PRF_TRANSITION_REASON reason);

    STDMETHOD(ManagedToUnmanagedTransition)( 
        /* [in] */ FunctionID functionId,
        /* [in] */ COR_PRF_TRANSITION_REASON reason);

    STDMETHOD(RuntimeSuspendStarted)( 
        /* [in] */ COR_PRF_SUSPEND_REASON suspendReason);

    STDMETHOD(RuntimeSuspendFinished)(void);

    STDMETHOD(RuntimeSuspendAborted)(void);

    STDMETHOD(RuntimeResumeStarted)(void);

    STDMETHOD(RuntimeResumeFinished)(void);

    STDMETHOD(RuntimeThreadSuspended)( 
        /* [in] */ ThreadID threadId);

    STDMETHOD(RuntimeThreadResumed)( 
        /* [in] */ ThreadID threadId);

    STDMETHOD(MovedReferences)( 
        /* [in] */ ULONG cMovedObjectIDRanges,
        /* [size_is][in] */ ObjectID oldObjectIDRangeStart[  ],
        /* [size_is][in] */ ObjectID newObjectIDRangeStart[  ],
        /* [size_is][in] */ ULONG cObjectIDRangeLength[  ]);

    STDMETHOD(ObjectAllocated)( 
        /* [in] */ ObjectID objectId,
        /* [in] */ ClassID classId);

    STDMETHOD(ObjectsAllocatedByClass)( 
        /* [in] */ ULONG cClassCount,
        /* [size_is][in] */ ClassID classIds[  ],
        /* [size_is][in] */ ULONG cObjects[  ]);

    STDMETHOD(ObjectReferences)( 
        /* [in] */ ObjectID objectId,
        /* [in] */ ClassID classId,
        /* [in] */ ULONG cObjectRefs,
        /* [size_is][in] */ ObjectID objectRefIds[  ]);

    STDMETHOD(RootReferences)( 
        /* [in] */ ULONG cRootRefs,
        /* [size_is][in] */ ObjectID rootRefIds[  ]);

    STDMETHOD(ExceptionThrown)( 
        /* [in] */ ObjectID thrownObjectId);

    STDMETHOD(ExceptionSearchFunctionEnter)( 
        /* [in] */ FunctionID functionId);

    STDMETHOD(ExceptionSearchFunctionLeave)(void);

    STDMETHOD(ExceptionSearchFilterEnter)( 
        /* [in] */ FunctionID functionId);

    STDMETHOD(ExceptionSearchFilterLeave)(void);

    STDMETHOD(ExceptionSearchCatcherFound)( 
        /* [in] */ FunctionID functionId);

    STDMETHOD(ExceptionOSHandlerEnter)( 
        /* [in] */ UINT_PTR __unused);

    STDMETHOD(ExceptionOSHandlerLeave)( 
        /* [in] */ UINT_PTR __unused);

    STDMETHOD(ExceptionUnwindFunctionEnter)( 
        /* [in] */ FunctionID functionId);

    STDMETHOD(ExceptionUnwindFunctionLeave)(void);

    STDMETHOD(ExceptionUnwindFinallyEnter)( 
        /* [in] */ FunctionID functionId);

    STDMETHOD(ExceptionUnwindFinallyLeave)(void);

    STDMETHOD(ExceptionCatcherEnter)( 
        /* [in] */ FunctionID functionId,
        /* [in] */ ObjectID objectId);

    STDMETHOD(ExceptionCatcherLeave)(void);

    STDMETHOD(COMClassicVTableCreated)( 
        /* [in] */ ClassID wrappedClassId,
        /* [in] */ REFGUID implementedIID,
        /* [in] */ void *pVTable,
        /* [in] */ ULONG cSlots);

    STDMETHOD(COMClassicVTableDestroyed)( 
        /* [in] */ ClassID wrappedClassId,
        /* [in] */ REFGUID implementedIID,
        /* [in] */ void *pVTable);

    STDMETHOD(ExceptionCLRCatcherFound)(void);

    STDMETHOD(ExceptionCLRCatcherExecute)(void);
#pragma endregion

};

OBJECT_ENTRY_AUTO(__uuidof(Engine), CEngine)
