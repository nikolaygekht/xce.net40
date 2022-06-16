using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.extension;
using gehtsoft.intellisense.cs;

namespace gehtsoft.xce.extension.csintellisense_impl
{
    internal class BrowserCsProjectCommand : IEditorCommand
    {
        public string Name
        {
            get
            {
                return "BrowseCsProject";
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

        public void Execute(Application application, string param)
        {
            if (application.ActiveWindow == null ||
                application.ActiveWindow[csintellisense.DATA_NAME] == null ||
                application.ActiveWindow[csintellisense.DATA_NAME] as CsParserFile == null)
            {
                application.ShowMessage("The file is not a part of the C# project or the extension is disabled", "C# Project");
                return ;                
            } 
            CsParserFile file = application.ActiveWindow[csintellisense.DATA_NAME] as CsParserFile;
            if (!file.Parser.Works)
            {
                application.ShowMessage("The parser does not work. Please check parser log for errors", "C# Project");
                return;                
            }
            
            BrowserSourceDialog dlg = new BrowserSourceDialog(application, file.Parser, application.ActiveWindow.Text.FileName, application.ActiveWindow.CursorRow, application.ActiveWindow.CursorColumn);
            if (dlg.DoModal() == BrowserSourceDialog.DialogResultOK)
            {
                string fileName = dlg.Selected.Name;
                bool found = false;
                TextWindow ww = null;
                foreach (TextWindow w in application.TextWindows)
                {
                    if (string.Compare(fileName, w.Text.FileName, true) == 0)
                    {
                        if (application.ActiveWindow != w)
                        {
                            application.ActivateWindow(w);
                            ww = w;
                            found = true;
                            break;
                        }
                    }
                }
                
                if (!found)
                    ww = application.OpenFile(fileName);
                    
                if (ww != null)
                {
                    ww.CursorRow = dlg.Selected.Line;
                    ww.CursorColumn = dlg.Selected.Column;
                    ww.EnsureCursorVisible();
                }
            }
            dlg = null;
            GC.Collect();
        }
    }
}
