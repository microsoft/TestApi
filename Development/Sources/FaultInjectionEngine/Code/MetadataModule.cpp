// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#include "stdafx.h"
#include "settings.h"
#include "MetadataModule.h"
#include "Exceptions.h"
#include "TraceAndLog.h"
#include "ILTemplates.h"

USING_DEFAULT_NAMESPACE

void CMetadataModule::AttachMetadata(CComQIPtr<ICorProfilerInfo> pCorProfilerInfo, ModuleID moduleId)
{
    ASSERT(NULL != pCorProfilerInfo);
    this->m_pCorProfilerInfo = pCorProfilerInfo;
    this->m_moduleId = moduleId;

    // Get interface IMetaDataImport by module id.
    HRESULT hr = this->m_pCorProfilerInfo->GetModuleMetaData(moduleId, ofRead, IID_IMetaDataImport,
        reinterpret_cast<IUnknown**>(&(this->m_pMetaDataImport)));
    if(FAILED(hr))
    {
        EventReportError(IDS_REPORT_FAILED_GET_MODULE_METADATA, hr, moduleId, _T("Read"), _T("IMetaDataImport"));
        CExceptionAsBreak::Throw();
    }
    ASSERT(NULL != this->m_pMetaDataImport);

    // Get interface IMetaDataAssemblyImport by module id.
    hr = this->m_pCorProfilerInfo->GetModuleMetaData(moduleId, ofRead, IID_IMetaDataAssemblyImport,
        reinterpret_cast<IUnknown**>(&(this->m_pMetaDataAssemblyImport)));
    if(FAILED(hr))
    {
        EventReportError(IDS_REPORT_FAILED_GET_MODULE_METADATA, hr, moduleId, _T("Read"), _T("IMetaDataAssemblyImport"));
        CExceptionAsBreak::Throw();
    }
    ASSERT(NULL != this->m_pMetaDataAssemblyImport);

    // Get interface IMetaDataEmit by module id.
    hr = this->m_pCorProfilerInfo->GetModuleMetaData(moduleId, ofRead | ofWrite, IID_IMetaDataEmit,
        reinterpret_cast<IUnknown**>(&(this->m_pMetaDataEmit)));
    if(FAILED(hr))
    {
        EventReportError(IDS_REPORT_FAILED_GET_MODULE_METADATA, hr, moduleId, _T("Read|Write"), _T("IMetaDataEmit"));
        CExceptionAsBreak::Throw();
    }
    ASSERT(NULL != this->m_pMetaDataEmit);

    // Get interface IMetaDataAssemblyEmit by module id.
    hr = this->m_pCorProfilerInfo->GetModuleMetaData(moduleId, ofRead | ofWrite, IID_IMetaDataAssemblyEmit,
        reinterpret_cast<IUnknown**>(&(this->m_pMetaDataAssemblyEmit)));
    if(FAILED(hr))
    {
        EventReportError(IDS_REPORT_FAILED_GET_MODULE_METADATA, hr, moduleId, _T("Read|Write"), _T("IMetaDataAssemblyEmit"));
        CExceptionAsBreak::Throw();
    }
    ASSERT(NULL != this->m_pMetaDataAssemblyEmit);

    hr = this->m_pCorProfilerInfo->GetILFunctionBodyAllocator(moduleId,
        reinterpret_cast<IMethodMalloc**>(&(this->m_pMethodMalloc)));
    if(FAILED(hr))
    {
        EventReportError(IDS_REPORT_FAILED_GET_FUNCTION_ALLOCATOR, hr, moduleId);
        CExceptionAsBreak::Throw();
    }
    ASSERT(NULL != this->m_pMethodMalloc);
}

void CMetadataModule::LoadMethodProperties(CMetadataMethod& rMethodInfo)
{
    ASSERT(NULL != this->m_pMetaDataImport);

    // Get method's properties
    mdTypeDef tkEnclosingTypeDef; // TypeDef token of the type where current method defined.
    CString szMethodName;
    ULONG nMethodNameLength;
    DWORD dwMethodAttributeFlags;
    PCCOR_SIGNATURE pvMethodSignature;
    ULONG nMethodSignatureSize;
    ULONG nMethodCodeRVA;
    DWORD dwMethodImplementationFlags;
    HRESULT hr = this->m_pMetaDataImport->GetMethodProps(rMethodInfo.GetMethodDefToken(),
        &tkEnclosingTypeDef,
        szMethodName.GetBufferSetLength(PREFERRED_NONQUALIFIED_METHOD_NAME_LENGTH),
        PREFERRED_NONQUALIFIED_METHOD_NAME_LENGTH, &nMethodNameLength,
        &dwMethodAttributeFlags, &pvMethodSignature, &nMethodSignatureSize,
        &nMethodCodeRVA, &dwMethodImplementationFlags);
    if(SUCCEEDED(hr) && (nMethodNameLength > PREFERRED_NONQUALIFIED_METHOD_NAME_LENGTH))
    {
        // The preferred length is too small. The required buffer length is nMethodNameLength.
        hr = this->m_pMetaDataImport->GetMethodProps(rMethodInfo.GetMethodDefToken(),
            &tkEnclosingTypeDef,
            szMethodName.GetBufferSetLength(nMethodNameLength),
            nMethodNameLength, &nMethodNameLength,
            &dwMethodAttributeFlags, &pvMethodSignature, &nMethodSignatureSize,
            &nMethodCodeRVA, &dwMethodImplementationFlags);
    }
    if(FAILED(hr))
    {
        EventReportError(IDS_REPORT_FAILED_GET_METHOD_PROPS, hr, rMethodInfo.GetMethodDefToken());
        CExceptionAsBreak::Throw();
    }
    szMethodName.ReleaseBufferSetLength(nMethodNameLength - 1);

    rMethodInfo.SetFullQualifiedMethodName(this->RetrieveFullQualifiedTypeName(tkEnclosingTypeDef)
        + CSettings::GetQualifiedNameSeparatorBeforeMethod() + szMethodName);
    rMethodInfo.SetMethodSignature(pvMethodSignature, nMethodSignatureSize);
}

