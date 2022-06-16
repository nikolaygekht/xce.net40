// Scintilla source code edit control
/** @file CellBuffer.h
 ** Manages the text of the document.
 **/
// Copyright 1998-2004 by Neil Hodgson <neilh@scintilla.org>
// The License.txt file describes the conditions under which this software may be distributed.

#ifndef CELLBUFFER_H
#define CELLBUFFER_H

#define PerLineAction_Init  0
#define PerLineAction_Insert  1
#define PerLineAction_Delete  2

/**
 * The line vector contains information about each of the lines in a cell buffer.
 */
class LineVector {
    Partitioning starts;
    int markers[16];
public:

    LineVector();
    ~LineVector();
    void Init();

    void InsertText(int line, int delta);
    void InsertLine(int line, int position, bool lineStart);
    void SetLineStart(int line, int position);
    void RemoveLine(int line);

    int Lines() const {
        return starts.Partitions();
    }
    int LineFromPosition(int pos) const;
    int LineStart(int line) const {
        return starts.PositionFromPartition(line);
    }

    int GetMarker(int marker);
    void SetMarker(int marker, int line);
};

enum actionType { insertAction, removeAction, startAction, containerAction };

/**
 * Actions are used to store all the information required to perform one undo/redo step.
 */
class Action {
public:
    actionType at;
    int position;
    wchar_t *data;
    int lenData;
    bool mayCoalesce;

    Action();
    ~Action();
    void Create(actionType at_, int position_=0, wchar_t *data_=0, int lenData_=0, bool mayCoalesce_=true);
    void Destroy();
    void Grab(Action *source);
};

/**
 *
 */
class UndoHistory {
    Action *actions;
    int lenActions;
    int maxAction;
    int currentAction;
    int undoSequenceDepth;
    int savePoint;

    void EnsureUndoRoom();

public:
    UndoHistory();
    ~UndoHistory();

    void AppendAction(actionType at, int position, wchar_t *data, int length, bool &startSequence, bool mayCoalesce=true);

    void BeginUndoAction();
    void EndUndoAction();
    void DropUndoSequence();
    void DeleteUndoHistory();

    /// The save point is a marker in the undo stack where the container has stated that
    /// the buffer was saved. Undo and redo can move over the save point.
    void SetSavePoint();
    bool IsSavePoint() const;

    /// To perform an undo, StartUndo is called to retrieve the number of steps, then UndoStep is
    /// called that many times. Similarly for redo.
    bool CanUndo() const;
    int StartUndo();
    const Action &GetUndoStep() const;
    void CompletedUndoStep();
    bool CanRedo() const;
    int StartRedo();
    const Action &GetRedoStep() const;
    void CompletedRedoStep();
};

/**
 * Holder for an expandable array of characters that supports undo and line markers.
 * Based on article "Data Structures in a Bit-Mapped Text Editor"
 * by Wilfred J. Hansen, Byte January 1987, page 183.
 */
class CellBuffer {
private:
    SplitVector<wchar_t> substance;
    bool readOnly;
    int codepage;

    bool collectingUndo;
    UndoHistory uh;

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
    const wchar_t *InsertString(int position, const wchar_t *s, int insertLength, bool &startSequence);
    const wchar_t *InsertChar(int position, wchar_t c, int insertLength, bool &startSequence);

    const wchar_t *DeleteChars(int position, int deleteLength, bool &startSequence);
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
    void AddUndoAction(int token, bool mayCoalesce);
    void DeleteUndoHistory();

    /// To perform an undo, StartUndo is called to retrieve the number of steps, then UndoStep is
    /// called that many times. Similarly for redo.
    bool CanUndo();
    int StartUndo();
    const Action &GetUndoStep() const;
    int PerformUndoStep();
    bool CanRedo();
    int StartRedo();
    const Action &GetRedoStep() const;
    int PerformRedoStep();

    void Load(unsigned char *buffer, int codepage);
    int GetSaveLength();
    void Save(unsigned char *);

};

typedef int(_stdcall *estring_length_ptr)(void *pdata);
typedef wchar_t(_stdcall *estring_char_ptr)(void *pdata, int index);

int _stdcall CellBufferLengthAccessor(void *pdata);
wchar_t _stdcall CellBufferCharAccessor(void *pdata, int index);


#endif
