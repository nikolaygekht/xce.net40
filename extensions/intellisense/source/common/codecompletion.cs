using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace gehtsoft.xce.intellisense.common
{
    public enum CodeCompletionItemType
    {
        Class,
        Method,
        Property,
        Field,
        Event,
        Tag,
        Text,
        Unknown,
    }

    public interface ICodeCompletionItem
    {
        int OverloadsCount
        {
            get;
        }

        CodeCompletionItemType Type
        {
            get;
        }

        string Name
        {
            get;
        }

        string Text
        {
            get;
        }
    }

    public interface ICodeCompletionItemCollection : IEnumerable<ICodeCompletionItem>
    {
        int Count
        {
            get;
        }

        ICodeCompletionItem this[int index]
        {
            get;
        }

        int DefaultIndex
        {
            get;
        }

        string Preselection
        {
            get;
        }
    }

    internal class GenericCodeCompletionItem : ICodeCompletionItem
    {

        public int OverloadsCount
        {
            get
            {
                return 0;
            }
        }

        private CodeCompletionItemType mType;
        public CodeCompletionItemType Type
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

        private string mText;
        public string Text
        {
            get
            {
                return mText;
            }
        }

        private object mUserData;
        internal object UserData
        {
            get
            {
                return mUserData;
            }
        }

        internal GenericCodeCompletionItem(string t) : this(CodeCompletionItemType.Text, t, t, null)
        {
        }

        internal GenericCodeCompletionItem(string t, object userData) : this(CodeCompletionItemType.Text, t, t, userData)
        {
        }

        internal GenericCodeCompletionItem(CodeCompletionItemType type, string name, string text) : this(type, name, text, null)
        {
        }

        internal GenericCodeCompletionItem(CodeCompletionItemType type, string name, string text, object userData)
        {
            mType = type;
            mName = name;
            mText = text;
            mUserData = userData;
        }
    }

    internal class GenericCodeCompletionItemCollection : ICodeCompletionItemCollection
    {
        private List<ICodeCompletionItem> mItems = new List<ICodeCompletionItem>();

        internal void Add(ICodeCompletionItem item)
        {
            mItems.Add(item);
        }

        public int Count
        {
            get
            {
                return mItems.Count;
            }
        }

        public ICodeCompletionItem this[int index]
        {
            get
            {
                return mItems[index];
            }
        }

        public IEnumerator<ICodeCompletionItem> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        private int mDefaultIndex;

        public int DefaultIndex
        {
            get
            {
                return mDefaultIndex;
            }
        }
        internal int _DefaultIndex
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

        private string mPreselection;
        public string Preselection
        {
            get
            {
                return mPreselection;
            }
        }
        public string _Preselection
        {
            get
            {
                return mPreselection;
            }
            set
            {
                mPreselection = value;
            }
        }

        static Comparison<ICodeCompletionItem> mComparison = new Comparison<ICodeCompletionItem>(comparison);

        static int comparison(ICodeCompletionItem s1, ICodeCompletionItem s2)
        {
            return string.Compare(s1.Name, s2.Name, true);
        }

        internal void Sort()
        {
            mItems.Sort(mComparison);
        }

    }
}
