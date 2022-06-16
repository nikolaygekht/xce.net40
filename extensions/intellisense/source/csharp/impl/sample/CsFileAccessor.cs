using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using gehtsoft.intellisense.cs;
using gehtsoft.xce.text;

namespace gehtsoft.xce.extension.csintellisense_impl
{
    internal class CsFileTextReader : TextReader
    {
        XceFileBuffer mText;
        int mPosition;

        internal CsFileTextReader(XceFileBuffer text)
        {
            mText = text;
            mPosition = 0;
        }

        public override int Peek()
        {
            if (mPosition < mText.Length)
                return (int)mText[mPosition];
            else
                return -1;
        }

        public override int Read()
        {
            if (mPosition < mText.Length)
                return (int)mText[mPosition++];
            else
                return -1;
        }

        public override int Read(char[] buffer, int index, int count)
        {
            if (count > mText.Length - mPosition)
                count = mText.Length - mPosition;
            if (count < 0)
                count = 0;
            if (count > 0)
            {
                mText.GetRange(mPosition, count, buffer, index);
                mPosition += count;
            }
            return count;
        }

        public override int ReadBlock(char[] buffer, int index, int count)
        {
            return Read(buffer, index, count);
        }

        public override string ReadLine()
        {
            if (mPosition >= mText.Length)
                return "";
            int lineStart = mText.LineStart(mPosition);
            int lineEnd = lineStart + mText.LineLength(mPosition);
            int lineEnd1 = lineEnd - 1;

            while ((mText[lineEnd1] == '\r' || mText[lineEnd1] == '\n') || lineEnd1 >= lineStart)
                lineEnd1--;
            int count = lineEnd1 - lineStart + 1;
            string r = "";
            if (count > 0)
                r = mText.GetRange(lineStart, count);
            mPosition = lineEnd;
            return r;
        }

        public override string ReadToEnd()
        {
            int count = mText.Length - mPosition;
            if (count < 0)
                count = 0;
            if (count > 0)
            {
                string s = mText.GetRange(mPosition, count);
                mPosition += count;
                return s;
            }
            else
                return "";
        }
    }


    internal class CsFileSource : ICsParserFileSource
    {
        private XceFileBuffer mBuffer;
        
        internal CsFileSource(XceFileBuffer buffer)
        {
            mBuffer = buffer;
            mBuffer.OnChanged += new XceFileBufferChangedDelegate(XceFileBufferChangedHandler);
            mChanged = true;
        }

        public TextReader CreateReader()
        {
            return new CsFileTextReader(mBuffer);
        }

        public string ReadTo(int length)
        {
            if (length > mBuffer.Length || length == -1)
                length = mBuffer.Length;
            if (length > 0)
                return mBuffer.GetRange(0, length);
            else
                return "";
        }
        
        public string ReadTo(int line, int column)
        {
            line--;
            column--;
            if (line >= mBuffer.LinesCount)
                return "";
            int ls = mBuffer.LineStart(line);
            int ll = mBuffer.LineLength(line);
            if (column > ll)
                column = ll;
            return ReadTo(ls + column);
        }

        public bool PositionToLine(int position, out int line, out int column)
        {
            if (position < 0 || position >= mBuffer.Length)
            {
                line = column = 0;
                return false;
            }
            
            line = mBuffer.LineFromPosition(position);
            int ls = mBuffer.LineStart(line);
            column = position - ls;
            line++;
            column++;
            return true;
        }
        
        public int LineToPosition(int line, int column)
        {
            line--;
            column--;
            if (line < 0)
                return -1;
            if (line >= mBuffer.Length)
                return mBuffer.Length;
            int ll = mBuffer.LineLength(line);
            if (column >= ll)
                column = ll;
            return mBuffer.LineStart(line) + column; 
        }
        
        private bool mChanged; 
        
        public bool Changed
        {
            get
            {
                return mChanged;
            }
            set
            {
                mChanged = value;
            }
        }
        
        void XceFileBufferChangedHandler(XceFileBuffer sender, int position, bool major)
        {
            mChanged = true;
        }
    }
}

