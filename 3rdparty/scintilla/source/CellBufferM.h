class CellBuffer;

namespace gehtsoft
{
namespace xce
{
namespace scintilla
{

public ref class CellBuffer
{
 private:
    ::CellBuffer *mCellBuffer;
    long long mLastChange;
 public:
    CellBuffer();
    ~CellBuffer();
    !CellBuffer();

    property wchar_t default[int]
    {
        wchar_t get(int position);
    };

    property wchar_t default[int, int]
    {
        wchar_t get(int line, int position);
    };

    property int CharCount
    {
        int get();
    }

    property int LinesCount
    {
        int get();
    }

    property long long LastChange
    {
        long long get();
    }

    int GetRange(int position, int length, cli::array<wchar_t >^ buffer, int offset);
    String ^GetRange(int position, int length);
    int LineStart(int line);
    int LineLength(int line);
    int LineFromPosition(int position);
    void InsertString(int position, cli::array<wchar_t >^ buffer, int offset, int length);
    void InsertString(int position, String ^buffer, int offset, int length);
    void InsertChar(int position, wchar_t c, int length);
    void DeleteChars(int position, int deleteLength);
    int GetMarker(int marker);
    void SetMarker(int marker, int line);
    bool IsReadOnly();
    void SetReadOnly(bool set);
    /// The save point is a marker in the undo stack where the container has stated that
    /// the buffer was saved. Undo and redo can move over the save point.
    void SetSavePoint();
    bool IsSavePoint();
    bool SetUndoCollection(bool collectUndo);
    bool IsCollectingUndo();
    void BeginUndoAction();
    void EndUndoAction();
    void DeleteUndoHistory();
    bool CanUndo();
    int Undo();
    bool CanRedo();
    int Redo();
    int GetCP();
    bool SetCP(int codepage);
    void GetNativeAccessor([OutAttribute]IntPtr %data, [OutAttribute]IntPtr %lengthacc, [OutAttribute] IntPtr %characc);
    int GetState(int index);
    void SetState(int index, int value);
};

}
}
}
