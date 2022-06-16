using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.configuration;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.extension.advnav_commands;
using gehtsoft.xce.editor.configuration;
using gehtsoft.xce.colorer;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.text;
using ConsoleColor = gehtsoft.xce.conio.ConsoleColor;

namespace gehtsoft.xce.extension.advnav_condensed
{
    internal struct CondensedLine
    {
        private int mLine;
        private int mPosition;

        internal int Line
        {
            get
            {
                return mLine;
            }
        }

        internal int Position
        {
            get
            {
                return mPosition;
            }
        }

        internal CondensedLine(int line)
        {
            mLine = line;
            mPosition = 0;
        }

        internal CondensedLine(int line, int position)
        {
            mLine = line;
            mPosition = position;
        }

    }


    internal class CondensedViewWindow : Window
    {
        private XceColorScheme mColors;
        private SyntaxHighlighter mHighlighter;
        private XceFileBuffer mBuffer;
        private List<CondensedLine> mLinesToShow;
        private int mTopLine;
        private int mSelLine;
        private bool mSelMade;
        private WindowManager mManager;

        internal CondensedViewWindow(XceFileBuffer buffer, List<CondensedLine> linesToShow, int curline, SyntaxHighlighter highlighter, XceColorScheme colors)
        {
            mBuffer = buffer;
            mLinesToShow = linesToShow;
            mHighlighter = highlighter;
            mColors = colors;
            mSelMade = false;
            mManager = null;

            mTopLine = 0;
            mSelLine = 0;
            for (int i = 0; i < linesToShow.Count; i++)
            {
                if (linesToShow[i].Line <= curline)
                    mSelLine = i;
            }
        }

        public CondensedLine SelSourceLine
        {
            get
            {
                return mLinesToShow[mSelLine];
            }
        }

        public override void  OnCreate()
        {
            base.OnCreate();
            OnSizeChanged();
        }

        public override void OnSizeChanged()
        {
            base.OnSizeChanged();
            if (Height > 0)
            {
                if (mTopLine > mSelLine)
                {
                    mTopLine = mSelLine;
                }
                else if (mTopLine + (Height - 2) <= mSelLine)
                {
                    mTopLine = mSelLine - (Height - 2);
                }
            }
        }

        public override void OnPaint(Canvas canvas)
        {
            base.OnPaint(canvas);
            int i, row, length;
            bool rc;
            AutoArray<char> buffer = new AutoArray<char>();
            buffer.Ensure(Width);
            string s;
            s = String.Format("Condensed View: {0,6} of {1,6} src line {2,6}", mSelLine + 1, mLinesToShow.Count, mLinesToShow[mSelLine].Line + 1);
            canvas.fill(0, 0, 1, Width, ' ', mColors.MenuItem);
            canvas.write(0, 0, s);
            canvas.fill(1, 0, Height - 1, Width, ' ', mColors.TextColor);
            mManager.setCaretPos(this, mSelLine - mTopLine + 1, 0);
            ConsoleColor clr = new ConsoleColor(0);
            for (i = mTopLine, row = 1; i < mLinesToShow.Count && row < Height; i++, row++)
            {
                if (i == mSelLine)
                    canvas.fill(row, 0, 1, Width, mColors.BlockTextColor);

                if (mLinesToShow[i].Line < mBuffer.LinesCount)
                    length = Math.Min(Width, mBuffer.LineLength(mLinesToShow[i].Line, false));
                else
                    length = 0;

                if (length > 0)
                {
                    mBuffer.GetRange(mBuffer.LineStart(mLinesToShow[i].Line), length, buffer.Array, 0);
                    canvas.write(row, 0, buffer.Array, 0, length);
                    if (i != mSelLine)
                    {
                        rc = mHighlighter.GetFirstRegion(mLinesToShow[i].Line);
                        while (rc)
                        {
                            SyntaxHighlighterRegion r = mHighlighter.CurrentRegion;
                            if (r.StartColumn < Width && r.HasColor)
                            {
                                length = r.Length;
                                if (length < 0)
                                    length = mBuffer.LineLength(mLinesToShow[i].Line, false);
                                if (r.StartColumn + length > Width)
                                    length = Width - r.StartColumn;
                                if (length > 0)
                                {
                                    clr.PaletteColor = r.ConsoleColor;
                                    clr.RGBForeground = r.ForeColor;
                                    clr.RGBBackground = r.BackColor;
                                    canvas.fill(row, r.StartColumn, 1, length, clr);
                                }
                            }
                            rc = mHighlighter.GetNextRegion();
                        }
                    }
                }
            }
        }


        public override void OnKeyPressed(int scanCode, char character, bool shift, bool ctrl, bool alt)
        {
            base.OnKeyPressed(scanCode, character, shift, ctrl, alt);
            if (scanCode == (int)ScanCode.UP && !ctrl && !alt && !shift)
            {
                if (mSelLine > 0)
                    mSelLine--;
                if (mTopLine > mSelLine)
                    mTopLine = mSelLine;
                invalidate();
            }
            else if (scanCode == (int)ScanCode.DOWN && !ctrl && !alt && !shift)
            {
                if (mSelLine < mLinesToShow.Count - 1)
                    mSelLine++;
                if (mTopLine + Height - 2 <= mSelLine)
                    mTopLine = mSelLine - (Height - 2);
                invalidate();
            }
            else if (scanCode == (int)ScanCode.PRIOR && !ctrl && !alt && !shift)
            {
                if (mSelLine > 0)
                    mSelLine -= Height - 1;
                if (mSelLine < 0)
                    mSelLine = 0;
                if (mTopLine > mSelLine)
                    mTopLine = mSelLine;
                invalidate();
            }
            else if (scanCode == (int)ScanCode.NEXT && !ctrl && !alt && !shift)
            {
                if (mSelLine < mLinesToShow.Count - 1)
                    mSelLine += Height - 1;
                if (mSelLine >= mLinesToShow.Count)
                    mSelLine = mLinesToShow.Count - 1;
                if (mTopLine + Height - 2 <= mSelLine)
                    mTopLine = mSelLine - (Height - 2);
                invalidate();
            }
            else if (scanCode == (int)ScanCode.HOME && !ctrl && !alt && !shift)
            {
                mSelLine = mTopLine = 0;
                invalidate();
            }
            else if (scanCode == (int)ScanCode.END && !ctrl && !alt && !shift)
            {
                mSelLine = mLinesToShow.Count - 1;
                mTopLine = mSelLine - (Height - 2);
                if (mTopLine < 0)
                    mTopLine = 0;
                invalidate();
            }
            else if (scanCode == (int)ScanCode.ESCAPE && !ctrl && !alt && !shift)
            {
                mManager.close(this);
            }
            else if (scanCode == (int)ScanCode.RETURN && !ctrl && !alt && !shift)
            {
                mSelMade = true;
                mManager.close(this);
            }
        }

        public bool DoModal(WindowManager manager)
        {
            int height = manager.ScreenHeight;
            int width = manager.ScreenWidth;
            mManager = manager;
            manager.doModal(this, 0, 0, height, width);
            return mSelMade;
        }
    }
}
