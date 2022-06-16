using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.text;
using gehtsoft.xce.editor.textwindow;

namespace gehtsoft.xce.editor.util
{
    public class BlockContent : IDisposable
    {
        TextWindowBlock mType;
        XceFileBuffer mBuffer;

        BlockContent()
        {
            mType = TextWindowBlock.None;
            mBuffer = null;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~BlockContent()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (mBuffer != null)
                mBuffer.Dispose();
            mBuffer = null;
        }

        public XceFileBuffer Buffer
        {
            get
            {
                return mBuffer;
            }
        }

        public TextWindowBlock Type
        {
            get
            {
                return mType;
            }
        }

        public BlockContent(string text, int tabLength)
        {
            mBuffer = new XceFileBuffer();
            mBuffer.TabSize = tabLength;
            mBuffer.InsertRange(0, text);
            mBuffer.ExpandTabs = true;
            mBuffer.ExpandTabsInRange(0, mBuffer.Length);
            mType = TextWindowBlock.Stream;
        }

        public BlockContent(TextWindow w)
        {
            mType = w.BlockType;
            if (mType != TextWindowBlock.None)
            {
                mBuffer = new XceFileBuffer();
                mBuffer.TabSize = w.Text.TabSize;
                mBuffer.ExpandTabs = true;
                BlockContentProcessor.GatherBlock(w, this);
            }
        }

        public void Put(TextWindow w, int row, int column)
        {
            BlockContentProcessor.PutBlock(w, this, row, column);
        }

    }

    public class BlockContentProcessor
    {
        static internal void GatherBlock(TextWindow w, BlockContent b)
        {
            switch (w.BlockType)
            {
            case TextWindowBlock.Stream:
                GatherStreamBlock(w, b);
                break;
            case TextWindowBlock.Box:
                GatherBoxBlock(w, b);
                break;
            case TextWindowBlock.Line:
                GatherLineBlock(w, b);
                break;
            }
        }

        static private void GatherStreamBlock(TextWindow w, BlockContent b)
        {
            int blockLineEnd = w.BlockLineEnd;
            int blockLineStart = w.BlockLineStart;
            int blockColumnEnd = w.BlockColumnEnd;
            int blockColumnStart = w.BlockColumnStart;

            if (blockLineStart >= w.Text.LinesCount)
                return;
            int s = w.Text.LineStart(blockLineStart);
            if (blockColumnStart < w.Text.LineLength(blockLineStart))
                s += blockColumnStart;
            else
                s += w.Text.LineLength(blockLineStart);

            if (s >= w.Text.Length)
                return;

            int e;

            if (blockLineEnd >= w.Text.LinesCount)
                e = w.Text.Length - 1;
            else
            {
                e = w.Text.LineStart(blockLineEnd);
                if (blockColumnEnd < w.Text.LineLength(blockLineEnd))
                    e += blockColumnEnd;
                else
                    e += w.Text.LineLength(blockLineEnd) - 1;
            }

            int l = e - s + 1;
            if (l > 0)
            {
                char[] buff = new char[l];
                w.Text.GetRange(s, l, buff, 0);
                b.Buffer.InsertRange(0, buff, 0, l);
                buff = null;
                GC.Collect();
            }
        }

        static private void GatherLineBlock(TextWindow w, BlockContent b)
        {
            int blockLineEnd = w.BlockLineEnd;
            int blockLineStart = w.BlockLineStart;
            int blockColumnEnd = w.BlockColumnEnd;
            int blockColumnStart = w.BlockColumnStart;
            AutoArray<char> t = new AutoArray<char>();
            for (int i = blockLineStart; i <= blockLineEnd; i++)
            {
                if (i < w.Text.LinesCount)
                {
                    int l = w.Text.LineLength(i, true);
                    if (l > 0)
                    {
                        t.Ensure(l);
                        w.Text.GetRange(w.Text.LineStart(i), l, t.Array, 0);
                        b.Buffer.InsertRange(b.Buffer.Length, t.Array, 0, l);
                    }
                    else
                        b.Buffer.InsertRange(b.Buffer.Length, w.Text.Eol, 0, w.Text.Eol.Length);
                }
            }
            t = null;
            GC.Collect();
        }

