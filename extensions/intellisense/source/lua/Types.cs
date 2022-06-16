using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.intellisense.common;

namespace gehtsoft.intellisense.lua
{
    public class LuaKeyword
    {
        private bool mIsTable;

        public bool IsTable
        {
            get
            {
                return mIsTable;
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

        public LuaKeyword(string name) : this(name, false)
        {

        }

        public LuaKeyword(string name, bool table)
        {
            mName = name;
            mIsTable = table;
        }
    }

    internal class LuaKeywordCollection : IEnumerable<LuaKeyword>
    {
        private Dictionary<string, LuaKeyword> mDict = new Dictionary<string, LuaKeyword>();
        private List<LuaKeyword> mList = new List<LuaKeyword>();

        public IEnumerator<LuaKeyword> GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return mList.Count;
            }

        }

        public LuaKeyword this[int index]
        {
            get
            {
                return mList[index];
            }

        }

        public LuaKeyword this[string name]
        {
            get
            {
                LuaKeyword k;
                if (!mDict.TryGetValue(name, out k))
                    k = null;
                return k;
            }
        }

        internal LuaKeywordCollection()
        {
        }

        internal void Add(LuaKeyword w)
        {
            mList.Add(w);
            mDict[w.Name] = w;
        }
    }

    public class LuaProperty : IInsightData
    {
        /** is function by . (true) or : (false) */
        private bool mStatic;
        public bool Static
        {
            get
            {
                return mStatic;
            }
        }

        /** The name of the function. */
        private string mName;
        public string Name
        {
            get
            {
                return mName;
            }
        }

        /** The table of the function. */
        private string mTable;
        public string Table
        {
            get
            {
                return mTable;
            }
        }

        /** Is function or property. */
        private bool mIsFunction;

        public bool IsFunction
        {
            get
            {
                return mIsFunction;
            }
        }

        /** Function arguments. */
        private string mArgs;
        public string Args
        {
            get
            {
                return mArgs;
            }
        }

        private int mArgc;

        public int ArgsCount
        {
            get
            {
                return mArgc;
            }
        }

        public LuaProperty(string name)
        {
            mStatic = true;
            mName = name;
            mTable = "_G";
            mIsFunction = false;
            mArgs = "";
            mArgc = 0;
        }

        public LuaProperty(string table, string name)
        {
            mStatic = true;
            mName = name;
            mTable = table;
            mIsFunction = false;
            mArgs = "";
            mArgc = 0;

        }

        public LuaProperty(string table, bool staticFunction, string name, string args)
        {
            mStatic = staticFunction;
            mName = name;
            if (table != null)
                mTable = table;
            else
                mTable = "_G";
            mIsFunction = true;
            mArgs = args;
            mArgc = 0;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == ',')
                    mArgc++;
                else if (i > 0 && (char.IsLetter(args[i]) || args[i] != '.') && mArgc == 0)
                {
                    mArgc = 1;
                }
            }
        }
        
        string mTip = null;
        
        public string Tip
        {
            get
            {
                if (mTip == null)
                {
                    string tail;
                    if (IsFunction)
                    {
                        tail = mName + mArgs;
                    }
                    else
                    {
                        tail = mName;
                    }
                    if (mTable != "_G")
                    {
                        if (mStatic)
                            mTip = mTable + "." + tail;
                        else
                            mTip = mTable + ":" + tail;
                    }
                    else
                    {
                        mTip = tail;
                    }
                }
                return mTip;
            }
        }
    }

    internal class LuaPropertyCollection : IEnumerable<LuaProperty>
    {
        private List<LuaProperty> mList = new List<LuaProperty>();

        public IEnumerator<LuaProperty> GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return mList.Count;
            }

        }

        public LuaProperty this[int index]
        {
            get
            {
                return mList[index];
            }
        }

        internal LuaPropertyCollection()
        {
        }

        internal void Add(LuaProperty p)
        {
            mList.Add(p);
        }

        static Comparison<LuaProperty> mComparison = new Comparison<LuaProperty>(comparison);

        static int comparison(LuaProperty s1, LuaProperty s2)
        {
            int i = string.Compare(s1.Table, s2.Table, true);
            if (i != 0)
                return i;
            else
                return string.Compare(s1.Name, s2.Name, true);
        }

        internal void Sort()
        {
            mList.Sort(mComparison);
        }
        
        internal void Clear()
        {
            mList.Clear();
        }
    }

    internal class LuaPropertyDictionary : LuaPropertyCollection
    {
        private Dictionary<string, LuaPropertyCollection> mNameDict = new Dictionary<string, LuaPropertyCollection>();
        private Dictionary<string, LuaPropertyCollection> mTableDict = new Dictionary<string, LuaPropertyCollection>();

        internal LuaPropertyDictionary()
        {
        }

        new internal void Add(LuaProperty p)
        {
            base.Add(p);
            if (mNameDict.ContainsKey(p.Name))
                mNameDict[p.Name].Add(p);
            else
            {
                LuaPropertyCollection coll = new LuaPropertyCollection();
                coll.Add(p);
                mNameDict[p.Name] = coll;
            }
            if (mTableDict.ContainsKey(p.Table))
                mTableDict[p.Table].Add(p);
            else
            {
                LuaPropertyCollection coll = new LuaPropertyCollection();
                coll.Add(p);
                mTableDict[p.Table] = coll;
            }
        }

        internal LuaPropertyCollection FindByName(string name)
        {
            LuaPropertyCollection coll;
            if (!mNameDict.TryGetValue(name, out coll))
                coll = null;
            return coll;
        }

        internal LuaPropertyCollection FindByTable(string name)
        {
            LuaPropertyCollection coll;
            if (!mTableDict.TryGetValue(name, out coll))
                coll = null;
            return coll;
        }
    }
}