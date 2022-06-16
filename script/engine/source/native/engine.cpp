#pragma unmanaged
#include <windows.h>
#include <ole2.h>
#include <rpc.h>
#include <comdef.h>
#include "..\win32\rpcsal.h"
#include "..\win32\activscp.h"
#include "..\win32\activdbg.h"
#include <vector>
#include "engine.h"

/** Constructor. */
CActiveScriptEngine::CActiveScriptEngine()
{
    m_cRefCount = 1;
    m_pActiveScript = 0;
    m_pActiveScriptParse = 0;
}

/** Description. */
CActiveScriptEngine::~CActiveScriptEngine()
{
    close();
}


/** Initialize the script. */
HRESULT CActiveScriptEngine::init(const wchar_t *wcsName)
{
    CLSID clsid;
    HRESULT hResult = E_FAIL;

    hResult = CLSIDFromProgID(wcsName, &clsid);
    if(FAILED(hResult))
        return hResult;

    ULONG ul = 0;
    hResult = E_FAIL;

    IActiveScriptSite* pActiveScriptSite = NULL;

    hResult = CoCreateInstance(clsid, NULL,
                               CLSCTX_INPROC_SERVER,
                               IID_IActiveScript,
                               (void**)&m_pActiveScript);

    if(SUCCEEDED(hResult))
    {
        hResult = m_pActiveScript->QueryInterface(IID_IActiveScriptParse,
                                                  (void**)&m_pActiveScriptParse);

        if(FAILED(hResult))
            goto err;
    }
    else
        goto err;

    hResult = QueryInterface(IID_IActiveScriptSite,
                             (void**)&pActiveScriptSite);

    if(SUCCEEDED(hResult))
    {
        hResult = m_pActiveScript->SetScriptSite(pActiveScriptSite);
                  pActiveScriptSite->Release();
        if(FAILED(hResult))
            goto err;
    }
    else
        goto err;

    hResult = m_pActiveScriptParse->InitNew();
    if(FAILED(hResult))
        goto err;

    return S_OK;

 err:
    if(m_pActiveScriptParse)
    {
        m_pActiveScriptParse->Release();
        m_pActiveScriptParse = NULL;
    }

    if(m_pActiveScript)
    {
        m_pActiveScript->Release();
        m_pActiveScript = NULL;
    }

    return hResult;
}

/** Add script text.

    @param wcsText      The text of script
    @param wcsName      Name of text piece
  */
HRESULT CActiveScriptEngine::load(const wchar_t *wcsText, const wchar_t *wcsName)
{
    long lCookie = m_awcsFiles.size();
    m_awcsFiles.push_back((wchar_t *)SysAllocString(wcsName));

    m_iContext = m_iLine = m_iPos = -1;
    m_sNativeDescription = "";
    m_hrError = 0;

    HRESULT hRC =  m_pActiveScriptParse->ParseScriptText(wcsText,
                                                         L"",
                                                         NULL, NULL,
                                                         lCookie,
                                                         0,
                                                         SCRIPTTEXT_ISVISIBLE,
                                                         NULL,
                                                         NULL);
    return hRC;
}

/** Connect to script engine. */
HRESULT CActiveScriptEngine::connect()
{
    m_iContext = m_iLine = m_iPos = -1;
    m_sNativeDescription = "";
    m_hrError = 0;

    HRESULT hr = m_pActiveScript->SetScriptState(SCRIPTSTATE_CONNECTED);
    return hr;
}

/** Connect to script engine. */
HRESULT CActiveScriptEngine::close()
{
    if(m_pActiveScript)
    {
        m_pActiveScript->SetScriptState(SCRIPTSTATE_DISCONNECTED);
        m_pActiveScript->Close();
        m_pActiveScript->Release();
        m_pActiveScript = 0;
    }
    if(m_pActiveScriptParse)
    {
        m_pActiveScriptParse->Release();
        m_pActiveScriptParse = 0;
    }
    int i;

    for (i = m_awcsFiles.size() - 1; i >= 0; i--)
        SysFreeString(m_awcsFiles[i]);
    m_awcsFiles.clear();

    for (i = m_awcsItemNames.size() - 1; i >= 0; i--)
        SysFreeString(m_awcsItemNames[i]);
    m_awcsItemNames.clear();

    for (i = m_awcsObjects.size() - 1; i >= 0; i--)
        m_awcsObjects[i]->Release();
    m_awcsObjects.clear();

    for (i = m_awcsTypes.size() - 1; i >= 0; i--)
    {
        if (m_awcsTypes[i] != 0)
            m_awcsTypes[i]->Release();
    }
    m_awcsTypes.clear();
    return S_OK;
}

/** Add object to global object table of script.

    @param wcsName      Name of the object
    @param pUnk         Unknown interface to the object
    @param bGlobal      If this flag is true - the methods
                        of object will be exposed as global
                        methods for script
  */
