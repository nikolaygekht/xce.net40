using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.editor.textwindow;

namespace gehtsoft.xce.editor.application
{
    internal class StatusLineWindow : Window
    {
        private Application mApp;
        private string mMessage;

        public StatusLineWindow(Application app) : base()
        {
            mMessage = "";
            mApp = app;
        }

        public string Message
        {
            get
            {
                return mMessage;
            }
            set
            {
                mMessage = value;
                invalidate();
            }
        }

        public override void OnMouseLButtonUp(int row, int column, bool shift, bool ctrl, bool alt)
        {
            if (row == 0 && column >= 0 && column < 3 && !shift && !ctrl && !alt)
                mApp.ShowMainMenu();
        }

        public override void OnPaint(Canvas canvas)
        {
            canvas.fill(0, 0, this.Height, this.Width, ' ', mApp.ColorScheme.StatusLineColor);
            canvas.write(0, 0, "[\u25a0] | ");

            if (mApp.ActiveWindow != null)
            {
                MakeStatusLine(Width - 6);
                canvas.write(0, 6, mStatusLine, 0, this.Width - 6);
            }
        }

        char[] mStatusLine;

        public override void OnSizeChanged()
        {
            if ((mStatusLine == null || mStatusLine.Length != Width) && Width > 6)
                mStatusLine = new char[Width - 6];
            this.invalidate();
        }

        private void MakeStatusLine(int width)
        {
            if ((mStatusLine == null || mStatusLine.Length != width) && width > 0)
                mStatusLine = new char[width];
            int i, l;

            l = mStatusLine.Length;
            for (i = 0; i < l; i++)
                mStatusLine[i] = ' ';

            if (mApp.ActiveWindow == null)
                return;

            mStatusLine[6] = '/';
            mStatusLine[13] = ':';
            mStatusLine[22] = '\'';
            mStatusLine[24] = '\'';
            mStatusLine[25] = '(';
            mStatusLine[26] = 'u';
            mStatusLine[31] = ')';
            mStatusLine[42] = '|';

            char c = mApp.ActiveWindow[mApp.ActiveWindow.CursorRow, mApp.ActiveWindow.CursorColumn];
            if (c == (char)0)
            {
                mStatusLine[23] = ' ';
                mStatusLine[27] = ' ';
                mStatusLine[28] = ' ';
                mStatusLine[29] = ' ';
                mStatusLine[30] = ' ';
            }
            else
            {
                mStatusLine[23] = c;
                WriteStatusLineHex((ushort)c, 27, 4);
            }
            WriteStatusLineInt(mApp.ActiveWindow.CursorRow + 1, 0, 6);
            WriteStatusLineInt(mApp.ActiveWindow.Text.LinesCount, 7, 6);
            WriteStatusLineInt(mApp.ActiveWindow.CursorColumn + 1, 14, 5);
            WriteStatusLineInt(mApp.ActiveWindow.Text.Encoding.CodePage, 37, 5);

            switch (mApp.ActiveWindow.BlockType)
            {
            case    TextWindowBlock.Stream:
                    mStatusLine[20] = 'S';
                    break;
            case    TextWindowBlock.Box:
                    mStatusLine[20] = 'B';
                    break;
            case    TextWindowBlock.Line:
                    mStatusLine[20] = 'L';
                    break;
            default:
                    mStatusLine[20] = ' ';
                    break;
            }


            if (!mApp.ActiveWindow.Text.AtSavePoint)
                mStatusLine[33] = '*';
            if (mApp.ActiveWindow.InsertMode)
                mStatusLine[35] = 'I';
            else
                mStatusLine[35] = 'O';

            string f = mApp.ActiveWindow.Text.FileName;
            if (f == "")
                WriteStatusLineString("<< new file >>", 46, mStatusLine.Length - 50);
            else
                WriteStatusLineString(f, 46, mStatusLine.Length - 50);
            mStatusLine[43] = '[';
            mStatusLine[44] = mApp.ActiveWindow.Id;
            mStatusLine[45] = ']';

            int t = mStatusLine.Length - 4;
            KeyboardLayout layout = mApp.WindowManager.KeyboardLayout;
            mStatusLine[t] = '[';
            mStatusLine[t + 1] = layout.LayoutName[0];
            mStatusLine[t + 2] = layout.LayoutName[1];
            mStatusLine[t + 3] = ']';
        }

        private void WriteStatusLineString(string value, int from, int max)
        {
            int valueFrom = 0;
            int valueCount = value.Length;

            if (valueCount > max)
            {
                valueFrom = valueCount - max + 3;
                valueCount = max - 3;
                mStatusLine[from] = '.';
                mStatusLine[from + 1] = '.';
                mStatusLine[from + 2] = '.';
                from += 3;
            }

            for (int i = 0; i < valueCount; i++)
                mStatusLine[from + i] = value[valueFrom + i];
        }

        private void WriteStatusLineInt(int value, int from, int max)
        {
            bool s = false;
            if (value < 0)
            {
                s = true;
                value = -value;
            }
            int p = from + max - 1;
            do {
                mStatusLine[p] = (char)((value % 10) + 0x30);
                value /= 10;
                p--;
            } while (value > 0 && p >= from);
            if (p > from && s)
                mStatusLine[p] = '-';
        }

        private void WriteStatusLineHex(ushort value, int from, int max)
        {
            int p = from + max - 1;
            do
            {
                ushort x = (ushort)(value % 16);
                if (x < 10)
                    mStatusLine[p] = (char)(x + 0x30);
                else
                    mStatusLine[p] = (char)(x - 10 + 0x61);
                value /= 16;
                p--;
            } while (value > 0 && p >= from);
        }



    }
}
