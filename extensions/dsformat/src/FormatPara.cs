using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.colorer;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.text;

namespace gehtsoft.xce.extension.dsformat_impl
{
    internal class FormatParaByWidth : IEditorCommand
    {
        public string Name
        {
            get
            {
                return "FormatParaByWidth";
            }
        }

        public bool IsEnabled(Application application, string param)
        {
            if (application.ActiveWindow == null)
                return false;
            else
                return true;
        }

        public bool IsChecked(Application application, string param)
        {
            return false;
        }

        public void Execute(Application application, string param)
        {
            if (application.ActiveWindow == null)
                return ;

            int width = 0;

            if (param == null || param.Length < 1 ||
                !Int32.TryParse(param, out width))
                width = application.ActiveWindow.Width;

            if (width < 0)
                width = application.ActiveWindow.Width + width;


            int line = application.ActiveWindow.CursorRow;

            XceFileBuffer b = application.ActiveWindow.Text;
            b.BeginUndoTransaction();

            bool docSource = false;

            if (application.ActiveWindow.Highlighter != null || application.ActiveWindow.Highlighter.FileType != null || application.ActiveWindow.Highlighter.FileType == "ds")
                docSource = true;

            //collect the para into one line


            try
            {
                while (!isEmpty(b, line + 1, docSource))
                    b.JoinWithNext(line);

                if (BreakLineByWidthCommand.Do(b, line, width) == -1)
                    application.ShowMessage("The line is adjusted farer than the maximum width which has been requested", "Split");
            }
            finally
            {
                application.ActiveWindow.Text.EndUndoTransaction();
            }
        }

        private static bool isEmpty(XceFileBuffer b, int line, bool docSource)
        {
            int s;
            int l;
            if (line >= b.LinesCount)
                return true;
            s = b.LineStart(line);
            l = b.LineLength(line);
            int i;
            for (i = 0; i < l; i++)
            {
                if (docSource && b[s + i] == '@')
                    return true;
                if (!char.IsWhiteSpace(b[s + i]))
                    break;
            }

            return i >= l;
        }

    }
}