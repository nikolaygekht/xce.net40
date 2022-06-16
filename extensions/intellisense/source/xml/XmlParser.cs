using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.colorer;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.text;

namespace gehtsoft.intellisense.xml
{
    internal enum XmlSyntaxRegionType
    {
        Unknown,
        Comment,
        OpenTagStart,
        OpenTagEnd,
        PiTagStart,
        PiTagEnd,
        CloseTagStart,
        CloseTagEnd,
        TagName,
        NamespaceDivisor,
        Namespace,
        Error,
        AttributeNamespace,
        AttributeNamespaceDivisor,
        AttributeName,
        AttributeEq,
        AttributeQuote,
        AttributeValue,
    }

    internal class XmlSyntaxRegion
    {
        private XmlSyntaxRegionType mType;

        internal XmlSyntaxRegionType Type
        {
            get
            {
                return mType;
            }
        }

        private int mLine;

        internal int Line
        {
            get
            {
                return mLine;
            }
        }

        private int mStartColumn;

        internal int StartColumn
        {
            get
            {
                return mStartColumn;
            }
        }

        private int mLength;

        internal int Length
        {
            get
            {
                return mLength;
            }
        }

        internal int EndColumn
        {
            get
            {
                return mStartColumn + mLength;
            }
        }

        internal XmlSyntaxRegion(XmlSyntaxRegionType type, int line, int startColumn, int length)
        {
            set(type, line, startColumn, length);
        }

        internal void set(XmlSyntaxRegionType type, int line, int startColumn, int length)
        {
            mType = type;
            mLine = line;
            mStartColumn = startColumn;
            mLength = length;
        }
    }

    internal interface IXmlSyntaxRegionEnum
    {
        bool Next();

        XmlSyntaxRegion Current
        {
            get;
        }

    }

    /// <summary>
    /// Investigates XML file structure using colorer's information
    /// </summary>
    internal class XmlParser
    {
        private SyntaxRegion xml_comment;
        private SyntaxRegion xml_openTagStart;
        private SyntaxRegion xml_openTagEnd;
        private SyntaxRegion xml_piTagStart;
        private SyntaxRegion xml_piTagEnd;
        private SyntaxRegion xml_closeTagStart;
        private SyntaxRegion xml_closeTagEnd;
        private SyntaxRegion xml_tagName;
        private SyntaxRegion xml_namespaceDivisor;
        private SyntaxRegion xml_namespace;
        private SyntaxRegion xml_error;
        private SyntaxRegion xml_attributeNamespace;
        private SyntaxRegion xml_attributeNamespaceDivisor;
        private SyntaxRegion xml_attributeName;
        private SyntaxRegion xml_attributeEq;
        private SyntaxRegion xml_attributeQuote;
        private SyntaxRegion xml_attributeValue;

        internal XmlParser(ColorerFactory colorer)
        {
            InitRegions(colorer);
        }

        private void InitRegions(ColorerFactory colorer)
        {
            xml_comment = colorer.FindSyntaxRegion("xmlg:comment");
            xml_openTagStart = colorer.FindSyntaxRegion("xmlg:openTagStart");
            xml_openTagEnd = colorer.FindSyntaxRegion("xmlg:openTagEnd");
            xml_piTagStart = colorer.FindSyntaxRegion("xmlg:piTagStart");
            xml_piTagEnd = colorer.FindSyntaxRegion("xmlg:piTagEnd");
            xml_closeTagStart = colorer.FindSyntaxRegion("xmlg:closeTagStart");
            xml_closeTagEnd = colorer.FindSyntaxRegion("xmlg:closeTagEnd");
            xml_tagName = colorer.FindSyntaxRegion("xmlg:tagName");
            xml_namespaceDivisor = colorer.FindSyntaxRegion("xmlg:namespaceDivisor");
            xml_namespace = colorer.FindSyntaxRegion("xmlg:namespace");
            xml_error = colorer.FindSyntaxRegion("xmlg:error");
            xml_attributeNamespace = colorer.FindSyntaxRegion("xmlg:attributeNamespace");
            xml_attributeNamespaceDivisor = colorer.FindSyntaxRegion("xmlg:attributeNamespaceDivisor");
            xml_attributeName = colorer.FindSyntaxRegion("xmlg:attributeName");
            xml_attributeEq = colorer.FindSyntaxRegion("xmlg:attributeEq");
            xml_attributeQuote = colorer.FindSyntaxRegion("xmlg:attributeQuote");
            xml_attributeValue = colorer.FindSyntaxRegion("xmlg:attributeValue");
        }

        internal XmlSyntaxRegionType Classify(SyntaxHighlighterRegion region)
        {
            if (region.Is(xml_comment)) return XmlSyntaxRegionType.Comment;
            if (region.Is(xml_openTagStart)) return XmlSyntaxRegionType.OpenTagStart;
            if (region.Is(xml_openTagEnd)) return XmlSyntaxRegionType.OpenTagEnd;
            if (region.Is(xml_piTagStart)) return XmlSyntaxRegionType.PiTagStart;
            if (region.Is(xml_piTagEnd)) return XmlSyntaxRegionType.PiTagEnd;
            if (region.Is(xml_closeTagStart)) return XmlSyntaxRegionType.CloseTagStart;
            if (region.Is(xml_closeTagEnd)) return XmlSyntaxRegionType.CloseTagEnd;
            if (region.Is(xml_tagName)) return XmlSyntaxRegionType.TagName;
            if (region.Is(xml_namespaceDivisor)) return XmlSyntaxRegionType.NamespaceDivisor;
            if (region.Is(xml_namespace)) return XmlSyntaxRegionType.Namespace;
            if (region.Is(xml_error)) return XmlSyntaxRegionType.Error;
            if (region.Is(xml_attributeNamespace)) return XmlSyntaxRegionType.AttributeNamespace;
            if (region.Is(xml_attributeNamespaceDivisor)) return XmlSyntaxRegionType.AttributeNamespaceDivisor;
            if (region.Is(xml_attributeName)) return XmlSyntaxRegionType.AttributeName;
            if (region.Is(xml_attributeEq)) return XmlSyntaxRegionType.AttributeEq;
            if (region.Is(xml_attributeQuote)) return XmlSyntaxRegionType.AttributeQuote;
            if (region.Is(xml_attributeValue)) return XmlSyntaxRegionType.AttributeValue;
            return XmlSyntaxRegionType.Unknown;
        }

        internal IXmlSyntaxRegionEnum GetForwardEnum(XmlParserAdapter adapter, int line, int column)
        {
            return new XmlSyntaxRegionForwardEnum(adapter, this, line, column);
        }

        internal IXmlSyntaxRegionEnum GetReverseEnum(XmlParserAdapter adapter, int line, int column)
        {
            return new XmlSyntaxRegionBackwardEnum(adapter, this, line, column);
        }
    }
}