CString CMetadataModule::RetrieveFullQualifiedTypeName(mdTypeDef tkTypeDef)
{
    ASSERT(NULL != this->m_pMetaDataImport);

    // Get Type Properties
    CString szTypeName;
    ULONG nTypeNameLength;
    DWORD dwMethodAttributeFlags;
    mdToken tokenBaseType;
    HRESULT hr = this->m_pMetaDataImport->GetTypeDefProps(tkTypeDef,
        szTypeName.GetBufferSetLength(PREFERRED_QUALIFIED_TYPE_NAME_LENGTH),
        PREFERRED_QUALIFIED_TYPE_NAME_LENGTH, &nTypeNameLength,
        &dwMethodAttributeFlags, &tokenBaseType);
    if(SUCCEEDED(hr) && (nTypeNameLength > PREFERRED_QUALIFIED_TYPE_NAME_LENGTH))
    {
        // The preferred length is too small. The required buffer length is nTypeNameLength.
        hr = this->m_pMetaDataImport->GetTypeDefProps(tkTypeDef,
            szTypeName.GetBufferSetLength(nTypeNameLength),
            nTypeNameLength, &nTypeNameLength,
            &dwMethodAttributeFlags, &tokenBaseType);
    }
    if(FAILED(hr))
    {
        EventReportError(IDS_REPORT_FAILED_GET_TYPE_PROPS, hr, tkTypeDef);
        CExceptionAsBreak::Throw();
    }
    szTypeName.ReleaseBufferSetLength(nTypeNameLength - 1);
    
    // Get properties of enclosing-type if this is a nested one.
    if((dwMethodAttributeFlags & tdVisibilityMask) >= tdNestedPublic)  // it's nested-type
    {
        // Get enclosing-type definition token.
        mdTypeDef tkEnclosingTypeDef;
        hr = this->m_pMetaDataImport->GetNestedClassProps(tkTypeDef, &tkEnclosingTypeDef);
        if(FAILED(hr))
        {
            EventReportError(IDS_REPORT_FAILED_GET_NESTED_CLASS_PROPS, hr, tkTypeDef);
            CExceptionAsBreak::Throw();
        }

        return this->RetrieveFullQualifiedTypeName(tkEnclosingTypeDef)
            + CSettings::GetQualifiedNameSeparatorBeforeNestedType() + szTypeName;
    }

    return szTypeName;
}

void CMetadataModule::LoadILMethodBody(CMetadataMethod &rMethodInfo)
{
    ASSERT(NULL != this->m_pCorProfilerInfo);

    LPCBYTE pILFunctionBody = NULL;
    ULONG nILFunctionBodySize = 0;
    HRESULT hr = this->m_pCorProfilerInfo->GetILFunctionBody(this->m_moduleId, rMethodInfo.GetMethodDefToken(),
        &pILFunctionBody, &nILFunctionBodySize);
    if (FAILED(hr))
    {
        EventReportError(IDS_REPORT_FAILED_GET_FUNCTION_BODY, hr, this->m_moduleId, rMethodInfo.GetMethodDefToken());
        CExceptionAsBreak::Throw();
    }

    ASSERT(NULL != pILFunctionBody);

    rMethodInfo.SetILMethodBody((LPVOID)pILFunctionBody, nILFunctionBodySize);
    DebugDump(rMethodInfo.GetILMethodBody(), _T("Original IL Method Body"));
    return;
}

