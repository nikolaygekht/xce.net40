using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.conio.win;

namespace gehtsoft.xce.editor.command.impl
{
    internal class SwitchWindowCommand : IEditorCommand
    {
        internal SwitchWindowCommand()
        {

        }

        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "SwitchWindow";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {

            TextWindow w;
            w = application.ActiveWindow;

            WindowListDialog dlg = new WindowListDialog("Switch to...", application);
            int cnt = 0;

            foreach (TextWindow w0 in application.TextWindows)
            {
                if (w != w0)
                {
                    dlg.AddWindow(w0);
                    cnt ++;
                }
            }

            if (cnt > 0)
            {
                if (dlg.DoModal(application.WindowManager) == Dialog.DialogResultOK)
                {
                    if (dlg.Window != null)
                        application.ActivateWindow(dlg.Window);
                }
            }
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
            return application.TextWindows.Count > 1;
        }
    }
}
