using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.text;
using gehtsoft.xce.editor.textwindow;

namespace gehtsoft.xce.extension.intellisense_impl
{
    internal class TooltipWindow : Window
    {
        private static TooltipWindow mTooltip = null;

        internal static TooltipWindow Tooltip
        {
            get
            {
                return mTooltip;
            }
        }

        protected Application mApplication;
        private TextWindow mActiveWindow;
        private KeyPressedHook mKeyPressedHook;
        private IdleHook mIdleHook;
        private int mHookLine;
        private int mHookColumn;
        private int mPrevTopRow, mPrevTopColumn;

        protected virtual int HookLine
        {
            get
            {
                return mHookLine;
            }
        }

        protected virtual int HookColumn
        {
            get
            {
                return mHookColumn;
            }
        }

        internal TooltipWindow(Application application) : this(application, 0, 0)
        {
        }

        internal TooltipWindow(Application application, int hookLine, int hookColumn) : base()
        {
            mApplication = application;
            mActiveWindow = application.ActiveWindow;
            mHookLine = hookLine;
            mHookColumn = hookColumn;
        }

        internal void create()
        {
            if (mTooltip != null)
                mApplication.WindowManager.close(mTooltip);
            mApplication.WindowManager.create(this, null, 0, 0, 0, 0);
        }

        public override void OnPaint(Canvas canvas)
        {
            base.OnPaint(canvas);
            canvas.box(0, 0, Height, Width, BoxBorder.SingleBorderBox, mApplication.ColorScheme.DialogBorder, ' ');
        }

        public override void OnCreate()
        {
            base.OnCreate();
            mApplication.KeyPressedEvent += (mKeyPressedHook = new KeyPressedHook(OnKeyPressed));
            mApplication.IdleEvent += (mIdleHook = new IdleHook(OnIdle));
            mTooltip = this;
            requestSizeAndPosition(0, 0);
        }

        public override void OnClose()
        {
            mApplication.KeyPressedEvent -= mKeyPressedHook;
            mApplication.IdleEvent -= mIdleHook;
            mKeyPressedHook = null;
            mTooltip = null;
            base.OnClose();
        }

        protected virtual bool HandleKeyPress(int scanCode,char character, bool shift, bool ctrl, bool alt)
        {
            return false;
        }

        protected void requestPosition()
        {
            requestSizeAndPosition(Height - 2, Width - 2);
        }

        protected void requestSizeAndPosition(int rows, int columns)
        {
            //expand size for the border
            rows += 2;
            columns += 2;
            //calculate the position
            int baseLine, baseColumn, textHeight, textWidth;
            int screenHookLine;
            int screenHookColumn;

            textHeight = mApplication.ActiveWindow.Height;
            textWidth = mApplication.ActiveWindow.Width;
            if (columns > textWidth)
                columns = textWidth;
            mApplication.ActiveWindow.windowToScreen(0, 0, out baseLine, out baseColumn);
            screenHookLine = HookLine - mApplication.ActiveWindow.TopRow;
            screenHookColumn = HookColumn - mApplication.ActiveWindow.TopColumn;
            if (screenHookLine < 0 || screenHookLine >= textHeight ||
                screenHookColumn < 0 || screenHookColumn >= textWidth)
            {
                show(false);
                return ;
            }
            mPrevTopRow = mApplication.ActiveWindow.TopRow;
            mPrevTopColumn = mApplication.ActiveWindow.TopColumn;


            screenHookLine += baseLine;
            screenHookColumn += baseColumn;

            int line, column;

            line = screenHookLine - rows;
            if (line < baseLine)
            {
                line = screenHookLine + 1;
                if (line + rows > textHeight - baseLine)
                    rows = textHeight - baseLine;
            }

            column = screenHookColumn;
            if (column + columns > textWidth)
                column = textWidth - columns;

            this.move(line, column, rows, columns);
        }

        private void OnKeyPressed(int scanCode, char character, bool shift, bool ctrl, bool alt, ref bool handled)
        {
            if (!handled)
                handled = HandleKeyPress(scanCode, character, shift, ctrl, alt);
        }

        protected virtual bool HandleIdle()
        {
            return false;
        }

        private void OnIdle()
        {
            if (!Visible)
            {
                mApplication.WindowManager.close(this);
                return ;
            }
            else if (mActiveWindow != mApplication.ActiveWindow)
            {
                mApplication.WindowManager.close(this);
                return ;
            }
            else if (HandleIdle())
            {
                mApplication.WindowManager.close(this);
                return ;
            }

            if (mPrevTopColumn != mApplication.ActiveWindow.TopColumn ||
                     mPrevTopRow != mApplication.ActiveWindow.TopRow)
                requestPosition();
        }
    }
}
