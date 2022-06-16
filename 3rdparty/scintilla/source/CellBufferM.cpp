#pragma unmanaged
#include <windows.h>
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <stdarg.h>
#include <vector>
#include "native/SplitVector.h"
#include "native/Partitioning.h"
#include "native/LineVector.h"
#include "native/File.h"
#include "native/Undo.h"
#include "native/CellBuffer.h"

#pragma managed
#using <mscorlib.dll>
using namespace System;
using namespace System::Runtime::InteropServices;
#include "CellBufferM.h"

namespace gehtsoft
{
namespace xce
{
namespace scintilla
{


CellBuffer::CellBuffer()
{
    mCellBuffer = new ::CellBuffer();
    mLastChange = 0;
}

CellBuffer::~CellBuffer()
{
    this->!CellBuffer();
}

CellBuffer::!CellBuffer()
{
    if (mCellBuffer != 0)
        delete mCellBuffer;
    mCellBuffer = 0;
}

wchar_t CellBuffer::default::get(int position)
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    if (position < 0 || position >= mCellBuffer->Length())
        throw gcnew ArgumentOutOfRangeException("position");
    return mCellBuffer->CharAt(position);
};

wchar_t CellBuffer::default::get(int line, int position)
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    int start, length;
    start = LineStart(line);
    length = LineStart(line);
    if (position < 0 || position >= length)
        throw gcnew ArgumentOutOfRangeException("position");
    return this[start + position];
};

int CellBuffer::CharCount::get()
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    return mCellBuffer->Length();

}

int CellBuffer::LinesCount::get()
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    return mCellBuffer->Lines();
}

int CellBuffer::GetRange(int position, int length, cli::array<wchar_t >^ buffer, int offset)
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    if (buffer == nullptr)
        throw gcnew ArgumentNullException("buffer");
    if (position < 0 || position >= mCellBuffer->Length())
        throw gcnew ArgumentOutOfRangeException("position");
    if (length < 0 || position + length > mCellBuffer->Length())
        throw gcnew ArgumentOutOfRangeException("length");
    if (offset < 0 || offset + length > buffer->Length)
        throw gcnew ArgumentOutOfRangeException("offset");
    GCHandle ^pinned = GCHandle::Alloc(buffer, GCHandleType::Pinned);
    IntPtr src =  Marshal::UnsafeAddrOfPinnedArrayElement(buffer, offset);
    int rc = mCellBuffer->GetCharRange((wchar_t *)(src.ToPointer()), position, length);
    pinned->Free();
    return rc;
}

String ^CellBuffer::GetRange(int position, int length)
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    if (length < 0)
        throw gcnew ArgumentOutOfRangeException("length");
    if (length == 0)
        return L"";
    cli::array<wchar_t > ^buff = gcnew cli::array<wchar_t >(length);
    length = GetRange(position, length, buff, 0);
    return gcnew String(buff, 0, length);
}

int CellBuffer::LineStart(int line)
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    if (line < 0 || line >= mCellBuffer->Lines())
        throw gcnew ArgumentOutOfRangeException("line");
    return mCellBuffer->LineStart(line);
}

int CellBuffer::LineLength(int line)
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    if (line < 0 || line >= mCellBuffer->Lines())
        throw gcnew ArgumentOutOfRangeException("line");
    return mCellBuffer->LineStart(line + 1) - mCellBuffer->LineStart(line);
}

int CellBuffer::LineFromPosition(int position)
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    if (position < 0 || position > mCellBuffer->Length())
        throw gcnew ArgumentOutOfRangeException("position");
    return mCellBuffer->LineFromPosition(position);
}

void CellBuffer::InsertChar(int position, wchar_t c, int length)
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    if (position < 0 || position > mCellBuffer->Length())
         throw gcnew ArgumentOutOfRangeException("position");
    mCellBuffer->InsertChar(position, c, length);
    mLastChange = DateTime::Now.Ticks;
}

void CellBuffer::InsertString(int position, cli::array<wchar_t >^ buffer, int offset, int length)
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    if (buffer == nullptr)
        throw gcnew ArgumentNullException("buffer");
    if (offset < 0 || offset >= buffer->Length)
        throw gcnew ArgumentOutOfRangeException("offset");
    if (length < 0 || offset + length > buffer->Length)
        throw gcnew ArgumentOutOfRangeException("length");
    if (position < 0 || position > mCellBuffer->Length())
         throw gcnew ArgumentOutOfRangeException("position");

    if (buffer->Length == 0)
        return ;
    GCHandle ^pinned = GCHandle::Alloc(buffer, GCHandleType::Pinned);
    IntPtr src =  Marshal::UnsafeAddrOfPinnedArrayElement(buffer, offset);
    mCellBuffer->InsertString(position, (const wchar_t *)(src.ToPointer()), length);
    pinned->Free();
    mLastChange = DateTime::Now.Ticks;
}

