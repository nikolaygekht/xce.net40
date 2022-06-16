#include "stdhead.h"
#include "iwindow.h"
#include "window.h"
#include "undo.h"

/** constructor. */
CUndo::CUndo(IWindow *pWindow)
{
    m_pWindow = pWindow;
    m_cIO.openTemp();
    m_lPos = -1;
}

/** Destructor. */
CUndo::~CUndo()
{
    m_aUndoPos.clear();
    m_lPos = -1;
    m_cIO.close();
}

/** Register begin transaction. */
void CUndo::beginTransaction(bool bNoStart)
{
    if (!bNoStart)
        m_cIO.startBinCache();

    if ((m_lPos + 1) != m_aUndoPos.size())
        m_aUndoPos.erase(m_aUndoPos.begin() + m_lPos + 1, m_aUndoPos.end());
    m_lPos = m_aUndoPos.size();
    m_aUndoPos.push_back(m_cIO.getPos());
    m_cIO.writeInt(eUndoBeginTransaction);
    WindowState s;
    getState(&s);
    writeState(&s);
}

/** Register end transaction. */
void CUndo::endTransaction(bool bNoStop)
{
    bool bStop;
    if (!bNoStop)
        bStop = m_cIO.stopBinCache();

    if (m_lPos == -1)
        return ;

    if (bStop)
    {
        //check is previous record "begin transaction" or not
        long lKeepPos = m_cIO.getPos();
        long lPrevPos = m_aUndoPos.at(m_lPos);
        long lCmd;
        m_cIO.goPos(lPrevPos);
        m_cIO.readInt(&lCmd);
        if (lCmd == eUndoBeginTransaction)
        {
            m_cIO.goPos(lPrevPos);
            m_aUndoPos.erase(m_aUndoPos.begin() + m_lPos, m_aUndoPos.end());
            m_lPos--;
            return;
        }
        else
            m_cIO.goPos(lKeepPos);
    }

    if ((m_lPos + 1) != m_aUndoPos.size())
        m_aUndoPos.erase(m_aUndoPos.begin() + m_lPos + 1, m_aUndoPos.end());
    m_lPos = m_aUndoPos.size();
    m_aUndoPos.push_back(m_cIO.getPos());

    m_cIO.writeInt(eUndoEndTransaction);
    WindowState s;
    getState(&s);
    writeState(&s);
}

/** Register replace line. */
void CUndo::replaceLine(int lLine, const char *szNewContent)
{
    if ((m_lPos + 1) != m_aUndoPos.size())
        m_aUndoPos.erase(m_aUndoPos.begin() + m_lPos + 1, m_aUndoPos.end());
    m_lPos = m_aUndoPos.size();
    m_aUndoPos.push_back(m_cIO.getPos());
    m_cIO.writeInt(eUndoReplaceLine);
    WindowState s;
    getState(&s);
    writeState(&s);
    m_cIO.writeInt(lLine);
    m_cIO.writeAsciz(szNewContent);
    const char *szOldContent = m_pWindow->getLine(lLine);
    m_cIO.writeAsciz(szOldContent);
}

/** Register append line. */
void CUndo::appendLine(const char *szNewContent)
{
    if ((m_lPos + 1) != m_aUndoPos.size())
        m_aUndoPos.erase(m_aUndoPos.begin() + m_lPos + 1, m_aUndoPos.end());
    m_lPos = m_aUndoPos.size();
    m_aUndoPos.push_back(m_cIO.getPos());

    m_cIO.writeInt(eUndoInsertLine);
    WindowState s;
    getState(&s);
    writeState(&s);
    m_cIO.writeInt(m_pWindow->getLinesCount());
    m_cIO.writeAsciz(szNewContent);
}

/** Register remove line. */
void CUndo::removeLine(int lLine)
{
    if ((m_lPos + 1) != m_aUndoPos.size())
        m_aUndoPos.erase(m_aUndoPos.begin() + m_lPos + 1, m_aUndoPos.end());
    m_lPos = m_aUndoPos.size();
    m_aUndoPos.push_back(m_cIO.getPos());

    m_cIO.writeInt(eUndoRemoveLine);
    WindowState s;
    getState(&s);
    writeState(&s);
    m_cIO.writeInt(lLine);
    m_cIO.writeAsciz(m_pWindow->getLine(lLine));
}



