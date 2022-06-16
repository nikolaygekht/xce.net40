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
    internal class EnterCommand : IEditorCommand
    {
        internal EnterCommand()
        {

        }

        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "Enter";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            TextWindow w = application.ActiveWindow;
            if (w != null)
            {
                bool alwaysInsert = false;
                if (parameter != null && parameter == "i")
                    alwaysInsert = true;
                int r = w.CursorRow;
                bool outOfRange = r >= w.Text.LinesCount;

                int i, l;
                int firstColumn = 0;
                if (w.FileTypeInfo.AutoIdent && !outOfRange)
                {
                    l = w.Text.LineLength(r);
                    for (i = 0; i < l && w[r, i] == ' '; i++)
                        firstColumn = i + 1;
                }

                bool contentSplit = false;
                int splitAt = w.CursorColumn;

                if (w.InsertMode || alwaysInsert)
                    contentSplit = w.SplitLineAtCursor();

                w.CursorRow = w.CursorRow + 1;
                w.CursorColumn = 0;

                firstColumn = Math.Min(splitAt, firstColumn);
                if ((w.InsertMode || alwaysInsert) && firstColumn != 0 && contentSplit)
                    w.Stroke(' ', firstColumn);
                w.CursorColumn = firstColumn;
                w.EnsureCursorVisible();
                w.invalidate();
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
