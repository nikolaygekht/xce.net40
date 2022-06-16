using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.text;
using gehtsoft.intellisense.cs;
using ICSharpCode.SharpDevelop.Dom;
using gehtsoft.xce.extension;

namespace gehtsoft.xce.extension.csintellisense_impl
{
    internal class PopupHintWindow : TooltipWindow
    {
        private InsightWindow mInsight;
        private CsCodeCompletionItemCollection mCodeCompletion;
        private SearchingListWindow mList;
        private int mStartLine, mStartColumn;
        private csintellisense mExtension;

        internal PopupHintWindow(Application application, int line, int column, CsCodeCompletionItemCollection codeCompletion, csintellisense extension)
            : base(application, line, column)
        {
            mExtension = extension;
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
            mList.StartMax = 2;
            foreach (ICsCodeCompletionItem item in codeCompletion)
                AddItem(item);
            mList.EnsureVisible(0);
            mStartLine = line;
            mStartColumn = column;
        }

        private void AddItem(ICsCodeCompletionItem item)
        {
            switch (item.Type)
            {
                case CsParserCodeCompletionItemType.Text:
                    mList.AddItem(item.Name, item);
                    break;
                case CsParserCodeCompletionItemType.Class:
                case CsParserCodeCompletionItemType.Member:
                    {
                        IEntity entity = item.Entity;
                        string s;
                        char p;
                        if (entity is IClass)
                            p = 'c';
                        else if (entity is IMethod)
                            p = 'm';
                        else if (entity is IProperty)
                            p = 'p';
                        else if (entity is IField)
                            p = 'f';
                        else if (entity is IEvent)
                            p = 'e';
                        else
                            p = '?';

                        if (item.OverloadsCount > 0)
                            s = string.Format("{0}:{1} +{2}", p, item.Name, item.OverloadsCount);
                        else
                            s = string.Format("{0}:{1}", p, item.Name);
                        mList.AddItem(s, item);
                    }
                    break;
                default:
                    mList.AddItem(string.Format("?:{0}", item.Name), item);
                    break;
            }
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
                
                if (cursorLine < mStartLine || (cursorLine == mStartLine && cursorColumn < mStartColumn))
                    return true;
                if (cursorLine > mStartLine)
                    return true;
                    
                XceFileBuffer b = mApplication.ActiveWindow.Text;
                
                int ls = b.LineStart(cursorLine);
                int ll = b.LineLength(cursorLine);
                int end;
                
                if (cursorColumn > ll)
                    return true;

                for (end = mStartColumn + 1; end < cursorColumn; end++)
                {
                    if (!char.IsLetterOrDigit(b[ls + end]))
                        return true;
                }   
                
                string s = "";             
                
                if (end > mStartColumn + 1)
                {
                    s = b.GetRange(ls + mStartColumn + 1, end - mStartColumn);
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
                return true;
            }
            else if (!ctrl && !alt && (character == ' ' || character == ',' || character == '.' || character == ':' || character == ')' || character == '(' || character == ';'))
            {
                if (mList.CurSel >= 0 && mList.CurSel < mList.Count &&
                    mApplication.ActiveWindow.CursorRow == mStartLine &&
                    mApplication.ActiveWindow.CursorColumn >= mStartColumn)
                {
                    int cc = mApplication.ActiveWindow.CursorColumn;
                    mApplication.ActiveWindow.CursorColumn = mStartColumn + 1;
                    if (cc > mStartColumn + 1)
                        mApplication.ActiveWindow.DeleteAtCursor(cc - mStartColumn - 1);
                    string s = (mList[mList.CurSel].UserData as ICsCodeCompletionItem).Text;
                    mApplication.ActiveWindow.Stroke(s, 0, s.Length);
                    mApplication.WindowManager.close(this);
                    if (character == '.' || character == '(')
                    {
                        bool handled = false;
                        mExtension.OnKeyPressed(scanCode, character, shift, ctrl, alt, ref handled);
                    }
                }
                return false;
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
