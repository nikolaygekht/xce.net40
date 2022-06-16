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
    internal class XmlIntellisenseProvider : IIntellisenseProvider
    {
        private bool mEnabled;
        private Application mApplication;
        internal const string DATA_NAME = "xmlintellisense-file";
        internal const string SCHEMA_NAME = "xmlintellisense-schema";
        private XmlParser mParser;


        public bool Initialize(Application app)
        {
            mApplication = app;
            ProfileSection config = app.Configuration["xmlintellisense"];

            if (config == null)
                mEnabled = false;
            else
                mEnabled = config["enabled", "false"].Trim() == "true";

            if (mEnabled)
            {
                mParser = new XmlParser(app.ColorerFactory);
            }

            return mEnabled;
        }

        public bool AcceptFile(Application application, TextWindow window)
        {
            FileInfo fi = new FileInfo(window.Text.FileName);
            return (string.Compare(fi.Extension, ".xml", true) == 0) ||
                   (string.Compare(fi.Extension, ".fb2", true) == 0) ||
                   (string.Compare(fi.Extension, ".xsl", true) == 0) ||
                   (string.Compare(fi.Extension, ".xslt", true) == 0) ||
                   (string.Compare(fi.Extension, ".xhtml", true) == 0) ||
                   (string.Compare(fi.Extension, ".xsd", true) == 0);


        }

        public bool Start(Application application, TextWindow window)
        {
            XmlParserAdapter adapter = new XmlParserAdapter(window.Text, application.ColorerFactory);
            window[DATA_NAME] = adapter;
            FileInfo fi = new FileInfo(window.Text.FileName);
            if ((string.Compare(fi.Extension, ".xsl", true) == 0) || (string.Compare(fi.Extension, ".xslt", true) == 0))
                window[SCHEMA_NAME] = XmlSchemaFactory.CreateXslt();
            else if (string.Compare(fi.Extension, ".xsd", true) == 0)
                window[SCHEMA_NAME] = XmlSchemaFactory.CreateXsd();
            return true;
        }

        public void Stop(Application application, TextWindow window)
        {
            XmlParserAdapter adapter = window[DATA_NAME] as XmlParserAdapter;
            window[DATA_NAME] = null;
            window[SCHEMA_NAME] = null;
            if (adapter != null)
                adapter.Dispose();
        }

        public bool CanGetCodeCompletionCollection(Application application, TextWindow window)
        {
            return true;
        }

        public ICodeCompletionItemCollection GetCodeCompletionCollection(Application application, TextWindow window, out int wline, out int wcolumn, out int wlength)
        {
            GenericCodeCompletionItemCollection coll = new GenericCodeCompletionItemCollection();
            wline = wcolumn = wlength = 0;
            if (coll.Count > 0)
            {
                coll.Sort();
            }
            return coll;
        }

        public bool IsShowOnTheFlyDataCharacter(char character)
        {
            return character == '<' || character == ' ' || character == '"';
        }

        public bool IsHideOnTheFlyDataCharacter(char character)
        {
            return character == '>' || character == '=' || character == ' ' || character == '"';
        }

        public bool ForwardEnterAtOneTheFlyEnd()
        {
            return false;
        }

        public bool IsOnTheFlyDataCharacter(char character)
        {
            return !IsHideOnTheFlyDataCharacter(character);
        }

        public bool CanGetOnTheFlyCompletionData(Application application, TextWindow window)
        {
            return CanGetCodeCompletionCollection(application, window);
        }

        enum Zone
        {
            NoZone,
            InsideOpenTag,
            InsideAttributeValue,
            InsideTag,

        }

        public ICodeCompletionItemCollection GetOnTheFlyCompletionData(Application application, TextWindow window, char pressed)
         {
            GenericCodeCompletionItemCollection coll = new GenericCodeCompletionItemCollection();
            XmlParserAdapter adapter = window[DATA_NAME] as XmlParserAdapter;
            XmlSchema schema = window[SCHEMA_NAME] as XmlSchema;

            if (adapter != null)
            {
                //find out name space prefix
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

                enumerator = mParser.GetReverseEnum(adapter, window.CursorRow, window.CursorColumn);

                Zone zone = Zone.NoZone;
                string tag = null;
                string ns = null;
                string lastName = null;
                string lastNs = null;
                List<string> attributes = new List<string>();
                List<string> level1tags = new List<string>();
                List<string> level1ns = new List<string>();
                int level = 0;
                XmlSyntaxRegionType firstType = XmlSyntaxRegionType.Unknown;
                bool firstRegion = true;

                while (true)
                {
                    if (enumerator.Current == null)
                        break;
                    XmlSyntaxRegion region = enumerator.Current;

                    if (firstRegion)
                    {
                        firstType = region.Type;
                        firstRegion = false;
                    }

                    if (region.Type == XmlSyntaxRegionType.CloseTagEnd ||
                       (region.Type == XmlSyntaxRegionType.OpenTagEnd && region.Length > 1))
                    {
                        if (zone == Zone.NoZone)
                            zone = Zone.InsideTag;

                        level++;
                    }
                    else if (region.Type == XmlSyntaxRegionType.OpenTagEnd && region.Length == 1)
                    {
                        if (zone == Zone.NoZone)
                            zone = Zone.InsideTag;
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
                        if (zone == Zone.NoZone)
                            zone = Zone.InsideOpenTag;

                        if (level == 1)
                        {
                            level1tags.Add(lastName);
                            level1ns.Add(lastNs);
                            lastName = null;
                            lastNs = null;
                        }
                        else if (level == 0)
                        {
                            tag = lastName;
                            ns = lastNs;
                            break;
                        }
                        level--;
                    }
                    else if (region.Type == XmlSyntaxRegionType.AttributeName)
                    {
                        if (zone == Zone.NoZone)
                            zone = Zone.InsideOpenTag;

                        if (level == 0 && zone == Zone.InsideOpenTag)
                            attributes.Add(window.Text.GetRange(window.Text.LineStart(region.Line) + region.StartColumn, region.Length));
                    }
                    else if (region.Type == XmlSyntaxRegionType.PiTagEnd)
                    {
                        break;
                    }
                    if (!enumerator.Next())
                        break;
                }

                if (zone == Zone.InsideOpenTag && firstType == XmlSyntaxRegionType.AttributeEq)
                    zone = Zone.InsideAttributeValue;


                if (zone == Zone.NoZone)
                {
                    if (window.CursorRow == 0 && window.CursorColumn <= 1 && pressed == '<')
                        coll.Add(new GenericCodeCompletionItem("?xml version=\"1.0\" encoding=\"" + window.Text.Encoding.WebName + "\"?>"));

                    else if (window.CursorRow > 0 && pressed == '<' && schema != null)
                    {
                        if (schema.DefaultPrefix.Length > 0)
                            coll.Add(new GenericCodeCompletionItem(schema.DefaultPrefix + ":" + schema.Root.Name + " " + "xmlns:" + schema.DefaultPrefix +  "=\"" + schema.NamespaceUrl + "\""));
                        else
                            coll.Add(new GenericCodeCompletionItem(schema.Root.Name + " " + "xmlns=\"" + schema.NamespaceUrl + "\""));
                    }
                }
                else if (zone == Zone.InsideOpenTag && pressed == ' ')
                {
                    if ((schema != null) && ((targetNamespace == null && ns == null) || (targetNamespace != null && ns != null && targetNamespace == ns)))
                    {
                        XmlSchemaTag stag = schema[tag];
                        if (stag != null)
                        {
                            for (int i = 0; i < stag.Attributes.Count; i++)
                            {
                                bool found = false;
                                for (int j = 0; j < attributes.Count && !found; j++)
                                    if (stag.Attributes[i].Name == attributes[j])
                                        found = true;
                                if (!found)
                                    coll.Add(new GenericCodeCompletionItem(stag.Attributes[i].Name));
                            }
                        }
                    }
                }
                else if (zone == Zone.InsideTag && pressed == '<' && tag != null)
                {
                    if (schema != null)
                    {
                        XmlSchemaTag stag;
                        if ((targetNamespace == null && ns == null) || (targetNamespace != null && ns != null && targetNamespace == ns))
                            stag = schema[tag];
                        else
                            stag = schema.Default;

                        if (stag != null)
                        {
                            for (int i = 0; i < stag.Children.Count; i++)
                            {
                                if (targetNamespace != null)
                                    coll.Add(new GenericCodeCompletionItem(targetNamespace + ":" + stag.Children[i]));
                                else
                                    coll.Add(new GenericCodeCompletionItem(stag.Children[i]));
                            }
                        }
                    }
                    if (ns != null)
                        coll.Add(new GenericCodeCompletionItem("/" + ns + ":" + tag));
                    else
                        coll.Add(new GenericCodeCompletionItem("/" + tag));
                    coll.Add(new GenericCodeCompletionItem("![CDATA["));
                }
                else if (zone == Zone.InsideAttributeValue && pressed == '"' && tag != null && schema != null && attributes.Count > 0 &&
                        ((targetNamespace == null && ns == null) || (targetNamespace != null && ns != null && targetNamespace == ns)))
                {
                    XmlSchemaTag stag;
                    XmlSchemaAttribute attr = null;
                    stag = schema[tag];
                    if (stag != null)
                    {
                        for (int i = 0; i < stag.Attributes.Count; i++)
                            if (attributes[0] != null && stag.Attributes[i].Name == attributes[0])
                                attr = stag.Attributes[i];

                    }

                    if (attr != null)
                    {
                        switch (attr.Type)
                        {
                        case    XmlSchemaAttributeType.Default:
                                break;
                        case    XmlSchemaAttributeType.XPath:
                                break;
                        case    XmlSchemaAttributeType.List:
                                {
                                    string nsreplace = "";
                                    if (targetNamespace != null)
                                        nsreplace = targetNamespace + ":";
                                    for (int i = 0; i < attr.Options.Count; i++)
                                        coll.Add(new GenericCodeCompletionItem(attr.Options[i].Replace("$ns$", nsreplace)));
                                    break;
                                }
                        }
                    }
                }

            }
            if (coll.Count > 0)
            {
                coll.Sort();
            }
            return coll;
        }

        public void PostOnTheFlyText(Application application, TextWindow w, int startColumn, int length, ICodeCompletionItem _item)
        {
        }

        public bool IsShowInsightDataCharacter(char character)
        {
            return false;
        }

        public bool CanGetInsightDataProvider(Application application, TextWindow window)
        {
            return false;
        }

        public IInsightDataProvider GetInsightDataProvider(Application application, TextWindow window, char pressed)
        {
            return null;
        }

        public bool CanGetProjectBrowserItemCollection(Application application, TextWindow window)
        {
            return false;
        }

        public IProjectBrowserItemCollection GetProjectBrowserItemCollection(Application application, TextWindow window, out IProjectBrowserItem curSel)
        {
            curSel = null;
            return null;
        }

        public IProjectBrowserItemCollection FindProjectBrowserItem(Application application, TextWindow window, out IProjectBrowserItem curSel)
        {
            curSel = null;
            return null;
        }


    }

}