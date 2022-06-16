using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.configuration;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.editor.util;

namespace gehtsoft.xce.editor.command.impl
{
    internal class ChangeCodePageDialog : XceDialog
    {
        DialogItemListBox mList;
        List<DialogItemListBoxString> mStrings = new List<DialogItemListBoxString>();
        DialogItemSingleLineTextBox mText;
        string mOldFilter;
        Encoding mEncoding;

        internal Encoding Encoding
        {
            get
            {
                return mEncoding;
            }
        }

        internal ChangeCodePageDialog(Application application, Encoding current, bool addDefault)
            : base(application, "Suggestions", false, 20, 60)
        {
            mEncoding = null;
            AddItem(mList = new DialogItemListBox(0x1000, 0, 0, 16, 58));
            AddItem(new DialogItemLabel("Find:", 0x1001, 16, 0));
            AddItem(mText = new DialogItemSingleLineTextBox("", 0x1002, 16, 5, 53));

            EncodingInfo[] encodings = Encoding.GetEncodings();

            if (addDefault)
            {
                mStrings.Add(new DialogItemListBoxString(string.Format("????? - Default Encoding"), null));
                mList.AddItem(mStrings[mStrings.Count - 1]);
                if (current == null)
                {
                    mList.CurSel = mList.Count - 1;
                    mList.EnsureVisible(mList.CurSel);
                }
            }

            foreach (EncodingInfo encoding1 in encodings)
            {
                mStrings.Add(new DialogItemListBoxString(string.Format("{0,5} - {1}", encoding1.CodePage, encoding1.Name), encoding1));
                mList.AddItem(mStrings[mStrings.Count - 1]);
                if (current != null && encoding1.CodePage == current.CodePage && mList.CurSel == -1)
                {
                    mList.CurSel = mList.Count - 1;
                    mList.EnsureVisible(mList.CurSel);
                }
            }
            mOldFilter = "";
            AddItem(new DialogItemButton("< &Ok >", Dialog.DialogResultOK, 17, 21));
            AddItem(new DialogItemButton("< &Cancel >", Dialog.DialogResultCancel, 17, 28));
        }

        ///0         1         2         3
        ///0123456789012345678901234567890123456789
        ///          < OK > < CANCEL >
        ///
        /// </summary>
        /// <param name="item"></param>

        public override void OnItemClick(DialogItem item)
        {
            if (item.ID == Dialog.DialogResultOK)
            {
                if (mList.CurSel >= 0)
                {
                    EncodingInfo ee = ((EncodingInfo)mList[mList.CurSel].UserData);
                    if (ee != null)
                        mEncoding = Encoding.GetEncoding(ee.CodePage);
                    else
                        mEncoding = null;
                }
                else
                {
                    mApplication.ShowMessage("Please choose a code page", "Choose Codepage");
                    return ;
                }
            }
            base.OnItemClick(item);
        }

        public override void OnItemChanged(DialogItem item)
        {
            base.OnItemChanged(item);
            if (item == mText)
            {
                if (mText.Text != mOldFilter)
                {
                    int cp = -1;
                    if (mList.CurSel >= 0)
                    {
                        EncodingInfo ee = (EncodingInfo)mList[mList.CurSel].UserData;
                        if (ee != null)
                            cp = ee.CodePage;
                        else
                            cp = -2;
                    }
                    mOldFilter = mText.Text;
                    mList.RemoveAllItems();
                    foreach (DialogItemListBoxString s in mStrings)
                    {
                        if (s.Label.IndexOf(mOldFilter, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            EncodingInfo ee = (EncodingInfo)s.UserData;
                            mList.AddItem(s);
                            if ((ee == null && cp == -2) || (ee != null && cp > 0 && ee.CodePage == cp))
                                mList.CurSel = mList.Count - 1;
                        }
                    }
                    if (mList.CurSel != -1)
                        mList.EnsureVisible(mList.CurSel);
                }
            }
        }
    }

    class ChangeCodePageCommand : IEditorCommand
    {
        internal ChangeCodePageCommand()
        {
        }

        public string Name
        {
            get
            {
                return "ChangeCodePage";
            }
        }

        public bool IsChecked(Application application, string param)
        {
            return false;
        }

        public bool IsEnabled(Application application, string param)
        {
            return application.ActiveWindow != null;
        }

        public void Execute(Application application, string param)
        {
            TextWindow w = application.ActiveWindow;
            if (w == null)
                return ;

            ChangeCodePageDialog dlg = new ChangeCodePageDialog(application, w.Text.Encoding, false);
            if (dlg.DoModal() == Dialog.DialogResultOK)
            {
                if (w.Text.Encoding.CodePage != dlg.Encoding.CodePage)
                {
                    if (w.Text.AtSavePoint)
                    {
                        string f = w.Text.FileName;
                        int tr, tc, cr, cc;
                        tr = w.TopRow;
                        tc = w.TopColumn;
                        cr = w.CursorRow;
                        cc = w.CursorColumn;
                        application.CloseWindow(w);
                        application.OpenFile(f, dlg.Encoding);
                        application.ActiveWindow.TopRow = tr;
                        application.ActiveWindow.TopColumn = tc;
                        application.ActiveWindow.CursorColumn = cc;
                        application.ActiveWindow.CursorRow = cr;
                        
                    }
                    else
                    {
                        w.Text.ChangeEncoding(dlg.Encoding);
                        application.ActiveWindow.invalidate();
                    }
                }
            }
        }
    }
}
