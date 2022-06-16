// Scintilla source code edit control
/** @file CellBuffer.h
 ** Manages the text of the document.
 **/
// Copyright 1998-2004 by Neil Hodgson <neilh@scintilla.org>
// The License.txt file describes the conditions under which this software may be distributed.

#ifndef CELLBUFFER_H
#define CELLBUFFER_H

/**
 * Holder for an expandable array of characters that supports undo and line markers.
 * Based on article "Data Structures in a Bit-Mapped Text Editor"
 * by Wilfred J. Hansen, Byte January 1987, page 183.
 */

class CUndo;

class CellBuffer {
private:
    SplitVector<wchar_t> substance;
    bool readOnly;
    int codepage;

    bool collectingUndo;
    CUndo *uh;
    int mState[16];

    LineVector lv;

    /// Actions without undo
    void BasicInsertString(int position, const wchar_t *s, int insertLength);
    void BasicDeleteChars(int position, int deleteLength);

public:
    CellBuffer();
    ~CellBuffer();

    int GetCodePage();
    bool SetCodePage(int codepage);

    /// Retrieving positions outside the range of the buffer works and returns 0
    wchar_t CharAt(int position) const;
    int GetCharRange(wchar_t *buffer, int position, int lengthRetrieve) const;
    const wchar_t *BufferPointer();

    int Length() const;
    void Allocate(int newSize);
    int Lines() const;
    int LineStart(int line) const;
    int LineFromPosition(int pos) const { return lv.LineFromPosition(pos); }
    void InsertLine(int line, int position, bool lineStart);
    void RemoveLine(int line);
    const wchar_t *InsertString(int position, const wchar_t *s, int insertLength);
    const wchar_t *InsertChar(int position, wchar_t c, int insertLength);
    const wchar_t *DeleteChars(int position, int deleteLength);
    const wchar_t *InsertStringNoUndo(int position, const wchar_t *s, int insertLength);
    const wchar_t *DeleteCharsNoUndo(int position, int deleteLength);
    int GetMarker(int marker);
    void SetMarker(int marker, int line);

    bool IsReadOnly() const;
    void SetReadOnly(bool set);

    /// The save point is a marker in the undo stack where the container has stated that
    /// the buffer was saved. Undo and redo can move over the save point.
    void SetSavePoint();
    bool IsSavePoint();

    bool SetUndoCollection(bool collectUndo);
    bool IsCollectingUndo() const;
    void BeginUndoAction();
    void EndUndoAction();
    void DeleteUndoHistory();

    /// To perform an undo, StartUndo is called to retrieve the number of steps, then UndoStep is
    /// called that many times. Similarly for redo.
    bool CanUndo();
    int Undo();
    bool CanRedo();
    int Redo();

    void Load(unsigned char *buffer, int codepage);
    int GetSaveLength();
    void Save(unsigned char *);
    int *GetState();
};

typedef int(_stdcall *estring_length_ptr)(void *pdata);
typedef wchar_t(_stdcall *estring_char_ptr)(void *pdata, int index);

int _stdcall CellBufferLengthAccessor(void *pdata);
wchar_t _stdcall CellBufferCharAccessor(void *pdata, int index);


#endif