HRESULT CActiveScriptEngine::addObject(const wchar_t *wcsName, IUnknown *pUnk, bool bGlobal)
{
    m_awcsItemNames.push_back((wchar_t *)SysAllocString(wcsName));
    pUnk->AddRef();
    m_awcsObjects.push_back(pUnk);

    HRESULT hr;
    IProvideClassInfo* pProvideClassInfo = NULL;
    hr = pUnk->QueryInterface(IID_IProvideClassInfo,
                              (void**)&pProvideClassInfo);
    ITypeInfo *pTypeInfo = 0;
    if(SUCCEEDED(hr))
    {
        pProvideClassInfo->GetClassInfo(&pTypeInfo);
        pProvideClassInfo->Release();
    }
    m_awcsTypes.push_back(pTypeInfo);

    return m_pActiveScript->AddNamedItem(wcsName,
                                         SCRIPTITEM_ISVISIBLE |
                                         (bGlobal ? SCRIPTITEM_GLOBALMEMBERS : 0));
}

/** Invoke function.

    @param wcsName          Function's name
    @param lParamCount      Number of parameters
    @param vParams          List of parameters
    @param vRetCode         Method's return code.
  */
HRESULT CActiveScriptEngine::invoke(const wchar_t *wcsMethod, int lParamCount, VARIANT *vParams, VARIANT *vRetCode)
{
    m_iContext = m_iLine = m_iPos = -1;
    m_sNativeDescription = "";
    m_hrError = 0;

    IDispatch *pDisp = 0;
    HRESULT hRes;
    hRes = m_pActiveScript->GetScriptDispatch(NULL, &pDisp);

    if (FAILED(hRes))
        return hRes;

    DISPID id;
    LCID lcid;
    GetLCID(&lcid);

    hRes = pDisp->GetIDsOfNames(IID_NULL, (LPOLESTR *)(&wcsMethod), 1, lcid, &id);
    if (FAILED(hRes))
    {
        pDisp->Release();
        return hRes;
    }

    VariantInit(vRetCode);

    DISPPARAMS aParm = {vParams, 0, lParamCount, 0};
    hRes = pDisp->Invoke(id, IID_NULL, lcid, DISPATCH_METHOD, &aParm, vRetCode, 0, 0);

    pDisp->Release();
    return hRes;
}

/** Returns context of last error. */
const wchar_t *CActiveScriptEngine::getLastErrorContext()
{
    if (m_iContext >= 0 &&
        m_iContext < (int)m_awcsFiles.size())
        return m_awcsFiles[m_iContext];
    else
        return L"unknown";
}

/** Returns line of last error. */
int CActiveScriptEngine::getLastErrorLine()
{
    return m_iLine;
}

/** Returns position of last error. */
int CActiveScriptEngine::getLastErrorPos()
{
    return m_iPos;
}

#include "errlist.h"

/** Returns message of last error. */
const wchar_t *CActiveScriptEngine::getErrorMsg(HRESULT hr)
{
    for (int i = 0; g_eErrorTable[i].iCode != 0; i++)
        if (g_eErrorTable[i].iCode == hr)
            return g_eErrorTable[i].wcsName;

    return L"unknown error";
}

/** Returns native message of last error. */
const wchar_t *CActiveScriptEngine::getErrorDescription()
{
    return m_sNativeDescription;
}


/** Implementation of IUnknown::QueryInterface

    Returns the interface, supported by object

    @param iid      Interface description
    @param ppv      [out] Pointer to interface
  */
HRESULT _stdcall CActiveScriptEngine::QueryInterface(REFIID iid, void **ppv)
{
    *ppv = 0;
    if(iid == IID_IUnknown)
    {
        *ppv = static_cast<IActiveScriptSiteWindow*>(this);
    }
    else if (iid == IID_IActiveScriptSiteWindow)
    {
        *ppv = static_cast<IActiveScriptSiteWindow*>(this);
    }
    else if(iid == IID_IActiveScriptSite)
    {
        *ppv = static_cast<IActiveScriptSite*>(this);
    }
    else if(iid == IID_IActiveScriptSiteDebug32)
    {
        *ppv = static_cast<IActiveScriptSiteDebug32*>(this);
    }
    if(*ppv)
    {
        ((IUnknown*)(*ppv))->AddRef();
        return S_OK;
    }
    else
        return E_NOINTERFACE;

}

/** Implementation of IUnknown::AddRef

    Add reference to object
  */
ULONG _stdcall CActiveScriptEngine::AddRef()
{
    return InterlockedIncrement(&m_cRefCount);
}

/** Implementation of IUnknown::Release

    Subtrace reference to object and destroy object
    if there is no much references
  */
ULONG _stdcall CActiveScriptEngine::Release()
{
    if (InterlockedDecrement(&m_cRefCount) == 0)
    {
        delete this;
        return 0;
    }
    return m_cRefCount;
}

