
typedef SString *(_stdcall *adapter_getline_ptr)(int lno);


class CColorerLineSourceAdapter : public LineSource
{
 public:
    /** Constructor.

        @param getLinePtr The callback for getting the line
      */
    CColorerLineSourceAdapter(adapter_getline_ptr getLinePtr);

    /** Destructor. */
    ~CColorerLineSourceAdapter();

    /** Overridable: Job started.

        @param lno      First line number.
      */
    virtual void startJob(int lno);

    /** Overridable: Job finished.

        @param lno      Last line number.
      */
    virtual void endJob(int lno);

    /** Overridable: Get contents of specified string

        @param lno      Line number to get

        @return         String object, which should be valid
                        until next call of getLine() or
                        endJob()
      */
    virtual String *getLine(int lno);
  protected:
    adapter_getline_ptr mGetLinePtr;
    SString *mLastString;
};

