using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.editor.util;

namespace gehtsoft.xce.extension.advnav_commands
{
    internal class ScreenHomeCommand : IEditorCommand
    {
        public string Name
        {
            get
            {
                return "ScreenHome";
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
            if (application.ActiveWindow != null)
            {
                application.ActiveWindow.CursorColumn = application.ActiveWindow.TopColumn;
                application.ActiveWindow.invalidate();
                return ;
            }
        }
    }
}
