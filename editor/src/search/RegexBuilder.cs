using System;
using System.Collections.Generic;
using System.Text;

namespace gehtsoft.xce.editor.search
{
    internal class RegexBuilder
    {
        internal static string Build(string searchString, bool regex, bool ignoreCase, bool wholeWord)
        {
            StringBuilder b  = new StringBuilder();
            b.Append('/');
            if (regex)
            {
                b.Append(searchString);
                b.Append('/');
                if (ignoreCase)
                    b.Append('i');
                b.Append('m');
            }
            else
            {
                if (wholeWord)
                    b.Append("\\b");
                    
                foreach (char c in searchString)
                {
                    if (char.IsLetter(c) || char.IsDigit(c))
                        b.Append(c);
                    else if (char.IsWhiteSpace(c))
                    {
                        b.Append("\\s");
                    }
                    else
                    {
                        b.Append('\\');
                        b.Append(c);
                    }
                }
                if (wholeWord)
                    b.Append("\\b");
                b.Append('/');
                if (ignoreCase)
                    b.Append('i');
                b.Append('m');                    
            }
            return b.ToString();
        }
    }
}