        static private void GatherBoxBlock(TextWindow w, BlockContent b)
        {
            int blockLineEnd = w.BlockLineEnd;
            int blockLineStart = w.BlockLineStart;
            int blockColumnEnd = w.BlockColumnEnd;
            int blockColumnStart = w.BlockColumnStart;
            char[] buff = new char[blockColumnEnd - blockColumnStart + 1];
            char[] eol = w.Text.Eol;
            if (eol == null)
                eol = new char[0];
            int eol_length = eol.Length;
            for (int i = blockLineStart; i <= blockLineEnd; i++)
            {
                if (i < w.Text.LinesCount)
                {
                    int s0, s, e, l;

                    s0 = w.Text.LineStart(i);
                    l = w.Text.LineLength(i);

                    s = s0 + blockColumnStart;
                    e = s0 + blockColumnEnd;

                    if (s >= s0 + l)
                    {
                        l = 0;
                    }
                    else
                    {
                        if (e >= s0 + l)
                            e = s0 + l - 1;

                        l = e - s + 1;
                    }
                    if (l > 0)
                    {
                        w.Text.GetRange(s, l, buff, 0);
                        b.Buffer.InsertRange(b.Buffer.Length, buff, 0, l);
                        b.Buffer.InsertRange(b.Buffer.Length, eol, 0, eol_length);
                    }
                    else
                    {
                        b.Buffer.InsertRange(b.Buffer.Length, eol, 0, eol_length);
                    }
                }
            }
            buff = null;
            GC.Collect();
        }


        static internal void PutBlock(TextWindow w, BlockContent b, int row, int column)
        {
            w.BeforeModify();
            switch (b.Type)
            {
                case TextWindowBlock.Stream:
                    PutStreamBlock(w, b, row, column);
                    break;
                case TextWindowBlock.Box:
                    PutBoxBlock(w, b, row, column);
                    break;
                case TextWindowBlock.Line:
                    PutLineBlock(w, b, row, column);
                    break;
            }
        }

        static private void PutStreamBlock(TextWindow w, BlockContent b, int row, int column)
        {
            if (b.Buffer.Length > 0)
            {
                w.Text.BeginUndoTransaction();
                //ensure that the file is long enough
                while (w.Text.LinesCount <= row)
                    w.Text.AppendLine();
                int l = w.Text.LineLength(row);
                if (l < column)
                    w.Text.InsertToLine(row, l, ' ', column - l);
                char []buff = new char[b.Buffer.Length];
                b.Buffer.GetRange(0, buff.Length, buff, 0);
                int s = w.Text.LineStart(row) + column;
                w.Text.InsertRange(s, buff, 0, buff.Length);
                int length = w.Text.ExpandTabsInRange(s, buff.Length);
                buff = null;
                int srow = row;
                int scolumn = column;
                int e = s + length - 1;
                row = w.Text.LineFromPosition(e);
                column = e - w.Text.LineStart(row);
                e++;
                int crow = w.Text.LineFromPosition(e);
                int ccolumn = e - w.Text.LineStart(row);
                w.CursorRow = crow;
                w.CursorColumn = ccolumn;
                w.StartBlock(TextWindowBlock.Stream, srow, scolumn);
                w.EndBlock(row, column);
                w.EnsureCursorVisible();
                w.Text.EndUndoTransaction();
            }
        }

        static private void PutBoxBlock(TextWindow w, BlockContent b, int row, int column)
        {
            if (b.Buffer.Length > 0)
            {
                w.Text.BeginUndoTransaction();
                //ensure that the file is long enough
                while (w.Text.LinesCount <= row)
                    w.Text.AppendLine();

                AutoArray<char> buff = new AutoArray<char>();
                char[] eol = w.Text.Eol;
                int eol_length = eol.Length;
                int i;
                int max = 0;
                for (i = 0; i < b.Buffer.LinesCount; i++)
                {
                    while (w.Text.LinesCount <= row + i)
                        w.Text.AppendLine();

                    int l0 = w.Text.LineLength(row + i);
                    if (l0 < column)
                        w.Text.InsertToLine(row + i, l0, ' ', column - l0);

                    int l = b.Buffer.LineLength(i, false);
                    if (l > 0)
                    {
                        buff.Ensure(l);
                        b.Buffer.GetRange(b.Buffer.LineStart(i), l, buff.Array, 0);
                        w.Text.InsertRange(w.Text.LineStart(row + i) + column, buff.Array, 0, l);
                    }

                    if (l > max)
                        max = l;
                }
                w.Text.EndUndoTransaction();
                w._CursorRow = row;
                w._CursorColumn = column;
                w.StartBlock(TextWindowBlock.Box, row, column);
                w.EndBlock(row + i - 2, column + max - 1);
                w.EnsureCursorVisible();
            }

        }

        static private void PutLineBlock(TextWindow w, BlockContent b, int row, int column)
        {
            w.Text.BeginUndoTransaction();
            while (w.Text.LinesCount <= row)
                w.Text.AppendLine();

            AutoArray<char> buff = new AutoArray<char>();
            char[] eol = w.Text.Eol;
            int eol_length = eol.Length;
            int i, la = 0;
            for (i = 0; i < b.Buffer.LinesCount; i++)
            {
                int l = b.Buffer.LineLength(i, false);
                if (l > 0)
                {
                    buff.Ensure(l);
                    b.Buffer.GetRange(b.Buffer.LineStart(i), l, buff.Array, 0);
                    w.Text.InsertRange(w.Text.LineStart(row + i), buff.Array, 0, l);
                }
                if (b.Buffer.LineStart(i) != b.Buffer.Length)
                {
                    la++;
                    w.Text.InsertRange(w.Text.LineStart(row + i) + l, eol, 0, eol_length);
                }
            }
            w.Text.EndUndoTransaction();
            w._CursorRow = row;
            w._CursorColumn = 0;
            w.StartBlock(TextWindowBlock.Line, row, 0);
            w.EndBlock(row + la - 1, 0);
        }

