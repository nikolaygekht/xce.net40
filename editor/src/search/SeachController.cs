using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.text;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.colorer;

namespace gehtsoft.xce.editor.search
{
    public class XceSearchController
    {
        public static void Search(Application application, string re, bool ignoreCase)
        {
            SearchInfo si = new SearchInfo();
            si.Search = re;
            si.SearchMode = SearchMode.Search;
            si.Regex = true;
            si.IgnoreCase = ignoreCase;
            SeachController.searchStep(application, si, application.ActiveWindow, true);
        }
    }

    internal class SeachController
    {
        private const string NOT_FOUND = "No more occurrences of the the text has been found";
        private const string ZERO_MATCH = "An empty string matched the search expression";
        private static MessageBoxButtonInfo[] mReplaceButtons = new MessageBoxButtonInfo[] { new MessageBoxButtonInfo("< &Replace >", MessageBoxButton.Ok),
                                                                                             new MessageBoxButtonInfo("< Replace &All >", MessageBoxButton.Option1),
                                                                                             new MessageBoxButtonInfo("< &Skip >", MessageBoxButton.Option2),
                                                                                             new MessageBoxButtonInfo("< &Cancel >", MessageBoxButton.Cancel) };
        private const MessageBoxButton REPACE_DO = MessageBoxButton.Ok;
        private const MessageBoxButton REPACE_DO_ALL = MessageBoxButton.Option1;
        private const MessageBoxButton REPACE_SKIP = MessageBoxButton.Option2;
        private const MessageBoxButton REPACE_CANCEL = MessageBoxButton.Cancel;

        static internal void searchStep(Application application, SearchInfo info, TextWindow window)
        {
            searchStep(application, info, window, false);
        }

