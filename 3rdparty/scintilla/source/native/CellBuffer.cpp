// Scintilla source code edit control
/** @file CellBuffer.cxx
 ** Manages a buffer of cells.
 **/
// Copyright 1998-2001 by Neil Hodgson <neilh@scintilla.org>
// The License.txt file describes the conditions under which this software may be distributed.

#pragma unmanaged

#include <windows.h>
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <stdarg.h>
#include <vector>

#include "SplitVector.h"
#include "Partitioning.h"
#include "LineVector.h"
#include "file.h"
#include "Undo.h"
#include "CellBuffer.h"

CellBuffer::CellBuffer() {
    readOnly = false;
    collectingUndo = true;
    codepage = 437;
    uh = new CUndo(this);
    memset(mState, 0, sizeof(int) * 16);
}

CellBuffer::~CellBuffer() {
    delete uh;
}

wchar_t CellBuffer::CharAt(int position) const {
    return substance.ValueAt(position);
}

int CellBuffer::GetCharRange(wchar_t *buffer, int position, int lengthRetrieve) const {
    if (lengthRetrieve < 0)
        return 0;
    if (position < 0)
        return 0;
    if (position >= substance.Length())
        return 0;
    if ((position + lengthRetrieve) > substance.Length()) {
        lengthRetrieve = substance.Length() - position;
    }
    substance.GetRange(buffer, position, lengthRetrieve);
    return lengthRetrieve;
}

const wchar_t *CellBuffer::BufferPointer() {
    return substance.BufferPointer();
}

// The char* returned is to an allocation owned by the undo history
const wchar_t *CellBuffer::InsertString(int position, const wchar_t *s, int insertLength) {
    wchar_t *data = 0;
    // InsertString and DeleteChars are the bottleneck though which all changes occur
    if (!readOnly) {
        if (collectingUndo) {
            uh->insertText(position, insertLength, s);
        }
        BasicInsertString(position, s, insertLength);
    }
    return data;
}

// The char* returned is to an allocation owned by the undo history
const wchar_t *CellBuffer::InsertStringNoUndo(int position, const wchar_t *s, int insertLength) {
    wchar_t *data = 0;
    // InsertString and DeleteChars are the bottleneck though which all changes occur
    if (!readOnly) {
        BasicInsertString(position, s, insertLength);
    }
    return data;
}


const wchar_t *CellBuffer::InsertChar(int position, wchar_t c, int insertLength)
{
    wchar_t *s;
    const wchar_t *rc = 0;

    if (insertLength > 0)
    {
        s = new wchar_t[insertLength];
        for (int i = 0; i < insertLength; i++)
            s[i] = c;
        rc = InsertString(position, s, insertLength);
        delete[] s;
    }
    return rc;
}

// The char* returned is to an allocation owned by the undo history
const wchar_t *CellBuffer::DeleteChars(int position, int deleteLength) {
    // InsertString and DeleteChars are the bottleneck though which all changes occur
    wchar_t *data = 0;
    if (!readOnly) {
        if (collectingUndo) {
            // Save into the undo/redo stack, but only the characters - not the formatting
            data = new wchar_t[deleteLength];
            for (int i = 0; i < deleteLength; i++) {
                data[i] = substance.ValueAt(position + i);
            }
            uh->deleteText(position, deleteLength, data);
        }
        BasicDeleteChars(position, deleteLength);
    }
    return data;
}

// The char* returned is to an allocation owned by the undo history
const wchar_t *CellBuffer::DeleteCharsNoUndo(int position, int deleteLength) {
    // InsertString and DeleteChars are the bottleneck though which all changes occur
    wchar_t *data = 0;
    if (!readOnly) {
        BasicDeleteChars(position, deleteLength);
    }
    return data;
}

int CellBuffer::Length() const {
    return substance.Length();
}

void CellBuffer::Allocate(int newSize) {
    substance.ReAllocate(newSize);
}

int CellBuffer::Lines() const {
    return lv.Lines();
}

int CellBuffer::LineStart(int line) const {
    if (line < 0)
        return 0;
    else if (line >= Lines())
        return Length();
    else
        return lv.LineStart(line);
}

bool CellBuffer::IsReadOnly() const {
    return readOnly;
}

