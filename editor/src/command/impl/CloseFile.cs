using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.text;

namespace gehtsoft.xce.editor.command.impl
{
    internal class CloseFileCommand : IEditorCommand
    {
        IEditorCommand mSaveCommand = null;

        internal CloseFileCommand()
        {

        }

        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "CloseFile";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            if (mSaveCommand == null)
                mSaveCommand = application.Commands["SaveFile"];

            TextWindow w = application.ActiveWindow;
            if (w == null)
                return ;

            if (!w.Text.AtSavePoint && w.SaveRequired)
            {
                MessageBoxButton b = MessageBox.Show(application.WindowManager, application.ColorScheme, "File is not saved! Save it now?", "Warning", MessageBoxButtonSet.YesNoCancel);
                switch (b)
                {
                case    MessageBoxButton.Yes:
                        mSaveCommand.Execute(application, null);
                        if (!w.Text.AtSavePoint)
                            return ;
                        break;
                case    MessageBoxButton.Cancel:
                        return ;
                }
            }

            application.CloseWindow(w);
            if (application.TextWindows.Count == 0)
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
            return application.ActiveWindow != null;
        }
    }
}