/** IActiveScriptSite - get locale identifier.

    @param pLCID    [out] pointer to locale id
  */
HRESULT _stdcall CActiveScriptEngine::GetLCID(LCID *pLCID)
{
    *pLCID = GetUserDefaultLCID();
    return S_OK;
}

/** IActiveScriptSite - get information about
    external script item

    @param pstrName         Item name
    @param dwReturnMask     Kind of information to return
    @param ppunkItem        [out] Pointer to item's interface
    @param ppTypeInfo       [out] Type information of the item
  */
HRESULT _stdcall CActiveScriptEngine::GetItemInfo(LPCOLESTR pstrName,
                             ULONG dwReturnMask,
                             IUnknown **ppunkItem,
                             ITypeInfo **ppTypeInfo)
{
    int i;
    for (i = 0; i < (int)m_awcsItemNames.size(); i++)
        if (!_wcsicmp(m_awcsItemNames[i], pstrName))
        {
            if(dwReturnMask & SCRIPTINFO_IUNKNOWN)
            {
                IUnknown *pUnk = m_awcsObjects[i];
                pUnk->AddRef();
                *ppunkItem = pUnk;
            }

            if(dwReturnMask & SCRIPTINFO_ITYPEINFO)
            {
                ITypeInfo *pInfo = m_awcsTypes[i];
                if (pInfo)
                    pInfo->AddRef();
                *ppTypeInfo = pInfo;
            }
            return S_OK;
        }
    return TYPE_E_ELEMENTNOTFOUND;
}

/** IActiveScriptSite - is not implemented */
HRESULT _stdcall CActiveScriptEngine::GetDocVersionString(BSTR *pstrVal)
{
    return S_OK;
}

/** IActiveScriptSite - is not implemented */
HRESULT _stdcall CActiveScriptEngine::OnScriptTerminate(const VARIANT *pvResult,
                                   const EXCEPINFO* pexcepinfo)
{
    return S_OK;
}

/** IActiveScriptSite - is not implemented */
HRESULT _stdcall CActiveScriptEngine::OnStateChange(tagSCRIPTSTATE ssScriptState)
{
    return S_OK;
}

/** IActiveScriptSite - is not implemented */
HRESULT _stdcall CActiveScriptEngine::OnScriptError(IActiveScriptError *pError)
{
    EXCEPINFO aExcp;
    memset(&aExcp, 0, sizeof(EXCEPINFO));

    m_iContext = m_iLine = m_iPos = -1;


    if (pError)
    {
        pError->GetSourcePosition((DWORD*)&m_iContext,
                                  (ULONG*)&m_iLine,
                                  (LONG*)&m_iPos);

        pError->GetExceptionInfo(&aExcp);

        if (aExcp.bstrSource)
            SysFreeString(aExcp.bstrSource);

        if (aExcp.bstrDescription)
        {
            m_sNativeDescription = aExcp.bstrDescription;
            SysFreeString(aExcp.bstrDescription);
        }

        if (aExcp.bstrHelpFile)
            SysFreeString(aExcp.bstrHelpFile);
    }
    return S_OK;

}

/** IActiveScriptSite - is not implemented */
HRESULT _stdcall CActiveScriptEngine::OnEnterScript()
{
    return S_OK;
}

/** IActiveScriptSite - is not implemented */
HRESULT _stdcall CActiveScriptEngine::OnLeaveScript()
{
    return S_OK;
}

/** IActiveScriptSite - is not implemented */
HRESULT _stdcall CActiveScriptEngine::GetWindow(HWND *phWnd)
{
    phWnd = 0;
    return S_OK;
}

/** IActiveScriptSite - is not implemented */
HRESULT _stdcall CActiveScriptEngine::EnableModeless(BOOL bEnable)
{
    return S_OK;
}

/** IActiveScriptSite - is not implemented */
HRESULT _stdcall CActiveScriptEngine::GetApplication(IDebugApplication **ppApp)
{
    return E_NOTIMPL;
}

/** IActiveScriptSite - is not implemented */
HRESULT _stdcall CActiveScriptEngine::GetDocumentContextFromPosition(DWORD, ULONG, ULONG, IDebugDocumentContext**)
{
    return E_NOTIMPL;
}

/** IActiveScriptSite - is not implemented */
HRESULT _stdcall CActiveScriptEngine::GetRootApplicationNode(IDebugApplicationNode **)
{
    return E_NOTIMPL;
}

/** IActiveScriptSite - get information about error

    @param perror       in-script error description
    @param pbCall       ?
    @param pbPass       ?
  */
HRESULT _stdcall CActiveScriptEngine::OnScriptErrorDebug(IActiveScriptErrorDebug *perror,
                                        BOOL *pbCall, BOOL *pbPass)
{
    return E_NOTIMPL;
}
#pragma managed
