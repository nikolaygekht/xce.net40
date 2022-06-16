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
using gehtsoft.xce.editor.util;



namespace gehtsoft.xce.editor.command.impl
{
    internal class ChooseSpellSuggestCommandDlg : XceDialog
    {
        DialogItemListBox mList;
        string mSuggestion;

        internal string Suggestion
        {
            get
            {
                return mSuggestion;
            }
        }

        internal ChooseSpellSuggestCommandDlg(Application application, ISpellcheckerSuggestions suggest) : base(application, "Suggestions", false, 16, 42)
        {
            mSuggestion = "";
            mList = new DialogItemListBox(0x1000, 0, 0, 13, 40);
            foreach (string s in suggest)
                mList.AddItem(new DialogItemListBoxString(s, null));
            mList.CurSel = 0;
            AddItem(mList);
            AddItem(new DialogItemButton("< &Ok >", Dialog.DialogResultOK, 13, 10));
            AddItem(new DialogItemButton("< &Cancel >", Dialog.DialogResultCancel, 13, 17));
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
                    mSuggestion = mList[mList.CurSel].Label;
            }
            base.OnItemClick(item);
        }
    }

    internal class ChooseSpellSuggestCommand : IEditorCommand
    {
        internal ChooseSpellSuggestCommand()
        {
        }

        public string Name
        {
            get
            {
                return "ChooseSpellSuggest";
            }
        }

        public bool IsChecked(Application application, string param)
        {
            return false;
        }

        public bool IsEnabled(Application application, string param)
        {
            int i1, i2;
            return application.ActiveWindow != null &&
                   application.Configuration.SpellCheckers.Count > 0 &&
                   application.ActiveWindow.Spellchecker != null &&
                   WordUnderCustor(application, out i1, out i2);
        }

        public void Execute(Application application, string param)
        {
            if (!IsEnabled(application, param))
                return ;

            int from, length;
            if (!WordUnderCustor(application, out from, out length))
                return ;

            TextWindow w = application.ActiveWindow;
            string word = application.ActiveWindow.Text.GetRange(w.Text.LineStart(w.CursorRow) + from, length);

            ISpellcheckerSuggestions suggest = w.Spellchecker.Suggest(word);
            ChooseSpellSuggestCommandDlg dlg = new ChooseSpellSuggestCommandDlg(application, suggest);
            if (dlg.DoModal() == Dialog.DialogResultOK)
            {
                if (dlg.Suggestion.Length > 0)
                {
                    w.CursorColumn = from;
                    w.DeleteAtCursor(length);
                    w.Stroke(dlg.Suggestion, 0, dlg.Suggestion.Length);
                    w.EnsureCursorVisible();
                }
            }
            application.ActiveWindow.invalidate();
        }

        private bool WordUnderCustor(Application application, out int from, out int length)
        {
            from = 0;
            length = 0;
            TextWindow t = application.ActiveWindow;
            if (t == null)
                return false;

            char c;
            CharClass ccl;
            c = t[t.CursorRow, t.CursorColumn];
            ccl = CharUtil.GetCharClass(c);
            if ((c == '\'' || c == '’') || ccl == CharClass.Word)
            {
                //we are in the word
                int i;
                for (i = t.CursorColumn; i >= 0; i--)
                {
                    c = t[t.CursorRow, i];
                    ccl = CharUtil.GetCharClass(c);
                    if ((c == '\'' || c == '’') || ccl == CharClass.Word)
                        continue;
                    else
                        break;
                }
                from = i + 1;
                for (i = t.CursorColumn; true; i++)
                {
                    c = t[t.CursorRow, i];
                    ccl = CharUtil.GetCharClass(c);
                    if ((c == '\'' || c == '’') || ccl == CharClass.Word)
                        continue;
                    else
                        break;
                }
                length = i - from;
                return true;
            }
            else
                return false;

        }
    }
}
