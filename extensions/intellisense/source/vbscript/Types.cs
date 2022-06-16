using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.intellisense.common;

namespace gehtsoft.intellisense.vbscript
{
    public enum VbKeywordContext
    {
        Expr,
        NotExpr,
        Bol,
        Inside,
        InsideAnywhere,
        InsideAndBol,
        InsideAnywhereAndBol,
    }

    public class VbKeyword
    {
        private string mName;
        public string Name
        {
            get
            {
                return mName;
            }
        }


        private string mAfter;
        public string After
        {
            get
            {
                return mAfter;
            }
        }

        private VbKeywordContext mContext;
        public VbKeywordContext Context
        {
            get
            {
                return mContext;
            }
        }

        private List<string> mInsides;

        public int Count
        {
            get
            {
                if (mInsides == null)
                    return 0;
                else
                    return mInsides.Count;
            }
        }

        public string this[int index]
        {
            get
            {
                if (mInsides == null || index < 0 || index >= mInsides.Count)
                    throw new ArgumentOutOfRangeException("index");
                return mInsides[index];
            }
        }

        public VbKeyword(string name)
        {
            mName = name;
            mAfter = null;
            mContext = VbKeywordContext.Expr;
        }

        public VbKeyword(string name, bool bolOnly)
        {
            mName = name;
            mAfter = null;
            mContext = VbKeywordContext.Bol;
        }

        public VbKeyword(string name, string after)
        {
            mName = name;
            mAfter = after;
            mContext = VbKeywordContext.NotExpr;
        }

        public VbKeyword(string name, bool anywhere, params string[] args)
        {
            mName = name;
            mAfter = null;
            mContext = anywhere ? VbKeywordContext.InsideAnywhereAndBol : VbKeywordContext.InsideAndBol;
            mInsides = new List<string>();
            foreach (string s in args)
                mInsides.Add(s);
        }

        public VbKeyword(string name, string after, bool anywhere, params string[] args)
        {
            mName = name;
            mAfter = after;
            mContext = anywhere ? VbKeywordContext.InsideAnywhere : VbKeywordContext.Inside;
            mInsides = new List<string>();
            foreach (string s in args)
                mInsides.Add(s);
        }
    }

    internal class VbKeywordCollection : IEnumerable<VbKeyword>
    {
        private Dictionary<string, VbKeyword> mDict = new Dictionary<string, VbKeyword>();
        private List<VbKeyword> mList = new List<VbKeyword>();

        public IEnumerator<VbKeyword> GetEnumerator()
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

        public VbKeyword this[int index]
        {
            get
            {
                return mList[index];
            }

        }

        public VbKeyword this[string name]
        {
            get
            {
                VbKeyword k;
                if (!mDict.TryGetValue(name, out k))
                    k = null;
                return k;
            }
        }

        internal VbKeywordCollection()
        {
        }

        internal void Add(VbKeyword w)
        {
            mList.Add(w);
            mDict[w.Name] = w;
        }
    }

    public class VbProperty : IInsightData
    {
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

        public VbProperty(string name)
        {
            mName = name;
            mTable = null;
            mIsFunction = false;
            mArgs = "";
            mArgc = 0;
        }

        public VbProperty(string table, string name)
        {
            mName = name;
            mTable = table;
            mIsFunction = false;
            mArgs = "";
            mArgc = 0;

        }

        public VbProperty(string table, string name, string args)
        {
            mName = name;
            mTable = table;
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
                        tail = mName + mArgs;
                    else
                        tail = mName;
                    if (mTable != null)
                        mTip = mTable + "." + tail;
                    else
                        mTip = tail;
                }
                return mTip;
            }
        }
    }

    internal class VbPropertyCollection : IEnumerable<VbProperty>
    {
        private List<VbProperty> mList = new List<VbProperty>();
        private Dictionary<string, bool> mPrefefinedNames = new Dictionary<string, bool>();
        
        internal void AddPredefinedName(string name)
        {
            mPrefefinedNames.Add(name.ToUpper(), true);
        }
        
        internal bool IsPredefinedName(string name)
        {
            return mPrefefinedNames.ContainsKey(name.ToUpper());
        }
        
        

        public IEnumerator<VbProperty> GetEnumerator()
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

        public VbProperty this[int index]
        {
            get
            {
                return mList[index];
            }
        }

        internal VbPropertyCollection()
        {
        }

        internal void Add(VbProperty p)
        {
            mList.Add(p);
        }

        static Comparison<VbProperty> mComparison = new Comparison<VbProperty>(comparison);

        static int comparison(VbProperty s1, VbProperty s2)
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

    internal class VbPropertyDictionary : VbPropertyCollection
    {
        private Dictionary<string, VbPropertyCollection> mNameDict = new Dictionary<string, VbPropertyCollection>();
        private VbPropertyCollection mGlobal = new VbPropertyCollection();
        private VbPropertyCollection mMethods = new VbPropertyCollection();

        internal VbPropertyDictionary()
        {
        }

        new internal void Add(VbProperty p)
        {
            base.Add(p);
            if (mNameDict.ContainsKey(p.Name))
                mNameDict[p.Name].Add(p);
            else
            {
                VbPropertyCollection coll = new VbPropertyCollection();
                coll.Add(p);
                mNameDict[p.Name] = coll;
            }
            if (p.Table == null)
                mGlobal.Add(p);
            else
                mMethods.Add(p);
        }

        internal VbPropertyCollection FindByName(string name)
        {
            VbPropertyCollection coll;
            if (!mNameDict.TryGetValue(name, out coll))
                coll = null;
            return coll;
        }

        internal VbPropertyCollection GetGlobal()
        {
            return mGlobal;
        }

        internal VbPropertyCollection GetMethods()
        {
            return mMethods;
        }
    }
}