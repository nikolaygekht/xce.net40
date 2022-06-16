using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.text;
using gehtsoft.xce.colorer;
using gehtsoft.xce.spellcheck;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.configuration;
using gehtsoft.xce.editor.util;
using ConsoleColor = gehtsoft.xce.conio.ConsoleColor;

namespace gehtsoft.xce.editor.textwindow
{
    public enum TextWindowBlock
    {
        None,
        Stream,
        Line,
        Box,
    }

    public class TextWindow : Window
    {
        private XceFileBuffer mText;
        private XceFileBufferColorerAdapter mAdapter;
        private Application mApp;
        private int mTopRow, mTopColumn;
        private bool mFormattingInBlock;
        private int mCursorRow
        {
            get
            {
                return mText.State[0];
            }
            set
            {
                mHighlightRangePosition = -1;
                mHighlightRangeLength = 0;
                mText.State[0] = value;
            }
        }

        private int mCursorColumn
        {
            get
            {
                return mText.State[1];
            }
            set
            {
                mHighlightRangePosition = -1;
                mHighlightRangeLength = 0;
                mText.State[1] = value;
            }
        }
        private FileTypeInfo mFileTypeInfo;
        private bool mInsertMode;
        private TextWindowBlock mBlockType
        {
            get
            {
                return (TextWindowBlock)mText.State[2];
            }
            set
            {
                mText.State[2] = (int)value;

            }
        }
        private int mBlockLineStart
        {
            get
            {
                return mText.State[3];
            }
            set
            {
                mText.State[3] = value;

            }
        }
        private int mBlockLineEnd
        {
            get
            {
                return mText.State[4];
            }
            set
            {
                mText.State[4] = value;

            }
        }
        private int mBlockColumnStart
        {
            get
            {
                return mText.State[5];
            }
            set
            {
                mText.State[5] = value;

            }
        }
        private int mBlockColumnEnd
        {
            get
            {
                return mText.State[6];
            }
            set
            {
                mText.State[6] = value;

            }
        }
        private ISpellchecker mSpellchecker;
        private int mLastPaintValidLine;
        private Dictionary<string, object> mCustomData = new Dictionary<string, object>();
        private bool mSaveRequired;
        private XceConfiguration mCfg;

        private int mHighlightRangePosition;

        public int HighlightRangePosition
        {
            get
            {
                return mHighlightRangePosition;
            }
            set
            {
                mHighlightRangePosition = value;
            }
        }

        private int mHighlightRangeLength;

        public int HighlightRangeLength
        {
            get
            {
                return mHighlightRangeLength;
            }
            set
            {
                mHighlightRangeLength = value;
            }
        }


        public event BeforeSaveWindowHook BeforeSaveWindowEvent;
        public event AfterPaintWindowHook AfterPaintWindowEvent;

        public void FireSaveWindowEvent()
        {
            if (BeforeSaveWindowEvent != null)
                BeforeSaveWindowEvent(this);
        }

        public bool SaveRequired
        {
            get
            {
                return mSaveRequired;
            }
            set
            {
                mSaveRequired = value;
            }
        }

        public TextWindowBlock BlockType
        {
            get
            {
                return mBlockType;
            }
        }

        public int BlockLineStart
        {
            get
            {
                return mBlockLineStart;
            }
        }

        public int BlockLineEnd
        {
            get
            {
                return mBlockLineEnd;
            }
        }

        public int BlockColumnStart
        {
            get
            {
                return mBlockColumnStart;
            }
        }

        public int BlockColumnEnd
        {
            get
            {
                return mBlockColumnEnd;
            }
        }

        internal int _BlockLineStart
        {
            get
            {
                return mBlockLineStart;
            }
            set
            {
                mBlockLineStart = value;
            }
        }

        public int _BlockLineEnd
        {
            get
            {
                return mBlockLineEnd;
            }
            set
            {
                mBlockLineEnd = value;
            }
        }

        public int _BlockColumnStart
        {
            get
            {
                return mBlockColumnStart;
            }
            set
            {
                mBlockColumnStart = value;
            }
        }

        public int _BlockColumnEnd
        {
            get
            {
                return mBlockColumnEnd;
            }
            set
            {
                mBlockColumnEnd = value;
            }
        }

        public XceFileBuffer Text
        {
            get
            {
                return mText;
            }
        }

        public SyntaxHighlighter Highlighter
        {
            get
            {
                return mAdapter.Highlighter;
            }
        }

        internal FileTypeInfo FileTypeInfo
        {
            get
            {
                return mFileTypeInfo;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("null");
                mFileTypeInfo = value;
                mIgnoreReload = mFileTypeInfo.IgnoreReload;
            }
        }

        public FileTypeInfo FileTypeInfo1
        {
            get
            {
                return mFileTypeInfo;
            }
        }


