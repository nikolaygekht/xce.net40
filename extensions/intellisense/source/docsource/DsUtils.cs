using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.text;

namespace gehtsoft.intellisense.docsource
{
    internal class DsUtils
    {
        internal static bool WordUnderCursor(TextWindow w, out int line, out int column, out int length, out string word)
        {
            word = null;
            bool rc = WordUnderCursor(w, out line, out column, out length);

            if (rc)
                word = w.Text.GetRange(w.Text.LineStart(line) + column, length);
            return rc;
        }

        internal static bool WordUnderCursor(TextWindow w, out int line, out int column, out int length)
        {
            return WordUnderCursor(w, w.CursorRow, w.CursorColumn, true, out line, out column, out length);
        }

        internal static bool WordUnderCursor(TextWindow w, int sline, int scolumn, bool allowExtent, out int line, out int column, out int length, out string word)
        {
            word = null;
            bool rc;
            rc = WordUnderCursor(w, sline, scolumn, allowExtent, out line, out column, out length);
            if (rc)
                word = w.Text.GetRange(w.Text.LineStart(line) + column, length);
            return rc;
        }

        internal static bool WordUnderCursor(TextWindow w, int sline, int scolumn, bool allowExtent, out int line, out int column, out int length)
        {
            int position = 0;
            line = -1;
            column = -1;
            length = -1;
            if (!CursorToPosition(w, sline, scolumn, out position, true))
                return false;
            int wordposition, wordlength;
            if (getWord(w.Text, position, allowExtent, out wordposition, out wordlength))
            {
                line = w.Text.LineFromPosition(wordposition);
                column = wordposition - w.Text.LineStart(line);
                length = wordlength;
                return true;
            }
            else
                return false;
        }

        private static bool CursorToPosition(TextWindow w, int line, int column, out int position, bool allowEol)
        {
            position = -1;
            int l;
            if (line < 0 || line >= w.Text.LinesCount)
                return false;
            l = w.Text.LineLength(line);
            if (allowEol)
            {
                if (column < 0 || column > l)
                    return false;
            }
            else
            {
                if (column < 0 || column >= l)
                    return false;
            }
            position = w.Text.LineStart(line) + column;
            return true;
        }

        private static bool isWord(XceFileBuffer b, int position)
        {
            if (position < 0 || position >= b.Length)
                return false;
            return (char.IsLetterOrDigit(b[position]) || b[position] == '_' || b[position] == '.' || b[position] == '-');
        }

        private static bool getWord(XceFileBuffer b, int position, bool allowExtent, out int wordposition, out int wordlength)
        {
            wordposition = -1;
            wordlength = 0;

            if (!isWord(b, position))
            {
                if (position > 0 && isWord(b, position - 1))
                    position--;
                else
                    return false;
            }

            //find start of word
            wordposition = position;
            while (isWord(b, wordposition))
                wordposition--;
            wordposition++;

            //find length of word
            wordlength = position - wordposition + 1;
            if (allowExtent)
            {
                while (isWord(b, wordposition + wordlength))
                    wordlength++;
            }
            return char.IsLetter(b[wordposition]) || b[wordposition] == '_';
        }
    }
}
