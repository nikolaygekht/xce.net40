using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using gehtsoft.intellisense.cs;
using gehtsoft.xce.colorer;
using gehtsoft.xce.text;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.configuration;
using gehtsoft.xce.intellisense.common;
using GehtSoft.DocCreator.Parser;

namespace gehtsoft.intellisense.vbscript
{
    internal class PairInfo
    {
        internal string Pair;
        internal string Outline;

        internal PairInfo(string pair, string outline)
        {
            Pair = pair;
            Outline = outline;
        }
    }

    internal class PairInfoStack
    {
        private PairInfo[] mPairs = new PairInfo[1024];
        private int mCount = 0;

        internal PairInfoStack()
        {
        }

        internal int Count
        {
            get
            {
                return mCount;
            }
        }

        internal PairInfo this[int index]
        {
            get
            {
                if (index < 0 || index >= mCount)
                    throw new ArgumentOutOfRangeException("index");
                return mPairs[index];
            }
        }

        internal void reset()
        {
            mCount = 0;
        }

        internal void add(string pair, string outline)
        {
            if (mCount < 1024)
            {
                mPairs[mCount] = new PairInfo(pair, outline);
                mCount++;
            }
        }
    }


    internal class VbSourceFileAnalyzer
    {
        private SyntaxRegion mPairStart;
        private SyntaxRegion mComment;
        private SyntaxRegion mString;
        private SyntaxRegion mOutline;
        private object mMutex;
        private TextWindow mWindow;
        private int mAnalyzedLine;
        private bool mAtBeginOfLine;
        private bool mInComment;
        private bool mInExpr;
        private string mPreviousWord;
        private string mCurrentWord;
        private int mCurrentWordColumn;
        private int mCurrentWordLength;
        private char mWordDivisor;

        private PairInfoStack mStack = new PairInfoStack();

        internal object Mutex
        {
            get
            {
                return mMutex;
            }
        }

        internal bool InExpr
        {
            get
            {
                return mInExpr;
            }
        }

        internal bool AtBeginOfLine
        {
            get
            {
                return mAtBeginOfLine;
            }
        }

        internal bool InComment
        {
            get
            {
                return mInComment;
            }
        }

        internal string CurrentWord
        {
            get
            {
                return mCurrentWord;
            }
        }

        internal int CurrentWordColumn
        {
            get
            {
                return mCurrentWordColumn;
            }
        }

        internal int CurrentWordLength
        {
            get
            {
                return mCurrentWordLength;
            }
        }

        internal char WordDivisor
        {
            get
            {
                return mWordDivisor;
            }
        }

        internal string PreviousWord
        {
            get
            {
                return mPreviousWord;
            }
        }

        internal VbSourceFileAnalyzer(SyntaxRegion pairStart, SyntaxRegion comment, SyntaxRegion str, SyntaxRegion outline)
        {
            mPairStart = pairStart;
            mComment = comment;
            mOutline = outline;
            mString = str;
            mMutex = new object();
        }

        internal void reset()
        {
            mAtBeginOfLine = false;
            mInExpr = false;
            mInComment = false;
            mPreviousWord = null;
            mCurrentWord = null;
            mWordDivisor = ' ';
            mStack.reset();
        }

        internal void analyze(TextWindow window, int line, int column, bool extend)
        {
            mWindow = window;
            mAnalyzedLine = line;

            for (int i = line; i >= 0; i--)
            {
                analyzeLine(window, i, i == line ? column : -1, i == line, extend);
                if (mInComment)
                    break;
            }
        }

        internal bool IsOnTopPair(string pair)
        {
            if (mStack.Count == 0)
                return false;
            return (string.Compare(mStack[0].Pair, pair, true) == 0);
        }

        internal bool HasPair(string pair)
        {
            for (int i = 0; i < mStack.Count; i++)
            {
                if (string.Compare(mStack[i].Pair, pair, true) == 0)
                    return true;
            }
            return false;
        }

        internal string CurrentOutline
        {
            get
            {
                string s = "";
                for (int i = mStack.Count - 1; i >= 0; i--)
                {
                    if (mStack[i].Outline != null)
                    {
                        s += ".";
                        s += mStack[i].Outline;
                    }
                }
                return s;
            }
        }