void CellBuffer::InsertString(int position, String ^buffer, int offset, int length)
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    if (buffer == nullptr)
        throw gcnew ArgumentNullException("buffer");
    InsertString(position, buffer->ToCharArray(), offset, length);
}

void CellBuffer::DeleteChars(int position, int deleteLength)
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    if (position < 0 || position >= mCellBuffer->Length())
        throw gcnew ArgumentOutOfRangeException("position");
    if (deleteLength < 0 || position + deleteLength > mCellBuffer->Length())
        throw gcnew ArgumentOutOfRangeException("deleteLength");
    mCellBuffer->DeleteChars(position, deleteLength);
    mLastChange = DateTime::Now.Ticks;
}

int CellBuffer::GetMarker(int marker)
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    return mCellBuffer->GetMarker(marker);
}

void CellBuffer::SetMarker(int marker, int line)
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    mCellBuffer->SetMarker(marker, line);
}

bool CellBuffer::IsReadOnly()
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    return mCellBuffer->IsReadOnly();
}

void CellBuffer::SetReadOnly(bool set)
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    mCellBuffer->SetReadOnly(set);
}

/// The save point is a marker in the undo stack where the container has stated that
/// the buffer was saved. Undo and redo can move over the save point.
void CellBuffer::SetSavePoint()
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    mCellBuffer->SetSavePoint();
}

bool CellBuffer::IsSavePoint()
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    return mCellBuffer->IsSavePoint();
}

bool CellBuffer::SetUndoCollection(bool collectUndo)
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    return mCellBuffer->SetUndoCollection(collectUndo);
}

bool CellBuffer::IsCollectingUndo()
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    return mCellBuffer->IsCollectingUndo();
}

void CellBuffer::BeginUndoAction()
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    mCellBuffer->BeginUndoAction();
}

void CellBuffer::EndUndoAction()
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    mCellBuffer->EndUndoAction();
}

void CellBuffer::DeleteUndoHistory()
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    mCellBuffer->DeleteUndoHistory();
}

bool CellBuffer::CanUndo()
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    return mCellBuffer->CanUndo();
}

int CellBuffer::Undo()
{
    int rc = -1;
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    if (mCellBuffer->CanUndo())
    {
        rc = mCellBuffer->Undo();
    }
    mLastChange = DateTime::Now.Ticks;
    return rc;
}

bool CellBuffer::CanRedo()
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    return mCellBuffer->CanRedo();
}

int CellBuffer::Redo()
{
    int rc = -1;
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    if (mCellBuffer->CanRedo())
    {
        rc = mCellBuffer->Redo();
    }
    mLastChange = DateTime::Now.Ticks;
    return rc;
}

int CellBuffer::GetCP()
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    return mCellBuffer->GetCodePage();
}

bool CellBuffer::SetCP(int codepage)
{
    if (mCellBuffer == 0)
        throw gcnew InvalidOperationException();
    bool rc = mCellBuffer->SetCodePage(codepage);
    mLastChange = DateTime::Now.Ticks;
    return rc;
}

void CellBuffer::GetNativeAccessor([OutAttribute]IntPtr %data, [OutAttribute]IntPtr %lengthacc, [OutAttribute] IntPtr %characc)
{
    data = IntPtr((void *)mCellBuffer);
    lengthacc = IntPtr((void *)(estring_length_ptr)CellBufferLengthAccessor);
    characc = IntPtr((void *)(estring_char_ptr)CellBufferCharAccessor);
    return ;
}

long long CellBuffer::LastChange::get()
{
    return mLastChange;
}

int CellBuffer::GetState(int index)
{
    if (index < 0 || index > 15)
        throw gcnew ArgumentOutOfRangeException("index");
    return mCellBuffer->GetState()[index];
}
void CellBuffer::SetState(int index, int value)
{
    if (index < 0 || index > 15)
        throw gcnew ArgumentOutOfRangeException("index");
    mCellBuffer->GetState()[index] = value;
}

}
}
}

