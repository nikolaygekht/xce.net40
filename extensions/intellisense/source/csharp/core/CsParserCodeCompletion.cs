using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using gehtsoft.xce.intellisense.common;

namespace gehtsoft.intellisense.cs
{
    public interface ICsCodeCompletionItem : ICodeCompletionItem, IComparable<ICsCodeCompletionItem>
    {
        CsCodeCompletionItemCollection Overloads
        {
            get;
        }

        IEntity Entity
        {
            get;
        }
    }

    internal class CsCodeCompletionItem : ICsCodeCompletionItem
    {
        private IEntity mEntity;
        private CodeCompletionItemType mType;
        private string mName;
        private string mText;
        private CsCodeCompletionItemCollection mOverloads = null;

        public CsCodeCompletionItemCollection Overloads
        {
            get
            {
                if (mOverloads == null)
                    mOverloads = new  CsCodeCompletionItemCollection();
                return mOverloads;
            }
        }

        public int OverloadsCount
        {
            get
            {
                if (mOverloads == null)
                    return 0;
                return mOverloads.Count;
            }
        }

        public CodeCompletionItemType Type
        {
            get
            {
                return mType;
            }
        }

        public string Name
        {
            get
            {
                return mName;
            }
        }

        public string Text
        {
            get
            {
                if (mText == null)
                    BuildText();
                return mText;
            }
        }

        private void BuildText()
        {
            //TBD
            mText = mName;
        }

        public IEntity Entity
        {
            get
            {
                return mEntity;
            }
        }

        internal CsCodeCompletionItem(string name) : this(name, name)
        {
        }

        internal CsCodeCompletionItem(string name, string text)
        {
            mName = name;
            mText = text;
            mEntity = null;
            mType = CodeCompletionItemType.Text;
        }

        internal CsCodeCompletionItem(IClass iclass)
        {
            mEntity = iclass;
            mName = iclass.Name;
            mText = null;
            mType = CodeCompletionItemType.Class;
        }

        internal CsCodeCompletionItem(IMember member) : this(member, null)
        {
        }

        internal CsCodeCompletionItem(IMember member, string text)
        {
            mEntity = member;
            mName = member.Name;
            mText = text;
            if (member is IMethod)
                mType = CodeCompletionItemType.Method;
            else if (member is IProperty)
                mType = CodeCompletionItemType.Property;
            else if (member is IField)
                mType = CodeCompletionItemType.Field;
            else if (member is IEvent)
                mType = CodeCompletionItemType.Event;
            else
                mType = CodeCompletionItemType.Unknown;

        }

        public int CompareTo(ICsCodeCompletionItem i)
        {
            return string.Compare(this.mName, i.Name, true);
        }
    }

    public class CsCodeCompletionItemCollection : IEnumerable<ICsCodeCompletionItem>, ICodeCompletionItemCollection
    {
        private List<ICsCodeCompletionItem> mList = new List<ICsCodeCompletionItem>();
        private Dictionary<string, ICsCodeCompletionItem> mIndex = new Dictionary<string, ICsCodeCompletionItem>();
        private int mDefaultIndex = 0;
        private string mPreselect;

        public int DefaultIndex
        {
            get
            {
                return mDefaultIndex;
            }
        }

        public string Preselection
        {
            get
            {
                return mPreselect;
            }
        }

        internal int IndexOf(ICsCodeCompletionItem item)
        {
            return mList.IndexOf(item);
        }

        internal void Sort()
        {
            mList.Sort();
        }

        internal int DefaultIndex_Internal
        {
            get
            {
                return mDefaultIndex;
            }
            set
            {
                mDefaultIndex = value;
            }
        }

        internal string Preselection_Internal
        {
            get
            {
                return mPreselect;
            }
            set
            {
                mPreselect = value;
            }
        }

        internal int Capacity
        {
            get
            {
                return mList.Capacity;
            }
            set
            {
                mList.Capacity = value;
            }
        }

        public int Count
        {
            get
            {
                return mList.Count;
            }
        }

        public ICsCodeCompletionItem this[int index]
        {
            get
            {
                return mList[index];
            }
        }

        ICodeCompletionItem ICodeCompletionItemCollection.this[int index]
        {
            get
            {
                return mList[index];
            }
        }

        public IEnumerator<ICsCodeCompletionItem> GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        IEnumerator<ICodeCompletionItem> IEnumerable<ICodeCompletionItem>.GetEnumerator()
        {
            return new EnumeratorConvertor<ICsCodeCompletionItem, ICodeCompletionItem>(mList.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        internal void Add(ICsCodeCompletionItem item)
        {
            mIndex[item.Name] = item;
            mList.Add(item);
        }

        public ICsCodeCompletionItem Find(string name)
        {
            ICsCodeCompletionItem ret;
            if (!mIndex.TryGetValue(name, out ret))
                ret = null;
            return ret;
        }
    }
}
