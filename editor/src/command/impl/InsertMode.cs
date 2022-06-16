using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;


namespace gehtsoft.xce.editor.command.impl
{
    internal class InsertModeCommand : IEditorCommand
    {
        public InsertModeCommand()
        {
        }

        public string Name
        {
            get
            {
                return "InsertMode";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            if (application.ActiveWindow == null)
                return ;

            if (parameter == null || parameter.Length == 0)
                application.ActiveWindow.InsertMode = !application.ActiveWindow.InsertMode;
            else if (parameter[0] == 'i' || parameter[0] == 'I')
                application.ActiveWindow.InsertMode = true;
            else if (parameter[0] == 'o' || parameter[0] == 'O')
                application.ActiveWindow.InsertMode = false;
            else
                application.ActiveWindow.InsertMode = !application.ActiveWindow.InsertMode;
        }

        /// <summary>
        /// Get checked status for the menu.
        /// </summary>
        public bool IsChecked(Application application, string parameter)
        {
            return false;
        }

        /// <summary>
        /// Get enabled status for the menu with parameter.
        /// </summary>
        public bool IsEnabled(Application application, string parameter)
        {
            return application.ActiveWindow != null;
        }

    }
}
