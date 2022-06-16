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
#include "undo.h"
#include "CellBuffer.h"

/** constructor. */
CUndo::CUndo(CellBuffer *buffer)
{
    mBuffer = buffer;
    m_cIO.openTemp();
    m_lPos = -1;
    mLastInsert = false;
}

/** Destructor. */
CUndo::~CUndo()
{
    m_aUndoPos.clear();
    m_lPos = -1;
    m_lSavePoint = -1;
    m_cIO.close();
}

/** Register begin transaction. */
void CUndo::beginTransaction()
{
    if ((m_lPos + 1) != m_aUndoPos.size())
        m_aUndoPos.erase(m_aUndoPos.begin() + (m_lPos + 1), m_aUndoPos.end());
    m_lPos = m_aUndoPos.size();
    if (m_lSavePoint >= m_lPos)
        m_lSavePoint = -2;
    m_aUndoPos.push_back(m_cIO.getPos());
    m_cIO.writeInt(eUndoBeginTransaction);
    writeState();
    mLastInsert = false;
}

/** Register end transaction. */
void CUndo::endTransaction()
{
    if (m_lPos == -1)
    {
        mLastInsert = false;
        return ;
    }

    //check is previous record "begin transaction" or not
    long lKeepPos = m_cIO.getPos();
    long lPrevPos = m_aUndoPos.at(m_lPos);
    long lCmd;
    m_cIO.goPos(lPrevPos);
    if (m_lSavePoint >= m_lPos)
        m_lSavePoint = -2;
    m_cIO.readInt(&lCmd);
    if (lCmd == eUndoBeginTransaction)
    {
        m_cIO.goPos(lPrevPos);
        m_aUndoPos.erase(m_aUndoPos.begin() + m_lPos, m_aUndoPos.end());
        m_lPos--;
        mLastInsert = false;
        return;
    }
    else
        m_cIO.goPos(lKeepPos);

    if ((m_lPos + 1) != m_aUndoPos.size())
        m_aUndoPos.erase(m_aUndoPos.begin() + (m_lPos + 1), m_aUndoPos.end());
    m_lPos = m_aUndoPos.size();
    m_aUndoPos.push_back(m_cIO.getPos());
    m_cIO.writeInt(eUndoEndTransaction);
    writeState();
    mLastInsert = false;
}

/** Register insert line. */
void CUndo::insertText(int position, int length, const wchar_t *content)
{
    if ((m_lPos + 1) != m_aUndoPos.size())
    {
        m_aUndoPos.erase(m_aUndoPos.begin() + (m_lPos + 1), m_aUndoPos.end());
        mLastInsert = false;
    }

    bool proc = false;
    if (mLastInsert && position == (mLastInsertPos + mLastInsertLength))
    {
        long lPrevPos = m_aUndoPos.at(m_lPos);
        m_cIO.goPos(lPrevPos + 72);
        m_cIO.writeInt(mLastInsertLength + length);
        m_cIO.goPos(lPrevPos + 76 + mLastInsertLength * sizeof(wchar_t));
        m_cIO.writeString(content, length);
        mLastInsertLength += length;
        proc = true;
    }

    if (!proc)
    {
        m_lPos = m_aUndoPos.size();
        if (m_lSavePoint >= m_lPos)
            m_lSavePoint = -2;
        m_aUndoPos.push_back(m_cIO.getPos());

        m_cIO.writeInt(eUndoInsert);            //0 (4)
        writeState();                           //4 (16 * 4 = 64)
        m_cIO.writeInt(position);               //68 (4)
        m_cIO.writeInt(length);                 //72 (4)
        m_cIO.writeString(content, length);     //76 (length * 2)

        mLastInsert = true;
        mLastInsertPos = position;
        mLastInsertLength = length;
    }

}

/** Register remove substring. */
void CUndo::deleteText(int position, int length, const wchar_t *content)
{
    if ((m_lPos + 1) != m_aUndoPos.size())
        m_aUndoPos.erase(m_aUndoPos.begin() + (m_lPos + 1), m_aUndoPos.end());
    m_lPos = m_aUndoPos.size();
    if (m_lSavePoint >= m_lPos)
        m_lSavePoint = -2;
    m_aUndoPos.push_back(m_cIO.getPos());

    m_cIO.writeInt(eUndoRemove);
    writeState();
    m_cIO.writeInt(position);
    m_cIO.writeInt(length);
    m_cIO.writeString(content, length);
    mLastInsert = false;
}


bool CUndo::canUndo()
{
    return m_lPos >= 0;
}

