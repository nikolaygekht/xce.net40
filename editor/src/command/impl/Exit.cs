using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;

namespace gehtsoft.xce.editor.command.impl
{
    internal class ExitCommand : IEditorCommand
    {
        private IEditorCommand mCloseFile = null;

        internal ExitCommand()
        {

        }

        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "Exit";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            if (mCloseFile == null)
                mCloseFile = application.Commands["CloseFile"];

            TextWindow w;

            w = application.ActiveWindow;
            while (application.TextWindows.Count != 0)
            {
                mCloseFile.Execute(application, null);
                if (w == application.ActiveWindow)
                    return ;
                w = application.ActiveWindow;
            }
            application.PostQuitMessage();
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
            return true;
        }
    }
}
