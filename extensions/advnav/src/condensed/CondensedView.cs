using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.text;
using gehtsoft.xce.editor.util;
using gehtsoft.xce.colorer;

namespace gehtsoft.xce.extension.advnav_condensed
{
    internal class CondensedViewCommand : IEditorCommand
    {
        public string Name
        {
            get
            {
                return "CondensedView";
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
            if (application.ActiveWindow == null)
                return ;
        
            List<CondensedLine> lines = null;
            if (param == null || param.Length == 0 || param == "top")
            {
                lines = PrepareByHome(application.ActiveWindow.Text, 0);
            }
            else if (param != null && param == "cur")
            {
                lines = PrepareByHome(application.ActiveWindow.Text, application.ActiveWindow.CursorColumn);
            }
            else if (param != null && param == "syntax")
            {
                lines = PrepareByOutline(application.ActiveWindow.Text, application.ColorerFactory, application.ActiveWindow.Highlighter);
            }
            else
            {
                application.ShowMessage("Unknown Mode", "Condensed View");
                return ;
            }
            
            if (lines != null)
            {
                CondensedViewWindow view = new CondensedViewWindow(application.ActiveWindow.Text, lines, application.ActiveWindow.CursorRow, application.ActiveWindow.Highlighter, application.ColorScheme);
                if (view.DoModal(application.WindowManager))
                {
                    application.ActiveWindow.CursorRow = view.SelSourceLine.Line;
                    application.ActiveWindow.CursorColumn = view.SelSourceLine.Position;
                    application.ActiveWindow.EnsureCursorVisible();
                }
            }
            else
                application.ShowMessage("No matching lines is found", "Condensed View");
            
            GC.Collect();
        }
        
        private List<CondensedLine> PrepareByHome(XceFileBuffer buffer, int pos)
        {
            List<CondensedLine> lines = new List<CondensedLine>();
            int i, s, j, l, ls;
            
            for (i = 0; i < buffer.LinesCount; i++)
            {
                s = buffer.LineStart(i);
                l = buffer.LineLength(i);
                
                ls = -1;
                for (j = 0; j < l; j++)
                {
                    if (!Char.IsWhiteSpace(buffer[s + j]))
                    {
                        ls = j;
                        break;
                    }
                }
                
                if (ls >= 0 && ls <= pos)
                    lines.Add(new CondensedLine(i, pos));
            }
            if (lines.Count > 0)
                return lines;
            else
                return null;
        }

        private List<CondensedLine> PrepareByOutline(XceFileBuffer buffer, ColorerFactory colorer, SyntaxHighlighter highlighter)
        {
            List<CondensedLine> lines = new List<CondensedLine>();
            
            SyntaxRegion r = colorer.FindSyntaxRegion("def:Outlined");
            int i;
            bool rc;
            
            for (i = 0; i < buffer.LinesCount; i++)
            {
                rc = highlighter.GetFirstRegion(i);
                while (rc)
                {
                    if (highlighter.CurrentRegion.Is(r))
                        lines.Add(new CondensedLine(i, highlighter.CurrentRegion.StartColumn));
                    rc = highlighter.GetNextRegion();
                }
            }
            if (lines.Count > 0)
                return lines;
            else
                return null;
        }
    }
}
