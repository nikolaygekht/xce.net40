using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.conio.win;

namespace gehtsoft.xce.editor.command.impl
{
    internal class GoToDialog : XceDialog
    {
        int mLine;
        DialogItemSingleLineTextBox mEdit;

        internal int Line
        {
            get
            {
                return mLine;
            }
        }

        internal GoToDialog(Application application, int line) : base(application, "Go Line", false, 4, 19)
        {
            AddItem(new DialogItemLabel("&Line:", 0x1000, 0, 0));
            string s = (line + 1).ToString();
            AddItem(mEdit = new DialogItemSingleLineTextBox(s, 0x1001, 0, 5, 8));
            mEdit.SetSel(0, s.Length);
            AddItem(new DialogItemButton("< &Ok >", DialogResultOK, 1, 0));
            AddItem(new DialogItemButton("< &Cancel >", DialogResultCancel, 1, 7));
        }

        public override bool OnOK()
        {
            string s = mEdit.Text;
            if (!Int32.TryParse(s, out mLine))
            {
                MessageBox.Show(WindowManager, Colors, "The line is not a number", "Go To", MessageBoxButtonSet.Ok);
                return false;
            }
            if (mLine <= 0)
            {
                MessageBox.Show(WindowManager, Colors, "The line shall be a positive integer", "Go To", MessageBoxButtonSet.Ok);
                return false;
            }
            mLine = mLine - 1;
            return true;
        }
    }


    internal class GoToCommand : IEditorCommand
    {
        internal GoToCommand()
        {

        }


        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "GoTo";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            if (application.ActiveWindow == null)
                return ;
            int line = 0;
            if (parameter == null || parameter.Length == 0 || !Int32.TryParse(parameter, out line))
            {
                GoToDialog dlg = new GoToDialog(application, application.ActiveWindow.CursorRow);
                if (dlg.DoModal() != Dialog.DialogResultOK)
                    return ;
                line = dlg.Line;
            }
            application.ActiveWindow.CursorRow = line;
            application.ActiveWindow.EnsureCursorVisibleInCenter();
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
