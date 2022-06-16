using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.colorer;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.text;


namespace gehtsoft.intellisense.xml
{
    class XmlSyntaxRegionForwardEnum : IXmlSyntaxRegionEnum
    {
        XmlParserAdapter mAdapter;
        XmlParser mParser;
        XmlSyntaxRegion [] mRegions = new XmlSyntaxRegion [1024];
        int mRegionsCount;
        int mCurrentRegion;
        int mCurrentLine;
        bool mFinished;

        internal XmlSyntaxRegionForwardEnum(XmlParserAdapter adapter, XmlParser parser, int startLine, int startColumn)
        {
            mAdapter = adapter;
            mParser = parser;
            mFinished = false;
            for (int i = 0; i < 1024; i++)
                mRegions[i] = null;
            InitLine(startLine, startColumn);
        }

        public XmlSyntaxRegion Current
        {
            get
            {
                if (mFinished)
                    return null;
                else
                    return mRegions[mCurrentRegion];
            }
        }

        public bool Next()
        {
            mCurrentRegion++;
            if (mCurrentRegion >= mRegionsCount)
            {
                InitLine(mCurrentLine + 1, 0);
                return !mFinished;
            }
            else
                return true;

        }

        private void InitLine(int line, int column)
        {
            while (true)
            {
                mRegionsCount = 0;
                mCurrentRegion = 0;

                if (line >= mAdapter.Text.LinesCount)
                {
                    mFinished = true;
                    return;
                }

                int length = mAdapter.Text.LineLength(line);
                if (column > length)
                {
                    line++;
                    continue ;
                }

                mAdapter.Highlighter.Colorize(line, 1);

                bool rc = mAdapter.Highlighter.GetFirstRegion(line);
                while (rc)
                {
                    SyntaxHighlighterRegion r = mAdapter.Highlighter.CurrentRegion;
                    if (r.StartColumn + r.Length >= column)
                    {
                        XmlSyntaxRegionType t = mParser.Classify(r);
                        if (t != XmlSyntaxRegionType.Unknown)
                        {
                            if (mRegions[mRegionsCount] == null)
                                mRegions[mRegionsCount] = new XmlSyntaxRegion(t, line, r.StartColumn, r.Length);
                            else
                                mRegions[mRegionsCount].set(t, line, r.StartColumn, r.Length);
                            mRegionsCount++;
                        }
                    }
                    rc = mAdapter.Highlighter.GetNextRegion();
                }

                if (mRegionsCount == 0)
                {
                    line++;
                    continue ;
                }

                mCurrentLine = line;
                return ;
            }
        }
    }

    class XmlSyntaxRegionBackwardEnum : IXmlSyntaxRegionEnum
    {
        XmlParserAdapter mAdapter;
        XmlParser mParser;
        XmlSyntaxRegion[] mRegions = new XmlSyntaxRegion[1024];
        int mRegionsCount;
        int mCurrentRegion;
        int mCurrentLine;
        bool mFinished;

        internal XmlSyntaxRegionBackwardEnum(XmlParserAdapter adapter, XmlParser parser, int startLine, int startColumn)
        {
            mAdapter = adapter;
            mParser = parser;
            mFinished = false;
            for (int i = 0; i < 1024; i++)
                mRegions[i] = null;
            InitLine(startLine, startColumn);
        }

        public XmlSyntaxRegion Current
        {
            get
            {
                if (mFinished)
                    return null;
                else
                    return mRegions[mCurrentRegion];
            }
        }

        public bool Next()
        {
            mCurrentRegion--;
            if (mCurrentRegion < 0)
            {
                InitLine(mCurrentLine - 1, Int32.MaxValue);
                return !mFinished;
            }
            else
                return true;

        }

        private void InitLine(int line, int column)
        {
            while (true)
            {
                mRegionsCount = 0;
                mCurrentRegion = 0;

                if (line >= mAdapter.Text.LinesCount)
                {
                    line = mAdapter.Text.LinesCount - 1;
                    column = Int32.MaxValue;
                }

                if (line < 0)
                {
                    mFinished = true;
                    return ;
                }

                mAdapter.Highlighter.Colorize(line, 1);
                bool rc = mAdapter.Highlighter.GetFirstRegion(line);
                while (rc)
                {
                    SyntaxHighlighterRegion r = mAdapter.Highlighter.CurrentRegion;
                    if (r.StartColumn <= column)
                    {
                        XmlSyntaxRegionType t = mParser.Classify(r);
                        if (t != XmlSyntaxRegionType.Unknown)
                        {
                            if (mRegions[mRegionsCount] == null)
                                mRegions[mRegionsCount] = new XmlSyntaxRegion(t, line, r.StartColumn, r.Length);
                            else
                                mRegions[mRegionsCount].set(t, line, r.StartColumn, r.Length);
                            mRegionsCount++;
                        }
                    }
                    rc = mAdapter.Highlighter.GetNextRegion();
                }

                if (mRegionsCount == 0)
                {
                    column = Int32.MaxValue;
                    line--;
                    continue;
                }

                mCurrentLine = line;
                mCurrentRegion = mRegionsCount - 1;
                return ;
            }
        }
    }
}
