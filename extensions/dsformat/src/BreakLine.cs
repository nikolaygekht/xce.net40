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
    internal class BreakLineByWidthCommand : IEditorCommand
    {
        public string Name
        {
            get
            {
                return "BreakLineByWidth";
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

            application.ActiveWindow.Text.BeginUndoTransaction();
            try
            {
                if (Do(application.ActiveWindow.Text, line, width) == -1)
                {
                    application.ShowMessage("The line is adjusted farer than the maximum width which has been requested", "Split");
                }
            }
            finally
            {
                application.ActiveWindow.Text.EndUndoTransaction();
            }
        }

        internal class Region
        {
            internal int from;
            internal int length;
            internal bool whitespace;
        }

        internal static int Do(XceFileBuffer b, int line, int maxWidth)
        {
            int l = b.LineLength(line, false);
            int s = b.LineStart(line);
            int i;
            int basecol;

            if (l <= maxWidth)
                return 1;

            List<Region> regions = new List<Region>();
            Region curr = null;
            //split line into regions
            for (i = s; i < s + l; i++)
            {
                if (curr == null)
                {
                    curr = new Region();
                    curr.from = i;
                    curr.length = 0;
                    curr.whitespace = char.IsWhiteSpace(b[i]);
                    regions.Add(curr);
                    continue;
                }

                if (char.IsWhiteSpace(b[i]) != curr.whitespace)
                {
                    curr.length = i - curr.from;
                    curr = new Region();
                    curr.from = i;
                    curr.length = 0;
                    curr.whitespace = char.IsWhiteSpace(b[i]);
                    regions.Add(curr);
                }
            }

            regions[regions.Count - 1].length = (s + l) - regions[regions.Count - 1].from;

            if (regions.Count < 2)
                return 1;

            if (regions[0].whitespace)
                basecol = regions[0].length;
            else
                basecol = 0;

            if (basecol >= maxWidth)
                return -1;

            char[] adjust = null;

            if (basecol > 0)
            {
                adjust = new char[basecol];
                for (i = 0; i < basecol; i++)
                    adjust[i] = ' ';
            }

            int lines = 1;
            int cpos = 0;
            int start = 0;

            start = b.LineStart(line);

            for (i = 0; i < regions.Count; i++)
            {
                if (i == 0 && regions[i].whitespace)
                {
                    cpos += regions[i].length;
                    continue;
                }

                if (cpos + regions[i].length > maxWidth)
                {
                    //if region to be split started from the beginning
                    //of the line - skip it!
                    if (regions[i].from == start + basecol)
                    {
                        cpos += regions[i].length;
                        continue;
                    }

                    //if region is not first - make a split
                    b.InsertRange(regions[i].from, "\r\n");
                    lines++;

                    for (int j = i; j < regions.Count; j++)
                        regions[j].from += 2;

                    start = regions[i].from;

                    //if we split on whitespace region
                    //delete it and insert basecol spaces
                    if (regions[i].whitespace)
                    {
                        b.DeleteRange(regions[i].from, regions[i].length);
                        if (basecol > 0)
                            b.InsertRange(start, adjust, 0, basecol);

                        for (int j = i; j < regions.Count; j++)
                            regions[j].from -= (regions[i].length - basecol);
                        cpos = basecol;
                    }
                    else
                    {
                        if (basecol > 0)
                            b.InsertRange(start, adjust, 0, basecol);

                        for (int j = i; j < regions.Count; j++)
                            regions[j].from += basecol;

                        cpos = basecol + regions[i].length;
                    }
                }
                else
                {
                    if (regions[i].whitespace && regions[i].length > 1)
                    {
                        int t = regions[i].length - 1;
                        b.DeleteRange(regions[i].from, t);
                        for (int j = i + 1; j < regions.Count; j++)
                            regions[j].from -= t;
                        regions[i].length = 1;
                    }
                    cpos += regions[i].length;
                }
            }
            return lines;
        }
    }
}