/** Register insert line. */
void CUndo::insertLine(int lLine, const char *szNewContent)
{
    if ((m_lPos + 1) != m_aUndoPos.size())
        m_aUndoPos.erase(m_aUndoPos.begin() + m_lPos + 1, m_aUndoPos.end());

    m_lPos = m_aUndoPos.size();
    m_aUndoPos.push_back(m_cIO.getPos());

    m_cIO.writeInt(eUndoInsertLine);
    WindowState s;
    getState(&s);
    writeState(&s);
    m_cIO.writeInt(lLine);
    m_cIO.writeAsciz(szNewContent);
}

/** Register insert substring. */
void CUndo::insertSubstring(int lLine, int lPos, const char *szNewContent)
{
    if ((m_lPos + 1) != m_aUndoPos.size())
        m_aUndoPos.erase(m_aUndoPos.begin() + m_lPos + 1, m_aUndoPos.end());
    m_lPos = m_aUndoPos.size();
    m_aUndoPos.push_back(m_cIO.getPos());

    m_cIO.writeInt(eUndoInsertSubstring);
    WindowState s;
    getState(&s);
    writeState(&s);
    m_cIO.writeInt(lLine);
    m_cIO.writeInt(lPos);
    m_cIO.writeInt(strlen(szNewContent));
    m_cIO.writeAsciz(szNewContent);

}

/** Register remove substring. */
void CUndo::removeSubstring(int lLine, int lPos, int lLen)
{
    if ((m_lPos + 1) != m_aUndoPos.size())
        m_aUndoPos.erase(m_aUndoPos.begin() + m_lPos + 1, m_aUndoPos.end());
    m_lPos = m_aUndoPos.size();
    m_aUndoPos.push_back(m_cIO.getPos());

    m_cIO.writeInt(eUndoRemoveSubstring);
    WindowState s;
    getState(&s);
    writeState(&s);
    m_cIO.writeInt(lLine);
    m_cIO.writeInt(lPos);
    m_cIO.writeInt(lLen);
    const char *szLine = m_pWindow->getLine(lLine);
    m_cIO.writeAsciz(szLine);
}

/** do undo. */
void CUndo::undo()
{
    WindowState w;
    if (m_lPos == -1)
    {
        memset(&w, 0, sizeof(w));
        setState(&w);
        return ;
    }


    //is last item "end of transaction"?

    long lCurrPos = m_cIO.getPos();         //keep current position in file
    m_cIO.goPos(m_aUndoPos.at(m_lPos));     //go to previous transaction record in the file
    long lType;
    m_cIO.readInt(&lType);
    if (lType != eUndoEndTransaction)
    {
        m_cIO.goPos(lCurrPos);     //restore position in the file
        return ;
    }
    else
    {
        readState(&w);              //read window state at end of last transaction
        if (!compareState(&w))
        {
            //if state is changed past last EOT
            //just restore it
            setState(&w);
            m_cIO.goPos(lCurrPos);     //restore position in the file
            return ;
        }
    }
    m_lPos--;

    int iDepth;             //depth in transactions
    iDepth = 1;
    long lLine;
    long lPos;
    long lLen;
    char szBuf[32768];
    char szBuf1[32768];


    //do undos
    while(true)
    {
        if (m_lPos == -1)                   //stack is empty, but transaction is not finished?
        {
            _asm int 3                      //looks like problem
            break;
        }

        //read type of another transaction
        m_cIO.goPos(m_aUndoPos.at(m_lPos));
        m_cIO.readInt(&lType);

        switch(lType)
        {
        case    eUndoReplaceLine:
                readState(&w);
                setState(&w);
                m_cIO.readInt(&lLine);
                m_cIO.readAsciz(szBuf);         //new content
                m_cIO.readAsciz(szBuf);         //old content
                m_pWindow->setLine(lLine, szBuf);
                break;
        case    eUndoInsertLine:
                readState(&w);
                setState(&w);
                m_cIO.readInt(&lLine);
                m_pWindow->removeLine(lLine);
                break;
        case    eUndoRemoveLine:
                readState(&w);
                setState(&w);
                m_cIO.readInt(&lLine);
                m_cIO.readAsciz(szBuf);         //new content
                m_pWindow->insertLine(lLine, szBuf);
                break;
        case    eUndoInsertSubstring:
                readState(&w);
                setState(&w);
                m_cIO.readInt(&lLine);
                m_cIO.readInt(&lPos);
                m_cIO.readInt(&lLen);
                m_pWindow->removeSubstring(lLine, lPos, lLen);
                break;
        case    eUndoRemoveSubstring:
                readState(&w);
                setState(&w);
                m_cIO.readInt(&lLine);
                m_cIO.readInt(&lPos);
                m_cIO.readInt(&lLen);
                m_cIO.readAsciz(szBuf);         //old content
                m_pWindow->setLine(lLine, szBuf);
                break;
        case    eUndoBeginTransaction:
                iDepth--;
                readState(&w);
                setState(&w);
                break;
        case    eUndoEndTransaction:
                //read state of window
                readState(&w);
                setState(&w);
                iDepth++;
                break;
        default:
                _asm int 3                  //debug: unknown transaction record type
                break;
        }
        m_cIO.goPos(m_aUndoPos.at(m_lPos)); //start write new items from last processed items
        m_lPos--;                           //remove processed item from stack

        if (iDepth == 0)                    //transaction processing finished?
            break;
    }
}

