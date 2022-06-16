using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.colorer;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.configuration;
using gehtsoft.xce.configuration;
using gehtsoft.xce.editor.textwindow;
using ConsoleColor = gehtsoft.xce.conio.ConsoleColor;

namespace gehtsoft.xce.editor.search
{
    internal class RegexSyntaxComboBox : DialogItemComboBox
    {
        internal class RegexSyntaxAdapter : ILineSource, IDisposable
        {
            private StringBuilder mBuffer;
            private SyntaxHighlighter mHighlighter;
            private SyntaxRegion mPairStart, mPairEnd, mError, mKeyword;
            private bool mHighlightEnabled;

            internal SyntaxHighlighter Highlighter
            {
                get
                {
                    return mHighlighter;
                }
            }

            internal bool HighlightEnabled
            {
                get
                {
                    return mHighlightEnabled;
                }
                set
                {
                    mHighlightEnabled = value;
                    mHighlighter.NotifyFileNameChange();
                    if (mHighlightEnabled)
                        mHighlighter.Colorize(0, 1);
                }
            }

            internal void UpdateHighlighter()
            {
                mHighlighter.NotifyMajorChange(0);
            }


            internal RegexSyntaxAdapter(StringBuilder buffer, Application app)
            {
                mHighlightEnabled = false;
                mBuffer = buffer;
                mPairStart = app.ColorerFactory.FindSyntaxRegion("def:PairStart");
                mPairEnd = app.ColorerFactory.FindSyntaxRegion("def:PairEnd");
                mError = app.ColorerFactory.FindSyntaxRegion("def:Error");
                mKeyword = app.ColorerFactory.FindSyntaxRegion("def:Keyword");
                mHighlighter = app.ColorerFactory.CreateHighlighter(this);
                mHighlighter.Colorize(0, 1);
            }

            ~RegexSyntaxAdapter()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
            }

            virtual protected void Dispose(bool disposing)
            {
                if (mHighlighter != null)
                    mHighlighter.Dispose();
                mHighlighter = null;
            }

            public string GetFileName()
            {
                if (mHighlightEnabled)
                    return "search.regexp";
                else
                    return "search.plaintext";
            }

            public int GetLinesCount()
            {
                return 1;
            }

            public int GetLineLength(int line)
            {
                return mBuffer.Length;
            }

            public int GetLine(int line, char[] buff, int positionFrom, int length)
            {
                int lineLength = GetLineLength(line);
                if (lineLength <= 0 || positionFrom >= lineLength)
                    return 0;
                int cc = 0;
                for (int i = positionFrom; i < positionFrom + length && i < lineLength; i++)
                {
                    if (i < mBuffer.Length)
                        buff[i] = mBuffer[i];
                    cc++;
                }
                return cc;
            }
            internal bool IsPairStart(SyntaxHighlighterRegion r)
            {
                return r.Is(mPairStart);
            }
            internal bool IsPairEnd(SyntaxHighlighterRegion r)
            {
                return r.Is(mPairEnd);
            }
            internal bool IsError(SyntaxHighlighterRegion r)
            {
                return r.Is(mError);
            }
            internal bool IsKeyword(SyntaxHighlighterRegion r)
            {
                return r.Is(mKeyword);
            }

        }

        RegexSyntaxAdapter mSyntaxAdapter;
        XceColorScheme mColors;
        Application mApplication;

        internal RegexSyntaxComboBox(Application application, string text, int id, int row, int column, int width) : base(text, id, row, column, width)
        {
            mSyntaxAdapter = new RegexSyntaxAdapter(mText, application);
            mColors = application.ColorScheme;
            mApplication = application;
            mSyntaxAdapter.HighlightEnabled = false;
        }