        static internal void searchStep(Application application, SearchInfo info, TextWindow window, bool _noui)
        {
            XceFileBufferRegex re = null;
            AutoArray<char> replacement = new AutoArray<char>();
            if (info.SearchMode == SearchMode.Replace)
                window.Text.BeginUndoTransaction();
            try
            {
                try
                {
                    re = new XceFileBufferRegex(RegexBuilder.Build(info.Search, info.Regex, info.IgnoreCase, info.WholeWord));
                }
                catch (ArgumentException e)
                {
                    MessageBox.Show(application.WindowManager, application.ColorScheme, "Can't parse search expression\r\n" + e.Message, "Search/Replace", MessageBoxButtonSet.Ok);
                    return;
                }

                int startLine = window.CursorRow;
                int startColumn = window.CursorColumn;

                if (window.CursorRow >= window.Text.LinesCount)
                {
                    if (_noui)
                    {
                        window.CursorRow = window.Text.LinesCount;
                        window.CursorColumn = 0;
                    }
                    else
                    {
                        window.HighlightRangePosition = -1;
                        window.HighlightRangeLength = 0;
                        MessageBox.Show(application.WindowManager, application.ColorScheme, NOT_FOUND, "Search/Replace", MessageBoxButtonSet.Ok);
                    }
                    return;
                }

                if (startColumn >= window.Text.LineLength(window.CursorRow))
                    startColumn = window.Text.LineLength(window.CursorRow);

                int pos = window.Text.LineStart(window.CursorRow) + window.CursorColumn;

                if (object.ReferenceEquals(window, info.LastSearchTarget) &&
                    pos == info.LastSearchPos)
                {
                    pos += info.LastSearchLength;
                    info.ResetLastSearch();
                }

                bool noui = _noui;

                if (info.SearchMode == SearchMode.Replace && info.ReplaceAll)
                    noui = true;

                bool inrange = false;
                if (info.SearchInRange)
                    inrange = true;

                while (true)
                {
                    int length = window.Text.Length - pos;
                    if (length <= 0)
                    {
                        if (_noui)
                        {
                            window.DeselectBlock();
                            window.CursorRow = window.Text.LinesCount;
                            window.CursorColumn = 0;
                        }
                        else
                        {
                            window.HighlightRangePosition = -1;
                            window.HighlightRangeLength = 0;
                            MessageBox.Show(application.WindowManager, application.ColorScheme, NOT_FOUND, "Search/Replace", MessageBoxButtonSet.Ok);
                        }
                        break;
                    }

                    bool rc = re.Match(window.Text, pos, length);

                    if (!rc)
                    {
                        if (_noui)
                        {
                            window.CursorRow = window.Text.LinesCount;
                            window.CursorColumn = 0;
                        }
                        else
                        {
                            window.HighlightRangePosition = -1;
                            window.HighlightRangeLength = 0;
                            MessageBox.Show(application.WindowManager, application.ColorScheme, NOT_FOUND, "Search/Replace", MessageBoxButtonSet.Ok);
                        }
                        break;
                    }
                    else
                    {
                        int pos1, length1, row_s, column_s, row_e, column_e;
                        length1 = re.Length(0);
                        if (length1 == 0)
                        {
                            window.HighlightRangePosition = -1;
                            window.HighlightRangeLength = 0;
                            MessageBox.Show(application.WindowManager, application.ColorScheme, ZERO_MATCH, "Search/Replace", MessageBoxButtonSet.Ok);
                            break;
                        }
                        pos1 = re.Start(0);

                        LineColumnFromPosition(window.Text, pos1, out row_s, out column_s);
                        pos1 = pos1 + length1 - 1;
                        LineColumnFromPosition(window.Text, pos1, out row_e, out column_e);

                        if (inrange && !IsRangeInBlock(window, re.Start(0), re.Length(0)))
                        {
                            pos += re.Length(0);
                            continue;
                        }

                        if (!noui || info.SearchMode == SearchMode.Search)
                        {
                            window.CursorRow = row_e;
                            window.CursorColumn = column_e;
                            window.EnsureCursorVisibleInCenter();
                            window.CursorRow = row_s;
                            window.CursorColumn = column_s;
                            window.EnsureCursorVisibleInCenter();
                            window.HighlightRangePosition = re.Start(0);
                            window.HighlightRangeLength = re.Length(0);
                            window.invalidate();
                        }

                        info.LastSearchTarget = window;
                        info.LastSearchPos = re.Start(0);
                        info.LastSearchLength = re.Length(0);

                        if (info.SearchMode == SearchMode.Search)
                            break;
                        else
                        {
                            MessageBoxButton answer;

                            if (noui)
                                answer = REPACE_DO;
                            else
                            {
                                string s = string.Format("Occurrence has been found at {0} {1}, {2} character long", row_e, column_e, re.Length(0));
                                answer = MessageBox.Show(application.WindowManager, application.ColorScheme, s, "Search/Replace", mReplaceButtons);
                            }
                            if (answer == REPACE_DO_ALL)
                            {
                                noui = true;
                                answer = REPACE_DO;
                            }

                            if (answer == REPACE_CANCEL)
                                break;
                            if (answer == REPACE_DO)
                            {
                                int replaceLength = 0;

                                if (info.Regex)
                                {
                                    StringBuilder r = new StringBuilder();
                                    string s = info.Replace;
                                    int l = s.Length;
                                    int transform = 0;

                                    for (int i = 0; i < l; i++)
                                    {
                                        if (s[i] == '\\' && i != l - 1)
                                        {
                                            switch (s[i + 1])
                                            {
                                                case '\\':
                                                    r.Append('\\');
                                                    break;
                                                case 't':
                                                    r.Append('\t');
                                                    break;
                                                case 'n':
                                                    r.Append(window.Text.Eol);
                                                    break;
                                                case 'u':
                                                    if (transform == 1)
                                                        transform = 0;
                                                    else
                                                        transform = 1;
                                                    break;
                                                case 'l':
                                                    if (transform == 2)
                                                        transform = 0;
                                                    else
                                                        transform = 2;
                                                    break;
                                                case '0':
                                                case '1':
                                                case '2':
                                                case '3':
                                                case '4':
                                                case '5':
                                                case '6':
                                                case '7':
                                                case '8':
                                                case '9':
                                                    {
                                                        int g = (int)(s[i + 1] - '0');
                                                        if (g >= 0 && g < re.MatchesCount)
                                                        {
                                                            int j = re.Length(g);
                                                            if (j > 0)
                                                            {

                                                                replacement.Ensure(j);
                                                                window.Text.GetRange(re.Start(g), j, replacement.Array, 0);
                                                                if (transform == 1)
                                                                    for (int ii = 0; ii < j; ii++)
                                                                        replacement.Array[ii] = Char.ToUpper(replacement.Array[ii]);
                                                                else if (transform == 2)
                                                                    for (int ii = 0; ii < j; ii++)
                                                                        replacement.Array[ii] = Char.ToLower(replacement.Array[ii]);
                                                                r.Append(replacement.Array, 0, j);
                                                            }
                                                        }
                                                    }
                                                    break;
                                            }
                                            i++;
                                        }
                                        else
                                        {
                                            r.Append(s[i]);
                                        }
                                    }
                                    replaceLength = r.Length;
                                    if (replaceLength > 0)
                                    {
                                        replacement.Ensure(replaceLength);
                                        r.CopyTo(0, replacement.Array, 0, replaceLength);
                                    }
                                }
                                else
                                {
                                    replaceLength = info.Replace.Length;
                                    if (replaceLength > 0)
                                    {
                                        replacement.Ensure(replaceLength);
                                        info.Replace.CopyTo(0, replacement.Array, 0, replaceLength);
                                    }
                                }
                                bool si, ei;
                                RangeRemoved(window, re.Start(0), re.Length(0), out si, out ei);
                                window.Text.DeleteRange(re.Start(0), re.Length(0));
                                if (replaceLength > 0)
                                {
                                    window.Text.InsertRange(re.Start(0), replacement.Array, 0, replaceLength);
                                    int l = window.Text.ExpandTabsInRange(re.Start(0), replaceLength);
                                    if (window.FileTypeInfo.AutoIdent)
                                        l = window.Text.AutoIndentLinesInRange(re.Start(0), l);
                                    RangeAdded(window, re.Start(0), l, si, ei);
                                    pos = re.Start(0) + l;
                                }
                                else
                                {
                                    pos = re.Start(0);
                                }
                            }
                            else if (answer == REPACE_SKIP)
                            {
                                pos = re.Start(0) + re.Length(0);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (re != null)
                    re.Dispose();
                if (info.SearchMode == SearchMode.Replace)
                    window.Text.EndUndoTransaction();
            }
        }

        private static void LineColumnFromPosition(XceFileBuffer buffer, int position, out int row, out int column)
        {
            if (position >= buffer.Length)
            {
                row = buffer.LinesCount;
                column = 0;
            }
            else
            {
                row = buffer.LineFromPosition(position);
                column = position - buffer.LineStart(row);
            }
        }

        private static bool IsRangeInBlock(TextWindow w, int start, int length)
        {
            bool rc = false;
            switch (w.BlockType)
            {
                case TextWindowBlock.Stream:
                    {
                        int streamStart;
                        int streamEnd;
                        int streamLength;
                        int length1;

                        streamStart = w.Text.LineStart(w.BlockLineStart);
                        length1 = w.Text.LineLength(w.BlockLineStart, true);
                        if (w.BlockColumnStart > length1)
                            streamStart += length1;
                        else
                            streamStart += w.BlockColumnStart;

                        streamEnd = w.Text.LineStart(w.BlockLineEnd);
                        length1 = w.Text.LineLength(w.BlockLineEnd, true);
                        if (w.BlockColumnEnd > length1)
                            streamEnd += length1;
                        else
                            streamEnd += w.BlockColumnEnd;

                        streamLength = streamEnd - streamStart + 1;

                        rc = start >= streamStart && start + length <= streamStart + streamLength;
                    }
                    break;
                case TextWindowBlock.Box:
                    {
                        int lineFrom = w.Text.LineFromPosition(start);
                        int lineTo = w.Text.LineFromPosition(start + length - 1);
                        if (lineFrom != lineTo)
                            rc = false;
                        else
                        {
                            int columnFrom = start - w.Text.LineStart(lineFrom);
                            int columnTo = columnFrom + length - 1;
                            rc = columnFrom >= w.BlockColumnStart && columnTo <= w.BlockColumnEnd;
                        }
                    }
                    break;
                case TextWindowBlock.Line:
                    {
                        int lineFrom = w.Text.LineFromPosition(start);
                        int lineTo = w.Text.LineFromPosition(start + length - 1);
                        rc = lineFrom >= w.BlockLineStart && lineTo <= w.BlockLineEnd;
                    }
                    break;
            }
            return rc;
        }

        private static void RangeRemoved(TextWindow w, int position, int length, out bool rangeIntersectBlockStart, out bool rangeIntersectBlockEnd)
        {
            rangeIntersectBlockStart = rangeIntersectBlockEnd = false;
            switch (w.BlockType)
            {
                case TextWindowBlock.Line:
                case TextWindowBlock.Box:
                    {
                        int lineStart, columnStart, lineEnd, columnEnd;
                        RangeToLineCol(w, position, length, out lineStart, out columnStart, out lineEnd, out columnEnd);
                        int lineLength = lineEnd - lineStart;
                        if (lineLength > 0)
                        {
                            if (lineEnd < w.BlockLineStart)
                            {
                                w._BlockLineStart -= lineLength;
                                w._BlockLineEnd -= lineLength;
                            }
                            else if (lineStart < w.BlockLineStart && lineEnd < w.BlockLineEnd)
                            {
                                rangeIntersectBlockStart = true;
                                w._BlockLineStart = lineStart;
                                w._BlockLineEnd -= lineLength;
                            }
                            else if (lineStart >= w.BlockLineStart && lineEnd < w.BlockLineEnd)
                            {
                                w._BlockLineEnd -= lineLength;
                            }
                            else if (lineStart <= w.BlockLineEnd && lineEnd >= w.BlockLineEnd)
                            {
                                rangeIntersectBlockEnd = true;
                                w._BlockLineEnd = lineStart - 1;
                            }
                        }
                    }
                    if (w._BlockLineEnd < w._BlockLineStart)
                        w.DeselectBlock();
                    break;
                case TextWindowBlock.Stream:
                    {
                        //stupid solution
                        int blockLineStart = w.BlockLineStart,
                            blockLineEnd = w.BlockLineEnd,
                            blockColumnStart = w.BlockColumnStart,
                            blockColumnEnd = w.BlockColumnEnd;
                        XceFileBuffer b = w.Text;
                        int row  = b.LineFromPosition(position);
                        int column = position - b.LineStart(row);
                        int end = position + length;
                        for (int i = position; i < end; i++)
                        {
                            //position is after the block, ignore
                            if (row > blockLineEnd || row == blockLineEnd && column > blockColumnEnd)
                                break;
                            switch (b[i])
                            {
                            case    '\r':
                                    if (row < blockLineStart - 1)
                                    {
                                        //a position deleted is before the block start
                                        blockLineStart--;
                                        blockLineEnd--;
                                    }
                                    else if (row == blockLineStart - 1)
                                    {
                                        //a block starts the next
                                        blockLineStart--;
                                        blockColumnStart = column + blockColumnStart;
                                        blockLineEnd--;
                                        if (blockLineEnd == blockLineStart)
                                            blockColumnEnd = column + blockColumnEnd;
                                    }
                                    else if (row >= blockLineStart)
                                    {
                                        if (row < blockLineEnd - 1)
                                        {
                                            blockLineEnd--;
                                        }
                                        else if (row == blockLineEnd - 1)
                                        {
                                            blockLineEnd--;
                                            blockColumnEnd = column + blockColumnEnd;
                                        }
                                    }
                                    break;
                            case    '\n':
                                    break;
                            default:
                                    //other character
                                    if (row == blockLineStart)
                                    {
                                        if (column < blockColumnStart && blockColumnStart > 0)
                                            blockColumnStart--;
                                    }
                                    if (row == blockLineEnd)
                                    {
                                        if (column <= blockColumnEnd && blockColumnEnd > 0)
                                            blockColumnEnd--;
                                    }
                                    break;
                            }
                        }

                        if (blockLineEnd < blockLineStart ||
                            (blockLineEnd == blockLineStart && blockColumnEnd < blockColumnStart))
                            w.DeselectBlock();
                        else
                        {
                            w._BlockColumnStart = blockColumnStart;
                            w._BlockColumnEnd = blockColumnEnd;
                            w._BlockLineStart = blockLineStart;
                            w._BlockLineEnd = blockLineEnd;
                        }
                    }

                    break;
            }
        }

        private static void RangeAdded(TextWindow w, int position, int length, bool rangeIntersectBlockStart, bool rangeIntersectBlockEnd)
        {
            switch (w.BlockType)
            {
                case TextWindowBlock.Line:
                case TextWindowBlock.Box:
                    {
                        int lineStart, columnStart, lineEnd, columnEnd;
                        RangeToLineCol(w, position, length, out lineStart, out columnStart, out lineEnd, out columnEnd);
                        int lineLength = lineEnd - lineStart;
                        if (lineLength > 0)
                        {
                            if (lineStart < w.BlockLineStart)
                            {
                                w._BlockLineStart += lineLength;
                                w._BlockLineEnd += lineLength;
                            }
                            else if (lineStart == w.BlockLineStart)
                            {
                                if (rangeIntersectBlockStart)
                                    w._BlockLineStart += lineLength;
                                w._BlockLineEnd += lineLength;
                            }
                            else if (lineStart <= w.BlockLineEnd)
                            {
                                w._BlockLineEnd += lineLength;
                            }
                            else if (lineStart == w.BlockLineEnd + 1 && rangeIntersectBlockEnd)
                            {
                                w._BlockLineEnd = lineEnd;
                            }
                        }
                    }
                    break;
                case TextWindowBlock.Stream:
                    {
                        int lineStart, columnStart, lineEnd, columnEnd;
                        RangeToLineCol(w, position, length, out lineStart, out columnStart, out lineEnd, out columnEnd);
                        int lineLength = lineEnd - lineStart;
                        if (lineStart < w.BlockLineStart)
                        {
                            w._BlockLineStart += lineLength;
                            w._BlockLineEnd += lineLength;
                        }
                        else if (lineStart == w.BlockLineStart)
                        {
                            w._BlockLineStart += lineLength;
                            w._BlockLineEnd += lineLength;
                            if (columnStart < w.BlockColumnStart)
                                w._BlockColumnStart += columnEnd - (lineStart == lineEnd ? columnStart : 0);
                            if (w.BlockLineStart == w.BlockLineEnd && columnStart <= w.BlockColumnEnd)
                            {
                                w._BlockColumnEnd += columnEnd - (lineStart == lineEnd ? columnStart : 0);

                            }
                        }
                        else if (lineStart < w.BlockLineEnd)
                        {
                            w._BlockLineEnd += lineLength;
                        }
                        else if (lineStart == w.BlockLineEnd && columnStart <= w.BlockColumnEnd)
                        {
                            w._BlockLineEnd += lineLength;
                            w._BlockColumnEnd += columnEnd - (lineStart == lineEnd ? columnStart : 0);
                        }

                    }



                    break;
            }
        }

        private static void RangeToLineCol(TextWindow w, int position, int length, out int lineStart, out int columnStart, out int lineEnd, out int columnEnd)
        {
            PositionToLineCol(w, position, out lineStart, out columnStart);
            PositionToLineCol(w, position + length, out lineEnd, out columnEnd);
        }

        private static void PositionToLineCol(TextWindow w, int position, out int line, out int column)
        {
            line = w.Text.LineFromPosition(position);
            column = position - w.Text.LineStart(line);
        }

        private enum PositionRelation
        {
            Before,         //range 1 completely before range 2
            Equals,         //range equals
            After,          //range 1 after range 2

        }

        private static PositionRelation ComparePositions(int line1, int column1, int line2, int column2)
        {
            if (line1 < line2)
                return PositionRelation.Before;
            if (line1 == line2)
            {
                if (column1 < column2)
                    return PositionRelation.Before;
                else if (column1 == column2)
                    return PositionRelation.Equals;
                else
                    return PositionRelation.After;
            }
            else
                return PositionRelation.After;
        }

    }
}
