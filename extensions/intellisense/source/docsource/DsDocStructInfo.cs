using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace gehtsoft.intellisense.docsource
{
    internal class BbCode
    {
        private string mCode;
        private bool mPair;
        private string mOpenTag;
        private string mCloseTag;

        internal BbCode(string code, bool hasValue, bool isPaired)
        {
            mCode = code;
            mPair = isPaired;
            mOpenTag = code;

            if (isPaired)
                mCloseTag = "/" + code;
            else
                mCloseTag = null;
        }

        internal string Code
        {
            get
            {
                return mCode;
            }
        }

        internal string OpenTag
        {
            get
            {
                return mOpenTag;
            }
        }

        internal bool IsPaired
        {
            get
            {
                return mCloseTag != null;
            }
        }

        internal string CloseTag
        {
            get
            {
                return mCloseTag;
            }
        }
    }


    internal class BbCodeCollection
    {
        private Dictionary<string, BbCode> mDict = new Dictionary<string, BbCode>();
        private List<BbCode> mList = new List<BbCode>();

        internal int Count
        {
            get
            {
                return mList.Count;
            }
        }

        internal BbCode this[int index]
        {
            get
            {
                return mList[index];
            }
        }

        internal BbCode this[string name]
        {
            get
            {
                BbCode t;
                if (!mDict.TryGetValue(name, out t))
                    t = null;
                return t;
            }
        }

        internal BbCode FindTag(string tag)
        {
            int len = tag.Length;
            if (len < 2)
                return null;
            int idxOfEq = tag.IndexOf('=');
            if (idxOfEq >= 0)
                tag = tag.Substring(1, idxOfEq - 1);
            else
                tag = tag.Substring(1, tag.Length - 2);
            if (tag.Length == 0)
                return null;

            return this[tag];
        }

        private void Add(BbCode c)
        {
            mList.Add(c);
            mDict[c.Code] = c;
        }

        internal BbCodeCollection ()
        {
            Add(new BbCode("b", false, true));
            Add(new BbCode("i", false, true));
            Add(new BbCode("u", false, true));
            Add(new BbCode("s", false, true));
            Add(new BbCode("c", false, true));
            Add(new BbCode("sup", false, true));
            Add(new BbCode("sub", false, true));
            Add(new BbCode("size", true, true));
            Add(new BbCode("color", true, true));
            Add(new BbCode("red", false, true));
            Add(new BbCode("green", false, true));
            Add(new BbCode("blue", false, true));
            Add(new BbCode("gray", false, true));
            Add(new BbCode("link", true, true));
            Add(new BbCode("clink", true, true));
            Add(new BbCode("url", true, true));
            Add(new BbCode("eurl", true, true));
            Add(new BbCode("img", false, false));
            Add(new BbCode("br", false, false));
            Add(new BbCode("nil", false, false));
        }
    }

    internal class TagAttribute
    {
        private string mName;
        private List<string> mValues;
        private bool mCanHaveBbCode;

        internal string Name
        {
            get
            {
                return mName;
            }
        }

        internal int Count
        {
            get
            {
                if (mValues == null)
                    return 0;
                else
                    return mValues.Count;
            }
        }

        internal string this[int index]
        {
            get
            {
                return mValues[index];
            }
        }

        internal bool CanHaveBbCode
        {
            get
            {
                return mCanHaveBbCode;
            }
        }

        internal TagAttribute(string name, bool canBbCode)
        {
            mName = name;
            mValues = null;
            mCanHaveBbCode = canBbCode;
        }

        internal TagAttribute(string name, string v1)
        {
            mName = name;
            mValues = new List<string>();
            mValues.Add(v1);
            mCanHaveBbCode = false;
        }

        internal TagAttribute(string name, string v1, string v2)
        {
            mName = name;
            mValues = new List<string>();
            mValues.Add(v1);
            mValues.Add(v2);
            mCanHaveBbCode = false;
        }

        internal TagAttribute(string name, string v1, string v2, string v3)
        {
            mName = name;
            mValues = new List<string>();
            mValues.Add(v1);
            mValues.Add(v2);
            mValues.Add(v3);
            mCanHaveBbCode = false;
        }

        internal TagAttribute(string name, string v1, string v2, string v3, string v4)
        {
            mName = name;
            mValues = new List<string>();
            mValues.Add(v1);
            mValues.Add(v2);
            mValues.Add(v3);
            mValues.Add(v4);
            mCanHaveBbCode = false;
        }

        internal TagAttribute(string name, string v1, string v2, string v3, string v4, string v5)
        {
            mName = name;
            mValues = new List<string>();
            mValues.Add(v1);
            mValues.Add(v2);
            mValues.Add(v3);
            mValues.Add(v4);
            mValues.Add(v5);
            mCanHaveBbCode = false;
        }

        internal TagAttribute(string name, string v1, string v2, string v3, string v4, string v5, string v6)
        {
            mName = name;
            mValues = new List<string>();
            mValues.Add(v1);
            mValues.Add(v2);
            mValues.Add(v3);
            mValues.Add(v4);
            mValues.Add(v5);
            mValues.Add(v6);
            mCanHaveBbCode = false;
        }

        internal TagAttribute(string name, string v1, string v2, string v3, string v4, string v5, string v6, string v7)
        {
            mName = name;
            mValues = new List<string>();
            mValues.Add(v1);
            mValues.Add(v2);
            mValues.Add(v3);
            mValues.Add(v4);
            mValues.Add(v5);
            mValues.Add(v6);
            mValues.Add(v7);
            mCanHaveBbCode = false;
        }
        internal TagAttribute(string name, string v1, string v2, string v3, string v4, string v5, string v6, string v7, string v8)
        {
            mName = name;
            mValues = new List<string>();
            mValues.Add(v1);
            mValues.Add(v2);
            mValues.Add(v3);
            mValues.Add(v4);
            mValues.Add(v5);
            mValues.Add(v6);
            mValues.Add(v7);
            mValues.Add(v8);
            mCanHaveBbCode = false;
        }
        internal TagAttribute(string name, string v1, string v2, string v3, string v4, string v5, string v6, string v7, string v8, string v9)
        {
            mName = name;
            mValues = new List<string>();
            mValues.Add(v1);
            mValues.Add(v2);
            mValues.Add(v3);
            mValues.Add(v4);
            mValues.Add(v5);
            mValues.Add(v6);
            mValues.Add(v7);
            mValues.Add(v8);
            mValues.Add(v9);
            mCanHaveBbCode = false;
        }
    }

    internal class Tag
    {
        private string mName;
        private TagCollection mParents = new TagCollection();
        private TagCollection mChildren = new TagCollection();
        private bool mCanHaveText;
        private List<TagAttribute> mAttributes = new List<TagAttribute>();

        internal string Name
        {
            get
            {
                return mName;
            }
        }

        internal TagCollection Children
        {
            get
            {
                return mChildren;
            }
        }

        internal int Count
        {
            get
            {
                return mAttributes.Count;
            }
        }

        internal bool CanHaveText
        {
            get
            {
                return mCanHaveText;
            }
        }

        internal TagAttribute this[int index]
        {
            get
            {
                return mAttributes[index];
            }
        }

        internal void Add(TagAttribute attr)
        {
            mAttributes.Add(attr);
        }

        internal Tag(string name, bool canHaveText)
        {
            mName = name;
            mCanHaveText = canHaveText;
            mChildren = new TagCollection();
        }

        internal void Add(Tag child)
        {
            mChildren.Add(child);
            child.mParents.Add(this);
        }
    }

    internal class TagCollection
    {
        private List<Tag> mList = new List<Tag>();
        private Dictionary<string, Tag> mDict = new Dictionary<string, Tag>();

        internal Tag this[string name]
        {
            get
            {
                Tag t;
                if (!mDict.TryGetValue(name, out t))
                    t = null;
                return t;
            }
        }

        internal int Count
        {
            get
            {
                return mList.Count;
            }
        }

        internal Tag this[int index]
        {
            get
            {
                return mList[index];
            }
        }

        internal void Add(Tag tag)
        {
            mList.Add(tag);
            mDict[tag.Name] = tag;
        }
    }

    internal class TagCollectionFactory
    {
        internal static TagCollection create()
        {
            TagCollection collection = new TagCollection();
            Tag mRoot = new Tag("@root", false);
            collection.Add(mRoot);

            List<Tag> mTextTags = new List<Tag>();

            Tag mTable = new Tag("@table", false);
            mTable.Add(new TagAttribute("@width", false));
            mTable.Add(new TagAttribute("@if", false));
            collection.Add(mTable);

            Tag mRow = new Tag("@row", false);
            mRow.Add(new TagAttribute("@header", "yes", "no"));
            mRow.Add(new TagAttribute("@if", false));
            mTable.Add(mRow);
            collection.Add(mRow);

            Tag mColumn = new Tag("@col", true);
            mColumn.Add(new TagAttribute("@width", false));
            mRow.Add(mColumn);
            collection.Add(mColumn);
            mTextTags.Add(mColumn);

            Tag mList = new Tag("@list", false);
            mList.Add(new TagAttribute("@type", "dot", "num"));
            collection.Add(mList);

            Tag mListItem = new Tag("@list-item", true);
            mList.Add(mListItem);
            collection.Add(mListItem);
            mTextTags.Add(mListItem);

            Tag mExampleTab = new Tag("@tab", true);
            mExampleTab.Add(new TagAttribute("@title", true));
            mExampleTab.Add(new TagAttribute("@if", false));
            collection.Add(mExampleTab);

            Tag mExample = new Tag("@example", true);
            mExample.Add(new TagAttribute("@title", true));
            mExample.Add(new TagAttribute("@transform", "yes", "no", "def"));
            mExample.Add(new TagAttribute("@show", "yes", "no", "always"));
            mExample.Add(new TagAttribute("@gray", "yes", "no"));
            mExample.Add(new TagAttribute("@tabs", "yes", "no"));
            mExample.Add(new TagAttribute("@if", false));
            mExample.Add(mExampleTab);
            collection.Add(mExample);

            Tag mHeadline = new Tag("@headline", true);
            mHeadline.Add(new TagAttribute("@level", false));
            collection.Add(mHeadline);

            Tag mGroup = new Tag("@group", true);
            mGroup.Add(new TagAttribute("@title", true));
            mGroup.Add(new TagAttribute("@brief", true));
            mGroup.Add(new TagAttribute("@ingroup", false));
            mGroup.Add(new TagAttribute("@key", false));
            mGroup.Add(new TagAttribute("@if", false));
            mGroup.Add(new TagAttribute("@sortgroups", "yes", "no"));
            mGroup.Add(new TagAttribute("@sortarticles", "yes", "no"));
            mGroup.Add(new TagAttribute("@sortclasses", "yes", "no"));
            mGroup.Add(new TagAttribute("@transform", "yes", "no", "def"));
            mGroup.Add(new TagAttribute("@order", "sort", "custom"));
            mGroup.Add(new TagAttribute("@import", false));
            mTextTags.Add(mGroup);
            collection.Add(mGroup);

            Tag mArticle = new Tag("@article", true);
            mArticle.Add(new TagAttribute("@title", true));
            mArticle.Add(new TagAttribute("@brief", true));
            mArticle.Add(new TagAttribute("@ingroup", false));
            mArticle.Add(new TagAttribute("@key", false));
            mArticle.Add(new TagAttribute("@if", false));
            mArticle.Add(new TagAttribute("@aliasId", false));
            mArticle.Add(new TagAttribute("@transform", "yes", "no", "def"));
            mArticle.Add(new TagAttribute("@excludeFromList", "yes", "no"));
            mTextTags.Add(mArticle);
            collection.Add(mArticle);

            Tag mClass = new Tag("@class", true);
            mClass.Add(new TagAttribute("@name", false));
            mClass.Add(new TagAttribute("@brief", true));
            mClass.Add(new TagAttribute("@ingroup", false));
            mClass.Add(new TagAttribute("@key", false));
            mClass.Add(new TagAttribute("@if", false));
            mClass.Add(new TagAttribute("@sig", false));
            mClass.Add(new TagAttribute("@import", false));
            mClass.Add(new TagAttribute("@prefix", false));
            mClass.Add(new TagAttribute("@parent", false));
            mClass.Add(new TagAttribute("@declname", false));
            mClass.Add(new TagAttribute("@type", "ref class", "class", "interface", "enum", "table", "re-library", "re-statement", "functions"));
            mClass.Add(new TagAttribute("@sort", "yes", "no"));
            mClass.Add(new TagAttribute("@transform", "yes", "no", "def"));
            mClass.Add(new TagAttribute("@classnameinkey", "true", "false", "both"));
            mTextTags.Add(mClass);
            collection.Add(mClass);

            mRoot.Add(mGroup);
            mRoot.Add(mArticle);
            mRoot.Add(mClass);

            Tag mMember = new Tag("@member", true);
            mMember.Add(new TagAttribute("@name", false));
            mMember.Add(new TagAttribute("@brief", true));
            mMember.Add(new TagAttribute("@key", false));
            mMember.Add(new TagAttribute("@if", false));
            mMember.Add(new TagAttribute("@sig", false));
            mMember.Add(new TagAttribute("@index", false));
            mMember.Add(new TagAttribute("@divisor", false));
            mMember.Add(new TagAttribute("@visibility", "public", "protected", "private", "package"));
            mMember.Add(new TagAttribute("@scope", "class", "instance"));
            mMember.Add(new TagAttribute("@type", "field", "property", "constructor", "method", "function"));
            mMember.Add(new TagAttribute("@excludeFromList", "yes", "no"));
            mMember.Add(new TagAttribute("@transform", "yes", "no", "def"));
            mClass.Add(mMember);
            mTextTags.Add(mMember);
            collection.Add(mMember);

            Tag mSeeAlso = new Tag("@see", true);
            mSeeAlso.Add(new TagAttribute("@title", true));
            mSeeAlso.Add(new TagAttribute("@key", false));
            mSeeAlso.Add(new TagAttribute("@if", false));
            mTextTags.Add(mSeeAlso);
            collection.Add(mSeeAlso);

            mGroup.Add(mSeeAlso);
            mArticle.Add(mSeeAlso);
            mClass.Add(mSeeAlso);
            mMember.Add(mSeeAlso);

            Tag mDeclaration = new Tag("@declaration", true);
            mDeclaration.Add(new TagAttribute("language", "cpp", "cs", "lua", "vb", "java", "re", "re1", "mq4", "el"));
            mDeclaration.Add(new TagAttribute("@name", true));
            mDeclaration.Add(new TagAttribute("@prefix", true));
            mDeclaration.Add(new TagAttribute("@suffix", true));
            mDeclaration.Add(new TagAttribute("@custom", false));
            mDeclaration.Add(new TagAttribute("@namesuffix", true));
            mDeclaration.Add(new TagAttribute("@params", true));
            mDeclaration.Add(new TagAttribute("@return", true));
            mDeclaration.Add(new TagAttribute("@if", false));
            mMember.Add(mDeclaration);
            collection.Add(mDeclaration);

            Tag mParameter = new Tag("@param", true);
            mParameter.Add(new TagAttribute("@name", true));
            mParameter.Add(new TagAttribute("@gray", "yes", "no"));
            mParameter.Add(new TagAttribute("@if", false));
            mClass.Add(mParameter);
            mMember.Add(mParameter);
            collection.Add(mParameter);
            mTextTags.Add(mParameter);

            Tag mReturn = new Tag("@return", true);
            mMember.Add(mReturn);
            collection.Add(mReturn);
            mTextTags.Add(mReturn);

            Tag mException = new Tag("@exception", true);
            mException.Add(new TagAttribute("@name", true));
            mMember.Add(mException);
            collection.Add(mException);
            mTextTags.Add(mException);

            foreach(Tag tag in mTextTags)
            {
                tag.Add(mList);
                tag.Add(mTable);
                tag.Add(mExample);
                tag.Add(mHeadline);
            }
            return collection;
        }
    }
}