CILMethodSect CMetadataModule::PrepareILMethodSect(CMetadataMethod &rMethodInfo, ULONG nShiftOffset, CAtlArray<BYTE> &vAllocator)
{
    ASSERT(rMethodInfo.GetILMethodBody().GetHeader().IsFat());

    CILMethodSect xOldILMethodSect = rMethodInfo.GetILMethodBody().GetSect();
    if(xOldILMethodSect.IsNull())
        return xOldILMethodSect; // return a null memory-ref
    
    vAllocator.RemoveAll();
    do
    {
        size_t nSizeBeforeAppending = (ULONG)(vAllocator.GetCount());
        if(xOldILMethodSect.IsExceptionHandler())
        {
            int nExceptionHandlerClauseCount = xOldILMethodSect.GetExceptionHandlerClauseCount();
            ASSERT(0 < nExceptionHandlerClauseCount);

            // prepare FAT EH, whatever the original one is small or fat.
            ASSERT(0 == (sizeof(IMAGE_COR_ILMETHOD_SECT_FAT) & (sizeof(DWORD)-1)));  // Make sure the FAT EH is dword align.
            size_t nAppendedSize = sizeof(IMAGE_COR_ILMETHOD_SECT_FAT) +
                nExceptionHandlerClauseCount * sizeof(IMAGE_COR_ILMETHOD_SECT_EH_CLAUSE_FAT);
            nAppendedSize = (nAppendedSize + (sizeof(DWORD)-1)) & (~(sizeof(DWORD)-1));  // make it dword align
            vAllocator.SetCount(nSizeBeforeAppending + nAppendedSize);  // allocate buffer for appended data
            CILMethodSect xTargetSect(vAllocator.GetData() + nSizeBeforeAppending, nAppendedSize);  // refer to appended buffer

            if(xOldILMethodSect.IsFat())
            {
                // It's FAT exception-handler. Copy data and shift offset
                DebugTrace(_T("Old Section Size: %d; AppendedSize: %d"), xOldILMethodSect.GetSectionDataSize(),nAppendedSize);
                ASSERT(xOldILMethodSect.GetSectionDataSize() == nAppendedSize);
                xTargetSect.MemoryCopy(xOldILMethodSect, xOldILMethodSect.GetSectionDataSize());

                for(int i = 0; i < nExceptionHandlerClauseCount; i++)
                {
                    IMAGE_COR_ILMETHOD_SECT_EH_CLAUSE_FAT &rNewFatClause = xTargetSect.GetFatExceptionHandlerClause(i);
                    switch(rNewFatClause.Flags)
                    {
                    case COR_ILEXCEPTION_CLAUSE_FILTER:  // EH entry for a filter
                        rNewFatClause.FilterOffset += nShiftOffset;
                        break;
                    case COR_ILEXCEPTION_CLAUSE_NONE:  // typed handler
                    case COR_ILEXCEPTION_CLAUSE_FINALLY:  // finally clause
                    case COR_ILEXCEPTION_CLAUSE_FAULT:  // fault clause (finally that is called on exception only)
                        // rNewFatClause.ClassToken;
                        break;
                    default:
                        ASSERT( FALSE );
                    }
                    rNewFatClause.TryOffset += nShiftOffset;
                    // rNewFatClause.TryLength += 0;  // relative to start of try block
                    rNewFatClause.HandlerOffset += nShiftOffset;
                    // rNewFatClause.HandlerLength += 0;  // relative to start of handler
                }
            }
            else
            {
                ASSERT(xOldILMethodSect.IsSmall());
                // It's SMALL exception-handler. Fill the data of new one.

                xTargetSect.MemoryCopy(  // fill the default FAT (EH) Section before any other operation on it.
                    CMemoryRef((&imageDefaultILMethodFatSection), sizeof(imageDefaultILMethodFatSection)));
                
                xTargetSect.SetAsHasMoreSections(xOldILMethodSect.AreThereMoreSections());  // correct MORE_SECTS flag
                xTargetSect.SetSectionDataSize(nExceptionHandlerClauseCount * sizeof(IMAGE_COR_ILMETHOD_SECT_EH_CLAUSE_FAT));  // correct data-size

                for(int i = 0; i < nExceptionHandlerClauseCount; i++)
                {
                    IMAGE_COR_ILMETHOD_SECT_EH_CLAUSE_SMALL& rOldSmallClause = xOldILMethodSect.GetSmallExceptionHandlerClause(i);
                    IMAGE_COR_ILMETHOD_SECT_EH_CLAUSE_FAT& rNewFatClause = xTargetSect.GetFatExceptionHandlerClause(i);
                    switch((rNewFatClause.Flags = (CorExceptionFlag)(rOldSmallClause.Flags)))
                    {
                    case COR_ILEXCEPTION_CLAUSE_FILTER:  // EH entry for a filter
                        rNewFatClause.FilterOffset = rOldSmallClause.FilterOffset + nShiftOffset;
                        break;
                    case COR_ILEXCEPTION_CLAUSE_NONE:  // typed handler
                    case COR_ILEXCEPTION_CLAUSE_FINALLY:  // finally clause
                    case COR_ILEXCEPTION_CLAUSE_FAULT:  // fault clause (finally that is called on exception only)
                        rNewFatClause.ClassToken = rOldSmallClause.ClassToken;
                        break;
                    default:
                        ASSERT( FALSE );
                    }
                    rNewFatClause.TryOffset = rOldSmallClause.TryOffset + nShiftOffset;
                    rNewFatClause.TryLength = rOldSmallClause.TryLength;  // relative to start of try block
                    rNewFatClause.HandlerOffset = rOldSmallClause.HandlerOffset + nShiftOffset;
                    rNewFatClause.HandlerLength = rOldSmallClause.HandlerLength;  // relative to start of handler
                }
            }
        }
        else
        {
            // It's optional IL Table. Just copy the data
            size_t nAppendedSize = xOldILMethodSect.GetSectionDataSize();
            vAllocator.SetCount(nSizeBeforeAppending + nAppendedSize);  // allocate buffer for appended data
            CILMethodSect xTargetSect(vAllocator.GetData() + nSizeBeforeAppending, nAppendedSize);  // refer to appended buffer
            xTargetSect.MemoryCopy(xOldILMethodSect, nAppendedSize);  // copy data
        }

        // navigate to next section
        xOldILMethodSect = xOldILMethodSect.GetNextSection();
    }
    while(!xOldILMethodSect.IsNull());

    return CILMethodSect((LPVOID)(vAllocator.GetData()), (ULONG)(vAllocator.GetCount()));
}

