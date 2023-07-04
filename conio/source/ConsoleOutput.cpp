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
    init(save, false, 0, 0, ConsoleOutputMode::ConEmu);
}

ConsoleOutput::ConsoleOutput(bool save, int rows, int columns)
{
    init(save, true, rows, columns, ConsoleOutputMode::ConEmu);

}

ConsoleOutput::ConsoleOutput(bool save, ConsoleOutputMode outputMode)
{
    init(save, false, 0, 0, outputMode);
}

ConsoleOutput::ConsoleOutput(bool save, int rows, int columns, ConsoleOutputMode outputMode)
{
    init(save, true, rows, columns, outputMode);
}

void ConsoleOutput::init(bool save, bool changeResolution, int rows, int columns, ConsoleOutputMode outputMode)
{
    mOutputMode = outputMode;
    if (outputMode == ConsoleOutputMode::VT)
    {
        DWORD dwMode = 0;
        GetConsoleMode(GetStdHandle(STD_OUTPUT_HANDLE), &dwMode);

        if (!SetConsoleMode(GetStdHandle(STD_OUTPUT_HANDLE), dwMode | ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN))
            if (SetConsoleMode(GetStdHandle(STD_OUTPUT_HANDLE), dwMode | ENABLE_VIRTUAL_TERMINAL_PROCESSING))
                throw gcnew ArgumentException("Terminal is not compatible with VT mode");
    }
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

    if (outputMode == ConsoleOutputMode::ConEmu)
    {
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

    if (mOutputMode == ConsoleOutputMode::Win32 ||
        mOutputMode == ConsoleOutputMode::ConEmu)
        paintWin32(canvas, fast);
    else
        paintVT(canvas, fast);

}

void ConsoleOutput::paintVT(Canvas ^canvas, bool fast)
{
    BOOL saveVisible;

    HANDLE oh = GetStdHandle(STD_OUTPUT_HANDLE);
    CONSOLE_CURSOR_INFO cci;

    memset(&cci, 0, sizeof(cci));
    cci.dwSize = sizeof(cci);
    GetConsoleCursorInfo(oh, &cci);
    saveVisible = cci.bVisible;

    cci.bVisible = false;
    SetConsoleCursorInfo(oh, &cci);
    ConsoleColor^ color = gcnew ConsoleColor(0x00);
    wchar_t symbol[2] = L" ";
    wchar_t currentEscape[256], previousEscape[256];
    wchar_t buff[256];

    *currentEscape = *previousEscape = 0;
    DWORD lw;
    for (int i = 0; i < mRows; i++)
    {
        COORD cc;
        cc.X = 0;
        cc.Y = i;
        SetConsoleCursorPosition(oh, cc);

        *previousEscape = 0;

        for (int j = 0; j < mColumns; j++)
        {
            canvas->get(i, j, symbol[0], color);
            EscapeCode(color, currentEscape);
            *buff = 0;

            if (wcscmp(previousEscape, currentEscape) != 0)
            {
                WriteVtSequence(currentEscape);
                wcscpy_s(previousEscape, currentEscape);
            }
            WriteVtSequence(symbol);
        }
    }
    cci.bVisible = saveVisible;
    SetConsoleCursorInfo(oh, &cci);
}

void ConsoleOutput::WriteVtSequence(wchar_t *sequence)
{
    DWORD dw = 0;
    WriteConsoleW(GetStdHandle(STD_OUTPUT_HANDLE), sequence, wcslen(sequence), &dw, NULL);
}

void ConsoleOutput::EscapeCode(ConsoleColor^ color, wchar_t *sequence)
{
    *sequence = 0;

    if (!color->RGBValid)
    {
        short paletteColor = color->PaletteColor;
        short fb = paletteColor & 0xf;
        short bg = (paletteColor & 0xf0) >> 4;

        switch (fb)
        {
        case    0x0:
                wcscat_s(sequence, 256, L"\x1b[30m");
                break;
        case    0x1:
                wcscat_s(sequence, 256, L"\x1b[34m");
                break;
        case    0x2:
                wcscat_s(sequence, 256, L"\x1b[32m");
                break;
        case    0x3:
                wcscat_s(sequence, 256, L"\x1b[36m");
                break;
        case    0x4:
                wcscat_s(sequence, 256, L"\x1b[31m");
                break;
        case    0x5:
                wcscat_s(sequence, 256, L"\x1b[35m");
                break;
        case    0x6:
                wcscat_s(sequence, 256, L"\x1b[33m");
                break;
        case    0x7:
                wcscat_s(sequence, 256, L"\x1b[37m");
                break;
        case    0x8:
                wcscat_s(sequence, 256, L"\x1b[90m");
                break;
        case    0x9:
                wcscat_s(sequence, 256, L"\x1b[94m");
                break;
        case    0xa:
                wcscat_s(sequence, 256, L"\x1b[92m");
                break;
        case    0xb:
                wcscat_s(sequence, 256, L"\x1b[96m");
                break;
        case    0xc:
                wcscat_s(sequence, 256, L"\x1b[91m");
                break;
        case    0xd:
                wcscat_s(sequence, 256, L"\x1b[95m");
                break;
        case    0xe:
                wcscat_s(sequence, 256, L"\x1b[93m");
                break;
        case    0xf:
                wcscat_s(sequence, 256, L"\x1b[97m");
                break;
        }
        switch (bg)
        {
        case    0x0:
                wcscat_s(sequence, 256, L"\x1b[40m");
                break;
        case    0x1:
                wcscat_s(sequence, 256, L"\x1b[44m");
                break;
        case    0x2:
                wcscat_s(sequence, 256, L"\x1b[42m");
                break;
        case    0x3:
                wcscat_s(sequence, 256, L"\x1b[46m");
                break;
        case    0x4:
                wcscat_s(sequence, 256, L"\x1b[41m");
                break;
        case    0x5:
                wcscat_s(sequence, 256, L"\x1b[45m");
                break;
        case    0x6:
                wcscat_s(sequence, 256, L"\x1b[43m");
                break;
        case    0x7:
                wcscat_s(sequence, 256, L"\x1b[47m");
                break;
        case    0x8:
                wcscat_s(sequence, 256, L"\x1b[100m");
                break;
        case    0x9:
                wcscat_s(sequence, 256, L"\x1b[104m");
                break;
        case    0xa:
                wcscat_s(sequence, 256, L"\x1b[102m");
                break;
        case    0xb:
                wcscat_s(sequence, 256, L"\x1b[106m");
                break;
        case    0xc:
                wcscat_s(sequence, 256, L"\x1b[101m");
                break;
        case    0xd:
                wcscat_s(sequence, 256, L"\x1b[105m");
                break;
        case    0xe:
                wcscat_s(sequence, 256, L"\x1b[103m");
                break;
        case    0xf:
                wcscat_s(sequence, 256, L"\x1b[107m");
                break;
        }
    }
    else
    {
        int fg = color->RGBForeground;
        int bg = color->RGBBackground;
        int fgr, fgb, fgg;
        int bgr, bgb, bgg;

        fgr = fg & 0xff;
        fgb = (fg >> 8) & 0xff;
        fgg = (fg >> 16) & 0xff;
        bgr = bg & 0xff;
        bgb = (bg >> 8) & 0xff;
        bgg = (bg >> 16) & 0xff;

        wsprintf(sequence, L"\x1b[38;2;%i;%i;%im\x1b[48;2;%i;%i;%im",
                           fgr, fgb, fgg, bgr, bgb, bgg);
    }
}


void ConsoleOutput::paintWin32(Canvas ^canvas, bool fast)
{
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
