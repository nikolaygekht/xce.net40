using System;
using System.Collections;
using System.Collections.Generic;

namespace gehtsoft.xce.intellisense.common
{
    public enum ProjectBrowserItemType
    {
        File,
        Class,
        Method,
        Property,
        Field,
        Event,
        Section,
        Tag,
        Text,
        Unknown,
    }

    public interface IProjectBrowserItem
    {
        ProjectBrowserItemType ItemType
        {
            get;
        }

        string Name
        {
            get;
        }

        string FileName
        {
            get;
        }

        int StartLine
        {
            get;
        }

        int StartColumn
        {
            get;
        }

        bool HasChildren
        {
            get;
        }

        IProjectBrowserItemCollection Children
        {
            get;
        }
    }

    internal class GenericProjectBrowserItem<T> : IProjectBrowserItem, IComparable<GenericProjectBrowserItem<T>>
    {
        ProjectBrowserItemType mItemType;
        public ProjectBrowserItemType ItemType
        {
            get
            {
                return mItemType;
            }
        }

        string mName;
        public string Name
        {
            get
            {
                return mName;
            }
        }

        string mFileName;
        public string FileName
        {
            get
            {
                return mFileName;
            }
        }

        int mStartLine;
        public int StartLine
        {
            get
            {
                return mStartLine;
            }
        }

        int mStartColumn;
        public int StartColumn
        {
            get
            {
                return mStartColumn;
            }
        }

        GenericProjectBrowserItemCollection<T> mChildren;

        public bool HasChildren
        {
            get
            {
                return mChildren != null && mChildren.Count > 0;
            }
        }

        public IProjectBrowserItemCollection Children
        {
            get
            {
                return mChildren;
            }
        }

        internal  GenericProjectBrowserItemCollection<T> _Children
        {
            get
            {
                if (mChildren == null)
                    mChildren = new GenericProjectBrowserItemCollection<T>();
                return mChildren;
            }
        }

        T mUserData;

        internal T UserData
        {
            get
            {
                return mUserData;
            }
        }

        internal GenericProjectBrowserItem(ProjectBrowserItemType type, string name, string file, int line, int column, T custom)
        {
            mItemType = type;
            mName = name;
            mFileName = file;
            mStartLine = line;
            mStartColumn = column;
            mChildren = null;
            mUserData = custom;
        }

        public int CompareTo(GenericProjectBrowserItem<T> obj)
        {
            return string.Compare(mName, obj.Name, true);
        }
    }

    public interface IProjectBrowserItemCollection : IEnumerable<IProjectBrowserItem>
    {
        int Count
        {
            get;
        }

        IProjectBrowserItem this[int index]
        {
            get;
        }
    }

    internal class GenericProjectBrowserItemCollection<T> : IProjectBrowserItemCollection
    {
        List<GenericProjectBrowserItem<T>> mList = new List<GenericProjectBrowserItem<T>>();

        internal GenericProjectBrowserItemCollection()
        {
        }

        public int Count
        {
            get
            {
                return mList.Count;
            }
        }

        IProjectBrowserItem IProjectBrowserItemCollection.this[int index]
        {
            get
            {
                return mList[index];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        public IEnumerator<IProjectBrowserItem> GetEnumerator()
        {
            return new EnumeratorConvertor<GenericProjectBrowserItem<T>, IProjectBrowserItem>(mList.GetEnumerator());
        }

        internal void Add(GenericProjectBrowserItem<T> item)
        {
            mList.Add(item);
        }

        internal void Clear()
        {
            mList.Clear();
        }
        
        internal void Sort()
        {
            mList.Sort();
        }
    }
}
