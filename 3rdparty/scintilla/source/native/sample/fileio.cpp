#include "stdhead.h"
#include "fileio.h"

const int CFileIO::BUFSIZE = 32768;
const int CFileIO::BINBUFSIZE = 131071;

/** Constructor. */
CFileIO::CFileIO()
{
    m_hFile = 0;
    m_pBuffer = 0;
    m_lBufLen = 0;
    m_lBufPos = 0;
    m_dwAttr = 0;
    m_bTemp = false;
    m_bBinCache = false;
}

/** Destructor. */
CFileIO::~CFileIO()
{
    close();
}

/** Does we reach eof. */
bool CFileIO::eof()
{
    if (!m_pBuffer)
        return true;
    else
    {
        if (m_bEOF && m_lBufPos == m_lBufLen)
            return true;
        else
            return false;
    }
}

/** Close file. */
void CFileIO::close()
{
    if (m_bBinCache)
        stopBinCache(true);

    if (m_pBuffer)
        delete m_pBuffer;

    if (m_hFile)
    {
        CloseHandle(m_hFile);
        if (m_dwAttr != 0 && m_dwAttr != -1)
            SetFileAttributes(m_szName, m_dwAttr | FILE_ATTRIBUTE_ARCHIVE);
    }

    if (m_bTemp)
        deleteFile(m_szName);

    m_bTemp = false;

    m_dwAttr = 0;
    m_bUnix = false;
    m_hFile = 0;
    m_pBuffer = 0;
    m_lBufLen = 0;
    m_lBufPos = 0;
}

/** Read piece of data. */
void CFileIO::readPiece()
{
    if (m_bEOF)
        m_lBufLen = m_lBufPos = 0;

    if (!m_pBuffer)
    {
        m_bEOF = true;
        return ;
    }

    DWORD dwRead = 0;
    ReadFile(m_hFile, m_pBuffer, BUFSIZE, &dwRead, 0);
    if (dwRead == 0)
    {
        m_bEOF = true;
        m_lBufLen = m_lBufPos = 0;
    }
    else
    {
        m_lBufPos = 0;
        m_lBufLen = dwRead;
    }
}

/** open the temporary file. */
bool CFileIO::openTemp()
{
    char szPath[MAX_PATH];
    char szName[MAX_PATH];

    GetTempPath(MAX_PATH, szPath);
    GetTempFileName(szPath, "xceundo", 0, szName);

    m_hFile = CreateFile(szName, GENERIC_READ | GENERIC_WRITE, 0,
                         0, CREATE_ALWAYS, FILE_ATTRIBUTE_TEMPORARY | FILE_FLAG_SEQUENTIAL_SCAN, 0);

    if (m_hFile != 0)
    {
        strcpy(m_szName, szName);
        m_bTemp = true;
        m_dwAttr = -1;
        return true;
    }
    else
        return false;
}

/** Write long value. */
bool CFileIO::writeInt(long lVal)
{
    if (m_bBinCache)
    {
        writeBinCache(&lVal, 4);
        return true;
    }
    else
    {
        DWORD dwWritten = 0;

        if (!WriteFile(m_hFile, &lVal, 4, &dwWritten, 0))
            return false;

        if (dwWritten != 4)
            return false;

        return true;
    }
}

/** Write long value. */
bool CFileIO::readInt(long *plVal)
{
    if (m_bBinCache)
        stopBinCache(true);

    DWORD dwRead = 0;
    if (!ReadFile(m_hFile, plVal, 4, &dwRead, 0))
        return false;
    if (dwRead != 4)
        return false;

    return true;
}

/** Write long value. */
bool CFileIO::writeAsciz(const char *szVal)
{
    long l = strlen(szVal);

    if (!writeInt(l + 1))
        return false;

    if (m_bBinCache)
        writeBinCache(szVal, l + 1);
    else
    {
        DWORD dwWritten = 0;
        if (!WriteFile(m_hFile, szVal, l + 1, &dwWritten, 0))
            return false;

        if (dwWritten != l + 1)
            return false;
    }
    return true;
}

/** Write long value. */
bool CFileIO::readAsciz(char *szVal)
{
    if (m_bBinCache)
        stopBinCache(true);

    long l;

    if (!readInt(&l))
        return false;

    DWORD dwRead = 0;
    if (!ReadFile(m_hFile, szVal, l, &dwRead, 0))
        return false;

    if (dwRead != l)
        return false;

    return true;
}

/** Get current file position. */
long CFileIO::getPos()
{
    long lPos = SetFilePointer(m_hFile, 0, 0, FILE_CURRENT);

    if (m_bBinCache)
        lPos += m_lBufPos;

    return lPos;
}

/** Get current file position. */
void CFileIO::goPos(long lPos)
{
    if (m_bBinCache)
        stopBinCache(true);

    SetFilePointer(m_hFile, lPos, 0, FILE_BEGIN);
}

/** remove vile. */
bool CFileIO::deleteFile(const char *szName)
{
    return DeleteFile(szName);
}

/** start binary write caching operation. */
void CFileIO::startBinCache()
{
    if (m_bBinCache)
    {
        m_iCacheDepth++;
        return ;
    }

    m_iCacheDepth = 1;
    m_pBuffer = new unsigned _int8[BINBUFSIZE];
    m_lBufPos = 0;
    m_lBufLen = BINBUFSIZE;
    m_bBinCache = true;
}

/** stop binary write caching operation. */
bool CFileIO::stopBinCache(bool bForce)
{
    if (!m_bBinCache)
        return true;

    m_iCacheDepth--;

    if (m_iCacheDepth <= 0 || bForce)
    {
        if (m_lBufPos || m_pBuffer != 0)
        {
            DWORD dw;
            WriteFile(m_hFile, m_pBuffer, m_lBufPos, &dw, 0);
        }
        delete m_pBuffer;
        m_pBuffer = 0;
        m_iCacheDepth = 0;
        m_lBufPos = 0;
        m_lBufLen = 0;
        m_bBinCache = false;
        return true;
    }
    else
        return false;
}

/** write to bin cache.

    @param pBuff        buffer to write
    @param lLength      length of data to be written
  */
void CFileIO::writeBinCache(const void *pBuff, unsigned long lLength)
{
    if (m_lBufPos + lLength >= m_lBufLen)
    {
        DWORD dw;
        WriteFile(m_hFile, m_pBuffer, m_lBufPos, &dw, 0);
        m_lBufPos = 0;
    }
    memcpy(m_pBuffer + m_lBufPos, pBuff, lLength);
    m_lBufPos += lLength;
}


