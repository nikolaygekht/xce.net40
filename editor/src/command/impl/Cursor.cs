using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.editor.util;
using gehtsoft.xce.editor.configuration;


namespace gehtsoft.xce.editor.command.impl
{
    internal class CursorCommand : IEditorCommand
    {
        XceConfiguration mCfg = null;

        public CursorCommand()
        {
        }

        public string Name
        {
            get
            {
                return "Cursor";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            if (application.ActiveWindow == null || parameter == null)
                return ;

            if (mCfg == null)
                mCfg = application.Configuration;

            int columnShift = 0;
            int lineShift = 0;
            int pageSize, width;

            TextWindow w = application.ActiveWindow;
            pageSize = w.Height;
            width = w.Width;

            int i1, i2, i3, i4;

            if (parameter.Length >= 2)
            {
                if (parameter[0] == 'l' && parameter[1] == 'c')
                {
                    if (w.CursorColumn > 0)
                        columnShift = -1;
                    else
                    {
                        if (w.CursorRow > 0)
                        {
                            lineShift = -1;
                            if (w.CursorRow - 1 < w.Text.LinesCount)
                            {
                                columnShift = w.Text.LineLength(w.CursorRow - 1, false);
                            }
                            else
                                columnShift = 0;
                        }
                    }
                }
                else if (parameter[0] == 'r' && parameter[1] == 'c')
                {
                    columnShift = 1;
                }
                else if (parameter[0] == 'u' && parameter[1] == 'c')
                {
                    lineShift = -1;
                }
                else if (parameter[0] == 'd' && parameter[1] == 'c')
                {
                    lineShift = 1;
                }
                else if (parameter[0] == 'u' && parameter[1] == 'p')
                {
                    lineShift = -pageSize;
                }
                else if (parameter[0] == 'd' && parameter[1] == 'p')
                {
                    lineShift = pageSize;
                }
                if (parameter[0] == 'h' && parameter[1] == 'l')
                {
                    i1 = 0;
                    if (w.CursorRow < w.Text.LinesCount)
                    {
                        i2 = w.Text.LineLength(w.CursorRow, false);
                        i3 = w.Text.LineStart(w.CursorRow);
                        for (i4 = 0; i4 < i2; i4++)
                        {
                            if (w.Text[i3 + i4] != ' ')
                                break;
                        }
                        if (i4 < w.CursorColumn)
                            i1 = i4;
                    }
                    columnShift = i1 - w.CursorColumn;
                }
                else if (parameter[0] == 'e' && parameter[1] == 'l')
                {
                    if (w.CursorRow < w.Text.LinesCount)
                        i1 = w.Text.LineLength(w.CursorRow, false);
                    else
                        i1 = 0;
                    columnShift = i1 - w.CursorColumn;
                }
                else if (parameter[0] == 'h' && parameter[1] == 't')
                {
                    lineShift = -w.CursorRow;
                    columnShift = -w.CursorColumn;
                }
                else if (parameter[0] == 'e' && parameter[1] == 't')
                {
                    i1 = w.Text.LinesCount - 1;
                    i2 = w.Text.LineLength(i1, false);
                    lineShift = i1 - w.CursorRow;
                    columnShift = i2 - w.CursorColumn;
                }
                else if (parameter[0] == 'l' && parameter[1] == 'w')
                {
                    lineShift = 0;
                    columnShift = 0;
                    i1 = w.CursorRow;
                    i2 = w.CursorColumn;
                    CharClass c1, c2;
                    int cnt = 0;
                    if (!(w.CursorRow == 0 && w.CursorColumn == 0))
                    {
                        while (true)
                        {
                            //previous position
                            i3 = i1;
                            i4 = i2 - 1;
                            if (i4 < 0)
                            {
                                if (i3 == 0)
                                    break;
                                i3--;
                                i4 = w.Text.LineLength(i3);
                            }

                            //current and previous char class
                            c1 = CharUtil.GetCharClass(w[i1, i2]);
                            c2 = CharUtil.GetCharClass(w[i3, i4]);
                            if (c1 != c2 && cnt > 0)
                                break;
                            i1 = i3;
                            i2 = i4;
                            cnt ++;
                        }

                        //here i1/i2 is the beginning of the word
                        lineShift = i1 - w.CursorRow;
                        columnShift = i2 - w.CursorColumn;
                    }
                }
                else if (parameter[0] == 'r' && parameter[1] == 'w')
                {
                    lineShift = 0;
                    columnShift = 0;
                    i1 = w.CursorRow;
                    i2 = w.CursorColumn;
                    CharClass c1, c2;
                    int cnt = 0;
                    int cl = 0;
                    if (!(w.CursorRow >= w.Text.LinesCount))
                    {
                        cl = w.Text.LineLength(i1);
                        while (true)
                        {
                            //next position
                            i3 = i1;
                            i4 = i2 + 1;
                            if (i4 > cl)
                            {
                                if (i3 >= w.Text.LinesCount - 1)
                                    break;
                                i3++;
                                cl = w.Text.LineLength(i3);
                                i4 = 0;
                            }
                            //current and next char class
                            c1 = CharUtil.GetCharClass(w[i1, i2]);
                            c2 = CharUtil.GetCharClass(w[i3, i4]);
                            i1 = i3;
                            i2 = i4;
                            if (c1 != c2 && cnt > 0)
                                break;
                            cnt++;
                        }

                        //here i1/i2 is the beginning of the word
                        lineShift = i1 - w.CursorRow;
                        columnShift = i2 - w.CursorColumn;
                    }
                }

            }

            if (lineShift == 0 && columnShift == 0)
                return ;

            TextWindowBlock block = TextWindowBlock.None;
            if (parameter.Length == 3)
            {
                if (parameter[2] == 's')
                    block = TextWindowBlock.Stream;
                else if (parameter[2] == 'b')
                    block = TextWindowBlock.Box;
                else if (parameter[2] == 'l')
                    block = TextWindowBlock.Line;
            }

            int row, column;
            int trow, tcolumn;

            row = w.CursorRow;
            column = w.CursorColumn;
            trow = w.TopRow;
            tcolumn = w.TopColumn;

            if (lineShift != 0)
            {
                row += lineShift;
                if (row < 0)
                    row = 0;

                if (row < trow)
                {
                    trow += lineShift;

                    if (trow < 0)
                        trow = 0;
                }
                else if (row >= trow + pageSize)
                {
                    if (parameter[0] == 'e')
                        trow = row - pageSize + 1;
                    else
                    {
                        trow += lineShift;
                        if (row >= trow + pageSize)
                        {
                            trow = row - pageSize + 1;
                        }
                    }
                }


            }

            if (columnShift != 0)
            {
                column += columnShift;
                if (column < 0)
                    column = 0;

                if (column < tcolumn)
                {
                    tcolumn += columnShift;

                    if (tcolumn < 0)
                        tcolumn = 0;
                }
                else if (column >= tcolumn + width)
                {
                    if (parameter[0] == 'e')
                        tcolumn = column - width + 1;
                    else
                    {
                        tcolumn += columnShift;
                        if (column >= tcolumn + width)
                        {
                            tcolumn = column - width + 1;
                        }
                    }
                }
            }

            if (block != TextWindowBlock.None)
            {
                if (block == TextWindowBlock.Box)
                {
                    int t1, t2;
                    if (block != w.BlockType)
                    {
                        w.StartBlock(block, w.CursorRow, w.CursorColumn);
                        w.EndBlock(row, column);
                    }
                    else if (w.CursorRow == w.BlockLineStart && w.CursorColumn == w.BlockColumnStart)
                    {
                        t1 = w.BlockLineEnd;
                        t2 = w.BlockColumnEnd;
                        w.StartBlock(block, row, column);
                        w.EndBlock(t1, t2);
                    }
                    else if (w.CursorRow == w.BlockLineStart && w.CursorColumn == w.BlockColumnEnd)
                    {
                        t1 = w.BlockLineEnd;
                        t2 = w.BlockColumnStart;
                        w.StartBlock(block, row, column);
                        w.EndBlock(t1, t2);
                    }
                    else if (w.CursorRow == w.BlockLineEnd && w.CursorColumn == w.BlockColumnStart)
                    {
                        t1 = w.BlockLineStart;
                        t2 = w.BlockColumnEnd;
                        w.StartBlock(block, row, column);
                        w.EndBlock(t1, t2);

                    }
                    else if (w.CursorRow == w.BlockLineEnd && w.CursorColumn == w.BlockColumnEnd)
                    {
                        t1 = w.BlockLineStart;
                        t2 = w.BlockColumnStart;
                        w.StartBlock(block, row, column);
                        w.EndBlock(t1, t2);
                    }
                    else
                    {
                        w.StartBlock(block, w.CursorRow, w.CursorColumn);
                        w.EndBlock(row, column);
                    }
                }
                else if (block == TextWindowBlock.Stream)
                {
                    int l, c, t;
                    if (w.BlockType != TextWindowBlock.Stream)
                    {
                        if (compare(w.CursorRow, w.CursorColumn, row, column) < 0)
                        {
                            //if cursor moved forward
                            previousPosition(w, row, column, out l, out c);
                            w.StartBlock(block, w.CursorRow, w.CursorColumn);
                            w.EndBlock(l, c);
                        }
                        else
                        {
                            //if cursor moved backward
                            previousPosition(w, w.CursorRow, w.CursorColumn, out l, out c);
                            w.StartBlock(block, row, column);
                            w.EndBlock(l, c);
                        }
                    }
                    else
                    {
                        previousPosition(w, w.CursorRow, w.CursorColumn, out l, out c);
                        if (l == w.BlockLineEnd && c == w.BlockColumnEnd)
                        {
                            //cursor is currently next to the end of the block
                            t = compare(row, column, w.BlockLineStart, w.BlockColumnStart);
                            if (t > 0)
                            {
                                //if new point is still after the start
                                previousPosition(w, row, column, out l, out c);
                                w.StartBlock(block, w.BlockLineStart, w.BlockColumnStart);
                                w.EndBlock(l, c);
                            }
                            else if (t < 0)
                            {
                                //if new point if behind old block start
                                w.StartBlock(block, row, column);
                                w.EndBlock(l, c);
                            }
                            else
                                w.DeselectBlock();
                        }
                        else if (w.CursorRow == w.BlockLineStart && w.CursorColumn == w.BlockColumnStart)
                        {
                            //cursor is currently at the start of the block - keep end as a point!
                            t = compare(row, column, w.BlockLineEnd, w.BlockColumnEnd);
                            if (t < 0)
                            {
                                //if new point before the block end - just move the start to a new position
                                l = w.BlockLineEnd;
                                c = w.BlockColumnEnd;
                                w.StartBlock(block, row, column);
                                w.EndBlock(l, c);

                            }
                            else if (t > 0)
                            {
                                //if new point if behind the block end
                                w.StartBlock(block, w.BlockLineEnd, w.BlockColumnEnd);
                                previousPosition(w, row, column, out l, out c);
                                w.EndBlock(l, c);
                            }
                            else
                                w.DeselectBlock();
                        }
                        else
                        {
                            if (compare(w.CursorRow, w.CursorColumn, row, column) < 0)
                            {
                                //if cursor moved forward
                                previousPosition(w, row, column, out l, out c);
                                w.StartBlock(block, w.CursorRow, w.CursorColumn);
                                w.EndBlock(l, c);
                            }
                            else
                            {
                                //if cursor moved backward
                                previousPosition(w, w.CursorRow, w.CursorColumn, out l, out c);
                                w.StartBlock(block, row, column);
                                w.EndBlock(l, c);
                            }
                        }
                    }
                }
            }
            else
            {
                if (w.BlockType != TextWindowBlock.None && !mCfg.PersistentBlock)
                    w.DeselectBlock();
            }

            w.TopRow = trow;
            w._CursorRow = row;
            w.TopColumn = tcolumn;
            w._CursorColumn = column;
            w.EnsureCursorVisible();
        }

        /// <summary>
        /// Get checked status for the menu.
        /// </summary>
        public bool IsChecked(Application application, string parameter)
        {
            return false;
        }

        /// <summary>
        /// Get enabled status for the menu with parameter.
        /// </summary>
        public bool IsEnabled(Application application, string parameter)
        {
            return application.ActiveWindow != null && parameter != null;
        }

        /// <summary>
        /// compare two cursor positions (before/after)
        /// </summary>
        /// <param name="row1"></param>
        /// <param name="col1"></param>
        /// <param name="row2"></param>
        /// <param name="col2"></param>
        /// <returns></returns>
        private int compare(int row1, int col1, int row2, int col2)
        {
            if (row1 < row2)
                return -1;
            else if (row1 > row2)
                return 1;
            else if (col1 < col2)
                return -1;
            else if (col1 > col2)
                return 1;
            else
                return 0;
        }

        private bool previousPosition(TextWindow w, int row, int column, out int row1, out int column1)
        {
            if (row < 0)
                throw new ArgumentException("row");
            if (column < 0)
                throw new ArgumentException("column");
            if (row == 0 && column == 0)
            {
                row1 = 0;
                column1 = 0;
                return false;
            }

            if (column > 0)
            {
                row1 = row;
                column1 = column - 1;
                return true;
            }

            row1 = row - 1;
            if (row1 >= w.Text.LinesCount)
                column1 = 0;
            else
                column1 = w.Text.LineLength(row1, false);
            return true;
        }
    }
}
