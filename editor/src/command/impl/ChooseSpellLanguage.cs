using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.configuration;
using gehtsoft.xce.spellcheck;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;


namespace gehtsoft.xce.editor.command.impl
{
    internal class ChooseSpellLanguageCommandDlg : XceDialog
    {
        DialogItemListBox mList;
        TextWindow mTextWin;
        XceConfiguration mCfg;

        internal ChooseSpellLanguageCommandDlg(Application application) : base(application, "Choose Language", false, 8, 22)
        {
            mCfg = application.Configuration;
            mTextWin = application.ActiveWindow;
            mList = new DialogItemListBox(0x1000, 0, 0, 5, 20);
            int sel = -1;
            int i = 0;
            foreach (ISpellchecker c in mCfg.SpellCheckers)
            {
                if (c == mTextWin.Spellchecker)
                    sel = i;
                mList.AddItem(new DialogItemListBoxString(c.Name, c));
                i++;
            }
            mList.AddItem(new DialogItemListBoxString("No Spelling", null));
            if (mTextWin.Spellchecker == null)
                sel = i;
            mList.CurSel = sel;
            AddItem(mList);
            AddItem(new DialogItemButton("< &Ok >", Dialog.DialogResultOK, 5, 1));
            AddItem(new DialogItemButton("< &Cancel >", Dialog.DialogResultCancel, 5, 8));
        }

        public override void OnItemClick(DialogItem item)
        {
            if (item.ID == Dialog.DialogResultOK)
            {
                if (mList.CurSel >= 0)
                    mTextWin.Spellchecker = (ISpellchecker)mList[mList.CurSel].UserData;
            }
            base.OnItemClick(item);
        }
    }

    internal class ChooseSpellLanguageCommand : IEditorCommand
    {
        internal ChooseSpellLanguageCommand()
        {
        }

        public string Name
        {
            get
            {
                return "ChooseSpellLanguage";
            }
        }

        public bool IsChecked(Application application, string param)
        {
            return false;
        }

        public bool IsEnabled(Application application, string param)
        {
            return application.ActiveWindow != null &&
                   application.Configuration.SpellCheckers.Count > 0;
        }

        public void Execute(Application application, string param)
        {
            if (!IsEnabled(application, param))
                return ;

            ChooseSpellLanguageCommandDlg dlg = new ChooseSpellLanguageCommandDlg(application);
            dlg.DoModal();
            application.ActiveWindow.invalidate();
        }
    }
}