        static internal void DeleteBlock(TextWindow w)
        {
            switch (w.BlockType)
            {
                case TextWindowBlock.Stream:
                    DeleteStreamBlock(w);
                    break;
                case TextWindowBlock.Box:
                    DeleteBoxBlock(w);
                    break;
                case TextWindowBlock.Line:
                    DeleteLineBlock(w);
                    break;
            }
        }

        static private void DeleteStreamBlock(TextWindow w)
        {
            w.Text.BeginUndoTransaction();
            int blockLineEnd = w.BlockLineEnd;
            int blockLineStart = w.BlockLineStart;
            int blockColumnEnd = w.BlockColumnEnd;
            int blockColumnStart = w.BlockColumnStart;
            w.DeselectBlock();

            if (blockLineStart >= w.Text.LinesCount)
                return ;
            int s = w.Text.LineStart(blockLineStart);
            if (blockColumnStart < w.Text.LineLength(blockLineStart))
                s += blockColumnStart;
            else
                s += w.Text.LineLength(blockLineStart);

            if (s >= w.Text.Length)
                return ;

            int e;

            if (blockLineEnd >= w.Text.LinesCount)
                e = w.Text.Length;
            else
            {
                e = w.Text.LineStart(blockLineEnd);
                if (blockColumnEnd < w.Text.LineLength(blockLineEnd))
                    e += blockColumnEnd;
                else
                    e += w.Text.LineLength(blockLineEnd);
            }

            int l = e - s + 1;
            if (l > 0)
                w.Text.DeleteRange(s, e - s + 1);
            w.Text.EndUndoTransaction();

            //update cursor
            if (w.CursorRow < blockLineStart)
                return ;
            if (w.CursorRow == blockLineStart && blockLineStart == blockLineEnd)
            {
                if (w.CursorColumn > blockColumnEnd)
                    w.CursorColumn -= (blockColumnEnd - blockColumnStart + 1);
                else if (w.CursorColumn >= blockColumnStart)
                    w.CursorColumn = blockColumnStart;
            }
            else if (w.CursorRow >= blockLineStart && w.CursorRow < blockLineEnd)
            {
                w.CursorRow = blockLineStart;
                w.CursorColumn = blockColumnStart;
            }
            else if (w.CursorRow == blockLineEnd)
            {
                if (w.CursorRow <= blockColumnEnd)
                {
                    w.CursorRow = blockLineStart;
                    w.CursorColumn = blockColumnStart;
                }
                else
                {
                    w.CursorRow = blockLineStart;
                    w.CursorColumn = blockColumnStart + (w.CursorColumn - blockColumnEnd + 1);
                }
            }
            else
            {
                w.CursorRow = w.CursorRow - (blockLineEnd - blockLineStart);
            }
        }

        static private void DeleteLineBlock(TextWindow w)
        {
            w.Text.BeginUndoTransaction();
            int blockLineEnd = w.BlockLineEnd;
            int blockLineStart = w.BlockLineStart;
            int blockColumnEnd = w.BlockColumnEnd;
            int blockColumnStart = w.BlockColumnStart;
            w.DeselectBlock();
            for (int i = blockLineEnd; i >= blockLineStart; i--)
            {
                if (i >= 0 && i < w.Text.LinesCount)
                {
                    w.Text.DeleteLine(i);
                    if (w.CursorRow >= i && w.CursorRow > 0)
                        w.CursorRow = w.CursorRow - 1;
                }
            }
            w.Text.EndUndoTransaction();
        }

        static private void DeleteBoxBlock(TextWindow w)
        {
            w.Text.BeginUndoTransaction();
            int blockLineEnd = w.BlockLineEnd;
            int blockLineStart = w.BlockLineStart;
            int blockColumnEnd = w.BlockColumnEnd;
            int blockColumnStart = w.BlockColumnStart;
            w.DeselectBlock();
            for (int i = blockLineEnd; i >= blockLineStart; i--)
            {
                if (i < w.Text.LinesCount)
                {
                    int s0, s, e, l;

                    s0 =  w.Text.LineStart(i);
                    l = w.Text.LineLength(i);

                    s = s0 + blockColumnStart;
                    e = s0 + blockColumnEnd;

                    if (s >= s0 + l)
                        continue;
                    if (e >= s0 + l)
                        e = s0 + l - 1;

                    l = e - s + 1;
                    if (l > 0)
                        w.Text.DeleteRange(s, l);
                }
            }
            w.Text.EndUndoTransaction();
        }
    }
}
