using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.text;
using gehtsoft.xce.editor.util;

namespace gehtsoft.xce.editor.command.impl
{
    internal class BackspaceWordCommand : IEditorCommand
    {
        IEditorCommand mCursor = null;
        IEditorCommand mBackspace = null;


        internal BackspaceWordCommand()
        {

        }

        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "BackspaceWord";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            if (mCursor == null)
                mCursor = application.Commands["Cursor"];
            if (mBackspace == null)
                mBackspace = application.Commands["Backspace"];

            TextWindow w = application.ActiveWindow;
            if (w != null)
            {
                int r1, c1, r2, c2;
                r1 = w.CursorRow;
                c1 = w.CursorColumn;

                mCursor.Execute(application, "lw");
                r2 = w.CursorRow;
                c2 = w.CursorColumn;

                w.CursorRow = r1;
                w.CursorColumn = c1;
                while (!(w.CursorRow == r2 && w.CursorColumn == c2))
                    mBackspace.Execute(application, null);
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
