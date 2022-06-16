using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.text;
using gehtsoft.xce.editor.textwindow;

namespace gehtsoft.xce.extension.advnav_commands
{
    internal class FindWordBase
    {
        protected bool CursorToPosition(TextWindow w, out int position)
        {
            return CursorToPosition(w, out position, false);
        }
        
        protected bool CursorToPosition(TextWindow w, out int position, bool allowEol)
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
        
        protected void GoPosition(TextWindow w, int position)
        {
            if (position <= 0 || w.Text.Length < 1)
            {
                w.CursorRow = w.CursorColumn = 0;
                w.EnsureCursorVisible();
                return ;
            }
            else if (position >= w.Text.Length)
            {
                int length = w.Text.LineLength(w.Text.LinesCount - 1, false);
                w.CursorRow = w.Text.LinesCount - 1;
                w.CursorColumn = length;
                w.EnsureCursorVisible();
            }
            else
            {
                int line = w.Text.LineFromPosition(position);
                int start = w.Text.LineStart(line);
                w.CursorRow = line;
                w.CursorColumn = position - start;
                w.EnsureCursorVisible();
                return ;
            }
        }

        protected bool isWord(XceFileBuffer b, int position)
        {
            if (position < 0 || position >= b.Length)
                return false;
            return (char.IsLetterOrDigit(b[position]));
        }

        protected bool getWord(XceFileBuffer b, int position, out int wordposition, out int wordlength)
        {
            wordposition = -1;
            wordlength = 0;
        
            if (!isWord(b, position))
                return false;
                
            //find start of word
            wordposition = position;
            while (isWord(b, wordposition))
                wordposition--;
            wordposition++;
            
            //find length of word
            wordlength = position - wordposition + 1;
            while (isWord(b, wordposition + wordlength))
                wordlength++;
            return true;
        }
    }
}

