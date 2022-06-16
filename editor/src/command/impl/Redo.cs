using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;

namespace gehtsoft.xce.editor.command.impl
{
    internal class RedoCommand : IEditorCommand
    {
        internal RedoCommand()
        {

        }

        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "Redo";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            if (application.ActiveWindow != null &&
                application.ActiveWindow.Text.CanRedo)
            {
                int position = application.ActiveWindow.Text.Redo();
                if (position == -1)
                    position = 0;
                int row, column;
                row = application.ActiveWindow.Text.LineFromPosition(position);
                column = position - application.ActiveWindow.Text.LineStart(row);
                application.ActiveWindow.CursorRow = row;
                application.ActiveWindow.CursorColumn = column;
                application.ActiveWindow.EnsureCursorVisible();
                application.ActiveWindow.DeselectBlock();
                application.ActiveWindow.HighlightRangePosition = -1;
                application.ActiveWindow.HighlightRangeLength = 0;
                application.ActiveWindow.invalidate();
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
            return (application.ActiveWindow != null && application.ActiveWindow.Text.CanRedo);
        }
    }
}