        public int CursorRow
        {
            get
            {
                return mCursorRow;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("value");
                if (mCursorRow != value && !mCfg.PersistentBlock)
                    DeselectBlock();
                mCursorRow = value;
                if (this == mApp.ActiveWindow)
                    ShowCaret();
                invalidate();
            }
        }

        public int CursorColumn
        {
            get
            {
                return mCursorColumn;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("value");
                if (mCursorColumn != value && !mCfg.PersistentBlock)
                    DeselectBlock();
                mCursorColumn = value;
                if (this == mApp.ActiveWindow)
                    ShowCaret();
                invalidate();
            }
        }

        internal int _CursorRow
        {
            get
            {
                return mCursorRow;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("value");
                mCursorRow = value;
                if (this == mApp.ActiveWindow)
                    ShowCaret();
                invalidate();
            }
        }

        internal int _CursorColumn
        {
            get
            {
                return mCursorColumn;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("value");
                mCursorColumn = value;
                if (this == mApp.ActiveWindow)
                    ShowCaret();
                invalidate();
            }
        }

        public int TopRow
        {
            get
            {
                return mTopRow;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("value");
                mTopRow = value;
                if (this == mApp.ActiveWindow)
                    ShowCaret();
                invalidate();
            }
        }

        public int TopColumn
        {
            get
            {
                return mTopColumn;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("value");
                mTopColumn = value;
                if (this == mApp.ActiveWindow)
                    ShowCaret();
                invalidate();
            }
        }

        public bool InsertMode
        {
            get
            {
                return mInsertMode;
            }
            set
            {
                mInsertMode = value;
                if (this == mApp.ActiveWindow)
                    ShowCaret();
            }
        }

        public ISpellchecker Spellchecker
        {
            get
            {
                return mSpellchecker;
            }
            set
            {
                mSpellchecker = value;
            }
        }

        internal TextWindow(XceFileBuffer text, Application application, FileTypeInfo fileType)
        {
            mApp = application;
            mText = text;
            mText.OnChanged += this.BufferChangedHandler;
            mAdapter = new XceFileBufferColorerAdapter(mText, application);
            mCursorRow = mCursorColumn = mTopRow = mTopColumn = 0;
            mFileTypeInfo = fileType;
            mIgnoreReload = fileType.IgnoreReload;
            mInsertMode = true;
            mCfg = application.Configuration;
            mSpellchecker = mCfg.SpellCheckers[fileType.DefaultSpellChecker];
            mText.OnNameChanged += NameChangedHandler;
            mSaveRequired = true;
            mFormattingInBlock = application.Configuration.FormattingInBlock;
            if (fileType.EOLMode == EOLMode.ForceLinux)
                mText.EolMode = XceFileBufferEndOfLine.eLf;
            else if (fileType.EOLMode == EOLMode.ForceMac)
                mText.EolMode = XceFileBufferEndOfLine.eCr;

        }

        protected override void Dispose(bool disposing)
        {
            if (mText != null)
                mText.Dispose();
            mText = null;

            if (mAdapter != null)
                mAdapter.Dispose();
            mAdapter = null;
        }

        char[] mBuffer;

