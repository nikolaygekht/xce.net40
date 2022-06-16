using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.text;

namespace gehtsoft.xce.extension.charmap_impl
{
    internal class CharmapCommand : IEditorCommand, IDialogCommand
    {
        public string Name
        {
            get
            {
                return "ShowCharmap";
            }
        }
        
        public bool IsEnabled(Application application, string param)
        {
            return application.ActiveWindow != null;
        }

        public bool IsChecked(Application application, string param)
        {
            return false;
        }
        
        public void Execute(Application application, string param)
        {
            CharmapDialog dlg = new CharmapDialog(application);
            if (dlg.DoModal() == Dialog.DialogResultOK)
            {
                application.ActiveWindow.Stroke(dlg.SelChar, 1);
            }
            return ;
        }
        
        public void DialogExecute(Application application, XceDialog dialog, string param)
        {
            if (application.WindowManager.getFocus() != null)
            {
                CharmapDialog dlg = new CharmapDialog(application);
                if (dlg.DoModal() == Dialog.DialogResultOK)
                {
                    application.WindowManager.getFocus().OnKeyPressed(0, dlg.SelChar, false, false, false);
                }
            }
        }

    }
}