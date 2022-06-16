using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;


namespace gehtsoft.xce.editor.command.impl
{
    internal class MarkBlockCommand : IEditorCommand
    {
        public MarkBlockCommand()
        {
        }

        public string Name
        {
            get
            {
                return "MarkBlock";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            if (parameter == null || parameter.Length != 1)
                return ;

            TextWindowBlock type = TextWindowBlock.None;

            switch (parameter[0])
            {
            case    's':
                    type = TextWindowBlock.Stream;
                    break;
            case    'b':
                    type = TextWindowBlock.Box;
                    break;
            case    'l':
                    type = TextWindowBlock.Line;
                    break;
            case    'n':
                    break;
            default:
                    return ;
            }

            TextWindow w = application.ActiveWindow;

            if (w == null)
                return;

            if (type == TextWindowBlock.None)
            {
                w.DeselectBlock();
                return ;
            }

            if (type != w.BlockType || (w.BlockColumnStart != w.BlockColumnEnd || w.BlockLineStart != w.BlockLineEnd))
                w.StartBlock(type, w.CursorRow, w.CursorColumn);
            else
                w.EndBlock(w.CursorRow, w.CursorColumn);
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
            if (parameter != null && parameter.Length == 1 &&
                application.ActiveWindow != null)
            {
                if (parameter[0] == 's' ||
                 parameter[0] == 'l' ||
                 parameter[0] == 'b')
                    return true;
                else if (parameter[0] == 'n' && application.ActiveWindow.BlockType != TextWindowBlock.None)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

    }
}