        public override void OnPaint(Canvas canvas)
        {
            if (mBuffer == null || mBuffer.Length != Width)
                mBuffer = new char[Width];

            int height = Height;
            int width = Width;
            int i, lineLength, length;
            XceColorScheme colors = mApp.ColorScheme;

            canvas.fill(0, 0, height, width, ' ', colors.TextColor);

            //highlight block
            int blockStartLine =  -1;
            int blockStartColumn = -1;
            int blockEndLine = -1;
            int blockEndColumn = -1;
            bool highlightBlock = false;

            if (mBlockType != TextWindowBlock.None)
            {
                highlightBlock = true;
                blockStartLine = mBlockLineStart;
                blockStartColumn = mBlockColumnStart;
                blockEndLine = mBlockLineEnd;
                blockEndColumn = mBlockColumnEnd;
            }

            int expectedPairLine = -1, expectedPairColumn = -1;
            ConsoleColor clr = new ConsoleColor(0x00);

            int screenRow;
            for (i = mTopRow, screenRow = 0; i < mText.LinesCount && screenRow < height; i++, screenRow++)
            {
                //paint text
                lineLength = mText.LineLength(i);
                int lineStart = mText.LineStart(i);
                if (lineLength > mTopColumn)
                {
                    length = lineLength - mTopColumn;
                    if (length > width)
                        length = width;
                    mText.GetRange(lineStart + mTopColumn, length, mBuffer, 0);
                    canvas.write(screenRow, 0, mBuffer, 0, length);
                    if (i == mCursorRow)
                        canvas.fill(screenRow, 0, 1, width, colors.CurrentLineTextColor);

                    //hightlight using colorer
                    bool rc = mAdapter.Highlighter.GetFirstRegion(i);

                    while (rc)
                    {
                        SyntaxHighlighterRegion r = mAdapter.Highlighter.CurrentRegion;
                        int sstart = r.StartColumn - mTopColumn;
                        if (r.HasColor)
                        {
                            int end = sstart + r.Length;
                            if (r.Length < 0)
                                end = lineLength;

                            if (end < 0) nop();               //do not paint if the block ends before the start of the screen
                            else if (sstart > width) nop();   //and do not paint if the block starts after the screen
                            {
                                if (sstart < 0)
                                    sstart = 0;
                                if (sstart < end)
                                {
                                    clr.PaletteColor = r.ConsoleColor;
                                    clr.RGBForeground = r.ForeColor;
                                    clr.RGBBackground = r.BackColor;
                                    clr.Style = r.Style;
                                    canvas.fill(screenRow, sstart, 1, end - sstart, clr);
                                }
                            }
                        }
                        if (mSpellchecker != null && mFileTypeInfo.IsSpellCheckRegion(r))
                        {
                            int start = r.StartColumn;
                            int end = r.EndColumn;
                            if (r.Length < 0)
                                end = lineLength;
                            int mode = 0;           //scan mode:
                            //0 - find bow
                            //1 - in word
                            int wordBegin = -1;
                            int l;
                            for (int j = start; j <= end; j++)
                            {
                                char c;
                                if (j == end)
                                    c = ' ';
                                else
                                    c = mText[lineStart + j];
                                bool isWord = char.IsLetter(c) || (mode == 1 && (c == '\'' || c == '’'));
                                if (mode == 0)
                                {
                                    if (isWord)
                                    {
                                        wordBegin = j;
                                        mode = 1;
                                    }
                                }
                                else
                                {
                                    if (!isWord)
                                    {
                                        //spellcheck!
                                        if (wordBegin >= 0)
                                        {
                                            l = j - wordBegin;
                                            if (l < mBuffer.Length)
                                            {
                                                mText.GetRange(lineStart + wordBegin, l, mBuffer, 0);
                                                if (!mSpellchecker.Spellcheck(mBuffer, 0, l))
                                                {
                                                    int sstart1 = wordBegin - mTopColumn;
                                                    int end1 = sstart1 + l;

                                                    if (end < 0) nop();
                                                    else if (sstart > width) nop();
                                                    {
                                                        if (sstart1 < 0)
                                                            sstart1 = 0;
                                                        if (sstart1 < end1)
                                                            canvas.fill(screenRow, sstart1, 1, end1 - sstart1, colors.SpellCheckErrorColor);
                                                    }
                                                }
                                            }
                                        }
                                        mode = 0;
                                    }
                                }
                            }
                        }

                        if (r.Line == mCursorRow && mCursorColumn >= r.StartColumn && (((r.Length == -1 || mCursorColumn < r.StartColumn + r.Length) && mAdapter.IsPairStart(r)) || ((r.Length == -1 || mCursorColumn < r.StartColumn + r.Length + 1) && mAdapter.IsPairEnd(r))))
                        {
                            expectedPairLine = i;
                            expectedPairColumn = r.StartColumn;
                        }

                        rc = mAdapter.Highlighter.GetNextRegion();
                    }
                }
                if (highlightBlock && i >= blockStartLine && i <= blockEndLine)
                {
                    int columnFrom = -1, columnTo = -1;
                    switch (mBlockType)
                    {
                    case TextWindowBlock.Stream:
                        {
                            if (i == blockStartLine && i == blockEndLine)
                            {
                                columnFrom = blockStartColumn;
                                columnTo = blockEndColumn;
                            }
                            else if (i == blockStartLine)
                            {
                                columnFrom = blockStartColumn;
                                columnTo = Int16.MaxValue;
                            }
                            else if (i == blockEndLine)
                            {
                                columnFrom = 0;
                                columnTo = blockEndColumn;
                            }
                            else
                            {
                                columnFrom = 0;
                                columnTo = Int16.MaxValue;
                            }
                        }
                        break;
                    case TextWindowBlock.Line:
                        {
                            columnFrom = 0;
                            columnTo = Int16.MaxValue;
                        }
                        break;
                    case TextWindowBlock.Box:
                        {
                            columnFrom = blockStartColumn;
                            columnTo = blockEndColumn;
                        }
                        break;
                    }

                    if (columnTo >= columnFrom)
                    {
                        int sstart;
                        int hl_length;

                        hl_length = columnTo - columnFrom + 1;
                        sstart = columnFrom - mTopColumn;
                        if (sstart < 0)
                        {
                            hl_length += sstart;
                            sstart = 0;
                        }
                        if (sstart + hl_length > width)
                            hl_length = width - sstart;
                        if (hl_length > 0)
                        {
                            if (mFormattingInBlock)
                                canvas.fillBg(screenRow, sstart, 1, hl_length, colors.BlockTextColor);
                            else
                                canvas.fill(screenRow, sstart, 1, hl_length, i == mCursorRow ? colors.CurrentLineBlockTextColor : colors.BlockTextColor);
                        }
                    }
                }
                if (mHighlightRangePosition >= 0)
                {
                    //check whether highligh range and the current line intersects
                    if (! (lineStart >= mHighlightRangePosition + mHighlightRangeLength ||  //ends before
                           lineStart + lineLength < mHighlightRangePosition))                //starts after
                    {
                        int from, to;
                        if (lineStart > mHighlightRangePosition)
                            from = lineStart;
                        else
                            from = mHighlightRangePosition;

                        if (lineStart + lineLength + 1 < mHighlightRangePosition + mHighlightRangeLength)
                            to = lineStart + lineLength + 1;
                        else
                            to = mHighlightRangePosition + mHighlightRangeLength;

                        from -= lineStart + mTopColumn;
                        to -= lineStart + mTopColumn;

                        if (to - from > 0)
                        {
                            if (mFormattingInBlock)
                                canvas.fillBg(screenRow, from, 1, to - from, colors.SearchHighlight);
                            else
                                canvas.fill(screenRow, from, 1, to - from, colors.SearchHighlight);
                        }
                    }
                }
            }

            mLastPaintValidLine = mAdapter.Highlighter.LastValidLine;

            if (expectedPairLine >= mTopRow && expectedPairLine < mTopRow + Height)
            {
                SyntaxHighlighterPair pair = Highlighter.MatchPair(expectedPairLine, expectedPairColumn);
                if (pair != null)
                {
                    HighlightSyntaxPair(canvas, pair, colors.PairHighlightColor);
                    pair.Dispose();
                }
            }
            mApp.WindowManager.setCaretPos(this, mCursorRow - mTopRow, mCursorColumn - mTopColumn);
            if (AfterPaintWindowEvent != null)
                AfterPaintWindowEvent(this, canvas);

        }

