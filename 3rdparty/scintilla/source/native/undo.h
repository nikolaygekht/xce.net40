/** Kind of undo record. */
typedef enum
{
    eUndoInsert = 0,            //  position, state, new content
    eUndoRemove,                //  position, state, old content
    eUndoBeginTransaction,      //
    eUndoEndTransaction,        //
}UndoType;

class CellBuffer;
class CFileIO;

class CUndo
{
 public:
    /** constructor. */
    CUndo(CellBuffer *buffer);

    /** Destructor. */
    ~CUndo();

    /** Register begin transaction. */
    void beginTransaction();

    /** Register end transaction. */
    void endTransaction();

    void insertText(int position, int length, const wchar_t *content);
    void deleteText(int position, int length, const wchar_t *content);

    bool canUndo();
    bool canRedo();
    void SetSavePoint();
    bool IsSavePoint();

    /** do undo. */
    int undo();

    /** do redo. */
    int redo();

    /** clear all undo. */
    void clear();

 protected:
    CellBuffer *mBuffer;                    //!< buffer
    std::vector<long> m_aUndoPos;           //!< positions of undo records
    long m_lPos;                            //!< position in undo list
    long m_lSave;                           //!< position of the save operation
    long m_lSavePoint;                      //
    CFileIO m_cIO;                          //!< file i/o object for undo

    typedef struct
    {
        long lBaseLine;
        long lBasePos;
        long lCursorPos;
        long lCursorLine;
        long lBlockType;
        long lBlockStartLine;
        long lBlockStartLinePos;
        long lBlockEndLine;
        long lBlockEndLinePos;
        long bChanged;
    }WindowState;

    void readState(bool restore = true);
    void writeState();

    bool mLastInsert;
    int mLastInsertPos;
    int mLastInsertLength;

};