ULONG CMetadataModule::FindAllAssembliesByName(LPCTSTR pstrAssemblyName,
                                               CAtlArray<CComQIPtr<IMetaDataImport, &IID_IMetaDataImport> > &rvpAssembliesMetaDataImport)
{
    ASSERT(NULL != pstrAssemblyName);

    CAtlArray<IUnknown*> vpAssembliesIUnk;
    vpAssembliesIUnk.SetCount(PREFERRED_DUPLICATED_ASSEMBLY_COUNT);

    ULONG nAssembliesCount;
    HRESULT hr = this->m_pMetaDataAssemblyImport->FindAssembliesByName(NULL, NULL, pstrAssemblyName,
        vpAssembliesIUnk.GetData(), PREFERRED_DUPLICATED_ASSEMBLY_COUNT, &nAssembliesCount);
    // Above function return S_OK if found, S_FALSE if not found.
    if((S_OK == hr) && (nAssembliesCount > PREFERRED_DUPLICATED_ASSEMBLY_COUNT))
    {
        // Find more than the buffer. Reallocate buffer and try fetch all possible ones.
        // Attention : Must free the refer-count of previous results first!
        for(ULONG i = 0; i < PREFERRED_DUPLICATED_ASSEMBLY_COUNT; i++)
        {
            ASSERT(NULL != vpAssembliesIUnk[i]);
            vpAssembliesIUnk[i]->Release();
        }
        vpAssembliesIUnk.SetCount(nAssembliesCount);
        hr = this->m_pMetaDataAssemblyImport->FindAssembliesByName(NULL, NULL, pstrAssemblyName,
            vpAssembliesIUnk.GetData(), nAssembliesCount, &nAssembliesCount);
    }
    if(S_OK == hr)
    {
        rvpAssembliesMetaDataImport.SetCount(nAssembliesCount);
        ULONG nSuccessfullyQueriedInterfaceCount = 0;
        for(ULONG i = 0; i < nAssembliesCount; i++)
        {
            rvpAssembliesMetaDataImport[nSuccessfullyQueriedInterfaceCount] = vpAssembliesIUnk[i];
            vpAssembliesIUnk[i]->Release();

            if(NULL == rvpAssembliesMetaDataImport[nSuccessfullyQueriedInterfaceCount])
            {
                DebugTrace(_T("Fail : (IUnknown*)(%X)->QueryInterface(IMetaDataImport)"), vpAssembliesIUnk[i]);
            }
            else
            {
                nSuccessfullyQueriedInterfaceCount++;
            }
        }

        DebugTrace(_T("There're %d assemblies named [%s] found; %d of them imported."), nAssembliesCount, pstrAssemblyName,
            nSuccessfullyQueriedInterfaceCount);

        rvpAssembliesMetaDataImport.SetCount(nSuccessfullyQueriedInterfaceCount);
        return nSuccessfullyQueriedInterfaceCount;
    }
    else
    {
        DebugTrace(_T("Assembly [%s] NOT found!"), pstrAssemblyName);
        // Do NOT treat it as error and throw exception here.
        // Just return empty buffer and let top-level functions have a chance to do something.
        rvpAssembliesMetaDataImport.RemoveAll();
        return 0;
    }
}

ULONG CMetadataModule::FindAllTypesByAssemblyAndName(LPCTSTR pstrAssemblyName, LPCTSTR pstrTypeName,
                                                     CAtlArray<CComQIPtr<IMetaDataImport,&IID_IMetaDataImport> > &rvpAssembliesMetaDataImport,
                                                     CAtlArray<mdTypeDef> &rvtkTypeDefTokens)
{
    ASSERT(NULL != pstrAssemblyName);
    ASSERT(NULL != pstrTypeName);

    ULONG nCount = this->FindAllAssembliesByName(pstrAssemblyName, rvpAssembliesMetaDataImport);

    ASSERT(rvpAssembliesMetaDataImport.GetCount() == nCount);

    rvtkTypeDefTokens.RemoveAll();
    for(ULONG i = 0; i < nCount; )
    {
        mdTypeDef tkTypeDef;
        HRESULT hr = rvpAssembliesMetaDataImport[i]->FindTypeDefByName(pstrTypeName, NULL, &tkTypeDef);
        if(SUCCEEDED(hr))
        {
            rvtkTypeDefTokens.Add(tkTypeDef);
            i++;
        }
        else
        {
            rvpAssembliesMetaDataImport.RemoveAt(i);
            nCount--;
        }
    }

#if defined(_DEBUG)
    if(0 < nCount)
    {
        DebugTrace(_T("There're %d types named [%s]%s found."), nCount, pstrAssemblyName, pstrTypeName);
    }
    else
    {
        DebugTrace(_T("Type [%s]%s NOT found."), pstrAssemblyName, pstrTypeName);
    }
#endif

    ASSERT(rvtkTypeDefTokens.GetCount() == nCount);
    ASSERT(rvpAssembliesMetaDataImport.GetCount() == nCount);

    return nCount;
}