        private void HighlightSyntaxPair(Canvas canvas, SyntaxHighlighterPair pair, ConsoleColor color)
        {
            if (pair != null)
            {
                int startScreenLine, startScreenPos, startScreenLenght;
                int endScreenLine, endScreenPos, endScreenLenght;

                if (LinePartToScreen(pair.Start.Line, pair.Start.StartColumn, pair.Start.Length, out startScreenLine, out startScreenPos, out startScreenLenght) &&
                    LinePartToScreen(pair.End.Line, pair.End.StartColumn, pair.End.Length, out endScreenLine, out endScreenPos, out endScreenLenght))
                {
                    canvas.fillFg(startScreenLine, startScreenPos, 1, startScreenLenght, color);
                    canvas.fillFg(endScreenLine, endScreenPos, 1, endScreenLenght, color);
                }
            }
        }

        private bool LinePartToScreen(int line, int column, int length, out int screenline, out int screencolumn, out int screenlength)
        {
            screenline = line - mTopRow;
            screencolumn = column - mTopColumn;
            screenlength = length;

            if (screenline < 0 || screenline >= Height)
                return false;
            if (screencolumn + length < 0 || screencolumn > Width)
                return false;

            if (screencolumn < 0)
            {
                screenlength = screenlength + screencolumn;
                screencolumn = 0;
            }

            if (screenlength <= 0)
                return false;

            if (screencolumn + screenlength > Width)
                screenlength = Width - screencolumn;
            return screenlength > 0;
        }

        private static void nop()
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override void OnSizeChanged()
        {
            base.OnSizeChanged();
            EnsureCursorVisible();
            invalidate();
        }

        public void EnsureCursorVisible()
        {
            int old = mTopRow;

            if (mCursorRow < mTopRow)
            {
                mTopRow = mCursorRow;
            }
            else if (mCursorRow >= mTopRow + Height)
            {
                mTopRow = mCursorRow - (Height - 1);
            }

            if (mCursorColumn < mTopColumn)
            {
                mTopColumn = mCursorColumn;
            }
            else if (mCursorColumn >= mTopColumn + Width)
            {
                mTopColumn = mCursorColumn - (Width - 1);
            }

            if (this == mApp.ActiveWindow)
                ShowCaret();
        }

