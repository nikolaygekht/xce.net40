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
    bool writeAsciz(const char *szVal);

    /** Write long value. */
    bool readAsciz(char *szVal);

    /** Get current file position. */
    long getPos();

    /** start binary write caching operation. */
    void startBinCache();

    /** stop binary write caching operation. */
    bool stopBinCache(bool bForce = false);

    /** write to bin cache.

        @param pBuff        buffer to write
        @param lLength      length of data to be written
      */
    void writeBinCache(const void *pBuff, unsigned long lLength);

    /** Get current file position. */
    void goPos(long lPos);

    bool deleteFile(const char *szName);
 protected:
    /** Read piece of data. */
    void readPiece();
    /** Read one character. */
    int inline readChar()
    {
        if (m_lBufLen == m_lBufPos && m_bEOF)
            return -1;
        if (m_lBufLen == m_lBufPos)
            readPiece();
        if (m_bEOF)
            return -1;

        return (int)(unsigned char)m_pBuffer[m_lBufPos++];
    };

    /** Rollback one character (may be called only once). */
    void inline rollback()
    {
        if (m_lBufPos)
            m_lBufPos--;
    };
    DWORD m_dwAttr;                 //!< file attributes
    char m_szName[MAX_PATH];        //!< file name
    bool m_bUnix;                   //!< unix mode
    bool m_bTemp;                   //!< file is temporary file
    HANDLE m_hFile;                 //!< handle of the file
    unsigned _int8 *m_pBuffer;      //!< file buffer
    long m_lBufLen;                 //!< length of data in the buffer
    long m_lBufPos;                 //!< position of data in the buffer
    bool m_bEOF;                    //!< buffer reachs EOF
    bool m_bBinCache;               //!< binary write cached operation
    int m_iCacheDepth;              //!< depth of binary cache starts
    static const int BUFSIZE;       //!< size of buffer
    static const int BINBUFSIZE;    //!< size of buffer for binary cache
};