ULONG CMetadataModule::FindAllMethodsByAssemblyAndName(LPCTSTR pstrAssemblyName, LPCTSTR pstrTypeName, LPCTSTR pstrMethodName,
                                                       CAtlArray<CComQIPtr<IMetaDataImport,&IID_IMetaDataImport> > &rvpAssembliesMetaDataImport,
                                                       CAtlArray<mdTypeDef> &rvTypeDefTokens, 
                                                       CAtlArray<mdMethodDef> &rvMethodDefTokens)
{
    ASSERT(NULL != pstrAssemblyName);
    ASSERT(NULL != pstrTypeName);
    ASSERT(NULL != pstrMethodName);

    ULONG nCount = this->FindAllTypesByAssemblyAndName(pstrAssemblyName, pstrTypeName,
        rvpAssembliesMetaDataImport, rvTypeDefTokens);

    ASSERT(rvTypeDefTokens.GetCount() == nCount);
    ASSERT(rvpAssembliesMetaDataImport.GetCount() == nCount);

    rvMethodDefTokens.RemoveAll();
    for(ULONG i = 0; i < nCount; )
    {
        mdMethodDef tkMethodDef;
        HRESULT hr = rvpAssembliesMetaDataImport[i]->FindMethod(rvTypeDefTokens[i],
            pstrMethodName, NULL, NULL, &tkMethodDef);
        if(SUCCEEDED(hr))
        {
            rvMethodDefTokens.Add(tkMethodDef);
            i++;
        }
        else
        {
            rvTypeDefTokens.RemoveAt(i);
            rvpAssembliesMetaDataImport.RemoveAt(i);
            nCount--;
        }
    }

#if defined(_DEBUG)
    if(0 < nCount)
    {
        DebugTrace(_T("There're %d methods named [%s]%s.%s found."), nCount, pstrAssemblyName, pstrTypeName, pstrMethodName);
    }
    else
    {
        DebugTrace(_T("Type [%s]%s.%s NOT found."), pstrAssemblyName, pstrTypeName, pstrMethodName);
    }
#endif

    ASSERT(rvTypeDefTokens.GetCount() == nCount);
    ASSERT(rvpAssembliesMetaDataImport.GetCount() == nCount);

    return nCount;
}

mdTypeRef CMetadataModule::EmitTypeRefToken(LPCTSTR pstrAssemblyName, LPCTSTR pstrTypeName)
{
    ASSERT(NULL != pstrTypeName);
    if(NULL == pstrAssemblyName)
        pstrAssemblyName = CSettings::GetCLISystemAssemblyName();    // [mscorlib]

    CAtlArray<mdTypeDef> vtkTypeDefTokens;
    CAtlArray<CComQIPtr<IMetaDataImport, &IID_IMetaDataImport> > vpAssembliesMetaDataImport;

    ULONG nCount = this->FindAllTypesByAssemblyAndName(pstrAssemblyName, pstrTypeName, vpAssembliesMetaDataImport, vtkTypeDefTokens);

    for(ULONG i = 0; i < nCount; i++)
    {
        CComQIPtr<IMetaDataAssemblyImport, &IID_IMetaDataAssemblyImport> pSourceAssemblyImport =
            vpAssembliesMetaDataImport[i];

        mdTypeRef tkTypeRef;
        HRESULT hr = this->m_pMetaDataEmit->DefineImportType(pSourceAssemblyImport, NULL, 0,
            vpAssembliesMetaDataImport[i], vtkTypeDefTokens[i], this->m_pMetaDataAssemblyEmit, &tkTypeRef);
        if(SUCCEEDED(hr))
        {
            DebugTrace(_T("Successfully emit type-ref token for [%s]%s"), pstrAssemblyName, pstrTypeName);
            return tkTypeRef;
        }
    }
    
    EventReportError(IDS_REPORT_FAILED_EMIT_TYPEREF_TOKEN, pstrAssemblyName, pstrTypeName);
    CExceptionAsBreak::Throw();
    return mdTypeRefNil;
}

mdMemberRef CMetadataModule::EmitMethodRefToken(LPCTSTR pstrAssemblyName, LPCTSTR pstrTypeName, LPCTSTR pstrMethodName)
{
    ASSERT(NULL != pstrTypeName);
    ASSERT(NULL != pstrMethodName);
    if(NULL == pstrAssemblyName)
        pstrAssemblyName = CSettings::GetCLISystemAssemblyName();    // [mscorlib]

    CAtlArray<mdTypeDef> vtkTypeDefTokens;
    CAtlArray<mdMethodDef> vtkMethodDefTokens;
    CAtlArray<CComQIPtr<IMetaDataImport, &IID_IMetaDataImport> > vpAssembliesMetaDataImport;

    ULONG nCount = this->FindAllMethodsByAssemblyAndName(pstrAssemblyName, pstrTypeName, pstrMethodName,
        vpAssembliesMetaDataImport, vtkTypeDefTokens, vtkMethodDefTokens);

    for(ULONG i = 0; i < nCount; i++)
    {
        CComQIPtr<IMetaDataAssemblyImport, &IID_IMetaDataAssemblyImport> pSourceAssemblyImport =
            vpAssembliesMetaDataImport[i];

        mdTypeRef tkTypeRef;
        HRESULT hr = this->m_pMetaDataEmit->DefineImportType(pSourceAssemblyImport, NULL, 0,
            vpAssembliesMetaDataImport[i], vtkTypeDefTokens[i], this->m_pMetaDataAssemblyEmit, &tkTypeRef);
        if(SUCCEEDED(hr))
        {
            mdMemberRef tkMethodRef;
            hr = this->m_pMetaDataEmit->DefineImportMember(pSourceAssemblyImport, NULL, 0,
                vpAssembliesMetaDataImport[i], vtkMethodDefTokens[i], this->m_pMetaDataAssemblyEmit, tkTypeRef,
                &tkMethodRef);
            if(SUCCEEDED(hr))
            {
                DebugTrace(_T("Successfully emit method-ref token for [%s]%s.%s"), pstrAssemblyName, pstrTypeName, pstrMethodName);
                return tkMethodRef;
            }
        }
    }
    
    EventReportError(IDS_REPORT_FAILED_EMIT_METHODREF_TOKEN, pstrAssemblyName, pstrTypeName, pstrMethodName);
    CExceptionAsBreak::Throw();
    return mdMemberRefNil;
}

