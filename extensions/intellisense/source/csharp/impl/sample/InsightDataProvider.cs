using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.intellisense.cs;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.text;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;

namespace gehtsoft.xce.extension.csintellisense_impl
{

    internal class InsightDataProvider
    {
        private List<InsightData> mList;
        private TextWindow mWindow;
        private int mStartRow, mStartColumn;
        private int mCurSel = 0;
        
        internal int CurSel
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
        
        internal int StartRow
        {
            get
            {
                return mStartRow;
            }
        }
        
        internal int StartColumn
        {
            get
            {
                return mStartColumn;
            }
        }

        internal int Count
        {
            get
            {
                return mList.Count;
            }
        }

        internal InsightData this[int index]
        {
            get
            {
                return mList[index];
            }
        }

        internal InsightDataProvider(TextWindow window, List<InsightData> list, int row, int column)
        {
            mWindow = window;
            mList = list;
            mStartRow = row;
            mStartColumn = column;
        }


        internal bool CursorPositionChanged(int line, int column, out int args)
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
            int curlyBrackets = 0;
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
                    case '\\':
                        if (insideString || insideChar)
                            i++;
                        break;
                    case '\'':
                        insideChar = !insideChar;
                        break;
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
                    case '}':
                        if (!(insideChar || insideString))
                        {
                            --curlyBrackets;
                        }
                        if (curlyBrackets < 0)
                        {
                            return true;
                        }
                        break;
                    case '{':
                        if (!(insideChar || insideString))
                        {
                            ++curlyBrackets;
                        }
                        break;
                    case ';':
                        if (!(insideChar || insideString))
                        {
                            return true;
                        }
                        break;
                }
            }
            return false;
        }
    }
}