/** do redo. */
void CUndo::redo()
{
    //nothing to redo
    if (m_lPos == m_aUndoPos.size() - 1)
        return;

    long lPos = m_lPos + 1;

    long lCurrPos = m_cIO.getPos();         //keep current position in file
    m_cIO.goPos(m_aUndoPos.at(lPos));       //go to next transaction record in the file
    long lType;
    m_cIO.readInt(&lType);
    if (lType != eUndoBeginTransaction)
    {
        _asm int 3
        m_cIO.goPos(lCurrPos);              //restore position in the file
        return ;
    }

    int iDepth = 1;
    lPos++;

    long lLine;
    long lLinePos;
    long lLen;
    char szBuf[32768];
    char szBuf1[32768];
    WindowState w;


    while(true)
    {
        if (lPos == m_aUndoPos.size())
        {
            _asm int 3
            lPos--;
            break;
        }

        //read type of another transaction
        m_cIO.goPos(m_aUndoPos.at(lPos));
        m_cIO.readInt(&lType);

        switch(lType)
        {
        case    eUndoReplaceLine:
                readState(&w);
                setState(&w);
                m_cIO.readInt(&lLine);
                m_cIO.readAsciz(szBuf);         //new content
                m_pWindow->setLine(lLine, szBuf);
                break;
        case    eUndoInsertLine:
                readState(&w);
                setState(&w);
                m_cIO.readInt(&lLine);
                m_cIO.readAsciz(szBuf);         //new content
                m_pWindow->insertLine(lLine, szBuf);
                break;
        case    eUndoRemoveLine:
                readState(&w);
                setState(&w);
                m_cIO.readInt(&lLine);
                m_pWindow->removeLine(lLine);
                break;
        case    eUndoInsertSubstring:
                readState(&w);
                setState(&w);
                m_cIO.readInt(&lLine);
                m_cIO.readInt(&lLinePos);
                m_cIO.readInt(&lLen);
                m_cIO.readAsciz(szBuf);         //new content
                m_pWindow->insertSubstring(lLine, lLinePos, szBuf);
                break;
        case    eUndoRemoveSubstring:
                readState(&w);
                setState(&w);
                m_cIO.readInt(&lLine);
                m_cIO.readInt(&lLinePos);
                m_cIO.readInt(&lLen);
                m_pWindow->removeSubstring(lLine, lLinePos, lLen);
                break;
        case    eUndoBeginTransaction:
                iDepth++;
                readState(&w);
                setState(&w);
                break;
        case    eUndoEndTransaction:
                //read state of window
                readState(&w);
                setState(&w);
                iDepth--;
                break;
        default:
                _asm int 3                  //debug: unknown transaction record type
                break;
        }


        if (iDepth == 0)                    //transaction processing finished?
            break;
        lPos++;                             //remove processed item from stack

    }
    m_lPos = lPos;
}

