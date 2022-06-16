using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.configuration;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;

namespace gehtsoft.xce.editor.search
{
    internal class RegexLibraryItem
    {
        private string mTitle;
        private string mSearch;
        private bool mIsReplace;
        private string mReplace;
        private bool mIgnoreCase;
        
        internal string Title
        {
            get
            {
                return mTitle;
            }
            set
            {
                mTitle = value;
            }
        }
        
        internal string Search
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
        
        internal string Replace
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
        
        internal bool IsReplace
        {
            get
            {
                return mIsReplace;
            }
            set
            {
                mIsReplace = value;
            }
        }
        
        internal bool IgnoreCase
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
        
        internal RegexLibraryItem(string title, string search, string replace, bool isReplace, bool ignoreCase)
        {
            mTitle = title;
            mSearch = search;
            mReplace = replace;
            mIsReplace = isReplace;
            mIgnoreCase = ignoreCase;
        }
    }
    
    internal class RegexLibrary : IEnumerable<RegexLibraryItem>
    {
        List<RegexLibraryItem> mItems = new List<RegexLibraryItem>();
    
        public IEnumerator<RegexLibraryItem> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }
        
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return mItems.GetEnumerator();
        }
        
        internal RegexLibrary()
        {
        }
        
        internal void Sort()
        {
            mItems.Sort(new Comparison<RegexLibraryItem>(Comparer));
        }
        
        private int Comparer(RegexLibraryItem t1, RegexLibraryItem t2)
        {
            return string.Compare(t1.Title, t2.Title, true);
        }
        
        internal int Count
        {
            get
            {
                return mItems.Count;
            }
        }
        
        internal RegexLibraryItem this[int index]
        {
            get
            {
                return mItems[index];
            }
        }
        
        internal void Add(RegexLibraryItem item)
        {
            mItems.Add(item);
        }
        
        internal void Remove(int index)
        {
            mItems.RemoveAt(index);
        }
        
        internal int Find(RegexLibraryItem item)
        {
            for (int i = 0; i < mItems.Count; i++)
                if (object.ReferenceEquals(item, mItems[i]))
                    return i;
            return -1;
        }
    }
    
    internal class RegexLibraryLoader
    {
        internal static RegexLibrary Load(string appPath)
        {
            RegexLibrary library = new RegexLibrary();
            
            try
            {
                Profile storage = new Profile();
                storage.Load(Path.Combine(appPath, "regex-library.ini"));
                
                foreach (ProfileSection section in storage)
                {
                    if (section.Name == "item" &&
                        section.Exists("title") &&
                        section.Exists("search") &&
                        section.Exists("replace") &&
                        section.Exists("is-replace") &&
                        section.Exists("is-caseless"))
                    {
                        string title = section["title"].Trim();
                        string search = section["search"].Trim();
                        string replace = section["replace"].Trim();
                        bool isreplace = section["is-replace"].Trim().Equals("true");
                        bool iscaseless = section["is-caseless"].Trim().Equals("true");
                        
                        library.Add(new RegexLibraryItem(title, search, replace, isreplace, iscaseless));
                    }
                }
                
                library.Sort();
            }
            catch (Exception )
            {
            
            }
            return library;
        }
        
        internal static void Save(string appPath, RegexLibrary library)
        {
            Profile storage = new Profile();
            
            foreach (RegexLibraryItem item in library)
            {
                ProfileSection s = storage.AddSection("item");
                s["title"] = item.Title;
                s["search"] = item.Search;
                s["replace"] = item.Replace;
                s["is-replace"] = item.IsReplace ? "true" : "false";
                s["is-caseless"] = item.IgnoreCase ? "true" : "false";
            }
            
            try
            {
                storage.Save(Path.Combine(appPath, "regex-library.ini"));
            }
            catch (Exception )
            {
            
            }
        }
    }
    
    internal class AddRegexLibraryItemDialog : XceDialog
    {
        private DialogItemSingleLineTextBox mTitle;
        private string mName;
        
        internal string Name
        {
            get
            {
                return mName;
            }
        }
        
        
        internal AddRegexLibraryItemDialog(Application application) : base(application, "Save Regex", false, 4, 40)
        {
            AddItem(new DialogItemLabel("&Name:", 0x1000, 0, 0));
            AddItem(mTitle = new DialogItemSingleLineTextBox("", 0x1001, 0, 6, 32));
            DialogItemButton b1, b2;
            AddItem(b1 = new DialogItemButton("< &Ok >", DialogResultOK, 1, 0));
            AddItem(b2 = new DialogItemButton("< &Cancel >", DialogResultCancel, 1, 0));
            CenterButtons(b1, b2);
        }

        public override bool OnOK()
        {
            if (mTitle.Text.Length < 1)
            {
                mApplication.ShowMessage("You must enter the name", "Error");
                return false;
            }
            mName = mTitle.Text;
            return true;
        }
    }
    
    internal class GetRegexLibraryDialog : XceDialog
    {
        DialogItemListBox mItems;
        DialogItemButton mOk;
        DialogItemButton mCancel;
        DialogItemButton mRemove;
        RegexLibrary mLibrary;
        RegexLibraryItem mChosen;
        
        internal RegexLibraryItem ChosenItem
        {
            get
            {
                return mChosen;
            }
        }
        
        
        internal GetRegexLibraryDialog(Application application) : base(application, "Load Regex", true, 15, 40)
        {
            mLibrary = RegexLibraryLoader.Load(application.ApplicationPath);
            AddItem(mItems = new DialogItemListBox(0x1000, 0, 0, 12, 38));
            AddItem(mOk = new DialogItemButton("< &Select >", DialogResultOK, 12, 0));
            AddItem(mCancel = new DialogItemButton("< &Cancel >", DialogResultCancel, 12, 0));
            AddItem(mRemove = new DialogItemButton("< &Delete >", 0x1001, 12, 0));
            CenterButtons(mOk, mCancel, mRemove);
            foreach (RegexLibraryItem item in mLibrary)
                mItems.AddItem(item.Title, item);
        }

        public override void OnSizeChanged()
        {
            base.OnSizeChanged();
            if (Height < 9 || Width < 40)
                move(Row, Column, Math.Max(Height, 9), Math.Max(Width, 40));
            else
            {
                int height, width;
                height = Height;
                width = Width;

                mItems.move(0, 0, height - 3, width - 2);
                mOk.move(height - 3, 0, 1, mOk.Title.Length);
                mCancel.move(height - 3, 0, 1, mCancel.Title.Length);
                mRemove.move(height - 3, 0, 1, mRemove.Title.Length);
                CenterButtons(mOk, mCancel, mRemove);
                invalidate();
            }
        }

        public override bool OnOK()
        {
            if (mItems.CurSel >= 0 && mItems.CurSel < mItems.Count &&
                mItems[mItems.CurSel].UserData != null)
            {
                mChosen = mItems[mItems.CurSel].UserData as RegexLibraryItem;
                return true;
            }
            else
            {
                mApplication.ShowMessage("An item must be chosen", "Error");
                return false;
            }
        }

        public override void OnItemClick(DialogItem item)
        {
            if (item == mCancel || item == mOk)
                base.OnItemClick(item);
            if (item == mRemove)
            {
                if (mItems.CurSel >= 0 && mItems.CurSel < mItems.Count &&
                    mItems[mItems.CurSel].UserData != null)
                {
                    RegexLibraryItem r = mItems[mItems.CurSel].UserData as RegexLibraryItem;
                    int idx = mLibrary.Find(r);
                    if (idx >= 0 && idx < mLibrary.Count)
                    {
                        mLibrary.Remove(idx);
                        RegexLibraryLoader.Save(mApplication.ApplicationPath, mLibrary);
                        mItems.RemoveItem(mItems.CurSel);
                        mItems.CurSel = -1;
                    }
                }
                else
                {
                    mApplication.ShowMessage("An item must be chosen", "Error");
                    return ;
                }
            }
        }    }
    
    
}
