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
    internal class TabCommand : IEditorCommand
    {
        IEditorCommand mCursor = null;


        internal TabCommand()
        {

        }

        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "Tab";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            if (mCursor == null)
                mCursor = application.Commands["Cursor"];

            if (application.ActiveWindow == null)
                return ;

            TextWindow w = application.ActiveWindow;
            XceFileBuffer b = w.Text;
            int i, dist;

            bool hasParam = parameter != null && parameter.Length > 0;


            if (!hasParam || (hasParam && parameter[0] != 'b'))
            {
                bool alwaysInsert = false;
                if (hasParam && parameter[0] == 'i')
                    alwaysInsert = true;
                //tab forward
                dist = b.TabLength(w.CursorColumn);
                if (dist == 0)
                    dist = b.TabSize;
                if ((w.InsertMode || alwaysInsert) &&
                    w.CursorRow < b.LinesCount &&
                    w.CursorColumn < b.LineLength(w.CursorRow))
                {
                    w.Stroke(' ', dist);
                }
                else
                {
                    for (i = 0; i < dist; i++)
                        mCursor.Execute(application, "rc");
                }
            }
            else
            {
                if (w.CursorColumn == 0)
                    return ;
                //tab backward
                dist = b.TabSize - b.TabLength(w.CursorColumn);
                if (dist == 0)
                    dist = b.TabSize;
                if (dist > w.CursorColumn)
                    dist = w.CursorColumn;
                for (i = 0; i < dist; i++)
                    mCursor.Execute(application, "lc");
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