/** clear all undo. */
void CUndo::clear()
{
    m_lPos = -1;
    m_aUndoPos.clear();
}


void CUndo::getState(WindowState *pState)
{
    pState->lBaseLine = m_pWindow->getBaseLine();
    pState->lBasePos = m_pWindow->getBasePos();
    pState->lCursorPos = m_pWindow->getCursorPos();
    pState->lCursorLine = m_pWindow->getCursorLine();
    pState->lBlockType = m_pWindow->getBlockType();
    pState->lBlockStartLine = m_pWindow->getBlockStartLine();
    pState->lBlockStartLinePos = m_pWindow->getBlockStartLinePos();
    pState->lBlockEndLine = m_pWindow->getBlockEndLine();
    pState->lBlockEndLinePos = m_pWindow->getBlockEndLinePos();
    pState->bChanged = m_pWindow->isChanged() ? 1 : 0;
}

void CUndo::setState(WindowState *pState)
{
    m_pWindow->setBaseLine(pState->lBaseLine);
    m_pWindow->setBasePos(pState->lBasePos);
    m_pWindow->setCursorPos(pState->lCursorPos);
    m_pWindow->setCursorLine(pState->lCursorLine);
    m_pWindow->setBlockType(pState->lBlockType);
    m_pWindow->setBlockStartLine(pState->lBlockStartLine);
    m_pWindow->setBlockStartLinePos(pState->lBlockStartLinePos);
    m_pWindow->setBlockEndLine(pState->lBlockEndLine);
    m_pWindow->setBlockEndLinePos(pState->lBlockEndLinePos);
    m_pWindow->setChanged(pState->bChanged != 0);
}

void CUndo::readState(WindowState *pState)
{
    m_cIO.readInt(&pState->lBaseLine);
    m_cIO.readInt(&pState->lBasePos);
    m_cIO.readInt(&pState->lCursorPos);
    m_cIO.readInt(&pState->lCursorLine);
    m_cIO.readInt(&pState->lBlockType);
    m_cIO.readInt(&pState->lBlockStartLine);
    m_cIO.readInt(&pState->lBlockStartLinePos);
    m_cIO.readInt(&pState->lBlockEndLine);
    m_cIO.readInt(&pState->lBlockEndLinePos);
    m_cIO.readInt(&pState->bChanged);
}

void CUndo::writeState(WindowState *pState)
{
    m_cIO.writeInt(pState->lBaseLine);
    m_cIO.writeInt(pState->lBasePos);
    m_cIO.writeInt(pState->lCursorPos);
    m_cIO.writeInt(pState->lCursorLine);
    m_cIO.writeInt(pState->lBlockType);
    m_cIO.writeInt(pState->lBlockStartLine);
    m_cIO.writeInt(pState->lBlockStartLinePos);
    m_cIO.writeInt(pState->lBlockEndLine);
    m_cIO.writeInt(pState->lBlockEndLinePos);
    m_cIO.writeInt(pState->bChanged);
}

bool CUndo::compareState(WindowState *pState)
{
    if (pState->lBaseLine != m_pWindow->getBaseLine()) return false;
    if (pState->lBasePos != m_pWindow->getBasePos()) return false;
    if (pState->lCursorPos != m_pWindow->getCursorPos()) return false;
    if (pState->lCursorLine != m_pWindow->getCursorLine()) return false;
    if (pState->lBlockType != m_pWindow->getBlockType()) return false;
    if (pState->lBlockStartLine != m_pWindow->getBlockStartLine()) return false;
    if (pState->lBlockStartLinePos != m_pWindow->getBlockStartLinePos()) return false;
    if (pState->lBlockEndLine != m_pWindow->getBlockEndLine()) return false;
    if (pState->lBlockEndLinePos != m_pWindow->getBlockEndLinePos()) return false;
    if ((pState->bChanged != 0) != m_pWindow->isChanged()) return false;
    return true;
}
