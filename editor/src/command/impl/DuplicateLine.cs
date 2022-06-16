using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;

namespace gehtsoft.xce.editor.command.impl
{
    internal class DuplicateLineCommand : IEditorCommand
    {
        internal DuplicateLineCommand()
        {

        }


        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "DuplicateLine";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            if (!IsEnabled(application, parameter))
                return ;
            string s = application.ActiveWindow.Text.GetRange(application.ActiveWindow.Text.LineStart(application.ActiveWindow.CursorRow), application.ActiveWindow.Text.LineLength(application.ActiveWindow.CursorRow, false));
            application.ActiveWindow.CursorRow = application.ActiveWindow.CursorRow + 1;
            application.ActiveWindow.CursorColumn = 0;
            application.ActiveWindow.Stroke(s, 0, s.Length);
            application.ActiveWindow.SplitLineAtCursor();
            application.ActiveWindow.EnsureCursorVisible();
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
            return application.ActiveWindow != null &&
                   application.ActiveWindow.CursorRow >= 0 &&
                   application.ActiveWindow.CursorRow < application.ActiveWindow.Text.LinesCount;
        }
    }
}
