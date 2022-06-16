using System;
using System.Collections.Generic;
using System.Text;
using ConsoleColor = gehtsoft.xce.conio.ConsoleColor;

namespace gehtsoft.xce.conio.win
{
    public class WindowManager : IDisposable, ConsoleInputListener
    {
        private LinkedList<Window> mTopLevelWindows = new LinkedList<Window>();
        private Canvas mScreenCanvas;
        private ConsoleOutput mConsoleOutput;
        private ConsoleInput mConsoleInput;
        private bool mFastDrawMode;
        private bool mForceRedraw = false;
        private Window mFocusWindow;
        private LinkedList<Window> mModalStack = new LinkedList<Window>();
        private Window mCaptureMouseWindow = null;
        private Window mCaretWindow = null;
        private int mCaretRow, mCaretColumn;
        private int mLayoutCode;
        private KeyboardLayout mLayout = null;
        private static KeyboardLayouts mLayouts = new KeyboardLayouts();
        private static ConsoleColor mDefaultColor = new ConsoleColor(0x03);

        public bool FastDrawMode
        {
            get
            {
                return mFastDrawMode;
            }
            set
            {
                mFastDrawMode = value;
            }
        }

        public int ScreenHeight
        {
            get
            {
                return mConsoleOutput.Rows;
            }
        }

        public int ScreenWidth
        {
            get
            {
                return mConsoleOutput.Columns;
            }
        }

        public KeyboardLayout KeyboardLayout
        {
            get
            {
                if (mLayout == null || mLayout.LayoutCode != mLayoutCode)
                    mLayout = mLayouts[mLayoutCode];
                return mLayout;
            }
        }

        #region constructor/destuctor
        public WindowManager(bool save)
        {
            mConsoleOutput = new ConsoleOutput(save);
            mScreenCanvas = new Canvas(mConsoleOutput.Rows, mConsoleOutput.Columns);
            mConsoleInput = new ConsoleInput();
            mLayoutCode = mConsoleInput.CurrentLayout;
            showCaret(false);
        }

        public WindowManager(bool save, int rows, int columns)
        {
            mConsoleOutput = new ConsoleOutput(save, rows, columns);
            mScreenCanvas = new Canvas(mConsoleOutput.Rows, mConsoleOutput.Columns);
            mConsoleInput = new ConsoleInput();
            mLayoutCode = mConsoleInput.CurrentLayout;
            showCaret(false);
        }

        ~WindowManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            foreach (Window window in mTopLevelWindows)
                window.close();
            mTopLevelWindows.Clear();

            if (mConsoleInput != null)
                mConsoleInput.Dispose();
            mConsoleInput = null;

            if (mConsoleOutput != null)
                mConsoleOutput.Dispose();
            mConsoleOutput = null;

            if (mScreenCanvas != null)
                mScreenCanvas.Dispose();
            mScreenCanvas = null;
        }
        #endregion

        #region window management
        /// <summary>
        /// Create new window
        /// </summary>
        /// <param name="window">window object</param>
        /// <param name="parent">parent window or null to create a top-level windiow</param>
        /// <param name="row">row of top-left corner</param>
        /// <param name="column">column of top-left corner</param>
        /// <param name="height">height of the window</param>
        /// <param name="width">width of the window</param>
        public void create(Window window, Window parent, int row, int column, int height, int width)
        {
            if (window == null)
                throw new ArgumentNullException("window");
            window.Manager = this;
            window.create(parent);
            window.move(row, column, height, width);
            if (parent == null)
                mTopLevelWindows.AddLast(window);
        }

        public void doModal(Window window, int row, int column, int height, int width)
        {
            if (window == null)
                throw new ArgumentNullException("window");
            createModal(window, row, column, height, width);
            Window focus = mFocusWindow;
            if (focus != null)
                focus.OnKillFocus();
            window.show(true);
            setFocus(window);
            while (window.Exists)
                pumpMessage(250);
            mFocusWindow = focus;
            if (mFocusWindow != null)
                mFocusWindow.OnSetFocus();
        }

        public void showPopupMenu(PopupMenu menu, int row, int column)
        {
            int width, height;
            menu.doLayout(out height, out width);
            doModal(menu, row, column, height, width);
        }

        public void createModal(Window window, int row, int column, int height, int width)
        {
            mModalStack.AddLast(window);
            create(window, null, row, column, height, width);
        }

        public void close(Window window)
        {
            if (window == null)
                throw new ArgumentNullException("window");
            if (window.Exists)
            {
                if (window.Parent == null)
                {
                    LinkedListNode<Window> _window = mTopLevelWindows.Find(window);
                    if (_window != null)
                        mTopLevelWindows.Remove(_window);
                    _window = mModalStack.Find(window);
                    if (_window != null)
                        mModalStack.Remove(_window);
                    mForceRedraw = true;
                }
                window.close();
            }
            if (mFocusWindow == window)
                mFocusWindow = null;
            if (mCaptureMouseWindow == window)
                mCaptureMouseWindow = null;
        }

