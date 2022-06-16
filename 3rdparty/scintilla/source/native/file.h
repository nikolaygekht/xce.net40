/** File reader/write. */
class CFileIO
{
 public:
    /** Constructor. */
    CFileIO();

    /** Destructor. */
    ~CFileIO();

    /** Does we reach eof. */
    bool eof();

    /** Close file. */
    void close();

    /** open the temporary file. */
    bool openTemp();

    /** Write long value. */
    bool writeInt(long lVal);

    /** Write long value. */
    bool readInt(long *plVal);

    /** Write long value. */
    bool writeString(const wchar_t *szVal, int length);

    /** Write long value. */
    bool readString(wchar_t *szVal, int length);

    /** Get current file position. */
    long getPos();

    /** Get current file position. */
    void goPos(long lPos);


 protected:
    bool deleteFile(const wchar_t *szName);

    wchar_t m_szName[MAX_PATH];        //!< file name
    bool m_bTemp;                   //!< file is temporary file
    FILE *mFile;
    static const int BINBUFSIZE;    //!< size of buffer for binary cache
    char *mBuffer;
};
