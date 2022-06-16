using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using ConsoleColor = gehtsoft.xce.conio.ConsoleColor;

namespace test1
{
    class MyWindow : Window
    {
        private bool mInFocus;
        private string mLastMessage;
        private ConsoleColor mWindowColor;
        private bool mCtrl;

        internal MyWindow(ConsoleColor windowColor, bool ctrl)
        {
            mInFocus = false;
            mWindowColor = windowColor;
            mLastMessage = "((none))";
            mCtrl = ctrl;
        }

        public override void OnPaint(Canvas canvas)
        {
            canvas.fill(0, 0, Height, Width, ' ', mWindowColor);
            if (mInFocus)
                canvas.write(0, 0, "in focus");
            else
                canvas.write(0, 0, "not focus");
            canvas.write(1, 0, mLastMessage);
        }

        public override void OnKeyPressed(int scanCode, char character, bool shift, bool ctrl, bool alt)
        {
            mLastMessage = string.Format("key: {0} ({1})", scanCode, character < 20 ? ' ' : character);
            invalidate();
            if (scanCode == ScanCode.ESCAPE)
            {
                if (mCtrl)
                    Program.WindowManager.close(Parent);
                else
                    Program.PostQuitMessage();
            }
        }

        public override void OnMouseLButtonDown(int row, int column, bool shift, bool ctrl, bool alt)
        {
            Program.WindowManager.setFocus(this);
            mLastMessage = string.Format("lbd: {0} {1}", row, column);
        }

        public override void OnMouseMove(int row, int column, bool shift, bool ctrl, bool alt, bool leftButton, bool rightButton)
        {
            mLastMessage = string.Format("mm: {0} {1}", row, column);
            invalidate();
        }

        public override void OnMouseRButtonDown(int row, int column, bool shift, bool ctrl, bool alt)
        {
            MyModal dlg = new MyModal(new ConsoleColor(0x60));
            Program.WindowManager.createModal(dlg, 20, 20, 20, 20);
            dlg.show(true);
        }

        public override void  OnSetFocus()
        {
             mInFocus = true;
             Program.WindowManager.setCaretPos(this, 0, 0);
             Program.WindowManager.showCaret(true);
             invalidate();
        }

        public override void OnClose()
        {
            if (mInFocus)
                Program.WindowManager.showCaret(false);
        }

        public override void OnSizeChanged()
        {
            if (mInFocus)
            {
            }
        }

        public override void  OnKillFocus()
        {
            mInFocus = false;
            Program.WindowManager.showCaret(false);
            invalidate();
        }
    }
}
