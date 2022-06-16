using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.scintilla;
using gehtsoft.xce.colorer;

namespace gehtsoft.xce.text
{
    public class XceFileBufferRegex : Regex
    {
        public XceFileBufferRegex(string regularExpression) : base(regularExpression)
        {
        }

        public bool Match(XceFileBuffer buffer, int startIndex, int length)
        {
            IntPtr a1, a2, a3;

            lock (buffer.Mutex)
            {
                buffer.GetNativeAccessors(out a1, out a2, out a3);
                return this.Match(a1, a2, a3, startIndex, length);
            }
        }
    }
}
