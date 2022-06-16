using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.extension;
using gehtsoft.xce.intellisense.common;


namespace gehtsoft.xce.extension.intellisense_impl
{
    internal class SuggestionCommand : IEditorCommand
    {
        public string Name
        {
            get
            {
                return "Intellisense_Suggestion";
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
            if (application.ActiveWindow[intellisense.DATA_NAME] == null)
                return false;
            if (application.ActiveWindow[intellisense.DATA_NAME] as IIntellisenseProvider == null)
                return false;
            return (application.ActiveWindow[intellisense.DATA_NAME] as IIntellisenseProvider).CanGetCodeCompletionCollection(application, application.ActiveWindow);
        }

        public void Execute(Application application, string param)
        {
            if (application.ActiveWindow == null)
                return ;
            if (application.ActiveWindow[intellisense.DATA_NAME] == null)
                return ;
            IIntellisenseProvider provider = application.ActiveWindow[intellisense.DATA_NAME] as IIntellisenseProvider;
            if (provider == null)
                return ;

            try
            {
                int wordLine, wordColumn, wordLength;

                ICodeCompletionItemCollection collection = provider.GetCodeCompletionCollection(application, application.ActiveWindow, out wordLine, out wordColumn, out wordLength);
                if (collection != null)
                {
                    if (TooltipWindow.Tooltip != null)
                        application.WindowManager.close(TooltipWindow.Tooltip);

                    SuggestionDialog dlg = new SuggestionDialog(application, collection);
                    if (dlg.DoModal() == XceDialog.DialogResultOK)
                    {
                        ICodeCompletionItem item = dlg.Selected;
                        if (item != null)
                        {
                            if (wordLength > 0)
                            {
                                application.ActiveWindow.CursorRow = wordLine;
                                application.ActiveWindow.CursorColumn = wordColumn;
                                application.ActiveWindow.DeleteAtCursor(wordLength);
                            }
                            int column;
                            column = application.ActiveWindow.CursorColumn;
                            application.ActiveWindow.Stroke(item.Text, 0, item.Text.Length);
                            provider.PostOnTheFlyText(application, application.ActiveWindow, column, item.Text.Length, item);
                        }
                    }
                    collection = null;
                    dlg = null;
                }
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
