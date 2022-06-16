using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.colorer;
using gehtsoft.xce.text;
using gehtsoft.xce.editor.application;

namespace gehtsoft.xce.editor.textwindow
{
    public class XceFileBufferColorerAdapter : ILineSource, IDisposable
    {
        private XceFileBuffer mBuffer;
        private SyntaxHighlighter mHighlighter;
        private SyntaxRegion mPairStart, mPairEnd;
        
        public SyntaxHighlighter Highlighter
        {
            get
            {
                return mHighlighter;
            }
        }
        
        internal XceFileBufferColorerAdapter(XceFileBuffer buffer, Application app)
        {
            mBuffer = buffer;
            mBuffer.OnChanged += this.BufferChangedHandler;
            mBuffer.OnNameChanged += this.NameChangedHandler;
            mPairStart = app.ColorerFactory.FindSyntaxRegion("def:PairStart");
            mPairEnd = app.ColorerFactory.FindSyntaxRegion("def:PairEnd");
            mHighlighter = app.ColorerFactory.CreateHighlighter(this);
        }
        
        ~XceFileBufferColorerAdapter()
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
            return mBuffer.FileName;
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
            if (major)
                mHighlighter.NotifyMajorChange(mBuffer.LineFromPosition(position));
            else
                mHighlighter.NotifyLineChange(mBuffer.LineFromPosition(position));
        }
        
        private void NameChangedHandler(XceFileBuffer sender)
        {
            mHighlighter.NotifyFileNameChange();
        }
        
        internal bool IsPairStart(SyntaxHighlighterRegion r)
        {
            return r.Is(mPairStart);
        }
        internal bool IsPairEnd(SyntaxHighlighterRegion r)
        {
            return r.Is(mPairEnd);
        }
    }
}
