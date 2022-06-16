using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.colorer;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.text;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.configuration;
using gehtsoft.xce.intellisense.common;


namespace gehtsoft.intellisense.xml
{
    internal class XmlAnalyzer
    {
        XmlParserAdapter mAdapter;
        XceFileBuffer mBuffer;
        XmlSchema mSchema;

        internal XmlAnalyzer(XmlParserAdapter adapter, XmlSchema schema, XceFileBuffer buffer)
        {
            mAdapter = adapter;
            mBuffer = buffer;
            mSchema = schema;
        }

        internal enum Zone
        {
            NoZone,
            InsideOpenTag,
            InsideAttributeValue,
            InsideTag,
        }

        private Zone mZone;

        public Zone Zone
        {
            get
            {
                return mZone;
            }
        }

        private List<string> mAttributes;

        public List<string> Attributes
        {
            get
            {
                return mAttributes;
            }
        }

        internal class Tag
        {
            private string mName;

            public string Name
            {
                get
                {
                    return mName;
                }
            }

            private string mNs;

            public string Ns
            {
                get
                {
                    return mNs;
                }
            }

            internal Tag(string name, string ns)
            {
                mName = name;
                mNs = ns;
            }
        }

        private Tag mCurrentTag;

        public Tag CurrentTag
        {
            get
            {
                return mCurrentTag;
            }
        }

        private List<Tag> mLevel1Tags;

        public List<Tag> Level1Tags
        {
            get
            {
                return mLevel1Tags;
            }
        }


        private string mTargetNamespace;

        public string TargetNamespace
        {
            get
            {
                return mTargetNamespace;
            }
        }

        private int mLine;

        public int Line
        {
            get
            {
                return mLine;
            }
        }

        private int mColumn;

        public int Column
        {
            get
            {
                return mColumn;
            }
        }

        private int mLength;

        public int Length
        {
            get
            {
                return mLength;
            }
        }

        void Analyze(int line, int column, bool onTheFly, char charPressed)
        {
            FindTargetNamespace();
            AnalyzePosition(line, column, onTheFly, charPressed);
        }

        void FindTargetNamespace()
        {
            IXmlSyntaxRegionEnum enumerator;
            string targetNamespace = null;

            if (schema != null)
            {
                enumerator = mParser.GetForwardEnum(adapter, 0, 0);
                string ans = null;
                string aname = null;
                string avalue = null;
                while (true)
                {
                    if (enumerator.Current == null)
                        break;

                    XmlSyntaxRegion region = enumerator.Current;
                    if (region.Type ==  XmlSyntaxRegionType.AttributeNamespace)
                    {
                        ans = window.Text.GetRange(window.Text.LineStart(region.Line) + region.StartColumn, region.Length);
                    }
                    else if (region.Type ==  XmlSyntaxRegionType.AttributeName)
                    {
                        aname = window.Text.GetRange(window.Text.LineStart(region.Line) + region.StartColumn, region.Length);
                    }
                    else if (region.Type ==  XmlSyntaxRegionType.AttributeValue)
                    {
                        avalue = window.Text.GetRange(window.Text.LineStart(region.Line) + region.StartColumn, region.Length);
                        if (avalue == schema.NamespaceUrl)
                        {
                            if (aname == "xmlns" && ans == null)
                            {
                                break;
                            }
                            if (ans == "xmlns")
                            {
                                targetNamespace = aname;
                                break;
                            }
                        }
                    }
                    else if (region.Type ==  XmlSyntaxRegionType.AttributeEq)
                    {
                        ; //do nothing
                    }
                    else if (region.Type ==  XmlSyntaxRegionType.AttributeQuote)
                    {
                        ; //do nothing
                    }
                    else if (region.Type ==  XmlSyntaxRegionType.AttributeNamespaceDivisor)
                    {
                        ; //do nothing
                    }
                    else
                    {
                        aname = ans = null;
                    }
                    if (region.Line > 10)
                        break;

                    if (!enumerator.Next())
                        break;
                }
            }
            this.mTargetNamespace = targetNamespace;
        }

