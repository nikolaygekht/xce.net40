using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.text;

namespace gehtsoft.xce.editor.command.impl
{
    internal class BackspaceCommand : IEditorCommand
    {
        IEditorCommand mCursor = null;


        internal BackspaceCommand()
        {

        }

        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "Backspace";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            if (mCursor == null)
                mCursor = application.Commands["Cursor"];

            TextWindow w = application.ActiveWindow;
            if (w != null)
            {
                if (!(w.CursorRow == 0 && w.CursorColumn == 0))
                {
                    bool maydelete = (w.CursorRow <= w.Text.LinesCount &&
                                     (w.CursorColumn == 0 || w.CursorColumn <= w.Text.LineLength(w.CursorRow)));
                    mCursor.Execute(application, "lc");
                    if (maydelete)
                        w.DeleteAtCursor(1);
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
            return (application.ActiveWindow != null);

        }
    }
}
