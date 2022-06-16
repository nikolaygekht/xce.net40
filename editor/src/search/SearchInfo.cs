using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.configuration;

namespace gehtsoft.xce.editor.search
{
    [Flags]
    internal enum SearchMode
    {
        Search = 1,
        Replace = 2,
    }


    internal class SearchInfo
    {
        private SearchMode mSearchMode = SearchMode.Search;

        public SearchMode SearchMode
        {
            get
            {
                return mSearchMode;
            }
            set
            {
                mSearchMode = value;
            }
        }

        private bool mIgnoreCase = true;
        private bool mWholeWord = false;
        private bool mRegex = false;

        public bool IgnoreCase
        {
            get
            {
                return mIgnoreCase;
            }
            set
            {
                mIgnoreCase = value;
            }
        }

        public bool WholeWord
        {
            get
            {
                return mWholeWord;
            }
            set
            {
                mWholeWord = value;
            }
        }


        public bool Regex
        {
            get
            {
                return mRegex;
            }
            set
            {
                mRegex = value;
            }
        }

        private string mSearch = "", mReplace = "";

        public string Search
        {
            get
            {
                return mSearch;
            }
            set
            {
                mSearch = value;
            }
        }

        public string Replace
        {
            get
            {
                return mReplace;
            }
            set
            {
                mReplace = value;
            }
        }

        private bool mReplaceAll = false;

        public bool ReplaceAll
        {
            get
            {
                return mReplaceAll;
            }
            set
            {
                mReplaceAll = value;
            }
        }

        private object mLastSearchTarget = null;
        public object LastSearchTarget
        {
            get
            {
                return mLastSearchTarget;
            }
            set
            {
                mLastSearchTarget = value;
            }
        }

        private int mLastSearchPos;

        public int LastSearchPos
        {
            get
            {
                return mLastSearchPos;
            }
            set
            {
                mLastSearchPos = value;
            }
        }

        private int mLastSearchLength = -1;

        public int LastSearchLength
        {
            get
            {
                return mLastSearchLength;
            }
            set
            {
                mLastSearchLength = value;
            }
        }

        public void ResetLastSearch()
        {
            mLastSearchTarget = null;
            mLastSearchPos = -1;
            mLastSearchLength = -1;
            mSearchInRange = false;
            mReplaceAll = false;
        }

        private bool mSearchInRange = false;

        public bool SearchInRange
        {
            get
            {
                return mSearchInRange;
            }
            set
            {
                mSearchInRange = value;
            }
        }
    }

    internal static class SearchInfoBuilder
    {
        internal static SearchInfo Restore(Application app)
        {
            SearchInfo i = new SearchInfo();
            Profile profile = new Profile();

            try
            {
                profile.Load(Path.Combine(app.ApplicationPath, "search-history.ini"));
                ProfileSection s = profile["last"];
                i.Search = s["search-text", ""];
                int t;
                if (!Int32.TryParse(s["replace", "0"].Trim(), out t))
                    t = 0;
                i.SearchMode = t == 0 ? SearchMode.Search : SearchMode.Replace;
                i.Replace = s["replace-text", ""];
                if (!Int32.TryParse(s["regex", "0"].Trim(), out t))
                    t = 0;
                i.Regex = t != 0;
                if (!Int32.TryParse(s["ignore-case", "0"].Trim(), out t))
                    t = 0;
                i.IgnoreCase = t != 0;
                if (!Int32.TryParse(s["whole-word", "0"].Trim(), out t))
                    t = 0;
                i.WholeWord = t != 0;
                if (!Int32.TryParse(s["in-range", "0"].Trim(), out t))
                    t = 0;
                i.SearchInRange = t != 0;
            }
            catch (Exception)
            {
            }
            return i;
        }

        internal static void Store(Application app, SearchInfo i)
        {
            Profile profile = new Profile();

            try
            {
                profile.Load(Path.Combine(app.ApplicationPath, "search-history.ini"));
            }
            catch (Exception)
            {
            }

            ProfileSection s = profile["last"];
            if (s == null)
            {
                profile["last", "dummy"] = "";
                s = profile["last"];
                s.Remove("dummy", 0);
            }
            s.Clear();
            s["search-text"] = i.Search;
            s["replace"] = i.SearchMode == SearchMode.Replace ? "1" : "0";
            s["replace-text"] = i.Replace;
            s["regex"] = i.Regex ? "1" : "0";
            s["ignore-case"] = i.IgnoreCase ? "1" : "0";
            s["whole-word"] = i.WholeWord ? "1" : "0";
            s["in-range"] = i.SearchInRange ? "1" : "0";

            try
            {
                profile.Save(Path.Combine(app.ApplicationPath, "search-history.ini"));
            }
            catch (Exception)
            {
            }
        }
    }
}