        internal void EnsureCursorVisibleInCenter()
        {
            int old = mTopRow;
            if (mCursorRow < mTopRow)
            {
                mTopRow = mCursorRow;
            }
            else if (mCursorRow >= mTopRow + Height)
            {
                mTopRow = mCursorRow - (Height / 3);
            }

            if (mCursorColumn < mTopColumn)
            {
                mTopColumn = mCursorColumn;
            }
            else if (mCursorColumn >= mTopColumn + Width)
            {
                mTopColumn = mCursorColumn - (Width - 1);
            }
            if (this == mApp.ActiveWindow)
                ShowCaret();
        }

        private void BufferChangedHandler(XceFileBuffer sender, int position, bool major)
        {
            invalidate();
        }

        public void ShowCaret()
        {
            if (mCursorRow >= mTopRow && mCursorRow < mTopRow + Height)
            {
                mApp.WindowManager.setCaretPos(this, mCursorRow - mTopRow, mCursorColumn - mTopColumn);
                mApp.WindowManager.setCaretType(mInsertMode ? 12 : 50, true);
                mApp.WindowManager.showCaret(true);
            }
            else
                mApp.WindowManager.showCaret(false);
        }

        public void OnActivate()
        {
            ShowCaret();
        }

        public void DeselectBlock()
        {
            mBlockType = TextWindowBlock.None;
            mBlockLineStart = mBlockLineEnd = mBlockColumnEnd = mBlockColumnStart = -1;
            invalidate();
        }

        public void StartBlock(TextWindowBlock type, int row, int column)
        {
            if (type == TextWindowBlock.None)
                DeselectBlock();
            else
            {
                mBlockType = type;
                mBlockLineStart = row;
                mBlockLineEnd = row;
                mBlockColumnStart = column;
                mBlockColumnEnd = column;
            }
            invalidate();
        }

        public void EndBlock(int row, int column)
        {
            switch (mBlockType)
            {
            case    TextWindowBlock.None:
                    break;
            case    TextWindowBlock.Line:
                    {
                        if (row < mBlockLineStart)
                        {
                            mBlockLineEnd = mBlockLineStart;
                            mBlockLineStart = row;
                        }
                        else
                            mBlockLineEnd = row;
                    }
                    break;
            case    TextWindowBlock.Box:
                    {
                        if (row < mBlockLineStart)
                        {
                            mBlockLineEnd = mBlockLineStart;
                            mBlockLineStart = row;
                        }
                        else
                            mBlockLineEnd = row;

                        if (column < mBlockColumnStart)
                        {
                            mBlockColumnEnd = mBlockColumnStart;
                            mBlockColumnStart = column;
                        }
                        else
                            mBlockColumnEnd = column;
                    }
                    break;
            case    TextWindowBlock.Stream:
                    {
                        if (row < mBlockLineStart ||
                            row == mBlockLineStart && column < mBlockColumnStart)
                        {
                            mBlockLineStart = row;
                            mBlockColumnStart = column;
                        }
                        else
                        {
                            mBlockLineEnd = row;
                            mBlockColumnEnd = column;
                        }
                    }
                    break;
            }
            invalidate();
        }

        public void Stroke(char c, int count)
        {
            if (mBlockType != TextWindowBlock.None && !mCfg.PersistentBlock)
            {
                mCursorRow = mBlockLineStart;
                mCursorColumn = mBlockColumnStart;
                BlockContentProcessor.DeleteBlock(this);
            }

            if (c == '\r')
            {
                IEditorCommand enter = mApp.Commands["Enter"];
                for (int i = 0; i < count; i++)
                    enter.Execute(mApp, null);
            }
            else if (c == '\t')
            {
                IEditorCommand tab = mApp.Commands["Tab"];
                for (int i = 0; i < count; i++)
                    tab.Execute(mApp, null);
            }
            else if (c == '\n')
            {
                //ignore
            }
            else
            {
                if (mInsertMode)
                {
                    mText.InsertToLine(mCursorRow, mCursorColumn, c, count);
                    if (mBlockType == TextWindowBlock.Stream)
                    {
                        if (mCursorRow == mBlockLineStart &&
                            mCursorColumn < mBlockColumnStart)
                            mBlockColumnStart += count;
                        if (mCursorRow == mBlockLineEnd &&
                            mCursorColumn < mBlockColumnEnd)
                            mBlockColumnEnd += count;
                    }
                }
                else
                {
                    mText.InsertToLine(mCursorRow, mCursorColumn, c, count);
                    mText.DeleteFromLine(mCursorRow, mCursorColumn + count, count);
                }

                mCursorColumn += count;
                if (mCursorColumn >= mTopColumn + Width)
                    mTopColumn = mCursorColumn - Width + 1;
            }
            invalidate();
        }

        public void Stroke(char c)
        {
            Stroke(c, 1);
        }