        override protected void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (mSyntaxAdapter != null)
                mSyntaxAdapter.Dispose();
            mSyntaxAdapter = null;
        }

        public void UpdateHighlighter()
        {
            mSyntaxAdapter.UpdateHighlighter();
        }

        public bool HighlighterEnabled
        {
            get
            {
                return mSyntaxAdapter.HighlightEnabled;
            }
            set
            {
                mSyntaxAdapter.HighlightEnabled = value;
                invalidate();
            }
        }

        private int mLastOppositePair = -1;

        private int[] toChecks = new int[128];
        int toCheckCount;

        public override void OnPaint(Canvas canvas)
        {
            base.OnPaint(canvas);
            if (mSyntaxAdapter.HighlightEnabled && mInFocus)
            {
                mSyntaxAdapter.UpdateHighlighter();
                bool rc = mSyntaxAdapter.Highlighter.GetFirstRegion(0);
                int curStart = -1;
                int curLength = 0;
                toCheckCount = 0;
                while (rc)
                {
                    SyntaxHighlighterRegion r = mSyntaxAdapter.Highlighter.CurrentRegion;
                    if (mSyntaxAdapter.IsError(r) && r.HasColor)
                    {
                        Highlight(canvas, r.StartColumn, r.Length, mColors.DialogItemEditColorError);
                    }
                    else if (mSyntaxAdapter.IsKeyword(r) && r.HasColor)
                    {
                        Highlight(canvas, r.StartColumn, r.Length, mColors.DialogItemEditColorKeyword);
                    }
                    else if (mSyntaxAdapter.IsPairStart(r))
                    {
                        if (mCaret >= r.StartColumn && mCaret < r.StartColumn + r.Length)
                        {
                            curStart = r.StartColumn;
                            curLength = r.Length;
                        }
                        else
                        {
                            toChecks[toCheckCount++] = r.StartColumn;
                        }
                    }
                    else if (mSyntaxAdapter.IsPairEnd(r))
                    {
                        if (mCaret >= r.StartColumn && mCaret <= r.StartColumn + r.Length)
                        {
                            curStart = r.StartColumn;
                            curLength = r.Length;
                        }
                        else
                        {
                            toChecks[toCheckCount++] = r.StartColumn;
                        }
                    }
                    rc = mSyntaxAdapter.Highlighter.GetNextRegion();
                }
                mLastOppositePair = -1;
                for (int i = 0; i < toCheckCount; i++)
                {
                    SyntaxHighlighterPair cur = mSyntaxAdapter.Highlighter.MatchPair(0, toChecks[i]);
                    if (cur == null)
                    {
                        Highlight(canvas, toChecks[i], 1, mColors.DialogItemEditColorError);
                    }
                    else
                        cur.Dispose();
                }
                if (curStart >= 0 && curLength > 0)
                {
                    SyntaxHighlighterPair cur = mSyntaxAdapter.Highlighter.MatchPair(0, curStart);
                    if (cur == null)
                    {
                        Highlight(canvas, curStart, curLength, mColors.DialogItemEditColorError);
                    }
                    else
                    {
                        Highlight(canvas, cur.Start.StartColumn, cur.Start.Length, mColors.DialogItemEditColorPair);
                        Highlight(canvas, cur.End.StartColumn, cur.End.Length, mColors.DialogItemEditColorPair);
                        if (cur.Start.StartColumn == curStart)
                            mLastOppositePair = cur.End.StartColumn;
                        else
                            mLastOppositePair = cur.Start.StartColumn;
                        cur.Dispose();
                    }
                }
            }
        }

        internal int LastOppositePair
        {
            get
            {
                return mLastOppositePair;
            }
        }

        internal void SetCaret(int i)
        {
            mCaret = i;
            if (mCaret - mOffset >= EditWidth)
                mOffset = EditWidth - mCaret + 1;
            if (mCaret < mOffset)
                mOffset = mCaret;
            if (mInFocus)
                mApplication.WindowManager.setCaretPos(this, 0, mCaret - mOffset);
            invalidate();
        }

        private void Highlight(Canvas canvas, int from, int length, ConsoleColor foreColor)
        {
            from -= mOffset;
            if (from < 0)
            {
                length += from;
                from = 0;
            }
            if (length > 0)
                canvas.fillFg(0, from, 1, length, foreColor);
        }

        public override void OnClose()
        {
            mSyntaxAdapter.Dispose();
        }
    }



    internal class SearchDialog : XceDialog
    {
        Application mApp;
        SearchInfo mInfo;
        XceColorScheme mColors;

        RegexSyntaxComboBox mSearchText;
        DialogItemCheckBox mReplaceMode;
        DialogItemComboBox mReplaceText;
        DialogItemCheckBox mRegex;
        DialogItemCheckBox mIgnoreCase;
        DialogItemCheckBox mWholeWord;
        DialogItemCheckBox mInRange;
        DialogItemCheckBox mReplaceAll;
        DialogItemButton mAddLib;
        DialogItemButton mFromLib;
        DialogItemButton mOk, mCancel;

        Profile mProfile;

        internal SearchDialog(Application application, SearchInfo info) : base(application, "Search/Replace", true, 10, 60)
        {
            mInfo = info;
            mColors = application.ColorScheme;
            mApp = application;

            AddItem(new DialogItemLabel("&Search", 0x1000, 0, 0));
            AddItem(mSearchText = new RegexSyntaxComboBox(application, mInfo.Search, 0x1001, 0, 12, 46));
            AddItem(mReplaceMode = new DialogItemCheckBox("&Replace", 0x1002, mInfo.SearchMode == SearchMode.Replace, 1, 0));
            AddItem(mReplaceText = new DialogItemComboBox(mInfo.SearchMode == SearchMode.Replace ? mInfo.Replace : "", 0x1003, 1, 12, 46));
            AddItem(mRegex = new DialogItemCheckBox("Regular e&xpression", 0x1004, mInfo.Regex, 2, 0));
            AddItem(mAddLib = new DialogItemButton("< Save >", 0x1100, 2, 23));
            AddItem(mFromLib = new DialogItemButton("< &Load >", 0x1100, 2, 32));
            AddItem(mIgnoreCase = new DialogItemCheckBox("&Ignore case", 0x1005, mInfo.IgnoreCase, 3, 0));
            AddItem(mWholeWord = new DialogItemCheckBox("&Whole word", 0x1006, mInfo.WholeWord, 4, 0));
            AddItem(mInRange = new DialogItemCheckBox("In &Block", 0x1007, mInfo.SearchInRange, 5, 0));
            AddItem(mReplaceAll = new DialogItemCheckBox("Replace &All", 0x1008, mInfo.ReplaceAll, 6, 0));

            AddItem(mOk = new DialogItemButton("< &Ok >", DialogResultOK, 7, 0));
            AddItem(mCancel = new DialogItemButton("< &Cancel >", DialogResultCancel, 7, 0));
            CenterButtons(mOk, mCancel);
            RestoreHistory("search", mSearchText);
            RestoreHistory("replace", mReplaceText);
            if (mSearchText.Text.Length > 0)
                mSearchText.SetSel(0, mSearchText.Text.Length);
            if (mReplaceText.Text.Length > 0)
                mReplaceText.SetSel(0, mReplaceText.Text.Length);

        }

        private const int MAX_HISTORY = 32;

        public override void OnCreate()
        {
            base.OnCreate();
            mReplaceText.Enable(mReplaceMode.Checked);
            mReplaceText.show(mReplaceMode.Checked);
            mReplaceAll.Enable(mReplaceMode.Checked);
            mWholeWord.Enable(!mRegex.Checked);
            mAddLib.Enable(mRegex.Checked);
            mFromLib.Enable(mRegex.Checked);
            mInRange.Enable(mApp.ActiveWindow.BlockType != TextWindowBlock.None);
            mSearchText.HighlighterEnabled = mRegex.Checked;
        }

        public override void OnSizeChanged()
        {
            base.OnSizeChanged();
            if (Height != 10 || Width < 30)
                move(Row, Column, 10, Math.Max(Width, 30));
            else
            {
                int height, width;
                height = Height;
                width = Width;

                mSearchText.move(0, 12, 1, width - 14);
                mReplaceText.move(1, 12, 1, width - 14);
                CenterButtons(mOk, mCancel);
                invalidate();
            }
        }


        public override void OnItemChanged(DialogItem item)
        {
            if (item == mReplaceMode)
            {
                mReplaceText.Enable(mReplaceMode.Checked);
                mReplaceText.show(mReplaceMode.Checked);
                mReplaceAll.Enable(mReplaceMode.Checked);
                mInRange.Enable(mApp.ActiveWindow.BlockType == TextWindowBlock.None);
            }
            else if (item == mRegex)
            {
                mWholeWord.Enable(!mRegex.Checked);
                mSearchText.HighlighterEnabled = mRegex.Checked;
                mAddLib.Enable(mRegex.Checked);
                mFromLib.Enable(mRegex.Checked);
            }
            else
                base.OnItemChanged(item);
        }

        public override void OnItemClick(DialogItem item)
        {
            base.OnItemClick(item);

            if (item == mAddLib)
            {
                AddRegexLibraryItemDialog dlg = new AddRegexLibraryItemDialog(mApplication);
                if (dlg.DoModal() == Dialog.DialogResultOK)
                {
                    RegexLibraryItem libitem = new RegexLibraryItem(dlg.Name, mSearchText.Text, mReplaceText.Text, mReplaceMode.Checked, mIgnoreCase.Checked);
                    RegexLibrary lib = RegexLibraryLoader.Load(mApplication.ApplicationPath);
                    lib.Add(libitem);
                    RegexLibraryLoader.Save(mApplication.ApplicationPath, lib);
                }
            }
            else if (item == mFromLib)
            {
                GetRegexLibraryDialog dlg = new GetRegexLibraryDialog(mApp);
                if (dlg.DoModal() == Dialog.DialogResultOK)
                {
                    mSearchText.Text = dlg.ChosenItem.Search;
                    mReplaceMode.Checked = dlg.ChosenItem.IsReplace;
                    OnItemChanged(mReplaceMode);
                    mReplaceText.Text = dlg.ChosenItem.Replace;
                    mIgnoreCase.Checked = dlg.ChosenItem.IgnoreCase;
                }
            }
        }

        public override bool OnOK()
        {
            //validate regexp
            string s = mSearchText.Text;

            if (s.Length == 0)
            {
                MessageBox.Show(WindowManager, mColors, "The search text is empty", "Search/Replace", MessageBoxButtonSet.Ok);
                return false;
            }

            string reText = RegexBuilder.Build(s, mRegex.Checked, mIgnoreCase.Checked, mWholeWord.Checked);

            try
            {
                Regex re = new Regex(reText);
                re.Dispose();
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(WindowManager, mColors, "Regular expression:\r\n" + reText + "\r\ncan't be compiled\r\n" + e.Message, "Search/Replace", MessageBoxButtonSet.Ok);
                return false;
            }

            if (mReplaceMode.Checked && mRegex.Checked)
            {
                s = mReplaceText.Text;
                bool prevSlash = false;
                for (int i = 0; i < s.Length; i++)
                {
                    if (prevSlash)
                    {
                        switch (s[i])
                        {
                        case    '\\':
                                break;
                        case    'n':
                                break;
                        case    'u':
                                break;
                        case    'l':
                                break;
                        case    't':
                                break;
                        case    '0':
                        case    '1':
                        case    '2':
                        case    '3':
                        case    '4':
                        case    '5':
                        case    '6':
                        case    '7':
                        case    '8':
                        case    '9':
                                break;
                        default:
                                MessageBox.Show(WindowManager, mColors, "The replace expression contains unknown slash code", "Search/Replace", MessageBoxButtonSet.Ok);
                                return false;
                        }
                        prevSlash = false;
                    }
                    else
                        if (s[i] == '\\')
                            prevSlash = true;
                }
                if (prevSlash)
                {
                    MessageBox.Show(WindowManager, mColors, "Replace expression ends be a slash", "Search/Replace", MessageBoxButtonSet.Ok);
                    return false;

                }
            }

            mInfo.Search = mSearchText.Text;
            mInfo.SearchMode = mReplaceMode.Checked ? SearchMode.Replace : SearchMode.Search;
            if (mReplaceMode.Checked)
                mInfo.Replace = mReplaceText.Text;
            mInfo.Regex = mRegex.Checked;
            mInfo.IgnoreCase = mIgnoreCase.Checked;
            mInfo.WholeWord = mWholeWord.Checked;

            if (mInRange.Enabled)
                mInfo.SearchInRange = mInRange.Checked;
            else
                mInfo.SearchInRange = false;

            if (mReplaceMode.Checked)
                mInfo.ReplaceAll = mReplaceAll.Checked;
            else
                mInfo.ReplaceAll = false;

            StoreHistory("search", mSearchText);
            if (mReplaceMode.Checked)
                StoreHistory("replace", mReplaceText);
            return true;
        }

        private void RestoreHistory(string key, DialogItemComboBox control)
        {
            mProfile = new Profile();
            try
            {
                mProfile.Load(Path.Combine(mApp.ApplicationPath, "search-history.ini"));
                ProfileSection s = mProfile[""];
                for (int i = MAX_HISTORY - 1; i >= 0; i--)
                {
                    string k = s[key, i, null];
                    if (k != null)
                        control.AddItem(new DialogItemListBoxString(k, null));
                }
            }
            catch (Exception)
            {
            }

        }

        private void StoreHistory(string key, DialogItemComboBox control)
        {
            ProfileSection s  = mProfile[""];
            string t = control.Text;
            for (int i = MAX_HISTORY - 1; i >= 0; i--)
            {
                string k = s[key, i, null];
                if (k != null && k == t)
                    s.Remove(key, i);
            }
            s.Add(key, t);
            if (s.CountOf(key) > MAX_HISTORY)
                s.Remove(key, 0);
            mProfile.Save(Path.Combine(mApp.ApplicationPath, "search-history.ini"));
        }
    }
}
