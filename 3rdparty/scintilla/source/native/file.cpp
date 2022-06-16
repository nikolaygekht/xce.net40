#pragma unmanaged

#include <windows.h>
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <stdarg.h>
#include "file.h"

const int CFileIO::BINBUFSIZE = 131072;

/** Constructor. */
CFileIO::CFileIO()
{
    mBuffer = 0;
    mFile = 0;
    m_bTemp = false;
}

/** Destructor. */
CFileIO::~CFileIO()
{
    close();
}

/** Does we reach eof. */
bool CFileIO::eof()
{
    if (mFile == 0)
        return true;
    else
        return feof(mFile) != 0;
}

/** Close file. */
void CFileIO::close()
{
    if (mFile != 0)
    {
        fclose(mFile);
        mFile = 0;
    }

    if (mBuffer != 0)
        delete[] mBuffer;
    mBuffer = 0;

    if (m_bTemp)
        deleteFile(m_szName);

    m_bTemp = false;
}

/** open the temporary file. */
bool CFileIO::openTemp()
{
    wchar_t szPath[MAX_PATH];
    wchar_t szName[MAX_PATH];

    GetTempPath(MAX_PATH, szPath);
    GetTempFileName(szPath, L"xce", 0, szName);

    mFile = _wfopen(szName, L"w+bT");
    if (mFile != 0)
    {
        wcscpy_s(m_szName, MAX_PATH, szName);
        m_bTemp = true;
        mBuffer = new char[BINBUFSIZE];
        setvbuf(mFile, mBuffer, _IOFBF, BINBUFSIZE);
        return true;
    }
    else
        return false;
}

/** Write long value. */
bool CFileIO::writeInt(long lVal)
{
    if (mFile == 0)
        return false;
    return (fwrite(&lVal, sizeof(long), 1, mFile) == 1);
}

/** Write long value. */
bool CFileIO::readInt(long *plVal)
{
    if (mFile == 0)
        return false;
    return (fread(plVal, sizeof(long), 1, mFile) == 1);
}

/** Write long value. */
bool CFileIO::writeString(const wchar_t *szVal, int length)
{
    if (mFile == 0)
        return false;
    return fwrite(szVal, sizeof(wchar_t), length, mFile) == length;
}

/** Write long value. */
bool CFileIO::readString(wchar_t *szVal, int length)
{
    if (mFile == 0)
        return false;
    return fread(szVal, sizeof(wchar_t), length, mFile) == length;
}

/** Get current file position. */
long CFileIO::getPos()
{
    if (mFile == 0)
        return 0;
    return ftell(mFile);
}

/** Get current file position. */
void CFileIO::goPos(long lPos)
{
    if (mFile == 0)
        return ;
    fseek(mFile, lPos, SEEK_SET);
}

/** remove vile. */
bool CFileIO::deleteFile(const wchar_t *szName)
{
    return DeleteFile(szName) != 0;
}


