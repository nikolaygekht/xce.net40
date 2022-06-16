using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.extension;
using gehtsoft.intellisense.cs;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;

namespace gehtsoft.xce.extension.csintellisense_impl
{
    internal class GoCsProjectDefinitionCommand : IEditorCommand
    {
        public string Name
        {
            get
            {
                return "GoCsProjectDefinition";
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
            
            string word;
            int wordline, wordcolumn, wordlength;
            if (!CsUtils.WordUnderCursor(application.ActiveWindow, out wordline, out wordcolumn, out wordlength, out word))
                return ;
            
            try
            {
                CsCodeCompletionItemCollection coll = file.Parser.GetCompletionData(file, wordline + 1, wordcolumn + 1 + wordlength, true);
                if (coll.Count > 0 && coll.DefaultIndex >= 0)
                {
                    ICsCodeCompletionItem item = coll[coll.DefaultIndex];
                    if (item.Entity != null)
                    {
                        BrowserSourceDialog dlg = new BrowserSourceDialog(application, file.Parser, application.ActiveWindow.Text.FileName, item.Entity);
                        if (dlg.SelectionMade)
                        {
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
                        }
                        else if (item.Name == word && item.Entity != null)
                        {
                            StringBuilder description = new StringBuilder("External definition\r\n");
                            CSharpAmbience a = new CSharpAmbience();
                            description.Append(a.Convert(item.Entity));
                            if (item.OverloadsCount > 0)
                            {
                                for (int i = 0; i < item.OverloadsCount && i < 7; i++)
                                {
                                    description.Append("\r\n");
                                    description.Append(a.Convert(item.Overloads[i].Entity));
                                }
                                if (item.OverloadsCount >= 7)
                                    description.Append("\r\n...");

                                    
                            }
                            application.ShowMessage(description.ToString(), TITLE);
                        }
                        else
                            application.ShowMessage(NOT_FOUND, TITLE);
                    }
                    else
                    {
                        application.ShowMessage(NOT_FOUND, TITLE);
                        return ;
                    }
                }
                else
                {
                    application.ShowMessage(NOT_FOUND, TITLE);
                    return;
                }
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
