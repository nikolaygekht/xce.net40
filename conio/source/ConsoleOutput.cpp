#pragma unmanaged
#include "stdhead.h"
#include "consoleannotation.h"
extern "C" HWND WINAPI GetConsoleWindow(void);

#pragma managed
#using <mscorlib.dll>
using namespace System;
using namespace System::Runtime::InteropServices;

#include "canvas.h"
#include "ConsoleOutput.h"

namespace gehtsoft
{
namespace xce
{
namespace conio
{

ConsoleOutput::ConsoleOutput(bool save)
{
    init(save, false, 0, 0);
}

ConsoleOutput::ConsoleOutput(bool save, int rows, int columns)
{
    init(save, true, rows, columns);
}

void ConsoleOutput::init(bool save, bool changeResolution, int rows, int columns)
{
    CONSOLE_SCREEN_BUFFER_INFO csbi;
    memset(&csbi, 0, sizeof(csbi));
    GetConsoleScreenBufferInfo(GetStdHandle(STD_OUTPUT_HANDLE), &csbi);

    //mOldCP = ::GetConsoleOutputCP();
    //::SetConsoleOutputCP(CP_UTF16);

    mOrgRows = csbi.dwSize.Y;
    mOrgColumns = csbi.dwSize.X;

    if (mOrgRows >= 300 && !changeResolution && (csbi.srWindow.Bottom - csbi.srWindow.Top + 1) < (mOrgRows - 20))
    {
        changeResolution = true;
        rows = columns = 0;
    }



    wchar_t shareName[255];
    wsprintf(shareName, AnnotationShareName, sizeof(AnnotationInfo), GetConsoleWindow());
    int m_dwSize = 0;
    hSharedMem = OpenFileMapping(FILE_MAP_ALL_ACCESS, FALSE, shareName);
    if (hSharedMem == 0)
    {
        mRGBHeader = 0;
        mRGBInfo = 0;
    }
    else
    {
        char *address = (char *)MapViewOfFile(hSharedMem, FILE_MAP_ALL_ACCESS, 0, 0, 0);
        mRGBHeader = (AnnotationHeader*)address;
        if (mRGBHeader == 0)
            mRGBInfo = 0;
        else
            mRGBInfo = (AnnotationInfo*) (address + mRGBHeader->struct_size);

        //wchar_t buff[256];
        //wsprintf(buff,  L"%i %i", mRGBHeader->struct_size, mRGBHeader->bufferSize);
        //MessageBox(0, buff, L"", MB_OK);
    }

    if (save)
    {
        mRestoreContent = true;
        mOrgContent = new CHAR_INFO[mOrgRows * mOrgColumns];
        COORD top = {0, 0}, size = {mOrgColumns, mOrgRows};
        SMALL_RECT rectScreen = {0, 0, mOrgColumns - 1, mOrgRows - 1};
        ReadConsoleOutputW(GetStdHandle(STD_OUTPUT_HANDLE), mOrgContent, size, top, &rectScreen);

        getCursorType(mCursorSize, mCursorVisible);
        getCursorPos(mCursorRow, mCursorColumn);

        if (mRGBInfo != 0)
        {
            mOrgInfo = new AnnotationInfo[mOrgRows * mOrgColumns];
            int copy = sizeof(AnnotationInfo) * mOrgRows * mOrgColumns;
            if (copy > mRGBHeader->bufferSize)
                copy = mRGBHeader->bufferSize;
            memcpy(mOrgInfo, mRGBInfo, copy);
        }
    }
    else
    {
        mRestoreContent = false;
        mOrgContent = 0;
    }

    if (changeResolution)
    {
        if (rows == 0 && columns == 0)
        {
            rows = csbi.srWindow.Bottom - csbi.srWindow.Top + 1;
            columns = csbi.srWindow.Right - csbi.srWindow.Left + 1;
        }

        COORD bufferSize;
        SMALL_RECT rect = {0, 0, 0, 0};
        bufferSize.Y = rows;
        bufferSize.X = columns;
        rect.Bottom = rows;
        rect.Right = columns;
        SetConsoleScreenBufferSize(GetStdHandle(STD_OUTPUT_HANDLE), bufferSize);
        SetConsoleWindowInfo(GetStdHandle(STD_OUTPUT_HANDLE), TRUE, &rect);
        mRows = rows;
        mColumns = columns;
        mRestoreRes = true;
    }
    else
    {
        mRestoreRes = false;
        mRows = mOrgRows;
        mColumns = mOrgColumns;
    }
}

void ConsoleOutput::updateSize()
{
    CONSOLE_SCREEN_BUFFER_INFO csbi;
    memset(&csbi, 0, sizeof(csbi));
    GetConsoleScreenBufferInfo(GetStdHandle(STD_OUTPUT_HANDLE), &csbi);
    mRows = csbi.dwSize.Y;
    mColumns = csbi.dwSize.X;
}

ConsoleOutput::~ConsoleOutput()
{
    this->!ConsoleOutput();
}

ConsoleOutput::!ConsoleOutput()
{
    if (mRestoreRes)
    {
        COORD bufferSize;
        SMALL_RECT rect = {0, 0, 0, 0};
        bufferSize.Y = mOrgRows;
        bufferSize.X = mOrgColumns;
        rect.Bottom = mOrgRows;
        rect.Right = mOrgColumns;
        SetConsoleScreenBufferSize(GetStdHandle(STD_OUTPUT_HANDLE), bufferSize);
        SetConsoleWindowInfo(GetStdHandle(STD_OUTPUT_HANDLE), TRUE, &rect);
        mRestoreRes = false;
    }

    if (mRestoreContent && mOrgContent != 0)
    {
        COORD top = {0, 0}, size = {mOrgColumns, mOrgRows};
        SMALL_RECT rectScreen = {0, 0, mOrgColumns - 1, mOrgRows - 1};
        WriteConsoleOutputW(GetStdHandle(STD_OUTPUT_HANDLE), mOrgContent, size, top, &rectScreen);

        mRestoreContent = 0;
        delete[] mOrgContent;
        mOrgContent = 0;

        setCursorType(mCursorSize, mCursorVisible);
        setCursorPos(mCursorRow, mCursorColumn);

        if (mRGBInfo != 0)
        {
            mRGBHeader->locked = TRUE;
            int copy = sizeof(AnnotationInfo) * mOrgRows * mOrgColumns;
            if (copy > mRGBHeader->bufferSize)
                copy = mRGBHeader->bufferSize;
            memcpy(mRGBInfo, mOrgInfo, copy);
            mRGBHeader->locked = FALSE;
            mRGBHeader->flushCounter++;
        }

    }

    if (mRGBHeader)
        UnmapViewOfFile(mRGBHeader);
    mRGBHeader = 0;
    mRGBInfo = 0;
    if (hSharedMem != 0)
        CloseHandle(hSharedMem);
    hSharedMem = 0;
}

int ConsoleOutput::Rows::get()
{
    return mRows;
}

int ConsoleOutput::Columns::get()
{
    return mColumns;
}

void ConsoleOutput::paint(Canvas ^canvas, bool fast)
{
    if (canvas->Rows != mRows ||
        canvas->Columns != mColumns)
            throw gcnew ArgumentException("Canvas size does not match the screen size");

    //update true color info
    if (mRGBHeader != 0)
    {
        mRGBHeader->locked = TRUE;

        int icount = canvas->Rows * canvas->Columns;
        if (icount <= mRGBHeader->bufferSize)
        {
            int *fgcolor, *bgcolor, *style;
            fgcolor = canvas->ForegroundColor;
            bgcolor = canvas->BackgroundColor;
            style = canvas->Style;
            bool f = true;
            for (int i = 0; i < icount; i++, fgcolor++, bgcolor++, style++)
            {
                int fg = *fgcolor;
                int bg = *bgcolor;
                int s = *style;
                AnnotationInfo *ai = mRGBInfo + i;
                memset(ai, 0, sizeof(AnnotationInfo));
                ai->border_visible = 0;
                ai->style = 0;
                if (fg != -1 && bg != -1)
                {
                    ai->bk_color = bg;
                    ai->fg_color = fg;
                    ai->bk_valid = 1;
                    ai->fg_valid = 1;
                    ai->style = s;
                /*
                    if (f && s != 0)
                    {
                        wchar_t buff[256];
                        wsprintf(buff,  L"[%06x %06x %04x],", bg, fg, s & 0xffff);

                        wchar_t buff1[256];

                        _int8 *p = (_int8 *)ai;
                        for (int k = 0; k < 32; k++)
                        {
                            wsprintf(buff1,  L"%02x,", ((int)*p)&0xff);
                            p++;
                            wcscat(buff, buff1);
                        }
                        MessageBox(0, buff, L"", MB_OK);
                        f = false;
                    }
                  */
                }
                else
                {
                    ai->bk_valid = 0;
                    ai->fg_valid = 0;
                }

            }
        }
    }


    if (fast)
    {
        COORD top = {0, 0}, size = {mColumns, mRows};
        SMALL_RECT rectScreen = {0, 0, mColumns - 1, mRows - 1};
        WriteConsoleOutputW(GetStdHandle(STD_OUTPUT_HANDLE), canvas->Data, size, top, &rectScreen);
    }
    else
    {
        //in slow draw mode only changed rows will be moved to the screen
        COORD top = {0, 0}, size = {mColumns, mRows};
        SMALL_RECT rectScreen = {0, 0, mColumns - 1, mRows - 1};
        CHAR_INFO *currImg = new CHAR_INFO[mRows * mColumns];
        ReadConsoleOutputW(GetStdHandle(STD_OUTPUT_HANDLE), currImg, size, top, &rectScreen);
        CHAR_INFO *buffer = canvas->Data;

        //scan all rows
        for (int i = 0; i < mRows; i++)
        {
            if (memcmp(currImg + i * mColumns, buffer + i * mColumns, mColumns * sizeof(CHAR_INFO)) != 0)
            {
                top.Y = i;
                rectScreen.Top = i;
                rectScreen.Bottom = i;
                rectScreen.Left = 0;
                rectScreen.Right = mColumns - 1;
                WriteConsoleOutputW(GetStdHandle(STD_OUTPUT_HANDLE), buffer, size, top, &rectScreen);
            }
        }
        delete currImg;
    }

    if (mRGBHeader != 0)
    {
        mRGBHeader->locked = FALSE;
        mRGBHeader->flushCounter++;
    }
}

void ConsoleOutput::setCursorType(int size, bool visible)
{
    CONSOLE_CURSOR_INFO cci;
    cci.dwSize = size;
    cci.bVisible = visible ? 1 : 0;
     ::SetConsoleCursorInfo(GetStdHandle(STD_OUTPUT_HANDLE), &cci);
}

void ConsoleOutput::getCursorType([Out]int %size, [Out]bool %visible)
{
    CONSOLE_CURSOR_INFO cci;
    ::GetConsoleCursorInfo(GetStdHandle(STD_OUTPUT_HANDLE),&cci);
    size = cci.dwSize;
    visible = (bool)(cci.bVisible != 0);
}

void ConsoleOutput::setCursorPos(int row, int column)
{
    COORD pos = {column, row};
    SetConsoleCursorPosition(GetStdHandle(STD_OUTPUT_HANDLE), pos);
}

void ConsoleOutput::getCursorPos([Out]int %row, [Out]int %column)
{
    CONSOLE_SCREEN_BUFFER_INFO csbi;
    memset(&csbi, 0, sizeof(csbi));
    GetConsoleScreenBufferInfo(GetStdHandle(STD_OUTPUT_HANDLE), &csbi);
    row = csbi.dwCursorPosition.Y;
    column = csbi.dwCursorPosition.X;
}


}
}
}
