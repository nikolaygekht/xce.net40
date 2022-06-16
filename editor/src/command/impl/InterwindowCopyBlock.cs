using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.editor.util;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;

namespace gehtsoft.xce.editor.command.impl
{

    internal class WindowListDialog : XceDialog
    {
        DialogItemListBox mWindowList;
        DialogItemButton mOk;
        DialogItemButton mCancel;
        TextWindow w = null;

        internal TextWindow Window
        {
            get
            {
                return w;
            }
        }

        internal WindowListDialog(string title, Application application) : base(application, title, false, 20, 60)
        {
            AddItem(mWindowList = new DialogItemListBox(0x1000, 0, 0, 17, 58));
            AddItem(mOk = new DialogItemButton("< &Ok >", Dialog.DialogResultOK, 17, 0));
            AddItem(mCancel = new DialogItemButton("< &Cancel >", Dialog.DialogResultCancel, 17, 0));
            CenterButtons(mOk, mCancel);
        }

        internal void AddWindow(TextWindow w)
        {
            string s = w.Text.FileName;
            if (s.Length > 53)
                s = "..." + s.Substring(s.Length - 53);
            mWindowList.AddItem(new DialogItemListBoxString(s, w));
            if (mWindowList.CurSel == -1)
                mWindowList.CurSel = 0;
        }

        public override bool OnOK()
        {
            if (mWindowList.CurSel >= 0 && mWindowList.CurSel < mWindowList.Count)
            {
                w = (TextWindow)mWindowList[mWindowList.CurSel].UserData;
                return true;
            }
            else
                return false;
        }
    }

    internal class InterwindowCopyBlockCommand : IEditorCommand
    {
        internal InterwindowCopyBlockCommand()
        {

        }

        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "InterwindowCopyBlock";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            TextWindow w = application.ActiveWindow;
            if (w == null)
                return ;

            WindowListDialog dlg = new WindowListDialog("Copy Block", application);
            int cnt = 0;

            foreach (TextWindow w0 in application.TextWindows)
                if (w0 != w && w0.BlockType != TextWindowBlock.None)
                {
                    dlg.AddWindow(w0);
                    cnt ++;
                }


            if (cnt > 0)
            {
                if (dlg.DoModal() == Dialog.DialogResultOK)
                {
                    TextWindow w0 = dlg.Window;

                    BlockContent block = new BlockContent(w0);
                    w.BeforeModify();

                    BlockContentProcessor.PutBlock(w, block, w.CursorRow, w.CursorColumn);
                    block = null;
                    GC.Collect();
                    if (w.BlockType == TextWindowBlock.Line)
                    {
                        w._CursorRow = w.BlockLineStart;
                        w._CursorColumn = 0;

                    }
                    else if (w.BlockType == TextWindowBlock.Box)
                    {
                        w._CursorRow = w.BlockLineStart;
                        w._CursorColumn = w.BlockColumnStart;

                    }
                    w.EnsureCursorVisible();
                    w.invalidate();
                }
            }
        }

        /// <summary>
        /// Get checked status for the menu.
        /// </summary>
        public bool IsChecked(Application application, string parameter)
        {
            return false;
        }

        /// <summary>
        /// Get enabled status for the menu with parameter.
        /// </summary>
        public bool IsEnabled(Application application, string parameter)
        {
            return application.ActiveWindow != null;
        }
    }
}