        private void analyzeLine(TextWindow window, int line, int column, bool currentLine, bool extend)
        {
            XceFileBuffer buffer = window.Text;
            SyntaxHighlighter highlight = window.Highlighter;

            if (line >= buffer.LinesCount)
            {
                if (column >= 0)
                {
                    mAtBeginOfLine = true;
                    return ;
                }
            }

            int ls = buffer.LineStart(line);
            int ll = buffer.LineLength(line);
            if (column == -1)
                column = ll;

            if (currentLine)
            {
                mAtBeginOfLine = true;
                for (int i = 0; i < ll && i < column; i++)
                {
                    char c = buffer[ls + i];
                    if (!char.IsWhiteSpace(c))
                    {
                        mAtBeginOfLine = false;
                    }

                    if (c == ',' || c == '=' || c == '(')
                        mInExpr = true;
                }
            }

            if (currentLine && column > 0 && !mAtBeginOfLine)
            {

                int wline, wcolumn, wlength;
                if (CommonUtils.WordUnderCursor(mWindow, line, column, true, out wline, out wcolumn, out wlength))
                {
                    if (!extend)
                    {
                        if (wcolumn < column)
                        {
                            if (wcolumn + wlength > column)
                                wlength = column - wcolumn;
                        }
                        if (wlength > 0)
                        {
                            mCurrentWord = buffer.GetRange(ls + wcolumn, wlength);
                            mCurrentWordColumn = wcolumn;
                            mCurrentWordLength = wlength;
                        }
                    }
                    else
                    {
                        mCurrentWord = buffer.GetRange(ls + wcolumn, wlength);
                        mCurrentWordColumn = wcolumn;
                        mCurrentWordLength = wlength;
                    }
                }
                else
                {
                    wcolumn = column;
                }
                if (wcolumn > 1)
                {
                    if (buffer[ls + wcolumn - 1] == '.')
                    {
                        int t = wcolumn - 2;
                        if (wcolumn > 1 && CommonUtils.WordUnderCursor(mWindow, line, t, false, out wline, out wcolumn, out wlength))
                            mPreviousWord = buffer.GetRange(ls + wcolumn, wlength);
                        mWordDivisor = '.';
                    }
                    else if (buffer[ls + wcolumn - 1] == ' ')
                    {
                        int t = wcolumn - 1;
                        while (t > 0 && char.IsWhiteSpace(buffer[ls + t]))
                            t--;
                        if (t >= 0)
                        {
                            if (CommonUtils.WordUnderCursor(mWindow, line, t + 1, false, out wline, out wcolumn, out wlength))
                                mPreviousWord = buffer.GetRange(ls + wcolumn, wlength);
                        }
                        mWordDivisor = ' ';
                    }
                }
            }

            bool rc;
            rc = highlight.GetFirstRegion(line);

            int pairInLine = -1;
            int pairLengthInLine = 0;
            string outlineInLine = null;

            while (rc)
            {
                if (currentLine)
                {
                    if (highlight.CurrentRegion.Is(mComment) && highlight.CurrentRegion.StartColumn <= column && (highlight.CurrentRegion.Length < 0  || (highlight.CurrentRegion.StartColumn + highlight.CurrentRegion.Length) >= column))
                    {
                        mInComment = true;
                        return ;
                    }
                    if (highlight.CurrentRegion.Is(mString) && highlight.CurrentRegion.StartColumn <= column && (highlight.CurrentRegion.Length < 0  || (highlight.CurrentRegion.StartColumn + highlight.CurrentRegion.Length) >= column))
                    {
                        mInComment = true;
                        return ;
                    }
                }

                if (highlight.CurrentRegion.Is(mPairStart) && char.IsLetter(buffer[ls + highlight.CurrentRegion.StartColumn]))
                {
                    pairInLine = highlight.CurrentRegion.StartColumn;
                    pairLengthInLine = highlight.CurrentRegion.Length;
                }

                if (highlight.CurrentRegion.Is(this.mOutline))
                    outlineInLine = buffer.GetRange(ls + highlight.CurrentRegion.StartColumn, highlight.CurrentRegion.Length);


                rc = highlight.GetNextRegion();
            }

            if (outlineInLine != null && currentLine)
                mInExpr = false;

            if (pairInLine >= 0)
            {
                SyntaxHighlighterPair pair = highlight.MatchPair(line, pairInLine);
                if (pair == null || pair.End.Line >= this.mAnalyzedLine)
                    mStack.add(buffer.GetRange(ls + pairInLine, pairLengthInLine), outlineInLine);
            }
            return ;
        }
    }
}



