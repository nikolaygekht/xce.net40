using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.text;
using gehtsoft.xce.extension.advnav_commands;

namespace gehtsoft.xce.extension.advnav_layout
{
    internal class FixLayoutMistypeCommand : FindWordBase, IEditorCommand
    {
        private readonly char[] en = "QWERTYUIOP{}|ASDFGHJKL:\"ZXCVBNM<>?qwertyuiop[]\\asdfghjkl;'zxcvbnm,./".ToCharArray();
        private readonly char[] ru = "ЙЦУКЕНГШЩЗХЪ/ФЫВАПРОЛДЖЭЯЧСМИТЬБЮ,йцукенгшщзхъ\\фывапролджэячсмитьбю.".ToCharArray();
        private bool mCanWork;

        internal FixLayoutMistypeCommand()
        {
            if (en.Length != ru.Length)
            {
                mCanWork = false;
                Console.WriteLine("Switch Layout extension error!");
            }
            else
            {
                mCanWork = true;
            }
        }

        public bool IsEnabled(Application application, string param)
        {
            return application.ActiveWindow != null && mCanWork;
        }

        public bool IsChecked(Application application, string param)
        {
            return false;
        }

        public void Execute(Application application, string param)
        {
            if (application.ActiveWindow == null)
                return ;

            char[] from = null;
            char[] to = null;

            if (param == null)
            {
                return ;
            }
            else if (param == "ru2en")
            {
                from = ru;
                to = en;
            }
            else if (param == "en2ru")
            {
                from = en;
                to = ru;
            }
            else
                return ;

            application.ActiveWindow.Text.BeginUndoTransaction();

            try
            {
                int pos, i, j;
                char c;
                XceFileBuffer b = application.ActiveWindow.Text;
                if (CursorToPosition(application.ActiveWindow, out pos, true))
                {
                    pos = pos - 1;
                    while (pos >= 0)
                    {
                        i = -1;
                        c = b[pos];
                        if (char.IsWhiteSpace(c))
                            pos = pos - 1;
                        for (j = 0; j < from.Length && i == -1; j++)
                            if (from[j] == c)
                                i = j;

                        if (i != -1)
                        {
                            b.DeleteRange(pos, 1);
                            b.InsertRange(pos, to, i, 1);
                        }
                        else
                            break;
                        pos = pos - 1;
                    }
                    GoPosition(application.ActiveWindow, pos + 1);
                }
            }
            finally
            {
                application.ActiveWindow.Text.EndUndoTransaction();
            }
        }

        public string Name
        {
            get
            {
                return "FixLayoutMistype";
            }
        }



    }
}