bool CUndo::canRedo()
{
    return m_lPos < (int)(m_aUndoPos.size() - 1);
}

void CUndo::SetSavePoint()
{
    mLastInsert = false;
    m_lSavePoint = m_lPos;
}

bool CUndo::IsSavePoint()
{
    return m_lSavePoint == m_lPos;
}

/** do undo. */
int CUndo::undo()
{
    mLastInsert = false;
    int rc = -1;
    //nothing to undo
    if (m_lPos == -1)
        return rc;

    int iDepth = 0;             //depth in transactions
    long position;
    long length;
    wchar_t *buffer = 0;
    int bufferLength = 0;
    long lType;

    //do undos
    while(true)
    {
        if (m_lPos < 0)                   //stack is empty
        {
            m_lPos = -1;
            break;
        }

        //read type of another transaction
        m_cIO.goPos(m_aUndoPos.at(m_lPos));
        m_cIO.readInt(&lType);

        switch(lType)
        {
        case    eUndoBeginTransaction:
                iDepth--;
                readState();
                break;
        case    eUndoEndTransaction:
                //read state of window
                readState();
                iDepth++;
                break;
        case    eUndoInsert:
                //read state of window
                readState();
                m_cIO.readInt(&position);
                m_cIO.readInt(&length);
                mBuffer->DeleteCharsNoUndo(position, length);
                rc = position;
                break;
        case    eUndoRemove:
                //read state of window
                readState();
                m_cIO.readInt(&position);
                m_cIO.readInt(&length);
                if (length > bufferLength)
                {
                    if (buffer != 0)
                        delete[] buffer;
                    bufferLength = (length / 4096 + 1) * 4096;
                    buffer = new wchar_t[bufferLength];
                }
                m_cIO.readString(buffer, length);
                mBuffer->InsertStringNoUndo(position, buffer, length);
                rc = position + length;
                break;
        default:
                _asm int 3                  //debug: unknown transaction record type
                break;
        }
        m_cIO.goPos(m_aUndoPos.at(m_lPos)); //start write new items from last processed items
        m_lPos--;                           //remove processed item from stack
        if (iDepth <= 0)                    //transaction processing finished?
            break;
    }
    return rc;
}

/** do redo. */
int CUndo::redo()
{
    mLastInsert = false;
    int rc = -1;
    //nothing to redo
    if (m_lPos == m_aUndoPos.size() - 1)
        return rc;

    long lPos = m_lPos + 1;

    int iDepth = 0;
    long lType;
    long position;
    long length;
    wchar_t *buffer = 0;
    int bufferLength = 0;


    while(true)
    {
        if (lPos == m_aUndoPos.size())
        {
            lPos--;
            break;
        }

        //read type of another transaction
        m_cIO.goPos(m_aUndoPos.at(lPos));
        m_cIO.readInt(&lType);

        switch(lType)
        {
        case    eUndoInsert:
                readState(false);
                m_cIO.readInt(&position);
                m_cIO.readInt(&length);
                if (length > bufferLength)
                {
                    if (buffer != 0)
                        delete[] buffer;
                    bufferLength = (length / 4096 + 1) * 4096;
                    buffer = new wchar_t[bufferLength];
                }
                m_cIO.readString(buffer, length);
                mBuffer->InsertStringNoUndo(position, buffer, length);
                rc = position + length;
                break;
        case    eUndoRemove:
                readState(false);
                m_cIO.readInt(&position);
                m_cIO.readInt(&length);
                mBuffer->DeleteCharsNoUndo(position, length);
                rc = position;
                break;
        case    eUndoBeginTransaction:
                iDepth++;
                break;
        case    eUndoEndTransaction:
                //read state of window
                iDepth--;
                break;
        default:
                _asm int 3                  //debug: unknown transaction record type
                break;
        }

        if (iDepth <= 0)                    //transaction processing finished?
            break;
        lPos++;                             //remove processed item from stack

    }
    m_lPos = lPos;
    return rc;
}

/** clear all undo. */
void CUndo::clear()
{
    mLastInsert = false;
    m_lPos = -1;
    m_aUndoPos.clear();
}


void CUndo::readState(bool restore)
{
    if (restore)
    {
        int *state = mBuffer->GetState();
        for (int i = 0; i < 16; i++)
             m_cIO.readInt((long *)&state[i]);
    }
    else
    {
        long t;
        for (int i = 0; i < 16; i++)
             m_cIO.readInt(&t);
    }
}

void CUndo::writeState()
{
    int *state = mBuffer->GetState();
    for (int i = 0; i < 16; i++)
         m_cIO.writeInt((long)state[i]);
}

