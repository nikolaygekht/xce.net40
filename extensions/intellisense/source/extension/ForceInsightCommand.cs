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
    internal class ForceInsightCommand : IEditorCommand
    {
        public string Name
        {
            get
            {
                return "Intellisense_ForceInsight";
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
            return (application.ActiveWindow[intellisense.DATA_NAME] as IIntellisenseProvider).CanGetInsightDataProvider(application, application.ActiveWindow);
        }

        public void Execute(Application application, string param)
        {
            Execute(application, (char)0);
        }

        internal void Execute(Application application, char character)
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
                IInsightDataProvider insightProvider = provider.GetInsightDataProvider(application, application.ActiveWindow, character);
                if (insightProvider != null && insightProvider.Count > 0)
                {
                    InsightWindow w = InsightWindow.GetInsightWindow(application);
                    w.AddInsightProvide(insightProvider);
                }
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
