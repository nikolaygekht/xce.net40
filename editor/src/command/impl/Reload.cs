using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;

namespace gehtsoft.xce.editor.command.impl
{
    internal class ReloadCommand : IEditorCommand
    {
        internal ReloadCommand()
        {

        }


        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "Reload";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            if (application.ActiveWindow == null)
                return ;
            FileInfo fi = new FileInfo(application.ActiveWindow.Text.FileName);
            if (!fi.Exists)
            {
                application.ShowMessage("The file does not exists", "Reload File");
                return ;
            }

            if (!application.ActiveWindow.Text.AtSavePoint)
            {
                if (!application.ShowYesNoMessage("The file has been changed. Reloading will cause that all changes will be lost.\n\rProcess the command anyway?", "Reload File"))
                {
                    return ;
                }
            }
            int topRow, topColumn;
            int cursorRow, cursorColumn;
            bool ignoreReload;
            string fileName;
            Encoding enc;

            topRow = application.ActiveWindow.TopRow;
            topColumn = application.ActiveWindow.TopColumn;
            cursorRow = application.ActiveWindow.CursorRow;
            cursorColumn = application.ActiveWindow.CursorColumn;
            ignoreReload = application.ActiveWindow.IgnoreReload;
            fileName = application.ActiveWindow.Text.FileName;
            enc = application.ActiveWindow.Text.Encoding;


            application.CloseWindow(application.ActiveWindow);
            application.OpenFile(fileName, enc);
            if (string.Compare(application.ActiveWindow.Text.FileName, fileName, true) == 0)
            {
                application.ActiveWindow.IgnoreReload = ignoreReload;
                application.ActiveWindow.TopRow = topRow;
                application.ActiveWindow.TopColumn = topColumn;
                application.ActiveWindow.CursorRow = cursorRow;
                application.ActiveWindow.CursorColumn = cursorColumn;
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
            return application.ActiveWindow != null;
        }
    }
}
