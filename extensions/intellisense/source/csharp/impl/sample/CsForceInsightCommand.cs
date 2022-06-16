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
    internal class CsForceInsightCommand : IEditorCommand
    {
        public string Name
        {
            get
            {
                return "CsForceInsight";
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
        
        private const string TITLE = "C# Insight";

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
            
            try
            {
                List<InsightData> l = file.Parser.GetListOfInsightMethods(file, application.ActiveWindow.CursorRow + 1, application.ActiveWindow.CursorColumn + 1);
                if (l.Count > 0)
                {
                    InsightWindow w = InsightWindow.GetInsightWindow(application);
                    w.AddInsightProvide(new InsightDataProvider(application.ActiveWindow, l, application.ActiveWindow.CursorRow, application.ActiveWindow.CursorColumn));
                }
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
