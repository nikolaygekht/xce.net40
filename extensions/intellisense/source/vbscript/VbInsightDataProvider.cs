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

namespace gehtsoft.intellisense.vbscript
{
    internal class VbInsightDataProvider : IInsightDataProvider
    {
        VbPropertyCollection mProps = new VbPropertyCollection();
        int mStartColumn, mStartRow;
        int mCurSel;
        TextWindow mWindow;
        
        internal VbInsightDataProvider(TextWindow window, int startRow, int startColumn)
        {
            mCurSel = 0;
            mStartRow = startRow;
            mStartColumn = startColumn;
            mWindow = window;
        }
        
        internal void Add(VbProperty prop)
        {
            mProps.Add(prop);
        }
        
        internal void Sort()
        {
            mProps.Sort();
        }
        
        public IInsightData this[int index]
        {
            get
            {
                return mProps[index];
            }
        }
        
        public int Count
        {
            get
            {
                return mProps.Count;
            }
        }
        
        public int StartColumn
        {
            get
            {
                return mStartColumn;
            }
        }

        public int StartRow
        {
            get
            {
                return mStartRow;
            }
        }

        public int CurSel
        {
            get
            {
                return mCurSel;
            }
            set
            {
                mCurSel = value;
            }
        }

        public bool CursorPositionChanged(int line, int column, out int args)
        {
            args = 0;

            if (line < mStartRow || (line == mStartRow && column < mStartColumn))
                return true;

            int from = mWindow.Text.LineStart(mStartRow) + mStartColumn;
            int to;
            if (line >= mWindow.Text.LinesCount)
                to = mWindow.Text.Length;
            else
            {
                int ls, ll;
                ls = mWindow.Text.LineStart(line);
                ll = mWindow.Text.LineLength(line);
                if (column > ll)
                    column = ll;
                to = ls + column;
            }

            int brackets = 0;
            bool insideChar = false;
            bool insideString = false;

            for (int i = from; i < to; i++)
            {
                char ch = mWindow.Text[i];

                if (!insideChar && !insideString && !char.IsWhiteSpace(ch) && brackets == 1)
                {
                    if (args == 0)
                        args = 1;
                    else if (ch == ',')
                        args++;
                }

                switch (ch)
                {
                    case '(':
                        if (!(insideChar || insideString))
                        {
                            ++brackets;
                        }
                        break;
                    case ')':
                        if (!(insideChar || insideString))
                        {
                            --brackets;
                        }
                        if (brackets <= 0)
                        {
                            return true;
                        }
                        break;
                    case '"':
                        insideString = !insideString;
                        break;
                }
            }
            return false;
        }
    }
}
