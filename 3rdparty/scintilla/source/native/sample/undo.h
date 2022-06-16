#ifndef __UndoH
#define __UndoH

class IWindow;
#include "fileio.h"

/** Kind of undo record. */
typedef enum
{
    eUndoReplaceLine = 0,       //0 line #, old content, new content
    eUndoInsertLine,            //1 line #, new content
    eUndoInsertSubstring,       //2 line #, col#, next content
    eUndoRemoveLine,            //3 line #, old content
    eUndoRemoveSubstring,       //4 line #, pos#, lenght, old content
    eUndoBeginTransaction,      //5 ---
    eUndoEndTransaction,        //6 window state
}UndoType;



class CUndo
{
 public:
    /** constructor. */
    CUndo(IWindow *pWindow);

    /** Destructor. */
    ~CUndo();

    /** Register begin transaction. */
    void beginTransaction(bool bNoStart = false);

    /** Register end transaction. */
    void endTransaction(bool bNoStop = false);

    /** Register replace line. */
    void replaceLine(int lLine, const char *szNewContent);

    /** Register remove line. */
    void removeLine(int lLine);

    /** Register append line. */
    void appendLine(const char *szNewContent);

    /** Register insert line. */
    void insertLine(int lLine, const char *szNewContent);

    /** Register insert substring. */
    void insertSubstring(int lLine, int lPos, const char *szNewContent);

    /** Register remove substring. */
    void removeSubstring(int lLine, int lPos, int lLen);

    /** do undo. */
    void undo();

    /** do redo. */
    void redo();

    /** clear all undo. */
    void clear();

    /** Register save. */
    void registerSave();

    /** Clear save registration record. */
    void clearSave();

 protected:
    std::vector<long> m_aUndoPos;           //!< positions of undo records
    long m_lPos;                            //!< position in undo list
    long m_lSave;                           //!< position of the save operation
    CFileIO m_cIO;                          //!< file i/o object for undo
    IWindow *m_pWindow;                     //!< reference to window object

    void _cdecl log(const char *szFrmt, ...);

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

    void getState(WindowState *pState);
    void setState(WindowState *pState);
    void readState(WindowState *pState);
    void writeState(WindowState *pState);
    bool compareState(WindowState *pState);
};

#endif