        public void Stroke(char[] c, int from, int count)
        {
            if (c == null)
                throw new ArgumentNullException("c");
            if (from < 0 || from >= c.Length)
                throw new ArgumentOutOfRangeException("from");
            if (count <= 0 || from + count > c.Length)
                throw new ArgumentOutOfRangeException("count");
            for (int i = from; i < count; i++)
                Stroke(c[i], 1);
        }

        public void Stroke(string c, int from, int count)
        {
            if (c == null)
                throw new ArgumentNullException("c");
            if (from < 0 || from >= c.Length)
                throw new ArgumentOutOfRangeException("from");
            if (count <= 0 || from + count > c.Length)
                throw new ArgumentOutOfRangeException("count");
            for (int i = from; i < count; i++)
                Stroke(c[i], 1);
        }


        public void BeforeModify()
        {
            if (mBlockType != TextWindowBlock.None && !mCfg.PersistentBlock)
            {
                mCursorRow = mBlockLineStart;
                mCursorColumn = mBlockColumnStart;
                BlockContentProcessor.DeleteBlock(this);
            }
        }

        public void DeleteAtCursor(int count)
        {
            if (mBlockType != TextWindowBlock.None && !mCfg.PersistentBlock)
            {
                mCursorRow = mBlockLineStart;
                mCursorColumn = mBlockColumnStart;
                BlockContentProcessor.DeleteBlock(this);
                return ;
            }
            mHighlightRangePosition = -1;
            mHighlightRangeLength = 0;
            if (mCursorRow >= mText.LinesCount)
                return ;
            if (count < 1)
                throw new ArgumentException("count");
            int r;
            int ll = mText.LineLength(mCursorRow);
            if (mCursorColumn < ll)
            {
                if (mCursorColumn + count >= ll)
                    r = ll - mCursorColumn;
                else
                    r = count;

                mText.DeleteFromLine(mCursorRow, mCursorColumn, r);
                if (mBlockType == TextWindowBlock.Stream &&
                    mCursorRow == mBlockLineStart &&
                    mCursorColumn <= mBlockColumnStart)
                {
                    if (mCursorColumn + r < mBlockColumnStart)
                        mBlockColumnStart -= r;
                    else
                        mBlockColumnStart = mCursorColumn;
                }
                if (mBlockType == TextWindowBlock.Stream &&
                    mCursorRow == mBlockLineEnd &&
                    mCursorColumn <= mBlockColumnEnd)
                {
                    mBlockColumnEnd -= r;
                    if (mBlockColumnEnd < mBlockColumnStart && mBlockLineStart == mBlockLineEnd)
                        mBlockType = TextWindowBlock.None;
                }
                invalidate();
            }
            else
            {
                JoinNextLineAtCursor();
            }
        }

        public void DeleteLineAtCursor()
        {
            if (mBlockType != TextWindowBlock.None && !mCfg.PersistentBlock)
            {
                mCursorRow = mBlockLineStart;
                mCursorColumn = mBlockColumnStart;
                BlockContentProcessor.DeleteBlock(this);
            }
            mHighlightRangePosition = -1;
            mHighlightRangeLength = 0;
            if (mCursorRow < mText.LinesCount)
                mText.DeleteLine(mCursorRow);
            switch (mBlockType)
            {
            case    TextWindowBlock.Line:
            case    TextWindowBlock.Box:
                    if (mCursorRow < mBlockLineStart)
                    {
                        mBlockLineStart--;
                        mBlockLineEnd--;
                    }
                    else if (mCursorRow <= mBlockLineEnd)
                    {
                        mBlockLineEnd--;
                        if (mBlockLineEnd < mBlockLineStart)
                            mBlockType = TextWindowBlock.None;
                    }
                    break;
            case    TextWindowBlock.Stream:
                    if (mCursorRow < mBlockLineStart)
                    {
                        mBlockLineStart--;
                        mBlockLineEnd--;
                    }
                    else if (mCursorRow == mBlockLineStart && mCursorRow == mBlockLineEnd)
                    {
                        mBlockType = TextWindowBlock.None;
                    }
                    else if (mCursorRow == mBlockLineStart)
                    {
                        mBlockColumnStart = 0;
                        mBlockLineEnd--;
                    }
                    else if (mCursorRow < mBlockLineEnd)
                    {
                        mBlockLineEnd--;
                    }
                    else if (mCursorRow == mBlockLineEnd)
                    {
                        mBlockLineEnd--;
                        if (mBlockLineEnd < mText.LinesCount)
                            mBlockColumnEnd = mText.LineLength(mBlockLineEnd);
                        else
                            mBlockColumnEnd = 0;
                    }
                    break;
            }
        }

