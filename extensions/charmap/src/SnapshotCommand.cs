using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.command;

namespace gehtsoft.xce.extension.charmap_impl
{
    class SnapshotCommand : IEditorCommand
    {
        public string Name
        {
            get
            {
                return "ShowDefaultSnapshot";
            }
        }
        
        public bool IsEnabled(Application application, string param)
        {
            return true;
        }

        public bool IsChecked(Application application, string param)
        {
            return false;
        }

        public void Execute(Application application, string param)
        {
            if (param == null || param.Length < 1)
                return ;
            SnapshotDialog dlg = new SnapshotDialog(application, param);
            dlg.DoModal();
        }
        
    }
}
