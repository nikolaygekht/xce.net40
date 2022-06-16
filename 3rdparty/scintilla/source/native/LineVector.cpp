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

#include "SplitVector.h"
#include "Partitioning.h"
#include "LineVector.h"

LineVector::LineVector() : starts(256)
{
    Init();
    for (int i = 0 ; i < 16; i++)
        markers[i] = -1;
}

LineVector::~LineVector()
{
    starts.DeleteAll();
}

void LineVector::Init()
{
    for (int i = 0 ; i < 16; i++)
        markers[i] = -1;
    starts.DeleteAll();
}

void LineVector::InsertText(int line, int delta) {
    starts.InsertText(line, delta);
}

void LineVector::InsertLine(int line, int position, bool lineStart) {
    starts.InsertPartition(line, position);
    for (int i = 0; i < 16; i++)
        if (markers[i] > line)
            markers[i]++;
}

void LineVector::SetLineStart(int line, int position) {
    starts.SetPartitionStartPosition(line, position);
}

void LineVector::RemoveLine(int line) {
    starts.RemovePartition(line);
    for (int i = 0; i < 16; i++)
    {
        if (markers[i] > line)
            markers[i]--;
    }
}

int LineVector::GetMarker(int marker)
{
    if (marker >= 0 && marker < 16)
        return markers[marker];
    else
        return -1;
}

void LineVector::SetMarker(int marker, int line)
{
    if (marker >= 0 && marker < 16)
        markers[marker] = line;
}

int LineVector::LineFromPosition(int pos) const {
    return starts.PartitionFromPosition(pos);
}