WORD CMetadataModule::EmitNewLocalVarToken(mdSignature tkOldLocalVarToken, mdSignature &tkNewLocalVarToken)
{
    DebugTrace(_T("Old Local Var Signature Token: %x\n"), tkOldLocalVarToken);

    ULONG nOldLocalVarCount = 0;
    // Net part of signature, enclude CallingConvention & LocalVarCount
    PCCOR_SIGNATURE pvOldLocalVarSigNetPart = NULL;
    ULONG nOldLocalVarSigNetPartSize = 0;

    if(NULL != tkOldLocalVarToken && mdSignatureNil != tkOldLocalVarToken)
    {
        PCCOR_SIGNATURE pvOldLocalVarSig = NULL;
        ULONG nOldLocalVarSigSize = 0;
        HRESULT hr = this->m_pMetaDataImport->GetSigFromToken(tkOldLocalVarToken, &pvOldLocalVarSig, &nOldLocalVarSigSize);
	    if(FAILED(hr))
	    {
            EventReportError(IDS_REPORT_FAILED_GET_SIG_FROM_TOKEN, hr, tkOldLocalVarToken);
            CExceptionAsBreak::Throw();
	    }
        ASSERT(NULL != pvOldLocalVarSig);

        pvOldLocalVarSigNetPart = pvOldLocalVarSig;
        
        ULONG nCallingConvention = ::CorSigUncompressCallingConv(pvOldLocalVarSigNetPart);
        nOldLocalVarCount = ::CorSigUncompressData(pvOldLocalVarSigNetPart);
        nOldLocalVarSigNetPartSize = nOldLocalVarSigSize - (ULONG)(pvOldLocalVarSigNetPart - pvOldLocalVarSig);
    }
    DebugTrace(_T("Old Local Var Count: %d"), nOldLocalVarCount);

    mdTypeRef tkExceptionTypeRef = this->EmitTypeRefToken(NULL, _T("System.Exception"));

    PCOR_SIGNATURE vLocalVarSignature = new COR_SIGNATURE[5 * sizeof(DWORD) + nOldLocalVarSigNetPartSize];
    PCOR_SIGNATURE signatureNewLocalVar = vLocalVarSignature;
    *signatureNewLocalVar++ = IMAGE_CEE_CS_CALLCONV_LOCAL_SIG;
    signatureNewLocalVar += CorSigCompressData(2 + nOldLocalVarCount, signatureNewLocalVar);

    if(0 < nOldLocalVarCount)
    {
        ASSERT(NULL != pvOldLocalVarSigNetPart);
        ASSERT(0 < nOldLocalVarSigNetPartSize);
        ::memmove(signatureNewLocalVar, pvOldLocalVarSigNetPart, nOldLocalVarSigNetPartSize);
        signatureNewLocalVar += nOldLocalVarSigNetPartSize;
    }

    signatureNewLocalVar += CorSigCompressElementType(ELEMENT_TYPE_CLASS, signatureNewLocalVar); // throwException
    signatureNewLocalVar += CorSigCompressToken(tkExceptionTypeRef, signatureNewLocalVar);
    signatureNewLocalVar += CorSigCompressElementType(ELEMENT_TYPE_OBJECT, signatureNewLocalVar); // returnValue
    ULONG nSize = (ULONG)(signatureNewLocalVar - vLocalVarSignature);
    DebugTrace(_T("New Local Var Signature Size: %d\n"), nSize);

//	mdToken tokenNewLocalVarSig = mdSignatureNil;
	HRESULT hr = this->m_pMetaDataEmit->GetTokenFromSig(vLocalVarSignature, nSize, &tkNewLocalVarToken);
    delete [] vLocalVarSignature;
	if ( FAILED(hr) )
	{
        EventReportError(IDS_REPORT_FAILED_GET_TOKEN_FROM_SIG, hr, vLocalVarSignature, nSize);
        CExceptionAsBreak::Throw();
	}
    DebugTrace(_T("New Local Var Signature Token: %x\n"), tkNewLocalVarToken);
	
    ASSERT(nOldLocalVarCount < (1 << (8 * sizeof(WORD))));
    return (WORD)nOldLocalVarCount;  // also the index of new inserted local-var
}