        /// <summary>
        /// Set focus to a window
        /// </summary>
        /// <param name="window"></param>
        public void setFocus(Window window)
        {
            if (window != null && mModalStack.Count > 0)
            {
                Window lastModal = mModalStack.Last.Value;
                if (window != lastModal && !window.hasParent(lastModal))
                    return ;
            }

            if (mFocusWindow != null)
                mFocusWindow.OnKillFocus();
            mFocusWindow = window;
            if (mFocusWindow != null)
                mFocusWindow.OnSetFocus();
        }

        public Window getFocus()
        {
            return mFocusWindow;
        }

        public bool captureMouse(Window window)
        {
            if (mCaptureMouseWindow != null)
                return false;
            mCaptureMouseWindow = window;
            return true;
        }

        public void releaseMouse(Window window)
        {
            if (mCaptureMouseWindow == window)
                mCaptureMouseWindow = null;
        }

        /// <summary>
        /// Get window by position
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public Window windowFromPos(int row, int column, bool visibleOnly)
        {
            for (LinkedListNode<Window> n = mTopLevelWindows.Last; n != null; n = n.Previous)
            {
                Window window = n.Value;
                if (visibleOnly && !window.Visible)
                    continue;
                 int windowRow, windowColumn;
                if (window.screenToWindow(row, column, out windowRow, out windowColumn))
                    return window.childFromPos(windowRow, windowColumn, visibleOnly);
            }
            return null;
        }

        /// <summary>
        /// Get window by position
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public Window windowFromPos(int row, int column)
        {
            return windowFromPos(row, column, false);
        }

        public void bringToFront(Window window)
        {
            if (window.Parent == null)
            {
                LinkedListNode<Window> _window = mTopLevelWindows.Find(window);
                if (_window != null)
                {
                    mTopLevelWindows.Remove(_window);
                    mTopLevelWindows.AddLast(window);
                    mForceRedraw = true;
                }
            }
            else
            {
                window.Parent.bringToFront(window);
            }
        }
        #endregion



        #region message
        public void pumpMessage(int timeout)
        {
            int layout = mConsoleInput.CurrentLayout;
            if (layout != mLayoutCode)
            {
                mLayoutCode = layout;
                if (mFocusWindow != null)
                    mFocusWindow.OnKeyboardLayoutChanged();
            }

            bool paint = false;

            if (mForceRedraw)
            {
                mForceRedraw = false;
                paint = true;
            }
            else
            {
                foreach (Window window in mTopLevelWindows)
                {
                    if (!window.Valid && window.Visible)
                    {
                        paint = true;
                        break;
                    }
                }
            }

            if (paint)
            {
                mScreenCanvas.fill(0, 0, mScreenCanvas.Rows, mScreenCanvas.Columns, ' ', mDefaultColor);
                foreach (Window window in mTopLevelWindows)
                {
                    if (window.Visible)
                    {
                        window.paint();
                        Canvas canvas = window.Canvas;
                        if (canvas != null)
                            mScreenCanvas.paint(window.Row, window.Column, canvas);
                    }
                }
                mConsoleOutput.paint(mScreenCanvas, mFastDrawMode);
            }
            updateCaretPos();
            mConsoleInput.read(this, timeout);
        }
        #endregion

        #region input events
        public void onKeyPressed(int scanCode, char character, bool shift, bool ctrl, bool alt)
        {
            if (mFocusWindow != null)
                mFocusWindow.OnKeyPressed(scanCode, character, shift, ctrl, alt);

            int layout = mConsoleInput.CurrentLayout;
            if (layout != mLayoutCode)
            {
                mLayoutCode = layout;
                if (mFocusWindow != null)
                    mFocusWindow.OnKeyboardLayoutChanged();
            }
        }

        public void onKeyReleased(int scanCode, char character, bool shift, bool ctrl, bool alt)
        {
            int layout = mConsoleInput.CurrentLayout;
            if (layout != mLayoutCode)
            {
                mLayoutCode = layout;
                if (mFocusWindow != null)
                    mFocusWindow.OnKeyboardLayoutChanged();
            }
        }

        public void onMouseMove(int row, int column, bool shift, bool ctrl, bool alt, bool lb, bool rb)
        {
            if (mCaptureMouseWindow != null)
            {
                mCaptureMouseWindow.OnMouseMove(row, column, shift, ctrl, alt, lb, rb);
                return ;
            }

            Window window = windowFromPos(row, column, true);
            if (window != null)
            {
                int windowRow, windowColumn;
                window.screenToWindow(row, column, out windowRow, out windowColumn);
                window.OnMouseMove(windowRow, windowColumn, shift, ctrl, alt, lb, rb);
            }
        }

