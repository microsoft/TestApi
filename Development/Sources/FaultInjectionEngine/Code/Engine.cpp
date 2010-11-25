// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#include "stdafx.h"
#include "Engine.h"
#include "Settings.h"
#include "Exceptions.h"
#include "TraceAndLog.h"
#include "MetadataMethod.h"
#include "MetadataModule.h"

USING_DEFAULT_NAMESPACE

#pragma region Implementation of CEngine

#pragma region Private methods

BOOL CEngine::LoadMethodFilter(void)
{
    this->m_vszMethodsToBeTrapped.RemoveAll();  // Clear namelist.

    // Open method-filter file.
    CReadTextFile xMethodFilterFile;
    xMethodFilterFile.Open(CSettings::GetMethodFilterFile());
    if(!xMethodFilterFile.IsOpened())
    {
        EventReportError(IDS_REPORT_OPEN_METHOD_FILTER_FAILED, CSettings::GetMethodFilterFile());
        return FALSE;
    }

    // Read method-filter file. Each line is one method's full-qualified name.
    while(!xMethodFilterFile.IsEndOfFile())
    {
        CString szMethodName = xMethodFilterFile.ReadLine(PREFERRED_QUALIFIED_METHOD_NAME_LENGTH);
        szMethodName.Trim();
        if(!szMethodName.IsEmpty())  // Skip empty lines.
        {
            // Check if the method is in protected namespaces.
            int i = 0;
            for(; NULL != CSettings::GetProtectedNamespaceList()[i]; i++)
            {
                if(0 == szMethodName.Find(CSettings::GetProtectedNamespaceList()[i]))
                {
                    EventReportWarning(IDS_REPORT_METHOD_INSIDE_PROTECTED_NAMESPACE,
                        szMethodName, CSettings::GetProtectedNamespaceList()[i]);
                    break;
                }
            }
            if(NULL == CSettings::GetProtectedNamespaceList()[i])
            {
                // Add method to name list only if not in protected-namespaces.
                this->m_vszMethodsToBeTrapped.Add(szMethodName);
            }
        }
    }

    // Log method filter.
    EventReportInfo(IDS_REPORT_METHOD_FILTER_LIST_HEADER);
    for(size_t i = 0; i < this->m_vszMethodsToBeTrapped.GetCount(); i++)
    {
        EventReportInfo(IDS_REPORT_METHOD_FILTER_LIST_ELEMENT, i, this->m_vszMethodsToBeTrapped[i]);
    }
    EventReportInfo(IDS_REPORT_METHOD_FILTER_LIST_FOOTER);

    return TRUE;
}

BOOL CEngine::ShouldMethodBeTrapped(CString szFullQualifiedMethodName) const
{
    // Check if the method's full-qualified name equal to someone of the method-filter.
    for(size_t i = 0; i < this->m_vszMethodsToBeTrapped.GetCount(); i++)
    {
        if(this->m_vszMethodsToBeTrapped[i] == szFullQualifiedMethodName)
        {
            return TRUE;
        }
    }
    return FALSE;
}

#pragma endregion

#pragma region Virtual Methods Derived from ICorProfilerCallback2 (Implemented Ones)

STDMETHODIMP CEngine::Initialize(/* [in] */  IUnknown *pICorProfilerInfoUnk)
{
    DebugTrace(_T("<!-- Enter: MS::WSS::FI::CEngine::Initialize() --->"));

    // Initialize event-log. Log the start-up info.
    CEventLog::Initialize();
    EventReportInfo(IDS_REPORT_ENGINE_START);

    // Load method filter. We will determine whether a method should be trapped based on this filter.
    if(!this->LoadMethodFilter())
    {
        return E_FAIL;  // If the method-filter loading fails, no method would be trapped.
    }

    // Query interface ICorProfilerInfo using CComQIPtr<ICorProfilerInfo>.
    this->m_pCorProfilerInfo = pICorProfilerInfoUnk;
    if(NULL == this->m_pCorProfilerInfo)
    {
        EventReportError(IDS_REPORT_FAILED_QUERY_INTERFACE, pICorProfilerInfoUnk, _T("IUnknown"), _T("ICorProfilerInfo"));
        return E_FAIL;
    }

    DebugTrace(_T("Connect to CLR : (ICorProfileInfo*)(0x%08x)"), this->m_pCorProfilerInfo.p);

    // Set the event mask to specify what events we want to receive.
    this->m_pCorProfilerInfo->SetEventMask(0
        // Use this macro to turn off monitoring classes under System namespace.
#if !defined(BYPASS_CLI_SYSTEM_CLASSES)
        | COR_PRF_MONITOR_ENTERLEAVE 
        | COR_PRF_MONITOR_CACHE_SEARCHES
#endif
        | COR_PRF_DISABLE_INLINING
        | COR_PRF_MONITOR_JIT_COMPILATION
        );

    return S_OK;
}

