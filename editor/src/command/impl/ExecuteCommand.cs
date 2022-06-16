using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;

namespace gehtsoft.xce.editor.command.impl
{
    internal class ExecuteCommandDialog : XceDialog
    {
        private DialogItemComboBox mCommandCtrl;
        private DialogItemSingleLineTextBox mParamCtrl;

        private IEditorCommand mCommand = null;
        private string mParam = "";

        internal IEditorCommand Command
        {
            get
            {
                return mCommand;
            }
        }

        internal string Param
        {
            get
            {
                return mParam;
            }
        }

        internal ExecuteCommandDialog(Application application) : base(application, "Execute", false, 5, 40)
        {
            AddItem(new DialogItemLabel("Co&mmand:", 0x1000, 0, 0));
            AddItem(mCommandCtrl = new DialogItemComboBox("", 0x1001, 0, 10, 28));
            AddItem(new DialogItemLabel("&Parameter:", 0x1002, 1, 0));
            AddItem(mParamCtrl = new DialogItemSingleLineTextBox("", 0x1003, 1, 10, 28));
            DialogItemButton b1, b2;
            AddItem(b1 = new DialogItemButton("< &Ok >", DialogResultOK, 2, 0));
            AddItem(b2 = new DialogItemButton("< Cancel >", DialogResultCancel, 2, 0));
            CenterButtons(b1, b2);
            foreach (IEditorCommand c in application.Commands)
                mCommandCtrl.AddItem(c.Name, c);
            mCommandCtrl.CurSel = 0;
            mCommandCtrl.Readonly = true;
        }

        public override bool OnOK()
        {
            if (mCommandCtrl.CurSel < 0 || mCommandCtrl.CurSel >= mCommandCtrl.Count)
                return false;
            mCommand = (IEditorCommand)(mCommandCtrl[mCommandCtrl.CurSel].UserData);
            mParam = mParamCtrl.Text;
            return true;
        }

    }


    internal class ExecuteCommand : IEditorCommand
    {
        internal ExecuteCommand()
        {

        }


        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "Execute";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            ExecuteCommandDialog dlg = new ExecuteCommandDialog(application);
            if (dlg.DoModal() == Dialog.DialogResultOK)
            {
                dlg.Command.Execute(application, dlg.Param);
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
            return true;
        }
    }
}
