using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.colorer;
using gehtsoft.xce.editor.search;

namespace gehtsoft.xce.editor.command.impl
{
    internal class CheckPairCommand : IEditorCommand, IDialogCommand
    {
        internal CheckPairCommand()
        {

        }

        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "CheckPair";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            TextWindow w = application.ActiveWindow;
            if (w.CursorRow < w.Text.LinesCount)
            {
                int l0 = w.Text.LineLength(w.CursorRow);
                if (w.CursorColumn <= l0)
                {
                    SyntaxHighlighter h = w.Highlighter;
                    h.Colorize(w.CursorRow, 1);
                    SyntaxHighlighterPair pair = null;
                    if (w.CursorColumn < l0)
                        pair = h.MatchPair(w.CursorRow, w.CursorColumn);
                    if (pair == null && w.CursorColumn > 0)
                        pair = h.MatchPair(w.CursorRow, w.CursorColumn - 1);

                    if (pair != null)
                    {
                        SyntaxHighlighterRegion s = pair.Start;
                        SyntaxHighlighterRegion e = pair.End;

                        if (s.Line == w.CursorRow && w.CursorColumn >= s.StartColumn && w.CursorColumn <= s.EndColumn)
                        {
                            w.CursorRow = e.Line;
                            w.CursorColumn = e.StartColumn;
                        }
                        else
                        {
                            w.CursorRow = s.Line;
                            w.CursorColumn = s.StartColumn;
                        }
                        w.EnsureCursorVisible();
                        w.invalidate();
                        pair.Dispose();
                    }
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
            return application.ActiveWindow != null;
        }
        
        public void DialogExecute(Application application, XceDialog dlg, string parameter)
        {
            if (dlg is SearchDialog && 
                application.WindowManager.getFocus() != null &&
                application.WindowManager.getFocus() is RegexSyntaxComboBox)
            {
                RegexSyntaxComboBox box = application.WindowManager.getFocus() as RegexSyntaxComboBox;
                int i = box.LastOppositePair;
                if (i >= 0)
                    box.SetCaret(i);
            }
        }
    }
}