STDMETHODIMP CEngine::JITInlining(/* [in] */  FunctionID callerId,
                                  /* [in] */  FunctionID calleeId,
                                  /* [out] */ BOOL *pfShouldInline)
{
    DebugTrace(_T("<!-- Enter: MS::WSS::FI::CEngine::JITInlining() --->"));

    // Trapped functions should never be called as inlining, if the CEngine is working.
    *pfShouldInline = (NULL == this->m_pCorProfilerInfo);
    return S_OK;
}

STDMETHODIMP CEngine::JITCompilationStarted(/* [in] */ FunctionID functionId,
                                            /* [in] */ BOOL fIsSafeToBlock)
{
    DebugTrace(_T("<!-- Enter: MS::WSS::FI::CEngine::JITCompilationStarted() --->"));

    if(NULL == this->m_pCorProfilerInfo)
    {
        return E_FAIL;
    }

    // Get module id and method-def token from CLR by function id.
    ClassID classId;
    ModuleID moduleId;
    mdMethodDef tkMethodDef;
    HRESULT hr = this->m_pCorProfilerInfo->GetFunctionInfo(functionId, &classId, &moduleId, &tkMethodDef);
    if(FAILED(hr))
    {
        EventReportError(IDS_REPORT_FAILED_GET_FUNCTION_INFO, hr, functionId);
        return E_FAIL;
    }

    try
    {
        CMetadataMethod xCurrentMethod(tkMethodDef);
        CMetadataModule xCurrentModule(this->m_pCorProfilerInfo, moduleId);

        xCurrentModule.LoadMethodProperties(xCurrentMethod);
        if(this->ShouldMethodBeTrapped(xCurrentMethod.GetFullQualifiedMethodName()))
        {
            DebugTrace(_T("Trap method: %s ..."), xCurrentMethod.GetFullQualifiedMethodName());
            xCurrentModule.InsertPrologueIntoMethod(xCurrentMethod);
            EventReportInfo(IDS_REPORT_SUCCESSFULLY_MODIFY_METHOD, xCurrentMethod.GetFullQualifiedMethodName());
        }
        else
        {
            DebugTrace(_T("Bypass method: %s"), xCurrentMethod.GetFullQualifiedMethodName());
        }

    }
    catch(CExceptionAsBreak* /*&sharedExceptionAsBreak*/)
    {
        // Do NOT delete the caught exception. It's shared (static) one.
        // Catching this exception just mean error in callees and execution should be broken.
        return E_FAIL;
    }
    return S_OK;
}

#pragma endregion

#pragma region Virtual Methods Derived from ICorProfilerCallback2 (Not-Implemented Ones)

STDMETHODIMP CEngine::Shutdown(void)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::AppDomainCreationStarted( 
    /* [in] */ AppDomainID appDomainId)
{
    return E_NOTIMPL;
}