        public void onMouseLButtonDown(int row, int column, bool shift, bool ctrl, bool alt)
        {
            if (mCaptureMouseWindow != null)
            {
                mCaptureMouseWindow.OnMouseLButtonDown(row, column, shift, ctrl, alt);
                return ;
            }

            Window window = windowFromPos(row, column, true);
            if (window != null)
            {
                int windowRow, windowColumn;
                window.screenToWindow(row, column, out windowRow, out windowColumn);
                window.OnMouseLButtonDown(windowRow, windowColumn, shift, ctrl, alt);
            }

        }

        public void onMouseLButtonUp(int row, int column, bool shift, bool ctrl, bool alt)
        {
            if (mCaptureMouseWindow != null)
            {
                mCaptureMouseWindow.OnMouseLButtonUp(row, column, shift, ctrl, alt);
                return;
            }

            Window window = windowFromPos(row, column, true);
            if (window != null)
            {
                int windowRow, windowColumn;
                window.screenToWindow(row, column, out windowRow, out windowColumn);
                window.OnMouseLButtonUp(windowRow, windowColumn, shift, ctrl, alt);
            }
        }

        public void onMouseRButtonDown(int row, int column, bool shift, bool ctrl, bool alt)
        {
            if (mCaptureMouseWindow != null)
            {
                mCaptureMouseWindow.OnMouseRButtonDown(row, column, shift, ctrl, alt);
                return;
            }

            Window window = windowFromPos(row, column, true);
            if (window != null)
            {
                int windowRow, windowColumn;
                window.screenToWindow(row, column, out windowRow, out windowColumn);
                window.OnMouseRButtonDown(windowRow, windowColumn, shift, ctrl, alt);
            }
        }

        public void onMouseRButtonUp(int row, int column, bool shift, bool ctrl, bool alt)
        {
            if (mCaptureMouseWindow != null)
            {
                mCaptureMouseWindow.OnMouseRButtonUp(row, column, shift, ctrl, alt);
                return;
            }

            Window window = windowFromPos(row, column, true);
            if (window != null)
            {
                int windowRow, windowColumn;
                window.screenToWindow(row, column, out windowRow, out windowColumn);
                window.OnMouseRButtonUp(windowRow, windowColumn, shift, ctrl, alt);
            }
        }

        public void onMouseWheelUp(int row, int column, bool shift, bool ctrl, bool alt)
        {
            if (mFocusWindow != null)
            {
                int windowRow, windowColumn;
                mFocusWindow.screenToWindow(row, column, out windowRow, out windowColumn);
                mFocusWindow.OnMouseWheelUp(windowRow, windowColumn, shift, ctrl, alt);
            }
        }

        public void onMouseWheelDown(int row, int column, bool shift, bool ctrl, bool alt)
        {
            if (mFocusWindow != null)
            {
                int windowRow, windowColumn;
                mFocusWindow.screenToWindow(row, column, out windowRow, out windowColumn);
                mFocusWindow.OnMouseWheelDown(windowRow, windowColumn, shift, ctrl, alt);
            }
        }

        public void onGetFocus(bool shift, bool ctrl, bool alt)
        {
            int layout = mConsoleInput.CurrentLayout;
            if (layout != mLayoutCode)
            {
                mLayoutCode = layout;
                if (mFocusWindow != null)
                    mFocusWindow.OnKeyboardLayoutChanged();
            }
        }

        public void onScreenBufferChanged(int rows, int columns)
        {
            mConsoleOutput.updateSize();
            mScreenCanvas.Dispose();
            mScreenCanvas = new Canvas(mConsoleOutput.Rows, mConsoleOutput.Columns);
            foreach (Window window in mTopLevelWindows)
                window.OnScreenSizeChanged(rows, columns);
        }
        #endregion

        #region caret
        public void setCaretPos(Window caretWindow, int row, int column)
        {
            mCaretWindow = caretWindow;
            mCaretRow = row;
            mCaretColumn = column;
            updateCaretPos();
        }

        private void updateCaretPos()
        {
            int caretType;
            bool visible;
            int crow, ccolumn;
            mConsoleOutput.getCursorType(out caretType, out visible);
            if (visible)
            {
                mConsoleOutput.getCursorPos(out crow, out ccolumn);
                int row, column;
                if (mCaretWindow != null && mCaretWindow.Exists)
                    mCaretWindow.windowToScreen(mCaretRow, mCaretColumn, out row, out column);
                else
                {
                    row = mCaretRow;
                    column = mCaretColumn;
                }
                if (crow != row || ccolumn != column)
                    mConsoleOutput.setCursorPos(row, column);
            }
        }

        public void showCaret(bool show)
        {
            int caretType;
            bool visible;
            mConsoleOutput.getCursorType(out caretType, out visible);
            mConsoleOutput.setCursorType(caretType, show);
        }

        public void setCaretType(int caretSize, bool show)
        {
            mConsoleOutput.setCursorType(caretSize, show);
        }
        #endregion
    }
}