void CellBuffer::SetReadOnly(bool set) {
    readOnly = set;
}

void CellBuffer::SetSavePoint() {
    uh->SetSavePoint();
}

bool CellBuffer::IsSavePoint() {
    return uh->IsSavePoint();
}

// Without undo
void CellBuffer::InsertLine(int line, int position, bool lineStart) {
    lv.InsertLine(line, position, lineStart);
}

void CellBuffer::RemoveLine(int line) {
    lv.RemoveLine(line);
}

void CellBuffer::BasicInsertString(int position, const wchar_t *s, int insertLength) {
    if (insertLength == 0)
        return;

    substance.InsertFromArray(position, s, 0, insertLength);

    int lineInsert = lv.LineFromPosition(position) + 1;
    bool atLineStart = lv.LineStart(lineInsert-1) == position;
    // Point all the lines after the insertion point further along in the buffer
    lv.InsertText(lineInsert-1, insertLength);
    wchar_t chPrev = substance.ValueAt(position - 1);
    wchar_t chAfter = substance.ValueAt(position + insertLength);
    if (chPrev == '\r' && chAfter == '\n') {
        // Splitting up a crlf pair at position
        InsertLine(lineInsert, position, false);
        lineInsert++;
    }
    wchar_t ch = ' ';
    for (int i = 0; i < insertLength; i++) {
        ch = s[i];
        if (ch == '\r') {
            InsertLine(lineInsert, (position + i) + 1, atLineStart);
            lineInsert++;
        } else if (ch == '\n') {
            if (chPrev == '\r') {
                // Patch up what was end of line
                lv.SetLineStart(lineInsert - 1, (position + i) + 1);
            } else {
                InsertLine(lineInsert, (position + i) + 1, atLineStart);
                lineInsert++;
            }
        }
        chPrev = ch;
    }
    // Joining two lines where last insertion is cr and following substance starts with lf
    if (chAfter == '\n') {
        if (ch == '\r') {
            // End of line already in buffer so drop the newly created one
            RemoveLine(lineInsert - 1);
        }
    }
}

void CellBuffer::BasicDeleteChars(int position, int deleteLength) {
    if (deleteLength == 0)
        return;

    if ((position == 0) && (deleteLength == substance.Length())) {
        // If whole buffer is being deleted, faster to reinitialise lines data
        // than to delete each line.
        lv.Init();
    } else {
        // Have to fix up line positions before doing deletion as looking at text in buffer
        // to work out which lines have been removed

        int lineRemove = lv.LineFromPosition(position) + 1;
        lv.InsertText(lineRemove-1, - (deleteLength));
        wchar_t chPrev = substance.ValueAt(position - 1);
        wchar_t chBefore = chPrev;
        wchar_t chNext = substance.ValueAt(position);
        bool ignoreNL = false;
        if (chPrev == '\r' && chNext == '\n') {
            // Move back one
            lv.SetLineStart(lineRemove, position);
            lineRemove++;
            ignoreNL = true;    // First \n is not real deletion
        }

        wchar_t ch = chNext;
        for (int i = 0; i < deleteLength; i++) {
            chNext = substance.ValueAt(position + i + 1);
            if (ch == '\r') {
                if (chNext != '\n') {
                    RemoveLine(lineRemove);
                }
            } else if (ch == '\n') {
                if (ignoreNL) {
                    ignoreNL = false;   // Further \n are real deletions
                } else {
                    RemoveLine(lineRemove);
                }
            }

            ch = chNext;
        }
        // May have to fix up end if last deletion causes cr to be next to lf
        // or removes one of a crlf pair
        wchar_t chAfter = substance.ValueAt(position + deleteLength);
        if (chBefore == '\r' && chAfter == '\n') {
            // Using lineRemove-1 as cr ended line before start of deletion
            RemoveLine(lineRemove - 1);
            lv.SetLineStart(lineRemove - 1, position + 1);
        }
    }
    substance.DeleteRange(position, deleteLength);
}

bool CellBuffer::SetUndoCollection(bool collectUndo) {
    collectingUndo = collectUndo;
    uh->clear();
    return collectingUndo;
}

bool CellBuffer::IsCollectingUndo() const {
    return collectingUndo;
}

