using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;

namespace gehtsoft.xce.editor.command.impl
{
    internal class UndoCommand : IEditorCommand
    {
        internal UndoCommand()
        {

        }

        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "Undo";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            if (application.ActiveWindow != null &&
                application.ActiveWindow.Text.CanUndo)
            {
                application.ActiveWindow.Text.Undo();
                /*
                int position = application.ActiveWindow.Text.Undo();
                int row, column;
                row = application.ActiveWindow.Text.LineFromPosition(position);
                column = position - application.ActiveWindow.Text.LineStart(row);
                application.ActiveWindow.CursorRow = row;
                application.ActiveWindow.CursorColumn = column;
                application.ActiveWindow.DeselectBlock();
                */
                application.ActiveWindow.EnsureCursorVisible();
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
            return (application.ActiveWindow != null && application.ActiveWindow.Text.CanUndo);

        }
    }
}
