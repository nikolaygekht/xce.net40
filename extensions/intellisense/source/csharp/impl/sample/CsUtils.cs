using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using gehtsoft.intellisense.cs;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.text;

namespace gehtsoft.xce.extension.csintellisense_impl
{
    internal class CsUtils
    {
        internal static CsProject FindCsProject(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            DirectoryInfo dir = fi.Directory;
            return SearchCsprojIn(dir, fileName);
        }
        
        private static CsProject SearchCsprojIn(DirectoryInfo dir, string fileName)
        {
            string[] files = Directory.GetFiles(dir.FullName, "*.csproj");
            foreach (string prjPath in files)
            {
                try
                {
                    CsProject prj = CsProjectLoader.load(prjPath);
                    if (prj.Sources.IndexOf(fileName) >= 0)
                        return prj;
                }
                catch (Exception )
                {
                }
            }
            if (dir.Parent != null)
                return SearchCsprojIn(dir.Parent, fileName);
            else
                return null;
        }
        
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
            int position = 0;
            line = -1;
            column = -1;
            length = -1;
            if (!CursorToPosition(w, out position, true))
                return false;
            int wordposition, wordlength;
            if (getWord(w.Text, position, out wordposition, out wordlength))
            {
                line = w.Text.LineFromPosition(wordposition);
                column = wordposition - w.Text.LineStart(line);
                length = wordlength;
                return true;
            }
            else
                return false;
        }

        private static bool CursorToPosition(TextWindow w, out int position, bool allowEol)
        {
            position = -1;
            int l;
            if (w.CursorRow < 0 || w.CursorRow >= w.Text.LinesCount)
                return false;
            l = w.Text.LineLength(w.CursorRow);
            if (allowEol)
            {
                if (w.CursorColumn < 0 || w.CursorColumn > l)
                    return false;
            }
            else
            {
                if (w.CursorColumn < 0 || w.CursorColumn >= l)
                    return false;
            }
            position = w.Text.LineStart(w.CursorRow) + w.CursorColumn;
            return true;
        }

        private static bool isWord(XceFileBuffer b, int position)
        {
            if (position < 0 || position >= b.Length)
                return false;
            return (char.IsLetterOrDigit(b[position]));
        }

        private static bool getWord(XceFileBuffer b, int position, out int wordposition, out int wordlength)
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
            while (isWord(b, wordposition + wordlength))
                wordlength++;
            return char.IsLetter(b[wordposition]);
        }
        
    }
}
