using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.colorer;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.text;

namespace gehtsoft.intellisense.xml
{
    internal class XmlParserAdapter : ILineSource, IDisposable
    {
        private XceFileBuffer mBuffer;
        private SyntaxHighlighter mHighlighter;
        private int mFirstDirtyLine;

        internal XceFileBuffer Text
        {
            get
            {
                return mBuffer;
            }
        }

        internal SyntaxHighlighter Highlighter
        {
            get
            {
                return mHighlighter;
            }
        }
        
        internal int FirstDirtyLine
        {
            get
            {
                return mFirstDirtyLine;
            }
            set
            {
                mFirstDirtyLine = value;
            }
        }
        
        internal XmlParserAdapter(XceFileBuffer buffer, ColorerFactory colorer)
        {
            mBuffer = buffer;
            mBuffer.OnChanged += this.BufferChangedHandler;
            mHighlighter = colorer.CreateHighlighter(this);
            mFirstDirtyLine = 0;
        }

        ~XmlParserAdapter()
        {
            Dispose(false);
        }
        
        public void Dispose()
        {
            Dispose(true);
        }
        
        virtual protected void Dispose(bool disposing)
        {
            if (mHighlighter != null)
                mHighlighter.Dispose();
            mHighlighter = null;
        }

        public string GetFileName()
        {
            return "intellisense.adapter.xmlg";
        }

        public int GetLinesCount()
        {
            return mBuffer.LinesCount;
        }

        public int GetLineLength(int line)
        {
            if (line < 0 || line >= mBuffer.LinesCount)
                return 0;
            else
                return mBuffer.LineLength(line, false);
        }

        public int GetLine(int line, char[] buff, int positionFrom, int length)
        {
            int lineLength = GetLineLength(line);
            if (lineLength <= 0 || positionFrom >= lineLength)
                return 0;
            int start = mBuffer.LineStart(line);
            if (positionFrom + length > lineLength)
                length = lineLength - positionFrom;
            mBuffer.GetRange(start + positionFrom, length, buff, 0);
            return length;
        }

        private void BufferChangedHandler(XceFileBuffer sender, int position, bool major)
        {
            int line = mBuffer.LineFromPosition(position);
            if (major)
            {
                mFirstDirtyLine = 0;
                mHighlighter.NotifyMajorChange(line);
            }
            else
            {
                mFirstDirtyLine = line;
                mHighlighter.NotifyLineChange(line);
            }
        }
    }
}
