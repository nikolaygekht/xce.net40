using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.Resources;
using gehtsoft.xce.colorer;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.editor.command;

namespace gehtsoft.xce.extension.charmap_impl
{
    internal class ShowRegionDialog : XceDialog
    {
        private DialogItemListBox mRegionList;
        private DialogItemSingleLineTextBox mInheritanceMap;
        private DialogItemButton mOk, mPrev, mNext;

        class ItemInfo
        {
            public int row;
            public int column;
            public int length;
            public string inheritance;

            internal ItemInfo(int _row, int _column, int _length, string _inheritance)
            {
                row = _row;
                column = _column;
                length = _length;
                inheritance = _inheritance;
            }
        }

        internal ShowRegionDialog(Application app) : base(app, "Syntax Regions", true, 10, 40)
        {
            AddItem(mRegionList = new DialogItemListBox(0x1000, 0, 0, 6, 38));
            AddItem(mInheritanceMap = new DialogItemSingleLineTextBox("", 0x1002, 7, 0, 38));
            mInheritanceMap.Readonly = true;
            AddItem(mOk = new DialogItemButton("< &Exit >", Dialog.DialogResultCancel, 7, 0, 8));
            AddItem(mPrev = new DialogItemButton("< &Prev >", 0x1003, 7, 0, 8));
            AddItem(mNext = new DialogItemButton("< &Next >", 0x1004, 7, 0, 8));
            CenterButtons(mOk, mPrev, mNext);
            InitData();
        }



        private void InitData()
        {
            mRegionList.RemoveAllItems();
            mInheritanceMap.Text = "";

            TextWindow w = mApplication.ActiveWindow;
            if (w == null)
            {
                mPrev.Enable(false);
                mNext.Enable(false);
            }
            else
            {
                mPrev.Enable(w.CursorRow > 0);
                mNext.Enable(w.CursorRow < w.Text.LinesCount - 1);
            }

            if (w != null && w.CursorRow < w.Text.LinesCount)
            {
                SyntaxHighlighter h = w.Highlighter;
                bool rc;
                int ll = w.Text.LineLength(w.CursorRow);
                rc = h.GetFirstRegion(w.CursorRow);
                while (rc)
                {
                    SyntaxHighlighterRegion r = h.CurrentRegion;
                    string name, inheritance;
                    if (r.IsSyntaxRegion)
                    {
                        if (r.Name != null)
                            name = r.Name;
                        else
                            name = "(null)";
                        if (r.DerivedFrom != null)
                            inheritance = r.DerivedFrom;
                        else
                            inheritance = "";
                    }
                    else
                    {
                        inheritance = "";
                        name = "(null)";
                    }

                    string item = string.Format("{2} {0}-{3}({1})", r.StartColumn, r.Length, name, r.StartColumn + r.Length - 1);
                    mRegionList.AddItem(item, new ItemInfo(w.CursorRow, r.StartColumn, r.Length >= 0 ? r.Length : ll - r.StartColumn, inheritance));
                    rc = h.GetNextRegion();
                }
            }
            if (mRegionList.Count > 0)
            {
                mRegionList.CurSel = 0;
                mRegionList.EnsureVisible(0);
                OnItemChanged(mRegionList);
            }
        }

        override public void OnItemChanged(DialogItem item)
        {
            if (item == mRegionList && mRegionList.CurSel >= 0 && mRegionList.CurSel < mRegionList.Count)
            {
                ItemInfo ii = mRegionList[mRegionList.CurSel].UserData as ItemInfo;
                if (ii != null)
                {
                    mInheritanceMap.Text = ii.inheritance;
                    TextWindow w = mApplication.ActiveWindow;

                    w.CursorRow = ii.row;
                    w.CursorColumn = ii.column + ii.length;
                    w.EnsureCursorVisible();
                    w.CursorRow = ii.row;
                    w.CursorColumn = ii.column;
                    w.EnsureCursorVisible();

                    if (ii.length > 0)
                    {
                        int ls = w.Text.LineStart(ii.row);
                        w.HighlightRangePosition = ls + ii.column;
                        w.HighlightRangeLength = ii.length;
                    }
                    else
                    {
                        w.HighlightRangePosition = -1;
                        w.HighlightRangeLength = 0;

                    }

                }
            }
        }

        override public void OnItemClick(DialogItem item)
        {
            if (item == mOk)
            {
                base.OnItemClick(mOk);
            }
            else if (item == mNext)
            {
                TextWindow w = mApplication.ActiveWindow;
                w.CursorRow = w.CursorRow + 1;
                w.EnsureCursorVisible();
                InitData();
            }
            else if (item == mPrev)
            {
                TextWindow w = mApplication.ActiveWindow;
                if (w.CursorRow > 0)
                {
                    w.CursorRow = w.CursorRow - 1;
                    w.EnsureCursorVisible();
                    InitData();
                }
            }
        }

        override public void OnSizeChanged()
        {
            base.OnSizeChanged();
            if (Height < 8 || Width < 30)
                move(Row, Column, Math.Max(Height, 8), Math.Max(Width, 30));
            else
            {
                int height, width;
                height = Height;
                width = Width;
                mRegionList.move(0, 0, height - 4, width - 2);
                mInheritanceMap.move(height - 4, 0, 1, width - 2);
                mOk.move(height - 3, 0, 1, 8);
                mNext.move(height - 3, 0, 1, 8);
                mPrev.move(height - 3, 0, 1, 8);
                CenterButtons(mOk, mPrev, mNext);
            }
        }
    }


    internal class ShowColorerRegionsCommand : IEditorCommand
    {
        public string Name
        {
            get
            {
                return "ShowColorerRegions";
            }
        }

        public bool IsEnabled(Application application, string param)
        {
            return application.ActiveWindow != null;
        }

        public bool IsChecked(Application application, string param)
        {
            return false;
        }

        public void Execute(Application application, string param)
        {
            ShowRegionDialog dlg = new ShowRegionDialog(application);
            dlg.DoModal();
            if (application.ActiveWindow != null)
            {
                application.ActiveWindow.HighlightRangePosition = -1;
                application.ActiveWindow.HighlightRangeLength = 0;

            }
            return ;
        }
    }

}