using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.editor.util;
using gehtsoft.xce.text;

namespace gehtsoft.xce.editor.command.impl
{
    internal class DeleteToEndOfWordCommand : IEditorCommand
    {
        internal DeleteToEndOfWordCommand()
        {

        }

        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "DeleteToEndOfWord";
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
                int r = w.CursorRow;
                if (r < w.Text.LinesCount)
                {
                    int cl = w.Text.LineLength(r);
                    if (w.CursorColumn < cl)
                    {
                        CharClass c1 = CharUtil.GetCharClass(w[r, w.CursorColumn]);
                        int i = w.CursorColumn + 1;
                        while (i < cl && c1 == CharUtil.GetCharClass(w[r, i]))
                            i++;
                        w.DeleteAtCursor(i - w.CursorColumn);

                    }
                    else
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
