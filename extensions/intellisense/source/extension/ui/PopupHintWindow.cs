using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.text;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.extension;
using gehtsoft.xce.intellisense.common;

namespace gehtsoft.xce.extension.intellisense_impl
{
    internal class PopupHintWindow : TooltipWindow
    {
        private InsightWindow mInsight;
        private ICodeCompletionItemCollection mCodeCompletion;
        private IIntellisenseProvider mProvider;
        private SearchingListWindow mList;
        private int mStartLine, mStartColumn;
        private intellisense mExtension;

        internal PopupHintWindow(Application application, int line, int column, ICodeCompletionItemCollection codeCompletion, IIntellisenseProvider provider, intellisense extension)
            : base(application, line, column)
        {
            mExtension = extension;
            mProvider = provider;

            if (TooltipWindow.Tooltip != null && TooltipWindow.Tooltip is InsightWindow)
            {
                mInsight = TooltipWindow.Tooltip as InsightWindow;
                mInsight.KeepDataOnClose = true;
            }
            else
                mInsight = null;

            mCodeCompletion = codeCompletion;
            mList = new SearchingListWindow(application.ColorScheme);
            mList.CaptureFocusByMouse = false;
            mList.CaseSensitiveSearch = true;
            mList.StartOnlySearch = true;
            foreach (ICodeCompletionItem item in codeCompletion)
                AddItem(item);
            mList.EnsureVisible(0);
            mStartLine = line;
            mStartColumn = column;
        }

        private void AddItem(ICodeCompletionItem item)
        {
            string s = "";
            int index;
            switch (item.Type)
            {
            case CodeCompletionItemType.Text:
            case CodeCompletionItemType.Tag:
                    s = item.Name;
                    index = 0;
                    break;
            case CodeCompletionItemType.Class:
                    s = string.Format("c:{0}", item.Name);
                    index = 2;
                    break;
            case CodeCompletionItemType.Method:
                    if (item.OverloadsCount > 0)
                        s = string.Format("m:{0} +{1}", item.Name, item.OverloadsCount);
                    else
                        s = string.Format("m:{0}", item.Name);
                    index = 2;
                    break;
            case CodeCompletionItemType.Property:
                    if (item.OverloadsCount > 0)
                        s = string.Format("p:{0} +{1}", item.Name, item.OverloadsCount);
                    else
                        s = string.Format("p:{0}", item.Name);
                    index = 2;
                    break;
            case CodeCompletionItemType.Field:
                    s = string.Format("f:{0}", item.Name);
                    index = 2;
                    break;
            case CodeCompletionItemType.Event:
                    s = string.Format("e:{0}", item.Name);
                    index = 2;
                    break;
            default:
                    s = string.Format("?:{0}", item.Name);
                    index = 2;
                    break;
            }
            mList.AddItem(s, item, index);
        }

        new internal void create()
        {
            base.create();
            requestSizeAndPosition(10, 50);
        }


        public override void OnCreate()
        {
            base.OnCreate();
            mApplication.WindowManager.create(mList, this, 0, 0, Height, Width);
            mList.show(true);
            show(true);
        }

        public override void OnSizeChanged()
        {
            base.OnSizeChanged();
            mList.move(0, 0, Height, Width);
            if (mList.CurSel < 0 || mList.CurSel >= mList.Count)
                mList.EnsureVisible(0);
            else
                mList.EnsureVisible(mList.CurSel);
            invalidate();

        }

        public override void OnClose()
        {
            mList.RemoveAllItems();
            mApplication.WindowManager.close(mList);
            mCodeCompletion = null;
            GC.Collect();
            base.OnClose();
            if (mInsight != null)
            {
                mInsight.Restore();
                mInsight.KeepDataOnClose = false;
            }
        }

        private string mLastHighlightSet = null;

