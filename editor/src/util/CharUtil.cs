using System;
using System.Collections.Generic;
using System.Text;

namespace gehtsoft.xce.editor.util
{
    public enum CharClass
    {
        Whitespace,
        Punctuation,
        Word,
        Unknown,
    }

    public class CharUtil
    {
        public static CharClass GetCharClass(char c)
        {
            if (c == 0 || Char.IsWhiteSpace(c) || Char.IsControl(c))
                return CharClass.Whitespace;
            else if (Char.IsLetterOrDigit(c))
                return CharClass.Word;
            else if (Char.IsPunctuation(c) || char.IsSeparator(c) || char.IsSymbol(c))
                return CharClass.Punctuation;
            else
                return CharClass.Unknown;
        }
    }
}
