/** Implementation of active script consuming interfaces. */
class CActiveScriptEngine : public IActiveScriptSiteWindow,
                            public IActiveScriptSite,
                            public IActiveScriptSiteDebug32
{
 public:
    /** Constructor. */
    CActiveScriptEngine();

    /** Description. */
    virtual ~CActiveScriptEngine();


    /** Initialize the script. */
    HRESULT init(const wchar_t *wcsName);

    /** Add script text.

        @param wcsText      The text of script
        @param wcsName      Name of text piece
      */
    HRESULT load(const wchar_t *wcsText, const wchar_t *wcsName);

    /** Connect to script engine. */
    HRESULT connect();

    /** Connect to script engine. */
    HRESULT close();

    /** Add object to global object table of script.

        @param wcsName      Name of the object
        @param pUnk         Unknown interface to the object
        @param bGlobal      If this flag is true - the methods
                            of object will be exposed as global
                            methods for script
      */
    HRESULT addObject(const wchar_t *wcsName, IUnknown *pUnk, bool bGlobal);

    /** Invoke function.

        @param wcsName          Function's name
        @param lParamCount      Number of parameters
        @param vParams          List of parameters
        @param vRetCode         Method's return code.
      */
    HRESULT invoke(const wchar_t *wcsMethod, int lParamCount, VARIANT *vParams, VARIANT *vRetCode);

    /** Returns context of last error. */
    const wchar_t *getLastErrorContext();

    /** Returns line of last error. */
    int getLastErrorLine();

    /** Returns position of last error. */
    int getLastErrorPos();

    /** Returns message of last error.

        @param hr       Error code or 0 if you want
                        get message for last error
      */
    static const wchar_t *getErrorMsg(HRESULT hr = 0);

    /** Returns native message of last error. */
    const wchar_t *getErrorDescription();

    /** Implementation of IUnknown::QueryInterface

        Returns the interface, supported by object

        @param iid      Interface description
        @param ppv      [out] Pointer to interface
      */
    HRESULT _stdcall QueryInterface(REFIID iid, void **ppv);

    /** Implementation of IUnknown::AddRef

        Add reference to object
      */
    ULONG _stdcall AddRef();

    /** Implementation of IUnknown::Release

        Subtrace reference to object and destroy object
        if there is no much references
      */
    ULONG _stdcall Release();

    /** IActiveScriptSite - get locale identifier.

        @param pLCID    [out] pointer to locale id
      */
    HRESULT _stdcall GetLCID(LCID *pLCID);

    /** IActiveScriptSite - get information about
        external script item

        @param pstrName         Item name
        @param dwReturnMask     Kind of information to return
        @param ppunkItem        [out] Pointer to item's interface
        @param ppTypeInfo       [out] Type information of the item
      */
    HRESULT _stdcall GetItemInfo(LPCOLESTR pstrName,
                                 ULONG dwReturnMask,
                                 IUnknown **ppunkItem,
                                 ITypeInfo **ppTypeInfo);

    /** IActiveScriptSite - is not implemented */
    HRESULT _stdcall GetDocVersionString(BSTR *pstrVal);

    /** IActiveScriptSite - is not implemented */
    HRESULT _stdcall OnScriptTerminate(const VARIANT *pvResult,
                                       const EXCEPINFO* pexcepinfo);

    /** IActiveScriptSite - is not implemented */
    HRESULT _stdcall OnStateChange(tagSCRIPTSTATE ssScriptState);

    /** IActiveScriptSite - is not implemented */
    HRESULT _stdcall OnScriptError(IActiveScriptError *pError);

    /** IActiveScriptSite - is not implemented */
    HRESULT _stdcall OnEnterScript();

    /** IActiveScriptSite - is not implemented */
    HRESULT _stdcall OnLeaveScript();


    /** IActiveScriptSite - is not implemented */
    HRESULT _stdcall GetWindow(HWND *phWnd);

    /** IActiveScriptSite - is not implemented */
    HRESULT _stdcall EnableModeless(BOOL bEnable);

    /** IActiveScriptSite - is not implemented */
    HRESULT _stdcall GetApplication(IDebugApplication **ppApp);

    /** IActiveScriptSite - is not implemented */
    HRESULT _stdcall GetDocumentContextFromPosition(DWORD, ULONG, ULONG, IDebugDocumentContext**);

    /** IActiveScriptSite - is not implemented */
    HRESULT _stdcall GetRootApplicationNode(IDebugApplicationNode **);

    /** IActiveScriptSite - get information about error

        @param perror       in-script error description
        @param pbCall       ?
        @param pbPass       ?
      */
    HRESULT _stdcall OnScriptErrorDebug(IActiveScriptErrorDebug *perror,
                                        BOOL *pbCall, BOOL *pbPass);
 protected:
    std::vector<wchar_t *> m_awcsFiles;         //!< the list of files

    std::vector<wchar_t *> m_awcsItemNames;     //!< list of item's name
    std::vector<IUnknown *> m_awcsObjects;      //!< list of item's objects
    std::vector<ITypeInfo *>m_awcsTypes;        //!< list of item's types

    int m_iContext;                             //!< last error context
    int m_iLine;                                //!< last error line
    int m_iPos;                                 //!< last error position
    HRESULT m_hrError;                          //!< last error code
    _bstr_t m_sNativeDescription;               //!< native error description


    IActiveScript* m_pActiveScript;             //!< script engine object
    IActiveScriptParse* m_pActiveScriptParse;   //!< script engine parser

    long m_cRefCount;                           //!< reference counter
};