void CMetadataModule::InsertPrologueIntoMethod(CMetadataMethod &rMethodInfo)
{
    this->LoadILMethodBody(rMethodInfo);

    CILMethodHeader xOldILMethodHeader = rMethodInfo.GetILMethodBody().GetHeader();

    CILMethodHeader xNewILMethodHeader;
    CILMethodSect xNewILMethodSect;
    CAtlArray<BYTE> vNewILMethodAllocation;
    if(xOldILMethodHeader.IsTiny())
    {
        DebugDump(xOldILMethodHeader, _T("Original TINY IL Method Header"));
        xNewILMethodHeader.Attach((LPVOID)(&imageDefaultILMethodFatHeader), sizeof(imageDefaultILMethodFatHeader));
    }
    else
    {
        ASSERT(xOldILMethodHeader.IsFat());
        DebugDump(xOldILMethodHeader, _T("Original FAT IL Method Header"));
        xNewILMethodHeader = xOldILMethodHeader;

        DebugDump(rMethodInfo.GetILMethodBody().GetSect(), _T("Original IL Method Sect"));

        DebugTrace(_T("SECT_EH_SMALL: %d; SECT_EH_CLAUSE: %d; SectDataSize: %d"),
            sizeof(IMAGE_COR_ILMETHOD_SECT_EH_SMALL), sizeof(IMAGE_COR_ILMETHOD_SECT_EH_CLAUSE_SMALL),
            rMethodInfo.GetILMethodBody().GetSect().GetSectionDataSize());

        xNewILMethodSect = this->PrepareILMethodSect(rMethodInfo, IL_SIZE__PROLOGUE,vNewILMethodAllocation);
    }
    DebugDump(xNewILMethodSect, _T("New FAT IL Method Section"));

    // Calcurate New ILMethodBody size
    ULONG nNewILMethodCodeSize = IL_SIZE__PROLOGUE + xNewILMethodHeader.GetCodeSize();
    nNewILMethodCodeSize = (nNewILMethodCodeSize + (sizeof(DWORD)-1)) & ~(sizeof(DWORD)-1);
    ULONG nNewILMethodBodySize =
        (ULONG)(xNewILMethodHeader.GetSize() + nNewILMethodCodeSize + xNewILMethodSect.GetSectionDataSize());

    // Allocate new ILMethodBody memory
    LPBYTE pMethodILBody = (LPBYTE)(this->m_pMethodMalloc->Alloc(nNewILMethodBodySize));
    if(NULL == pMethodILBody)
    {
        EventReportError(IDS_REPORT_FAILED_ALLOC, E_OUTOFMEMORY, nNewILMethodBodySize);
        CExceptionAsBreak::Throw();
    }
    memset(pMethodILBody, 0, nNewILMethodBodySize);

    // EmitLocalVarToken()
    mdSignature tkNewLocalVar;
    WORD nIndexOfNewLocalVar = this->EmitNewLocalVarToken(xOldILMethodHeader.GetLocalVarToken(), tkNewLocalVar);

    // Copy and adjust new header
    CILMethodBody xNewILMethodBody(pMethodILBody, nNewILMethodBodySize);
    xNewILMethodBody.MemoryCopy(xNewILMethodHeader);
    xNewILMethodHeader = xNewILMethodBody.GetHeader();

    xNewILMethodHeader.SetCodeSize(xOldILMethodHeader.GetCodeSize() + IL_SIZE__PROLOGUE);
    ULONG nMaxStack = xOldILMethodHeader.GetMaxStack();
    if(nMaxStack < IL_NUMBER__MIN_STACK)
        nMaxStack = IL_NUMBER__MIN_STACK;
    xNewILMethodHeader.SetMaxStack(nMaxStack);
    xNewILMethodHeader.SetLocalVarToken(tkNewLocalVar);
    DebugDump(xNewILMethodHeader, _T("New FAT IL Method Header"));

    // Find method FaultDispatcher.Trap
    mdMemberRef tkTrapMethodRef = this->EmitMethodRefToken(
        CSettings::GetDispatcherAssemblyName(),
        CSettings::GetDispatcherFullQualifiedClassName(),
        CSettings::GetDispatcherNonQualifiedMethodName());

    // Write Prologue
    xNewILMethodBody.MemoryCopyAt(xNewILMethodHeader.GetSize(),
        CMemoryRef(IL_CODE__PROLOGUE, IL_SIZE__PROLOGUE));
    // set local-var index
    int i;
    for(i = 0; 0 != IL_OFFSET__LOCALVAR_1[i]; i++)
    {
        xNewILMethodBody.MemoryCopyAt(xNewILMethodHeader.GetSize() + IL_OFFSET__LOCALVAR_1[i],
            CMemoryRef(&nIndexOfNewLocalVar, sizeof(WORD))); // faultedException
    }
    nIndexOfNewLocalVar++; // faultedReturnValue
    for(i = 0; 0 != IL_OFFSET__LOCALVAR_2[i]; i++)
    {
        xNewILMethodBody.MemoryCopyAt(xNewILMethodHeader.GetSize() + IL_OFFSET__LOCALVAR_2[i],
            CMemoryRef(&nIndexOfNewLocalVar, sizeof(WORD))); // faultedReturnValue
    }
    // set method call
    xNewILMethodBody.MemoryCopyAt(xNewILMethodHeader.GetSize() + IL_OFFSET__CALL_TRAP,
        CMemoryRef(&tkTrapMethodRef, sizeof(DWORD)));  //

    // fix by return-type
    mdToken tkReturnType;
    switch(this->ParseReturnType(rMethodInfo, tkReturnType))
    {
    case ELEMENT_TYPE_VOID:
        xNewILMethodBody.MemoryCopyAt(xNewILMethodHeader.GetSize() + IL_OFFSET__RETURN,
            CMemoryRef(IL_CODE__JUST_RETURN, IL_SIZE__RETURN_VOID));
        break;
    case ELEMENT_TYPE_OBJECT:
        xNewILMethodBody.MemoryCopyAt(xNewILMethodHeader.GetSize() + IL_OFFSET__UNBOX,
            CMemoryRef(IL_CODE__JUST_RETURN, IL_SIZE__RETURN_OBJECT));
        break;
    default:
        xNewILMethodBody.MemoryCopyAt(xNewILMethodHeader.GetSize() + IL_OFFSET__RETURN_TYPE,
            CMemoryRef(&tkReturnType, sizeof(DWORD)));
        break;
    }

    // TODO: copy the old code ... and SEH
    xNewILMethodBody.GetCode().MemoryCopyAt(IL_SIZE__PROLOGUE,
        rMethodInfo.GetILMethodBody().GetCode());
    if(!xNewILMethodSect.IsNull())
        xNewILMethodBody.GetSect().MemoryCopy(xNewILMethodSect);

    DebugDump(xNewILMethodBody, _T("New IL Body:"));

    // set method body
    HRESULT hr = this->m_pCorProfilerInfo->SetILFunctionBody(this->m_moduleId, rMethodInfo.GetMethodDefToken(),
        pMethodILBody);
    if(FAILED(hr))
    {
        EventReportError(IDS_REPORT_FAILED_SET_FUNCTION_BODY, hr, this->m_moduleId, rMethodInfo.GetMethodDefToken(), pMethodILBody);
        CExceptionAsBreak::Throw();
    }
    DebugTrace(_T("Function Modified!!!!!!!!!!!!!!\n"));

    return;
}

