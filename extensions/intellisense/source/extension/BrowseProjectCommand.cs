using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.text;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.extension;
using gehtsoft.xce.intellisense.common;


namespace gehtsoft.xce.extension.intellisense_impl
{
    internal class BrowseProjectCommand : IEditorCommand
    {
        public string Name
        {
            get
            {
                return "Intellisense_BrowseProject";
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
            return (application.ActiveWindow[intellisense.DATA_NAME] as IIntellisenseProvider).CanGetProjectBrowserItemCollection(application, application.ActiveWindow);
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


            IProjectBrowserItem curSel;
            IProjectBrowserItemCollection collection = provider.GetProjectBrowserItemCollection(application, application.ActiveWindow, out curSel);

            if (collection != null)
            {
                BrowseSourceDialog dlg = new BrowseSourceDialog(application, collection, curSel);
                if (dlg.DoModal() == Dialog.DialogResultOK)
                {
                    curSel = dlg.Selected;
                    if (curSel != null)
                    {
                        bool found = false;
                        TextWindow ww = null;
                        foreach (TextWindow w in application.TextWindows)
                        {
                            if (string.Compare(curSel.FileName, w.Text.FileName, true) == 0)
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
                            ww = application.OpenFile(curSel.FileName);

                        if (ww != null)
                        {
                            ww.CursorRow = curSel.StartLine;
                            ww.CursorColumn = curSel.StartColumn;
                            ww.EnsureCursorVisible();
                        }
                    }
                }
                dlg = null;
                collection = null;
            }
            GC.Collect();
        }
    }
}