        public void JoinNextLineAtCursor()
        {
            if (mBlockType != TextWindowBlock.None && !mCfg.PersistentBlock)
            {
                mCursorRow = mBlockLineStart;
                mCursorColumn = mBlockColumnStart;
                BlockContentProcessor.DeleteBlock(this);
            }
            mHighlightRangePosition = -1;
            mHighlightRangeLength = 0;
            if (mCursorRow >= mText.LinesCount)
                return ;
            int ll = mText.LineLength(mCursorRow);
            if (mCursorColumn > mText.LineLength(mCursorRow))
                mText.InsertToLine(mCursorRow, ll, ' ', mCursorColumn - ll);
            mText.JoinWithNext(mCursorRow, mCursorColumn);

            switch (mBlockType)
            {
                case TextWindowBlock.Line:
                case TextWindowBlock.Box:
                    if (mCursorRow + 1 < mBlockLineStart)
                    {
                        mBlockLineStart--;
                        mBlockLineEnd--;
                    }
                    else if (mCursorRow + 1 <= mBlockLineEnd)
                    {
                        mBlockLineEnd--;
                        if (mBlockLineEnd < mBlockLineStart)
                            mBlockType = TextWindowBlock.None;
                    }
                    break;
                case TextWindowBlock.Stream:
                    if (mCursorRow + 1 < mBlockLineStart)
                    {
                        mBlockLineStart--;
                        mBlockLineEnd--;
                    }
                    else if (mCursorRow + 1 == mBlockLineStart && mCursorRow + 1 == mBlockLineEnd)
                    {
                        mBlockLineStart = mBlockLineEnd = mCursorRow;
                        mBlockColumnStart += mCursorColumn;
                        mBlockColumnEnd += mCursorColumn;
                    }
                    else if (mCursorRow + 1 == mBlockLineStart)
                    {
                        mBlockColumnStart += mCursorColumn;
                        mBlockLineStart--;
                        mBlockLineEnd--;
                    }
                    else if (mCursorRow + 1 < mBlockLineEnd)
                    {
                        mBlockLineEnd--;
                    }
                    else if (mCursorRow + 1 == mBlockLineEnd)
                    {
                        mBlockLineEnd--;
                        mBlockColumnEnd += mCursorColumn;
                    }
                    break;
            }
            invalidate();
        }

        public char this[int row, int column]
        {
            get
            {
                if (row < mText.LinesCount)
                {
                    if (column < mText.LineLength(row))
                    {
                        return mText[mText.LineStart(row) + column];
                    }
                    else
                        return (char)0;
                }
                else
                    return (char)0;

            }
        }

        public bool SplitLineAtCursor()
        {
            if (mBlockType != TextWindowBlock.None && !mCfg.PersistentBlock)
            {
                mCursorRow = mBlockLineStart;
                mCursorColumn = mBlockColumnStart;
                BlockContentProcessor.DeleteBlock(this);
            }
            if (mCursorRow >= mText.LinesCount)
                return false;

            bool rc;

            if (mCursorColumn >= mText.LineLength(mCursorRow))
            {
                mText.InsertLine(mCursorRow + 1);
                rc = false;
            }
            else
            {
                mText.SplitLine(mCursorRow, mCursorColumn);
                rc = true;
            }

            switch (mBlockType)
            {
            case    TextWindowBlock.Line:
            case    TextWindowBlock.Box:
                    if (mCursorRow < mBlockLineStart)
                    {
                        mBlockLineStart++;
                        mBlockLineEnd++;
                    }
                    else if (mCursorRow <= mBlockLineEnd)
                    {
                        mBlockLineEnd++;
                    }
                    break;
            case    TextWindowBlock.Stream:
                    if (mCursorRow < mBlockLineStart)
                    {
                        mBlockLineStart++;
                        mBlockLineEnd++;
                    }
                    else if (mCursorRow == mBlockLineStart && mCursorRow == mBlockLineEnd)
                    {
                        if (mCursorColumn <= mBlockColumnStart)
                        {
                            mBlockLineStart++;
                            mBlockLineEnd++;
                            mBlockColumnStart -= mCursorColumn;
                            mBlockColumnEnd -= mCursorColumn;
                        }
                        if (mCursorColumn <= mBlockColumnEnd)
                        {
                            mBlockLineEnd++;
                            mBlockColumnEnd -= mCursorColumn;
                        }
                    }
                    else if (mCursorRow == mBlockLineStart)
                    {
                        if (mCursorColumn <= mBlockColumnStart)
                        {
                            mBlockLineStart++;
                            mBlockColumnStart -= mCursorColumn;
                        }
                        mBlockLineEnd++;
                    }
                    else if (mCursorRow < mBlockLineEnd)
                    {
                        mBlockLineEnd++;
                    }
                    else if (mCursorRow == mBlockLineEnd)
                    {
                        if (mCursorColumn <= mBlockColumnEnd)
                        {
                            mBlockLineEnd++;
                            mBlockColumnEnd -= mCursorColumn;
                        }
                    }
                    break;
            }
            return rc;
        }