        void AnalyzePosition(int line, int column, bool onTheFly, char charPressed)
        {
            IXmlSyntaxRegionEnum enumerator;
            enumerator = mParser.GetReverseEnum(adapter, line, column);

            mZone = Zone.NoZone;
            string lastName = null;
            string lastNs = null;
            int level = 0;
            XmlSyntaxRegionType firstType = XmlSyntaxRegionType.Unknown;
            bool firstRegion = true;

            mTag = null;
            mAttributes = new List<string>();
            mLevel1Tags = new List<Tag>();
            mLine = mColumn = mLength = -1;

            while (true)
            {
                if (enumerator.Current == null)
                    break;

                XmlSyntaxRegion region = enumerator.Current;

                if (region.Line == line && column >= region.StartColumn <= column && column < region.StartColumn + region.Length)
                {
                    mLine = line;
                    mColumn = region.StartColumn;
                    mLength = region.Length;
                }

                if (firstRegion)
                {
                    firstType = region.Type;
                    firstRegion = false;
                }

                if (region.Type == XmlSyntaxRegionType.CloseTagEnd ||
                   (region.Type == XmlSyntaxRegionType.OpenTagEnd && region.Length > 1))
                {
                    if (mZone == Zone.NoZone)
                        mZone = Zone.InsideTag;

                    level++;
                }
                else if (region.Type == XmlSyntaxRegionType.OpenTagEnd && region.Length == 1)
                {
                    if (mZone == Zone.NoZone)
                        mZone = Zone.InsideTag;
                }
                else if (region.Type == XmlSyntaxRegionType.TagName)
                {
                    if (level < 2)
                        lastName = window.Text.GetRange(window.Text.LineStart(region.Line) + region.StartColumn, region.Length);
                }
                else if (region.Type == XmlSyntaxRegionType.Namespace)
                {
                    if (level < 2)
                        lastNs = window.Text.GetRange(window.Text.LineStart(region.Line) + region.StartColumn, region.Length);
                }
                else if (region.Type == XmlSyntaxRegionType.OpenTagStart)
                {
                    if (mZone == Zone.NoZone)
                        mZone = Zone.InsideOpenTag;

                    if (level == 1)
                    {
                        mLevel1Tags.Add(new Tag(lastName, lastNs));
                        lastName = null;
                        lastNs = null;
                    }
                    else if (level == 0)
                    {
                        mCurrentTag = new Tag(tag, ns);
                        break;
                    }
                    level--;
                }
                else if (region.Type == XmlSyntaxRegionType.AttributeName)
                {
                    if (mZone == Zone.NoZone)
                        mZone = Zone.InsideOpenTag;

                    if (level == 0 && mZone == Zone.InsideOpenTag)
                        mAttributes.Add(window.Text.GetRange(window.Text.LineStart(region.Line) + region.StartColumn, region.Length));
                }
                else if (region.Type == XmlSyntaxRegionType.PiTagEnd)
                {
                    break;
                }
                else if (region.Type == XmlSyntaxRegionType.AttributeValue)
                {
                    zone = Zone.InsideAttributeValue;
                }
                else if (region.Type == XmlSyntaxRegionType.AttributeEq && zone == Zone.NoZone && charPressed == '"')
                {
                    zone = Zone.InsideAttributeValue;
                }

                if (!enumerator.Next())
                    break;
            }

            if (onTheFly)
            {
                if (zone == Zone.InsideOpenTag)
                    zone = Zone.InsideAttributeValue;
            }
            else
            {
                if (zone == Zone.InsideOpenTag && firstType == XmlSyntaxRegionType.AttributeValue)
                    zone = Zone.InsideAttributeValue;
            }
        }
    }
}