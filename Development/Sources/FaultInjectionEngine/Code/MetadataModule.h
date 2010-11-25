// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#pragma once
#include "MetadataMethod.h"

BEGIN_DEFAULT_NAMESPACE

class CMetadataModule
{
public:
    CMetadataModule(CComQIPtr<ICorProfilerInfo> pCorProfilerInfo, ModuleID moduleId)
    {
        this->AttachMetadata(pCorProfilerInfo, moduleId);
    };
    ~CMetadataModule(void){};

public:
    void AttachMetadata(CComQIPtr<ICorProfilerInfo> pCorProfilerInfo, ModuleID moduleId);
    void LoadILMethodBody(CMetadataMethod &rMethodInfo);
    void LoadMethodProperties(CMetadataMethod &rMethodInfo);
    void InsertPrologueIntoMethod(CMetadataMethod &rMethodInfo);
    ULONG FindAllAssembliesByName(LPCTSTR pstrAssemblyName,
        CAtlArray<CComQIPtr<IMetaDataImport, &IID_IMetaDataImport> > &rvpAssembliesMetaDataImport);
    ULONG FindAllTypesByAssemblyAndName(LPCTSTR pstrAssemblyName, LPCTSTR pstrTypeName,
        CAtlArray<CComQIPtr<IMetaDataImport, &IID_IMetaDataImport> > &rvpAssembliesMetaDataImport,
        CAtlArray<mdTypeDef> &rvTypeDefTokens);
    ULONG FindAllMethodsByAssemblyAndName(LPCTSTR pstrAssemblyName, LPCTSTR pstrTypeName, LPCTSTR pstrMethodName,
        CAtlArray<CComQIPtr<IMetaDataImport, &IID_IMetaDataImport> > &rvpAssembliesMetaDataImport,
        CAtlArray<mdTypeDef> &rvTypeDefTokens, CAtlArray<mdMethodDef> &rvMethodDefTokens);

protected:
    mdTypeRef EmitTypeRefToken(LPCTSTR pstrAssemblyName, LPCTSTR pstrTypeName);
    mdMemberRef EmitMethodRefToken(LPCTSTR pstrAssemblyName, LPCTSTR pstrTypeName, LPCTSTR pstrMethodName);
    CString RetrieveFullQualifiedTypeName(mdTypeDef tkTypeDef);
    CILMethodSect PrepareILMethodSect(CMetadataMethod &rMethodInfo, ULONG nShiftOffset, CAtlArray<BYTE> &rvAllocator);
    WORD EmitNewLocalVarToken(mdSignature tkOldLocalVarToken, mdSignature &tkNewLocalVarToken);
    CorElementType ParseReturnType(CMetadataMethod &rMethodInfo, mdToken &tkReturnType);

private:
    ModuleID m_moduleId;
    CComPtr<IMethodMalloc> m_pMethodMalloc;
    CComPtr<IMetaDataEmit> m_pMetaDataEmit;
    CComPtr<IMetaDataImport> m_pMetaDataImport;
    CComPtr<IMetaDataAssemblyEmit> m_pMetaDataAssemblyEmit;
    CComPtr<IMetaDataAssemblyImport> m_pMetaDataAssemblyImport;
    CComQIPtr<ICorProfilerInfo> m_pCorProfilerInfo;
};

END_DEFAULT_NAMESPACE