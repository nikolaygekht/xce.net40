using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.colorer;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.text;


namespace gehtsoft.intellisense.xml
{
    enum XmlSchemaAttributeType
    {
        Default,
        XPath,
        List,
    };

    class XmlSchemaAttribute
    {
        private XmlSchemaAttributeType mType;

        public XmlSchemaAttributeType Type
        {
            get
            {
                return mType;
            }
        }

        private string mName;

        public string Name
        {
            get
            {
                return mName;
            }
        }

        private List<string> mOptions;

        public List<string> Options
        {
            get
            {
                return mOptions;
            }
        }

        internal XmlSchemaAttribute(string name)
        {
            mType = XmlSchemaAttributeType.Default;
            mName = name;
        }

        internal XmlSchemaAttribute(string name, XmlSchemaAttributeType type)
        {
            mType = type;
            mName = name;
            if (type == XmlSchemaAttributeType.List)
                mOptions = new List<string>();
        }

        internal void AddOption(string option)
        {
            mOptions.Add(option);
        }
    };

    class XmlSchemaTag
    {
        private string mName;                   //tag name
        private List<XmlSchemaAttribute> mAttributes;       //tag attributes
        private List<string> mChildren;         //children tags
        private bool mOrdered;                  //ordered tags

        internal XmlSchemaTag(string name, bool ordered)
        {
            mName = name;
            mAttributes = new List<XmlSchemaAttribute>();
            mChildren = new List<string>();
            mOrdered = ordered;
        }

        internal void AddAttribute(XmlSchemaAttribute attribute)
        {
            mAttributes.Add(attribute);
        }

        internal void AddChild(string name)
        {
            mChildren.Add(name);
        }

        internal string Name
        {
            get
            {
                return mName;
            }
        }

        internal bool Ordered
        {
            get
            {
                return mOrdered;
            }
        }

        internal List<XmlSchemaAttribute> Attributes
        {
            get
            {
                return mAttributes;
            }
        }

        internal List<string> Children
        {
            get
            {
                return mChildren;
            }
        }
    }

    class XmlSchema
    {
        Dictionary<string, XmlSchemaTag> mTags = new Dictionary<string, XmlSchemaTag>();

        private XmlSchemaTag mRoot;

        public XmlSchemaTag Root
        {
            get
            {
                return mRoot;
            }
        }

        private XmlSchemaTag mDefault;

        public XmlSchemaTag Default
        {
            get
            {
                return mDefault;
            }
        }

        private string mNamespaceUrl;

        public string NamespaceUrl
        {
            get
            {
                return mNamespaceUrl;
            }
        }

        private string mDefaultPrefix;

        public string DefaultPrefix
        {
            get
            {
                return mDefaultPrefix;
            }
        }




        internal XmlSchema(XmlSchemaTag root, XmlSchemaTag defaultTag, string namespaceUrl, string defaultPrefix)
        {
            mRoot = root;
            mDefault = defaultTag;
            mNamespaceUrl = namespaceUrl;
            mDefaultPrefix = defaultPrefix;
            Add(root);
        }

        internal void Add(XmlSchemaTag tag)
        {
            mTags[tag.Name] = tag;
        }

        internal XmlSchemaTag this[string name]
        {
            get
            {
                XmlSchemaTag tag;
                if (!mTags.TryGetValue(name, out tag))
                    tag = null;
                return tag;
            }
        }
    }
}