STDMETHODIMP CEngine::AppDomainCreationFinished( 
    /* [in] */ AppDomainID appDomainId,
    /* [in] */ HRESULT hrStatus)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::AppDomainShutdownStarted( 
    /* [in] */ AppDomainID appDomainId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::AppDomainShutdownFinished( 
    /* [in] */ AppDomainID appDomainId,
    /* [in] */ HRESULT hrStatus)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::AssemblyLoadStarted( 
    /* [in] */ AssemblyID assemblyId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::AssemblyLoadFinished( 
    /* [in] */ AssemblyID assemblyId,
    /* [in] */ HRESULT hrStatus)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::AssemblyUnloadStarted( 
    /* [in] */ AssemblyID assemblyId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::AssemblyUnloadFinished( 
    /* [in] */ AssemblyID assemblyId,
    /* [in] */ HRESULT hrStatus)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ModuleLoadStarted( 
                                        /* [in] */ ModuleID moduleId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ModuleLoadFinished( 
    /* [in] */ ModuleID moduleId,
    /* [in] */ HRESULT hrStatus)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ModuleUnloadStarted( 
    /* [in] */ ModuleID moduleId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ModuleUnloadFinished( 
    /* [in] */ ModuleID moduleId,
    /* [in] */ HRESULT hrStatus)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ModuleAttachedToAssembly( 
    /* [in] */ ModuleID moduleId,
    /* [in] */ AssemblyID AssemblyId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ClassLoadStarted( 
                                       /* [in] */ ClassID classId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ClassLoadFinished( 
                                        /* [in] */ ClassID classId,
                                        /* [in] */ HRESULT hrStatus)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ClassUnloadStarted( 
    /* [in] */ ClassID classId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ClassUnloadFinished( 
    /* [in] */ ClassID classId,
    /* [in] */ HRESULT hrStatus)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::FunctionUnloadStarted( 
    /* [in] */ FunctionID functionId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::JITCompilationFinished( 
    /* [in] */ FunctionID functionId,
    /* [in] */ HRESULT hrStatus,
    /* [in] */ BOOL fIsSafeToBlock)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::JITCachedFunctionSearchStarted( 
    /* [in] */ FunctionID functionId,
    /* [out] */ BOOL *pbUseCachedFunction)
{

    if (pbUseCachedFunction == NULL)
    {
        return E_POINTER;
    }

    // Never use the cached function
    *pbUseCachedFunction = FALSE;

    return E_NOTIMPL;
}

STDMETHODIMP CEngine::JITCachedFunctionSearchFinished( 
    /* [in] */ FunctionID functionId,
    /* [in] */ COR_PRF_JIT_CACHE result)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::JITFunctionPitched( 
    /* [in] */ FunctionID functionId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ThreadCreated( 
                                    /* [in] */ ThreadID threadId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ThreadDestroyed( 
                                      /* [in] */ ThreadID threadId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ThreadAssignedToOSThread( 
    /* [in] */ ThreadID managedThreadId,
    /* [in] */ DWORD osThreadId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::RemotingClientInvocationStarted(void)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::RemotingClientSendingMessage( 
    /* [in] */ GUID *pCookie,
    /* [in] */ BOOL fIsAsync)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::RemotingClientReceivingReply( 
    /* [in] */ GUID *pCookie,
    /* [in] */ BOOL fIsAsync)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::RemotingClientInvocationFinished(void)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::RemotingServerReceivingMessage( 
    /* [in] */ GUID *pCookie,
    /* [in] */ BOOL fIsAsync)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::RemotingServerInvocationStarted(void)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::RemotingServerInvocationReturned(void)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::RemotingServerSendingReply( 
    /* [in] */ GUID *pCookie,
    /* [in] */ BOOL fIsAsync)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::UnmanagedToManagedTransition( 
    /* [in] */ FunctionID functionId,
    /* [in] */ COR_PRF_TRANSITION_REASON reason)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ManagedToUnmanagedTransition( 
    /* [in] */ FunctionID functionId,
    /* [in] */ COR_PRF_TRANSITION_REASON reason)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::RuntimeSuspendStarted( 
    /* [in] */ COR_PRF_SUSPEND_REASON suspendReason)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::RuntimeSuspendFinished(void)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::RuntimeSuspendAborted(void)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::RuntimeResumeStarted(void)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::RuntimeResumeFinished(void)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::RuntimeThreadSuspended( 
    /* [in] */ ThreadID threadId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::RuntimeThreadResumed( 
    /* [in] */ ThreadID threadId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::MovedReferences( 
                                      /* [in] */ ULONG cMovedObjectIDRanges,
                                      /* [size_is][in] */ ObjectID oldObjectIDRangeStart[  ],
                                      /* [size_is][in] */ ObjectID newObjectIDRangeStart[  ],
                                      /* [size_is][in] */ ULONG cObjectIDRangeLength[  ])
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ObjectAllocated( 
                                      /* [in] */ ObjectID objectId,
                                      /* [in] */ ClassID classId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ObjectsAllocatedByClass( 
    /* [in] */ ULONG cClassCount,
    /* [size_is][in] */ ClassID classIds[  ],
    /* [size_is][in] */ ULONG cObjects[  ])
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ObjectReferences( 
                                       /* [in] */ ObjectID objectId,
                                       /* [in] */ ClassID classId,
                                       /* [in] */ ULONG cObjectRefs,
                                       /* [size_is][in] */ ObjectID objectRefIds[  ])
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::RootReferences( 
                                     /* [in] */ ULONG cRootRefs,
                                     /* [size_is][in] */ ObjectID rootRefIds[  ])
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ExceptionThrown( 
                                      /* [in] */ ObjectID thrownObjectId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ExceptionSearchFunctionEnter( 
    /* [in] */ FunctionID functionId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ExceptionSearchFunctionLeave(void)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ExceptionSearchFilterEnter( 
    /* [in] */ FunctionID functionId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ExceptionSearchFilterLeave(void)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ExceptionSearchCatcherFound( 
    /* [in] */ FunctionID functionId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ExceptionOSHandlerEnter( 
    /* [in] */ UINT_PTR __unused)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ExceptionOSHandlerLeave( 
    /* [in] */ UINT_PTR __unused)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ExceptionUnwindFunctionEnter( 
    /* [in] */ FunctionID functionId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ExceptionUnwindFunctionLeave(void)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ExceptionUnwindFinallyEnter( 
    /* [in] */ FunctionID functionId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ExceptionUnwindFinallyLeave(void)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ExceptionCatcherEnter( 
    /* [in] */ FunctionID functionId,
    /* [in] */ ObjectID objectId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ExceptionCatcherLeave(void)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::COMClassicVTableCreated( 
    /* [in] */ ClassID wrappedClassId,
    /* [in] */ REFGUID implementedIID,
    /* [in] */ void *pVTable,
    /* [in] */ ULONG cSlots)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::COMClassicVTableDestroyed( 
    /* [in] */ ClassID wrappedClassId,
    /* [in] */ REFGUID implementedIID,
    /* [in] */ void *pVTable)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ExceptionCLRCatcherFound(void)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ExceptionCLRCatcherExecute(void)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::ThreadNameChanged( 
                                        /* [in] */ ThreadID threadId,
                                        /* [in] */ ULONG cchName,
                                        /* [in] */ WCHAR name[  ])
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::GarbageCollectionStarted( 
    /* [in] */ int cGenerations,
    /* [length_is][size_is][in] */ BOOL generationCollected[  ],
    /* [in] */ COR_PRF_GC_REASON reason)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::SurvivingReferences( 
    /* [in] */ ULONG cSurvivingObjectIDRanges,
    /* [size_is][in] */ ObjectID objectIDRangeStart[  ],
    /* [size_is][in] */ ULONG cObjectIDRangeLength[  ])
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::GarbageCollectionFinished(void)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::FinalizeableObjectQueued( 
    /* [in] */ DWORD finalizerFlags,
    /* [in] */ ObjectID objectID)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::RootReferences2( 
                                      /* [in] */ ULONG cRootRefs,
                                      /* [size_is][in] */ ObjectID rootRefIds[  ],
                                      /* [size_is][in] */ COR_PRF_GC_ROOT_KIND rootKinds[  ],
                                      /* [size_is][in] */ COR_PRF_GC_ROOT_FLAGS rootFlags[  ],
                                      /* [size_is][in] */ UINT_PTR rootIds[  ])
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::HandleCreated( 
                                    /* [in] */ GCHandleID handleId,
                                    /* [in] */ ObjectID initialObjectId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CEngine::HandleDestroyed( 
                                      /* [in] */ GCHandleID handleId)
{
    return E_NOTIMPL;
}

#pragma endregion

#pragma endregion