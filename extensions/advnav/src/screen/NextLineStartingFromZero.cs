using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.editor.util;
using gehtsoft.xce.text;

namespace gehtsoft.xce.extension.advnav_commands
{
    internal class NextLineStartingFromZeroCommand : IEditorCommand
    {
        public string Name
        {
            get
            {
                return "NextLineStartingFromZero";
            }
        }

        public bool IsEnabled(Application application, string param)
        {
            return application.ActiveWindow != null;
        }

        public bool IsChecked(Application application, string param)
        {
            return false;
        }

        public void Execute(Application application, string param)
        {
            int s;
            XceFileBuffer b = application.ActiveWindow.Text;
            int row = application.ActiveWindow.CursorRow + 1;
            while (row < b.LinesCount)
            {
                s = b.LineStart(row);
                int l = b.LineLength(row);
                if (l > 0)
                {
                    if (!char.IsWhiteSpace(b[s]))
                        break;
                }
                row++;
            }
            if (row < b.LinesCount)
            {
                application.ActiveWindow.CursorRow = row;
                application.ActiveWindow.EnsureCursorVisible();
            }
        }
    }
}