        protected override bool HandleIdle()
        {
            bool rc = base.HandleIdle();
            if (!rc)
            {
                int cursorLine = mApplication.ActiveWindow.CursorRow;
                int cursorColumn = mApplication.ActiveWindow.CursorColumn;

                if (cursorLine < mStartLine || (cursorLine == mStartLine && cursorColumn <= mStartColumn))
                    return true;
                if (cursorLine > mStartLine)
                    return true;

                XceFileBuffer b = mApplication.ActiveWindow.Text;

                int ls = b.LineStart(cursorLine);
                int ll = b.LineLength(cursorLine);
                int end;

                if (cursorColumn > ll)
                    return true;

                for (end = mStartColumn + 1; end < cursorColumn && end < ll; end++)
                {
                    if (!mProvider.IsOnTheFlyDataCharacter(b[ls + end]))
                        return true;
                }

                string s = "";


                if (end > mStartColumn + 1)
                {
                    s = b.GetRange(ls + mStartColumn + 1, end - mStartColumn - 1);
                }


                if (mLastHighlightSet == null || s != mLastHighlightSet)
                {
                    if (s.Length > 0)
                        mList.setHighlight(s);
                    else
                    {
                        mList.setHighlight(0, "");
                    }
                    mLastHighlightSet = s;
                }
            }
            return rc;
        }

        protected override bool HandleKeyPress(int scanCode, char character, bool shift, bool ctrl, bool alt)
        {

            if (!shift && !ctrl && !alt && (scanCode == (int)ScanCode.UP || scanCode == (int)ScanCode.DOWN))
            {
                mList.OnKeyPressed(scanCode, character, shift, ctrl, alt);
                if (mList.CurSel >= 0 && mList.CurSel < mList.Count)
                {
                    int cc = mApplication.ActiveWindow.CursorColumn;
                    mApplication.ActiveWindow.CursorColumn = mStartColumn + 1;
                    if (cc > mStartColumn + 1)
                        mApplication.ActiveWindow.DeleteAtCursor(cc - mStartColumn - 1);
                    string s = (mList[mList.CurSel].UserData as ICodeCompletionItem).Text;
                    mApplication.ActiveWindow.Stroke(s, 0, s.Length);
                }
                return true;
            }
            else if (!ctrl && !alt && (mProvider.IsHideOnTheFlyDataCharacter(character) || scanCode == (int)ScanCode.RETURN))
            {
                if (mList.CurSel >= 0 && mList.CurSel < mList.Count && mList.getHighlight().Length > 0 &&
                    mApplication.ActiveWindow.CursorRow == mStartLine &&
                    mApplication.ActiveWindow.CursorColumn >= mStartColumn)
                {
                    int cc = mApplication.ActiveWindow.CursorColumn;
                    mApplication.ActiveWindow.CursorColumn = mStartColumn + 1;
                    if (cc > mStartColumn + 1)
                        mApplication.ActiveWindow.DeleteAtCursor(cc - mStartColumn - 1);
                    ICodeCompletionItem cci = mList[mList.CurSel].UserData as ICodeCompletionItem;
                    string s = cci.Text;
                    mApplication.ActiveWindow.Stroke(s, 0, s.Length);
                    mProvider.PostOnTheFlyText(mApplication, mApplication.ActiveWindow, mStartColumn + 1, s.Length, cci);
                    mApplication.WindowManager.close(this);
                }
                else
                {
                    mApplication.WindowManager.close(this);
                }

                if (mProvider.IsShowInsightDataCharacter(character) || mProvider.IsShowOnTheFlyDataCharacter(character))
                {
                    bool handled = false;
                    mExtension.OnKeyPressed(scanCode, character, shift, ctrl, alt, ref handled);
                }
                if (mProvider.ForwardEnterAtOneTheFlyEnd())
                    return false;
                else
                    return scanCode == (int)ScanCode.RETURN;
            }
            else if (!shift && !ctrl && !alt && scanCode == (int)ScanCode.ESCAPE)
            {
                show(false);
                return true;
            }
            else
                return false;
        }
    }
}
