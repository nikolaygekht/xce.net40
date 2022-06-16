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
    internal class ScreenRightCommand : IEditorCommand
    {
        public string Name
        {
            get
            {
                return "ScreenRight";
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
            if (application.ActiveWindow != null && application.ActiveWindow.TopRow > 0)
            {
                application.ActiveWindow.TopColumn = application.ActiveWindow.TopColumn + 1;
                application.ActiveWindow.CursorColumn = application.ActiveWindow.CursorColumn + 1;
                application.ActiveWindow.invalidate();
                return ;
            }
        }
    }
}