CorElementType CMetadataModule::ParseReturnType(CMetadataMethod &rMethodInfo, mdToken &tkReturnType)
{
    CMethodDefSigBlob xMethodSigBlob = rMethodInfo.GetMethodSignature();
    PCCOR_SIGNATURE pRetTypeSignature = xMethodSigBlob.LocateReturnType();

    PCCOR_SIGNATURE pTempSignature = pRetTypeSignature;
    CorElementType nElementType = ::CorSigUncompressElementType(pTempSignature);
    xMethodSigBlob.EnsureWithin(pTempSignature);

    switch(nElementType)
    {
    case ELEMENT_TYPE_VOID           :
    case ELEMENT_TYPE_OBJECT         :
        break;

    case ELEMENT_TYPE_BOOLEAN        :
        tkReturnType = this->EmitTypeRefToken(NULL, _T("System.Boolean"));
        break;
    case ELEMENT_TYPE_CHAR           :
        tkReturnType = this->EmitTypeRefToken(NULL, _T("System.Char"));
        break;
    case ELEMENT_TYPE_I1             :
        tkReturnType = this->EmitTypeRefToken(NULL, _T("System.SByte"));
        break;
    case ELEMENT_TYPE_U1             :
        tkReturnType = this->EmitTypeRefToken(NULL, _T("System.Byte"));
        break;
    case ELEMENT_TYPE_I2             :
        tkReturnType = this->EmitTypeRefToken(NULL, _T("System.Int16"));
        break;
    case ELEMENT_TYPE_U2             :
        tkReturnType = this->EmitTypeRefToken(NULL, _T("System.UInt16"));
        break;
    case ELEMENT_TYPE_I4             :
        tkReturnType = this->EmitTypeRefToken(NULL, _T("System.Int32"));
        break;
    case ELEMENT_TYPE_U4             :
        tkReturnType = this->EmitTypeRefToken(NULL, _T("System.UInt32"));
        break;
    case ELEMENT_TYPE_I8             :
        tkReturnType = this->EmitTypeRefToken(NULL, _T("System.Int64"));
        break;
    case ELEMENT_TYPE_U8             :
        tkReturnType = this->EmitTypeRefToken(NULL, _T("System.UInt64"));
        break;
    case ELEMENT_TYPE_R4             :
        tkReturnType = this->EmitTypeRefToken(NULL, _T("System.Single"));
        break;
    case ELEMENT_TYPE_R8             :
        tkReturnType = this->EmitTypeRefToken(NULL, _T("System.Double"));
        break;
    case ELEMENT_TYPE_STRING         :
        tkReturnType = this->EmitTypeRefToken(NULL, _T("System.String"));
        break;
    case ELEMENT_TYPE_TYPEDBYREF     :
        tkReturnType = this->EmitTypeRefToken(NULL, _T("System.TypedReference"));
        break;
    case ELEMENT_TYPE_I              :
        tkReturnType = this->EmitTypeRefToken(NULL, _T("System.IntPtr"));
        break;
    case ELEMENT_TYPE_U              :
        tkReturnType = this->EmitTypeRefToken(NULL, _T("System.UIntPtr"));
        break;

    case ELEMENT_TYPE_VALUETYPE      :
    case ELEMENT_TYPE_CLASS          :
        tkReturnType = ::CorSigUncompressToken(pTempSignature);
        xMethodSigBlob.EnsureWithin(pTempSignature);
        break;

    default:
        pTempSignature = pRetTypeSignature;
        xMethodSigBlob.ParseRetTypeSig(pTempSignature);
        {
            HRESULT hr = this->m_pMetaDataEmit->GetTokenFromTypeSpec(pRetTypeSignature,
                (ULONG)((LPCBYTE)pTempSignature - (LPCBYTE)pRetTypeSignature), &tkReturnType);
            if(FAILED(hr))
            {
                EventReportError(IDS_REPORT_FAILED_GET_TOKEN_FROM_TYPESPEC, hr, pRetTypeSignature,
                    (ULONG)((LPCBYTE)pTempSignature - (LPCBYTE)pRetTypeSignature));
                DebugDump(xMethodSigBlob, _T("The method signature blob"));
                CExceptionAsBreak::Throw();
            }
            DebugTrace(_T("Successfully emit type-spec token for return-type of current method."));
        }
        break;
    }
    return nElementType;
}
