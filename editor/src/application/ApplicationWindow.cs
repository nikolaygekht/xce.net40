using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.editor.configuration;
using gehtsoft.xce.editor.textwindow;

namespace gehtsoft.xce.editor.application
{
    internal class ApplicationWindow : Window
    {
        private Application mApp;
        private StatusLineWindow mStatusLine;

        public ApplicationWindow(Application app)
        {
            mApp = app;
            mStatusLine = new StatusLineWindow(app);
        }

        public void create()
        {
            mApp.WindowManager.create(this, null, 0, 0, mApp.WindowManager.ScreenHeight, mApp.WindowManager.ScreenWidth);
        }

        public override void OnCreate()
        {
            mApp.WindowManager.create(mStatusLine, this, 0, 0, 1, this.Width);
            mStatusLine.show(true);
            show(true);
            mApp.WindowManager.setFocus(this);
        }

        public override void OnSetFocus()
        {
            if (mApp.ActiveWindow != null)
            {
                mApp.ActiveWindow.ShowCaret();
                mApp.ActiveWindow.invalidate();
            }
        }

        public override void OnSizeChanged()
        {
            mStatusLine.move(0, 0, 1, this.Width);
            invalidate();
            mStatusLine.invalidate();
            foreach (TextWindow tw in mApp.TextWindows)
            {
                tw.move(mStatusLine.Height, 0, Height - mStatusLine.Height, Width);
                tw.invalidate();
            }
        }

        public override void OnScreenSizeChanged(int height, int width)
        {
            move(0, 0, height, width);
        }

        public override void OnKeyPressed(int scanCode, char character, bool shift, bool ctrl, bool alt)
        {
            bool handled = false;
            mApp.FireKeyPressedEvent(scanCode, character, shift, ctrl, alt, ref handled);
            if (handled)
                return ;
        
            KeyboardShortcut shortcut = mApp.Keymap.Find(ctrl, alt, shift, scanCode);
            if (shortcut != null)
            {
                shortcut.Command.Execute(mApp, shortcut.Parameter);
                invalidate();
                mStatusLine.invalidate();
                return ;
            }
            else if (character >= 0x20 && !ctrl && !alt)
            {
                //TODO: pass character to an editor window
                if (mApp.ActiveWindow != null)
                {
                    mApp.ActiveWindow.Stroke(character);
                }
                invalidate();
                mStatusLine.invalidate();
            }
        }

        public override void OnMouseWheelUp(int row, int column, bool shift, bool ctrl, bool alt)
        {
            if (mApp.ActiveWindow != null)
                mApp.ActiveWindow.OnMouseWheelUp(row, column, shift, ctrl, alt);
        }

        public override void OnMouseWheelDown(int row, int column, bool shift, bool ctrl, bool alt)
        {
            if (mApp.ActiveWindow != null)
                mApp.ActiveWindow.OnMouseWheelDown(row, column, shift, ctrl, alt);
        }

        public override void OnKeyboardLayoutChanged()
        {
            mStatusLine.invalidate();
        }


        new public void invalidate()
        {
            base.invalidate();
            mStatusLine.invalidate();
        }

        public int StatusLineHeight
        {
            get
            {
                return mStatusLine.Height;
            }
        }

        public string StatusLine
        {
            get
            {
                return mStatusLine.Message;
            }
            set
            {
                if (value == null)
                    throw new ArgumentException("value");
                mStatusLine.Message = value;
                mStatusLine.invalidate();
            }
        }
    }
}
