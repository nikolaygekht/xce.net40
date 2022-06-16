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
    internal class FormatDocSourceCommand : IEditorCommand
    {
        public string Name
        {
            get
            {
                return "FormatDocSource";
            }
        }

        public bool IsEnabled(Application application, string param)
        {
            if (application.ActiveWindow == null ||
                application.ActiveWindow.Highlighter == null ||
                application.ActiveWindow.Highlighter.FileType == null ||
                application.ActiveWindow.Highlighter.FileType != "ds")
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
            if (!IsEnabled(application, param))
            {
                application.ShowMessage("File must be a doc source (ds) file", "DS Format");
                return ;
            }

            SyntaxRegion pairStart = application.ColorerFactory.FindSyntaxRegion("def:PairStart");
            SyntaxRegion pairEnd = application.ColorerFactory.FindSyntaxRegion("def:PairEnd");
            SyntaxRegion error = application.ColorerFactory.FindSyntaxRegion("def:Error");

            TextWindow w = application.ActiveWindow;
            XceFileBuffer b = w.Text;
            SyntaxHighlighter h = w.Highlighter;
            int i, depth;
            bool rc;
            List<int> linesToBreak = new List<int>();
            List<int> linesToInsert = new List<int>();

            //pass one - check balance
            for (i = 0, depth = 0; i < b.LinesCount; i++)
            {
                rc = h.GetFirstRegion(i);
                while (rc)
                {
                    if (h.CurrentRegion.Is(error) && h.CurrentRegion.Length > 0)
                    {
                        w.CursorRow = i;
                        w.CursorColumn = h.CurrentRegion.StartColumn;
                        w.EnsureCursorVisible();
                        application.ShowMessage("Error region found", "DS Format");
                        return ;
                    }
                    else if (h.CurrentRegion.Is(pairStart))
                        depth++;
                    else if (h.CurrentRegion.Is(pairEnd))
                    {
                        if (depth == 0)
                        {
                            w.CursorRow = i;
                            w.CursorColumn = h.CurrentRegion.StartColumn;
                            w.EnsureCursorVisible();
                            application.ShowMessage("Umatches close pair found", "DS Format");
                            return ;
                        }
                        else
                            depth--;
                    }
                    rc = h.GetNextRegion();
                }
            }

            if (depth != 0)
            {
                application.ShowMessage("Not all blocks are properly closed", "DS Format");
            }

            //bool prevLineIsEmpty = true;
            int adjustTo;
            b.BeginUndoTransaction();
            //pass two - process
            for (i = 0, depth = 0; i < b.LinesCount; i++)
            {
                //1) truncate eol splace
                TruncEolSpaces(w, i);
                if (b.LineLength(i, false) == 0)
                {
                    //prevLineIsEmpty = true;
                    continue;
                }

                adjustTo = depth * 4;
                bool hasTag = false;
                int s = b.LineStart(i);
                int l = b.LineLength(i);
                rc = h.GetFirstRegion(i);
                
                for (int j = s; j < s + l; j++)
                {
                    if (b[j] == '!')
                    {
                        hasTag = true;
                        break;
                    }
                    if (!char.IsWhiteSpace(b[j]))
                        break;
                }
                
                while (rc)
                {
                    if (h.CurrentRegion.Is(pairStart) && b[s + h.CurrentRegion.StartColumn] == '@')
                    {
                        //if (!prevLineIsEmpty)
                        //    linesToInsert.Add(i);
                        depth++;
                    }
                    else if (h.CurrentRegion.Is(pairEnd) && b[s + h.CurrentRegion.StartColumn] == '@')
                    {
                        depth--;
                        adjustTo = depth * 4;
                    }
                    if (h.CurrentRegion.IsSyntaxRegion && b[s + h.CurrentRegion.StartColumn] == '@')
                        hasTag = true;
                    rc = h.GetNextRegion();
                }
                adjustLineTo(w, i, adjustTo);

                if (!hasTag && b.LineLength(i) > 100)
                    linesToBreak.Add(i);

                //prevLineIsEmpty = false;
            }

            int t, added = 0;
            for (i = 0; i < linesToInsert.Count; i++)
            {
                b.InsertLine(linesToInsert[i] + added);
                added++;
            }

            for (i = 0; i < linesToBreak.Count; i++)
            {
                t = BreakLineByWidthCommand.Do(b, linesToBreak[i] + added, 100);
                if (t > 1)
                    added += (t - 1);
            }


            b.EndUndoTransaction();
            return ;
        }

        private void adjustLineTo(TextWindow w, int line, int adjPos)
        {
            XceFileBuffer b = w.Text;
            int l = b.LineLength(line, false);
            int s = b.LineStart(line);
            int i;

            //find first non-space character
            for (i = s; i < s + l; i++)
                if (!char.IsWhiteSpace(b[i]))
                    break;
            i = i - s;
            //here i is the position of the first non-space character
            if (i > adjPos)
            {
                b.DeleteFromLine(line, 0, i - adjPos);
            }
            else if (i < adjPos)
            {
                b.InsertToLine(line, 0, ' ', adjPos - i);
            }
        }

        private void TruncEolSpaces(TextWindow w, int line)
        {
            XceFileBuffer b = w.Text;
            int l = b.LineLength(line, false);
            int s = b.LineStart(line);
            int i;

            for (i = l - 1; i >= 0; i--)
                if (!char.IsWhiteSpace(b[s + i]))
                    break;

            i++;
            //now i is the last non-whitespace char
            if (i < l)
                b.DeleteFromLine(line, i, l - i);
            return ;
        }
    }
}