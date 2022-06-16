using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.intellisense.common;

namespace gehtsoft.xce.extension.intellisense_impl
{
    internal class InsightWindow : TooltipWindow
    {
        internal static InsightWindow GetInsightWindow(Application application)
        {
            if (Tooltip != null && Tooltip is InsightWindow)
                return Tooltip as InsightWindow;

            if (Tooltip != null)
                application.WindowManager.close(Tooltip);

            InsightWindow w = new InsightWindow(application);
            w.create();
            return w;
        }

        private Stack<IInsightDataProvider> mProviders = new Stack<IInsightDataProvider>();
        private int mCursorLine, mCursorColumn;

        protected override int HookLine
        {
            get
            {
                if (mProviders.Count > 0)
                    return mProviders.Peek().StartRow;
                else
                    return 0;
            }
        }

        protected override int HookColumn
        {
            get
            {
                if (mProviders.Count > 0)
                    return mProviders.Peek().StartColumn;
                else
                    return 0;
            }
        }

        internal InsightWindow(Application application) : base(application)
        {
            mCursorLine = application.ActiveWindow.CursorRow;
            mCursorColumn = application.ActiveWindow.CursorColumn;
        }

        internal void AddInsightProvide(IInsightDataProvider provider)
        {
            if (!Visible)
                show(true);
            mProviders.Push(provider);
            UpdateTip();
        }

        internal void Restore()
        {
            if (mProviders != null && mProviders.Count > 0)
            {
                create();
                mCursorLine = mApplication.ActiveWindow.CursorRow;
                mCursorColumn = mApplication.ActiveWindow.CursorColumn;
                show(true);
                UpdateTip();
            }
        }

        private void UpdateTip()
        {
            requestSizeAndPosition(1, mProviders.Peek()[mProviders.Peek().CurSel].Tip.Length);
            invalidate();
        }

        private bool mKeepDataOnClose = false;

        public bool KeepDataOnClose
        {
            get
            {
                return mKeepDataOnClose;
            }
            set
            {
                mKeepDataOnClose = value;
            }
        }

        public override void OnClose()
        {
            base.OnClose();
            if (!mKeepDataOnClose)
            {
                mProviders = null;
                GC.Collect();
            }
        }

        public override void OnPaint(Canvas canvas)
        {
            base.OnPaint(canvas);
            if (mProviders.Count > 0)
            {
                IInsightDataProvider p = mProviders.Peek();
                string s = string.Format("[{0}/{1}]", p.CurSel + 1, p.Count);
                canvas.write(0, 2, s);
                s = p[p.CurSel].Tip;
                if (s.Length > Width - 2)
                {
                    int  l = Width - 5;
                    if (l > 0)
                    {
                        canvas.write(1, 1, s, 0, l);
                        canvas.write(1, 1 + l, "...");
                    }
                }
                else
                {
                    canvas.write(1, 1, s);
                }

            }
        }

        protected override bool HandleIdle()
        {
            bool rc = base.HandleIdle();
            if (!rc)
            {
                if (mProviders.Count < 1)
                    return true;

                if (mApplication.ActiveWindow.CursorRow != mCursorLine ||
                    mApplication.ActiveWindow.CursorColumn != mCursorColumn)
                {
                    mCursorLine = mApplication.ActiveWindow.CursorRow;
                    mCursorColumn = mApplication.ActiveWindow.CursorColumn;
                    int args;
                    rc = mProviders.Peek().CursorPositionChanged(mApplication.ActiveWindow.CursorRow, mApplication.ActiveWindow.CursorColumn, out args);
                    if (rc)
                    {
                        mProviders.Pop();
                        if (mProviders.Count < 1)
                            return true;
                        else
                        {
                            UpdateTip();
                            return false;
                        }
                    }
                    else
                    {
                        if (mProviders != null && mProviders.Count > 0)
                        {
                            IInsightDataProvider p = mProviders.Peek();
                            IInsightData data;

                            data = p[p.CurSel];
                            if (data.ArgsCount < args)
                            {
                                int minMatching = 10000;
                                int minMatchingIndex = -1;
                                for (int i = 0; i < p.Count; i++)
                                {
                                    if (p[i].ArgsCount >= args)
                                    {
                                        if (p[i].ArgsCount < minMatching)
                                        {
                                            minMatching = p[i].ArgsCount;
                                            minMatchingIndex = i;
                                        }
                                    }
                                }
                                if (minMatchingIndex >= 0)
                                {
                                    p.CurSel = minMatchingIndex;
                                    UpdateTip();

                                }
                            }
                        }
                    }
                    return false;
                }
                else
                    return false;
            }
            else
                return true;
        }

        protected override bool HandleKeyPress(int scanCode, char character, bool shift, bool ctrl, bool alt)
        {
            if (mProviders == null || mProviders.Count < 1)
                return false;

            if (!shift && !ctrl && !alt && scanCode == (int)ScanCode.UP)
            {
                IInsightDataProvider p = mProviders.Peek();
                if (p.Count > 1)
                {
                    if (p.CurSel >= 1)
                        p.CurSel--;
                    else
                        p.CurSel = p.Count - 1;
                    UpdateTip();
                }
                return true;
            }
            else if (!shift && !ctrl && !alt && scanCode == (int)ScanCode.DOWN)
            {
                IInsightDataProvider p = mProviders.Peek();
                if (p.Count > 1)
                {
                    if (p.CurSel < p.Count - 1)
                        p.CurSel++;
                    else
                        p.CurSel = 0;
                    UpdateTip();
                }
                return true;
            }
            else if (scanCode == (int)ScanCode.ESCAPE)
            {
                show(false);
                return true;
            }
            else
                return false;
        }
    }
}