void CellBuffer::BeginUndoAction() {
    uh->beginTransaction();
}

void CellBuffer::EndUndoAction() {
    uh->endTransaction();
}

void CellBuffer::DeleteUndoHistory() {
    uh->clear();
}

bool CellBuffer::CanUndo() {
    return uh->canUndo();
}

int CellBuffer::Undo() {
    return uh->undo();
}

bool CellBuffer::CanRedo() {
    return uh->canRedo();
}

int CellBuffer::Redo() {
    return uh->redo();
}

int CellBuffer::GetMarker(int marker)
{
    return lv.GetMarker(marker);
}
void CellBuffer::SetMarker(int marker, int line)
{
    lv.SetMarker(marker, line);
}

int CellBuffer::GetCodePage()
{
    return codepage;
}

bool CellBuffer::SetCodePage(int newcodepage)
{
    if (Length() == 0)
    {
        codepage = newcodepage;
        return false;
    }

    BeginUndoAction();
    bool rc = false;
    wchar_t *buff = 0;
    char *buff1 = 0;
    DWORD wc2mbFlags = WC_NO_BEST_FIT_CHARS;
    BOOL UsedDefaultChar = FALSE;
    LPBOOL lpUsedDefaultChar = &UsedDefaultChar;

    if (codepage == CP_UTF7 || codepage == CP_UTF8) // BUGBUG: CP_SYMBOL, 50xxx, 57xxx too
    {
        wc2mbFlags = 0;
        lpUsedDefaultChar = 0;
    }

    DWORD mb2wcFlags = MB_ERR_INVALID_CHARS;

    if (newcodepage == CP_UTF7) // BUGBUG: CP_SYMBOL, 50xxx, 57xxx too
    {
        mb2wcFlags = 0;
    }

    if (codepage != newcodepage && Length() > 0)
    {
        int cbuff = 4096;
        buff = new wchar_t[cbuff];
        int cbuff1 = 4096;
        buff1 = new char[cbuff1];
        int lc = Lines();
        for (int i = 0; i < Lines(); i++)
        {
            int p = LineStart(i);
            int l = LineStart(i + 1) - p;
            if (l > 0)
            {
                if (l > cbuff)
                {
                    delete[] buff;
                    cbuff = (l / 4096 + 1) * 4096;
                    buff = new wchar_t[cbuff];
                }
                GetCharRange(buff, p, l);


                int l1 = WideCharToMultiByte(codepage, wc2mbFlags, buff, l, 0, 0, 0, lpUsedDefaultChar);

                if (UsedDefaultChar)
                    rc |= true;

                if (l1 >= cbuff1)
                {
                    delete[] buff1;
                    cbuff1 = (l1 / 4096 + 1) * 4096;
                    buff1 = new char[cbuff1];
                }

                WideCharToMultiByte(codepage, 0, buff, l, buff1, l1, 0, 0);
                int l2 = MultiByteToWideChar(newcodepage, mb2wcFlags, buff1, l1, 0, 0);
                if (l2 == 0 && GetLastError() == ERROR_NO_UNICODE_TRANSLATION)
                {
                    rc |= true;
                    l2 = MultiByteToWideChar(newcodepage, 0, buff1, l1, 0, 0);
                }

                if (l2 > cbuff)
                {
                    delete[] buff;
                    cbuff = (l2 / 4096 + 1) * 4096;
                    buff = new wchar_t[cbuff];
                }

                int l3 = MultiByteToWideChar(newcodepage, 0, buff1, l1, buff, l2);
                DeleteChars(p, l);
                InsertString(p, buff, l3);
            }
        }
    }
    if (buff != 0)
        delete[] buff;
    if (buff1 != 0)
        delete[] buff1;
    codepage = newcodepage;

    EndUndoAction();
    return rc;
}

int _stdcall CellBufferLengthAccessor(void *pdata)
{
    CellBuffer *buff = (CellBuffer *)pdata;
    return buff->Length();
}

wchar_t _stdcall CellBufferCharAccessor(void *pdata, int index)
{
    CellBuffer *buff = (CellBuffer *)pdata;
    return buff->CharAt(index);
}

int *CellBuffer::GetState()
{
    return &mState[0];
}