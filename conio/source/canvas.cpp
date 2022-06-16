#pragma unmanaged
#include "stdhead.h"

#pragma managed
#using <mscorlib.dll>
using namespace System;
using namespace System::Runtime::InteropServices;
#include "canvas.h"

namespace gehtsoft
{
namespace xce
{
namespace conio
{

int ConsoleColor::rgb(int red, int green, int blue)
{
    return red | (green << 8) | (blue << 16);
}

ConsoleColor::ConsoleColor(short palColor)
{
    mPalColor = palColor;
    mRGBFg = mRGBBg = -1;
    mStyle = 0;
}

ConsoleColor::ConsoleColor(short palColor, int RGBFg, int RGBBg)
{
    mPalColor = palColor;
    mRGBFg = RGBFg;
    mRGBBg = RGBBg;
    mStyle = 0;
}

ConsoleColor::ConsoleColor(short palColor, int RGBFg, int RGBBg, int style)
{
    mPalColor = palColor;
    mRGBFg = RGBFg;
    mRGBBg = RGBBg;
    mStyle = style;
}

bool ConsoleColor::RGBValid::get()
{
    return mRGBFg >= 0 && mRGBFg <= 0xffffff && mRGBBg >= 0 && mRGBBg <= 0xffffff;
}

short ConsoleColor::PaletteColor::get()
{
    return mPalColor;
}
void ConsoleColor::PaletteColor::set(short v)
{
    mPalColor = v;
}

int ConsoleColor::RGBForeground::get()
{
    return mRGBFg;
}
void ConsoleColor::RGBForeground::set(int v)
{
    mRGBFg = v;
}

int ConsoleColor::RGBBackground::get()
{
    return mRGBBg;
}

void ConsoleColor::RGBBackground::set(int v)
{
    mRGBBg = v;
}

int ConsoleColor::Style::get()
{
    return mStyle;
}

void ConsoleColor::Style::set(int v)
{
    mStyle = v;
}

BoxBorder::BoxBorder(wchar_t topLeft, wchar_t top, wchar_t topRight, wchar_t left, wchar_t right, wchar_t bottomLeft, wchar_t bottom, wchar_t bottomRight)
{
    mTopLeft = topLeft;
    mTop = top;
    mTopRight = topRight;
    mLeft = left;
    mRight = right;
    mBottomLeft = bottomLeft;
    mBottom = bottom;
    mBottomRight = bottomRight;
}

wchar_t BoxBorder::TopLeft::get() { return mTopLeft; }
wchar_t BoxBorder::Top::get() { return mTop; }
wchar_t BoxBorder::TopRight::get() { return mTopRight; }
wchar_t BoxBorder::Left::get() { return mLeft; }
wchar_t BoxBorder::Right::get() { return mRight; }
wchar_t BoxBorder::BottomLeft::get() { return mBottomLeft; }
wchar_t BoxBorder::Bottom::get() { return mBottom; }
wchar_t BoxBorder::BottomRight::get() { return mBottomRight; }

BoxBorder ^BoxBorder::SingleBorderBox::get()
{
    if (mSingleBorder == nullptr)
        mSingleBorder = gcnew BoxBorder(0x250c, 0x2500, 0x2510, 0x2502, 0x2502, 0x2514, 0x2500, 0x2518);
    return mSingleBorder;
}

BoxBorder ^BoxBorder::DoubleBorderBox::get()
{
    if (mDoubleBorder == nullptr)
        mDoubleBorder = gcnew BoxBorder(0x2554, 0x2550, 0x2557, 0x2551, 0x2551, 0x255a, 0x2550, 0x255d);
    return mDoubleBorder;
}


Canvas::Canvas(int rows, int columns)
{
    mRows = rows;
    mColumns = columns;
    int s = rows * columns;
    mData = new CHAR_INFO[s];
    mBackgroundColor = new int[s];
    memset(mBackgroundColor, 0xff, sizeof(int) * s);
    mForegroundColor = new int[s];
    memset(mForegroundColor, 0xff, sizeof(int) * s);
    mStyle = new int[s];
    memset(mStyle, 0x0, sizeof(int) * s);
}

Canvas::~Canvas()
{
    this->!Canvas();
}

Canvas::!Canvas()
{
    if (mData != 0)
        delete mData;
    mData = 0;
    if (mBackgroundColor != 0)
        delete mBackgroundColor;
    mBackgroundColor = 0;
    if (mForegroundColor != 0)
        delete mForegroundColor;
    mForegroundColor = 0;
    if (mStyle != 0)
        delete mStyle;
    mStyle = 0;
}

CHAR_INFO *Canvas::Data::get()
{
    return mData;
}

int *Canvas::BackgroundColor::get()
{
    return mBackgroundColor;
}

int *Canvas::ForegroundColor::get()
{
    return mForegroundColor;
}

int *Canvas::Style::get()
{
    return mStyle;
}

int Canvas::Rows::get()
{
    return mRows;
}

int Canvas::Columns::get()
{
    return mColumns;
}

void Canvas::write(int row, int column, wchar_t chr)
{
    if (row >= 0 && row < mRows && column >= 0 && column < mColumns)
        mData[row * mColumns + column].Char.UnicodeChar = chr;
}

void Canvas::write(int row, int column, ConsoleColor ^color)
{
    if (row >= 0 && row < mRows && column >= 0 && column < mColumns)
    {
        int offset = row * mColumns + column;
        mData[offset].Attributes = color->PaletteColor;
        if (color->RGBValid)
        {
            mForegroundColor[offset] = color->RGBForeground;
            mBackgroundColor[offset] = color->RGBBackground;
            mStyle[offset] = color->Style;
        }
        else
        {
            mForegroundColor[offset] = -1;
            mBackgroundColor[offset] = -1;
            mStyle[offset] = 0;
        }
    }
}

void Canvas::write(int row, int column, wchar_t chr, ConsoleColor ^color)
{
    if (row >= 0 && row < mRows && column >= 0 && column < mColumns)
    {
        int offset = row * mColumns + column;
        CHAR_INFO *t = mData + offset;
        t->Char.UnicodeChar = chr;
        t->Attributes = color->PaletteColor;
        if (color->RGBValid)
        {
            mForegroundColor[offset] = color->RGBForeground;
            mBackgroundColor[offset] = color->RGBBackground;
            mStyle[offset] = color->Style;
        }
        else
        {
            mForegroundColor[offset] = -1;
            mBackgroundColor[offset] = -1;
            mStyle[offset] = 0;
        }
    }
}

void Canvas::write(int row, int column, String ^text)
{
    write(row, column, text, 0, text->Length);
}

void Canvas::write(int row, int column, String ^text, ConsoleColor ^color)
{
    write(row, column, text, 0, text->Length, color);
}

void Canvas::write(int row, int column, String ^text, int offset, int length)
{
    if (row >= 0 && row < mRows)
    {

        if (column < 0)
        {
            length += column;
            offset -= column;
            column = 0;
        }

        if (offset >= text->Length)
            return ;

        if (length > text->Length - offset)
            length = text->Length - offset;
        if (length <= 0)
            return ;

        int limit = column + length;
        if (limit > mColumns)
            limit = mColumns;
        CHAR_INFO *bbase = mData + row * mColumns + column;
        int tbase = offset;
        for (int i = column; i < limit; i++, bbase++, tbase++)
            bbase->Char.UnicodeChar = text[tbase];
    }
}

void Canvas::write(int row, int column, array<wchar_t> ^text, int offset, int length)
{
    if (row >= 0 && row < mRows)
    {

        if (column < 0)
        {
            length += column;
            offset -= column;
            column = 0;
        }

        if (offset >= text->Length)
            return ;

        if (length > text->Length - offset)
            length = text->Length - offset;
        if (length <= 0)
            return ;

        int limit = column + length;
        if (limit > mColumns)
            limit = mColumns;
        CHAR_INFO *bbase = mData + row * mColumns + column;
        int tbase = offset;
        for (int i = column; i < limit; i++, bbase++, tbase++)
            bbase->Char.UnicodeChar = text[tbase];
    }
}


void Canvas::write(int row, int column, System::Text::StringBuilder ^text, int offset, int length)
{
    if (row >= 0 && row < mRows)
    {

        if (column < 0)
        {
            length += column;
            offset -= column;
            column = 0;
        }

        if (offset >= text->Length)
            return ;

        if (length > text->Length - offset)
            length = text->Length - offset;
        if (length <= 0)
            return ;

        int limit = column + length;
        if (limit > mColumns)
            limit = mColumns;
        CHAR_INFO *bbase = mData + row * mColumns + column;
        int tbase = offset;
        for (int i = column; i < limit; i++, bbase++, tbase++)
            bbase->Char.UnicodeChar = text[tbase];
    }
}

void Canvas::write(int row, int column, String ^text, int offset, int length, ConsoleColor ^color)
{
    if (row >= 0 && row < mRows)
    {
        if (column < 0)
        {
            length += column;
            offset -= column;
            column = 0;
        }

        if (offset >= text->Length)
            return ;

        if (length > text->Length - offset)
            length = text->Length - offset;

        if (length <= 0)
            return ;

        int limit = column + length;
        if (limit > mColumns)
            limit = mColumns;

        int offset1 = row * mColumns + column;
        CHAR_INFO *bbase = mData + offset1;
        int *fcbase = mForegroundColor + offset1;
        int *bcbase = mBackgroundColor + offset1;
        int *sbase = mStyle + offset1;

        short pcolor = color->PaletteColor;
        bool rgbvalid = color->RGBValid;
        int fg = color->RGBForeground;
        int bg = color->RGBBackground;
        int s = color->Style;

        int tbase = offset;

        for (int i = column; i < limit; i++, bbase++, tbase++, fcbase++, bcbase++, sbase++)
        {
            bbase->Char.UnicodeChar = text[tbase];
            bbase->Attributes = pcolor;
            if (rgbvalid)
            {
                *fcbase = fg;
                *bcbase = bg;
                *sbase = s;
            }
            else
            {
                *fcbase = -1;
                *bcbase = -1;
                *sbase = 0;
            }
        }
    }
}

void Canvas::write(int row, int column, System::Text::StringBuilder ^text, int offset, int length, ConsoleColor ^color)
{
    if (row >= 0 && row < mRows)
    {
        if (column < 0)
        {
            length += column;
            offset -= column;
            column = 0;
        }

        if (offset >= text->Length)
            return ;

        if (length > text->Length - offset)
            length = text->Length - offset;

        if (length <= 0)
            return ;

        int limit = column + length;
        if (limit > mColumns)
            limit = mColumns;

        int offset1 = row * mColumns + column;
        CHAR_INFO *bbase = mData + offset1;
        int *fcbase = mForegroundColor + offset1;
        int *bcbase = mBackgroundColor + offset1;
        int *sbase = mStyle + offset1;
        short pcolor = color->PaletteColor;
        bool rgbvalid = color->RGBValid;
        int fg = color->RGBForeground;
        int bg = color->RGBBackground;
        int s = color->Style;

        int tbase = offset;

        for (int i = column; i < limit; i++, bbase++, tbase++, fcbase++, bcbase++, sbase++)
        {
            bbase->Char.UnicodeChar = text[tbase];
            bbase->Attributes = pcolor;
            if (rgbvalid)
            {
                *fcbase = fg;
                *bcbase = bg;
                *sbase = s;
            }
            else
            {
                *fcbase = -1;
                *bcbase = -1;
                *sbase = 0;
            }
        }
    }
}

void Canvas::fill(int row, int column, int rows, int columns, wchar_t chr)
{
    if (row < 0)
    {
        rows += row;
        row = 0;
        return ;
    }
    if (column < 0)
    {
        columns += column;
        column = 0;
        return ;
    }

    if (row >= mRows)
        return ;
    if (rows <= 0 || columns <= 0)
        return ;

    int rowLimit = row + rows;
    if (rowLimit > mRows)
        rowLimit = mRows;

    int colLimit = column + columns;
    if (colLimit >= mColumns)
        colLimit = mColumns;
    int i, j;
    for (i = row; i < rowLimit; i++)
    {
        CHAR_INFO *bbase = mData + i * mColumns + column;
        for (j = column; j < colLimit; j++, bbase++)
        {
            bbase->Char.UnicodeChar = chr;
        }
    }

}

void Canvas::fill(int row, int column, int rows, int columns, ConsoleColor ^color)
{
    if (row < 0)
    {
        rows += row;
        row = 0;
        return ;
    }
    if (column < 0)
    {
        columns += column;
        column = 0;
        return ;
    }

    if (row >= mRows)
        return ;
    if (rows <= 0 || columns <= 0)
        return ;
    int rowLimit = row + rows;
    if (rowLimit > mRows)
        rowLimit = mRows;

    int colLimit = column + columns;
    if (colLimit >= mColumns)
        colLimit = mColumns;
    int i, j;
    short pcolor = color->PaletteColor;
    bool rgbvalid = color->RGBValid;
    int fg = color->RGBForeground;
    int bg = color->RGBBackground;
    int s = color->Style;
    for (i = row; i < rowLimit; i++)
    {
        int offset = i * mColumns + column;
        CHAR_INFO *bbase = mData + offset;
        int *fcbase = mForegroundColor + offset;
        int *bcbase = mBackgroundColor + offset;
        int *sbase = mStyle + offset;
        for (j = column; j < colLimit; j++, bbase++, fcbase++, bcbase++, sbase++)
        {
            bbase->Attributes = pcolor;
            if (rgbvalid)
            {
                *fcbase = fg;
                *bcbase = bg;
                *sbase = s;
            }
            else
            {
                *fcbase = -1;
                *bcbase = -1;
                *sbase = 0;
            }
        }
    }

}

void Canvas::fillFg(int row, int column, int rows, int columns, ConsoleColor ^color)
{
    if (row < 0)
    {
        rows += row;
        row = 0;
        return ;
    }
    if (column < 0)
    {
        columns += column;
        column = 0;
        return ;
    }

    if (row >= mRows)
        return ;
    if (rows <= 0 || columns <= 0)
        return ;
    int rowLimit = row + rows;
    if (rowLimit > mRows)
        rowLimit = mRows;

    int colLimit = column + columns;
    if (colLimit >= mColumns)
        colLimit = mColumns;
    int i, j;
    short pcolor = color->PaletteColor & 0xf;
    bool rgbvalid = color->RGBValid;
    int fg = color->RGBForeground;
    int s = color->Style;

    for (i = row; i < rowLimit; i++)
    {
        int offset = i * mColumns + column;
        CHAR_INFO *bbase = mData + offset;
        int *fcbase = mForegroundColor + offset;
        int *sbase = mStyle + offset;
        for (j = column; j < colLimit; j++, bbase++, fcbase++, sbase++)
        {

            bbase->Attributes = (bbase->Attributes & 0xf0) | pcolor;
            if (rgbvalid)
            {
                *fcbase = fg;
                *sbase = s;
            }
            else
            {
                *fcbase = -1;
                *sbase = 0;
            }
        }
    }
}

void Canvas::fillBg(int row, int column, int rows, int columns, ConsoleColor ^color)
{
    if (row < 0)
    {
        rows += row;
        row = 0;
        return ;
    }
    if (column < 0)
    {
        columns += column;
        column = 0;
        return ;
    }

    if (row >= mRows)
        return ;
    if (rows <= 0 || columns <= 0)
        return ;
    int rowLimit = row + rows;
    if (rowLimit > mRows)
        rowLimit = mRows;

    int colLimit = column + columns;
    if (colLimit >= mColumns)
        colLimit = mColumns;
    int i, j;
    short pcolor = color->PaletteColor & 0xf0;
    bool rgbvalid = color->RGBValid;
    int fg = color->RGBBackground;

    for (i = row; i < rowLimit; i++)
    {
        int offset = i * mColumns + column;
        CHAR_INFO *bbase = mData + offset;
        int *fcbase = mBackgroundColor + offset;
        for (j = column; j < colLimit; j++, bbase++, fcbase++)
        {

            bbase->Attributes = (bbase->Attributes & 0x0f) | pcolor;
            if (rgbvalid)
            {
                *fcbase = fg;
            }
            else
            {
                *fcbase = -1;
            }
        }
    }
}

void Canvas::fill(int row, int column, int rows, int columns, wchar_t chr, ConsoleColor ^color)
{
    if (row < 0)
    {
        rows += row;
        row = 0;
        return ;
    }
    if (column < 0)
    {
        columns += column;
        column = 0;
        return ;
    }

    if (row >= mRows)
        return ;
    if (rows <= 0 || columns <= 0)
        return ;
    int rowLimit = row + rows;
    if (rowLimit > mRows)
        rowLimit = mRows;

    int colLimit = column + columns;
    if (colLimit >= mColumns)
        colLimit = mColumns;
    int i, j;
    short pcolor = color->PaletteColor;
    bool rgbvalid = color->RGBValid;
    int fg = color->RGBForeground;
    int bg = color->RGBBackground;
    int s = color->Style;
    for (i = row; i < rowLimit; i++)
    {
        int offset = i * mColumns + column;
        CHAR_INFO *bbase = mData + offset;
        int *fcbase = mForegroundColor + offset;
        int *bcbase = mBackgroundColor + offset;
        int *sbase = mStyle + offset;
        for (j = column; j < colLimit; j++, bbase++, fcbase++, bcbase++, sbase++)
        {
            bbase->Char.UnicodeChar = chr;
            bbase->Attributes = pcolor;
            if (rgbvalid)
            {
                *fcbase = fg;
                *bcbase = bg;
                *sbase = s;
            }
            else
            {
                *fcbase = -1;
                *bcbase = -1;
                *sbase = 0;
            }
        }
    }

}

void Canvas::box(int row, int column, int rows, int columns, BoxBorder ^border, ConsoleColor ^color)
{
    if (rows >= 2 && columns >= 2)
    {
        //borders
        fill(row, column + 1, 1, columns - 2, border->Top, color);
        fill(row + rows - 1, column + 1, 1, columns - 2, border->Bottom, color);
        fill(row + 1, column, rows - 2, 1, border->Left, color);
        fill(row + 1, column + columns - 1, rows - 2, 1, border->Right, color);
        //edges
        write(row, column, border->TopLeft, color);
        write(row, column + columns - 1, border->TopRight, color);
        write(row + rows - 1, column, border->BottomLeft, color);
        write(row + rows - 1, column + columns - 1, border->BottomRight, color);
    }
}

void Canvas::box(int row, int column, int rows, int columns, BoxBorder ^border, ConsoleColor ^color, wchar_t fillchar)
{
    box(row, column, rows, columns, border, color);
    if (rows > 2 && columns > 2)
        fill(row + 1, column + 1, rows - 2, columns - 2, fillchar, color);
}

void Canvas::paint(int row, int column, Canvas ^canvas)
{
    int rows = canvas->Rows;
    int columns = canvas->Columns;
    int columnsOrg = columns;
    int srcRow = 0;
    int srcColumn = 0;

    if (row < 0)
    {
        rows += row;
        srcRow += -row;
        row = 0;
    }

    if (column < 0)
    {
        columns += column;
        srcColumn += -column;
        column = 0;
    }

    if (row >= mRows)
        return ;

    if (columns > mColumns - column)
        columns = mColumns - column;

    if (rows > mRows - row)
        rows = mRows - row;

    if (rows <= 0 || columns <= 0)
        return ;

    CHAR_INFO *buffer = canvas->Data;
    int *fg = canvas->ForegroundColor;
    int *bg = canvas->BackgroundColor;
    int *s = canvas->Style;

    int i, j;

    for (i = row, j = 0; j < rows; i++, j++)
    {
        int offset1 = i * mColumns + column;
        int offset2 = (j + srcRow) * columnsOrg + srcColumn;
        int len2 = sizeof(int) * columns;
        memcpy(mData + offset1, buffer + offset2, sizeof(CHAR_INFO) * columns);
        memcpy(mForegroundColor + offset1, fg + offset2, len2);
        memcpy(mBackgroundColor + offset1, bg + offset2, len2);
        memcpy(mStyle + offset1, s + offset2, len2);
    }
}

bool Canvas::get(int row, int column, [Out]wchar_t %chr, ConsoleColor ^color)
{
    if (row >= 0 && row < mRows && column >= 0 && column < mColumns)
    {
        int offset = row * mColumns + column;
        CHAR_INFO *t = mData + offset;
        chr = t->Char.UnicodeChar;
        color->PaletteColor = t->Attributes;
        color->RGBForeground = *(mForegroundColor + offset);
        color->RGBBackground = *(mBackgroundColor + offset);
        color->Style = *(mStyle + offset);
        return true;
    }
    else
        return false;

}

}
}
}
