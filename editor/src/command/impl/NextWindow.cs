using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;


namespace gehtsoft.xce.editor.command.impl
{
    internal class NextWindowCommand : IEditorCommand
    {
        public NextWindowCommand()
        {
        }

        public string Name
        {
            get
            {
                return "NextWindow";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            TextWindow w = application.ActiveWindow;
            TextWindowCollection c = application.TextWindows;
            if (c.Count < 2)
                return ;
            int x = c.Find(w);
            x++;
            if (x >= c.Count)
                x = 0;
            w = c[x];
            application.ActivateWindow(w);
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
