using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.editor.util;
using gehtsoft.xce.conio;

namespace gehtsoft.xce.editor.command.impl
{
    internal class MoveBlockCommand : IEditorCommand
    {
        internal MoveBlockCommand()
        {

        }

        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "MoveBlock";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            TextWindow w = application.ActiveWindow;
            if (w == null)
                return ;
            switch (w.BlockType)
            {
            case TextWindowBlock.Line:
                if (w.CursorRow >= w.BlockLineStart && w.CursorRow <= w.BlockLineEnd)
                    return ;
                break;
            case TextWindowBlock.Box:
                if (w.CursorRow >= w.BlockLineStart && w.CursorRow <= w.BlockLineEnd &&
                    w.CursorColumn >= w.BlockColumnStart && w.CursorColumn <= w.BlockColumnEnd)
                    return;
                break;
            case TextWindowBlock.Stream:
                break;
            default:
                return ;
            }
            BlockContent block = new BlockContent(w);
            w.Text.BeginUndoTransaction();
            try
            {
                int shift = 0;
                if (w.BlockType == TextWindowBlock.Box &&
                    w.CursorRow >= w.BlockLineStart && w.CursorRow <= w.BlockLineEnd &&
                    w.CursorColumn > w.BlockColumnEnd)
                    shift = w.BlockColumnEnd - w.BlockColumnStart + 1;
                BlockContentProcessor.DeleteBlock(w);
                BlockContentProcessor.PutBlock(w, block, w.CursorRow, w.CursorColumn - shift);
            }
            finally
            {
                w.Text.EndUndoTransaction();
            }
            block = null;
            GC.Collect();
            if (w.BlockType == TextWindowBlock.Line)
            {
                w._CursorRow = w.BlockLineStart;
                w._CursorColumn = 0;

            }
            else if (w.BlockType == TextWindowBlock.Box)
            {
                w._CursorRow = w.BlockLineStart;
                w._CursorColumn = w.BlockColumnStart;

            }
            w.EnsureCursorVisible();
            w.invalidate();
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
            return application.ActiveWindow != null && application.ActiveWindow.BlockType != TextWindowBlock.None;
        }
    }
}