        private int mBlockStartRow, mBlockStartColumn;

        public override void OnMouseLButtonDown(int row, int column, bool shift, bool ctrl, bool alt)
        {
            mCursorRow = row + mTopRow;
            mCursorColumn = column + mTopColumn;

            if (mBlockType != TextWindowBlock.None && !mCfg.PersistentBlock)
                DeselectBlock();

            ShowCaret();
            if (shift && !ctrl && !alt)
            {
                StartBlock(TextWindowBlock.Stream, mCursorRow, mCursorColumn);
                mBlockStartRow = mCursorRow;
                mBlockStartColumn = mCursorColumn;
            }
            else if (!shift && ctrl && !alt)
            {
                StartBlock(TextWindowBlock.Line, mCursorRow, mCursorColumn);
                mBlockStartRow = mCursorRow;
                mBlockStartColumn = mCursorColumn;
            }
            else if (!shift && !ctrl && alt)
            {
                StartBlock(TextWindowBlock.Box, mCursorRow, mCursorColumn);
                mBlockStartRow = mCursorRow;
                mBlockStartColumn = mCursorColumn;
            }
            else
            {
                invalidate();
            }
            mApp.repaint();
        }

        public override void OnMouseMove(int row, int column, bool shift, bool ctrl, bool alt, bool leftButton, bool rightButton)
        {
            if (shift && leftButton && !ctrl && !alt)
            {
                if (mBlockStartRow != -1 && mBlockStartColumn != -1)
                {
                    StartBlock(TextWindowBlock.Stream, mCursorRow, mCursorColumn);
                    EndBlock(mTopRow + row, mTopColumn + column);
                }
            }
            else if (!shift && leftButton && !ctrl && alt)
            {
                if (mBlockStartRow != -1 && mBlockStartColumn != -1)
                {
                    StartBlock(TextWindowBlock.Box, mCursorRow, mCursorColumn);
                    EndBlock(mTopRow + row, mTopColumn + column);
                }
            }
            else if (!shift && leftButton && ctrl && !alt)
            {
                if (mBlockStartRow != -1 && mBlockStartColumn != -1)
                {
                    StartBlock(TextWindowBlock.Line, mCursorRow, mCursorColumn);
                    EndBlock(mTopRow + row, mTopColumn + column);
                }
            }
            else if (leftButton && !shift && !ctrl && !alt)
            {
                mCursorRow = mTopRow + row;
                mCursorColumn = mTopColumn + column;
                ShowCaret();
                mBlockStartRow = -1;
                mBlockStartColumn = -1;
            }
            else
            {
                mBlockStartRow = -1;
                mBlockStartColumn = -1;
            }
            mApp.repaint();
        }

        public override void OnMouseWheelUp(int row, int column, bool shift, bool ctrl, bool alt)
        {
            if (mTopRow < 3)
                mTopRow = 0;
            else
                mTopRow -= 3;
            ShowCaret();
            invalidate();

        }

        public override void OnMouseWheelDown(int row, int column, bool shift, bool ctrl, bool alt)
        {
            mTopRow += 3;
            ShowCaret();
            invalidate();
        }

        private void NameChangedHandler(XceFileBuffer sender)
        {
            FileTypeInfo fi = mApp.FileTypes.Find(mText.FileName);
            if (fi != mFileTypeInfo)
            {
                mFileTypeInfo = fi;
                mIgnoreReload = fi.IgnoreReload;
                mSpellchecker = mApp.Configuration.SpellCheckers[fi.DefaultSpellChecker];
                mText.TabSize = fi.TabSize;
                mText.TrimSpace = fi.TrimEolSpace;
                invalidate();
            }
        }

        internal void OnIdle()
        {
            if (mAdapter.Highlighter != null)
            {
                mAdapter.Highlighter.IdleJob(0);
                if (mLastPaintValidLine <= mTopRow &&
                    mAdapter.Highlighter.LastValidLine >= mTopRow)
                        invalidate();
            }
        }

        public object this[string data]
        {
            get
            {
                object x = null;
                if (!mCustomData.TryGetValue(data, out x))
                    x = null;
                return x;
            }
            set
            {
                if (value == null)
                    mCustomData.Remove(data);
                else
                    mCustomData[data] = value;
            }
        }

        private char mId;

        public char Id
        {
            get
            {
                return mId;
            }
        }

        internal void SetId(char id)
        {
            mId = id;
        }

        private bool mIgnoreReload = false;

        internal bool IgnoreReload
        {
            get
            {
                return mIgnoreReload;
            }
            set
            {
                mIgnoreReload = value;
            }
        }
    }
}
