using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.extension;
using gehtsoft.intellisense.cs;
using ICSharpCode.SharpDevelop.Dom;

namespace gehtsoft.xce.extension.csintellisense_impl
{
    internal class GoCsSuggestionCommand : IEditorCommand
    {
        public string Name
        {
            get
            {
                return "GoCsSuggestion";
            }
        }
        
        public bool IsChecked(Application application, string param)
        {
            return false;
        }

        public bool IsEnabled(Application application, string param)
        {
            if (application.ActiveWindow == null)
                return false;
            if (application.ActiveWindow[csintellisense.DATA_NAME] == null)
                return false;
            if (application.ActiveWindow[csintellisense.DATA_NAME] as CsParserFile == null)                
                return false;
            return true;
        }
        
        private const string TITLE = "C# Reference";
        private const string NOT_FOUND = "Declaration is not found";

        public void Execute(Application application, string param)
        {
            if (application.ActiveWindow == null ||
                application.ActiveWindow[csintellisense.DATA_NAME] == null ||
                application.ActiveWindow[csintellisense.DATA_NAME] as CsParserFile == null)
            {
                application.ShowMessage("The file is not a part of the C# project or the extension is disabled", TITLE);
                return ;                
            } 
            CsParserFile file = application.ActiveWindow[csintellisense.DATA_NAME] as CsParserFile;
            if (!file.Parser.Works)
            {
                application.ShowMessage("The parser does not work. Please check parser log for errors", TITLE);
                return;                
            }
            
            int wordline, wordcolumn, wordlength;
            int row, column;
            bool wordUnderCursor;
            if (CsUtils.WordUnderCursor(application.ActiveWindow, out wordline, out wordcolumn, out wordlength))
            {
                wordUnderCursor = true;
                row = wordline;
                column = wordcolumn + wordlength;
            }
            else
            {
                wordUnderCursor = false;
                row = application.ActiveWindow.CursorRow;
                column = application.ActiveWindow.CursorColumn;
            }
            try
            {
                CsCodeCompletionItemCollection coll = null;
                if (wordUnderCursor)
                {
                    string word = application.ActiveWindow.Text.GetRange(application.ActiveWindow.Text.LineStart(wordline) + wordcolumn, wordlength);
                    if (word == "override")
                        coll = file.Parser.GetOverloadData(file, row + 1, column + 1);
                    else if (word == "base")
                        coll = file.Parser.GetConstructorData(file, row + 1, column + 1);
                }
                if (coll == null)
                    coll = file.Parser.GetCompletionData(file, row + 1, column + 1, true);
                SuggestionDialog dlg = new SuggestionDialog(application, coll);
                if (dlg.DoModal() == XceDialog.DialogResultOK)
                {
                    ICsCodeCompletionItem item = dlg.Selected;
                    if (item != null)
                    {
                        if (wordUnderCursor)
                        {
                            
                            application.ActiveWindow.CursorRow = wordline;
                            application.ActiveWindow.CursorColumn = wordcolumn;
                            application.ActiveWindow.DeleteAtCursor(wordlength);
                        }
                        application.ActiveWindow.Stroke(item.Text, 0, item.Text.Length);
                    }
                }
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
