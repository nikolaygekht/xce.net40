using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using gehtsoft.intellisense.cs;
using gehtsoft.xce.colorer;
using gehtsoft.xce.text;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.configuration;
using gehtsoft.xce.intellisense.common;
using GehtSoft.DocCreator.Parser;

namespace gehtsoft.intellisense.docsource
{
    internal class PairInfoCollection
    {
        private struct PairInfo
        {
            public bool End;
            public int Start;
            public int Length;
        }

        private PairInfo[] mInfo = new PairInfo[256];
        private int mCount;

        internal void reset()
        {
            mCount = 0;
        }

        internal void add(bool end, int start, int length)
        {
            if (mCount < 256)
            {
                mInfo[mCount].End = end;
                mInfo[mCount].Start = start;
                mInfo[mCount].Length = length;
                mCount++;
            }
        }

        internal int Count
        {
            get
            {
                return mCount;
            }
        }

        internal bool get(int index, out bool end, out int start, out int length)
        {
            if (index >= 0 && index < mCount)
            {
                end = mInfo[index].End;
                start = mInfo[index].Start;
                length = mInfo[index].Length;
                return true;
            }
            else
            {
                end = false;
                start = length = 0;
                return false;
            }
        }
    }


    internal class DocSourceFileAnalyzer
    {
        private SyntaxRegion mKeyword;
        private SyntaxRegion mPairStart;
        private SyntaxRegion mPairEnd;
        private SyntaxRegion mSymbol;
        private SyntaxRegion mError;
        private object mMutex;
        private PairInfoCollection mPairCollection = new PairInfoCollection();
        private TagCollection mTags = TagCollectionFactory.create();
        private BbCodeCollection mBbCodes = new BbCodeCollection();
        private TextWindow mWindow;

        internal object Mutex
        {
            get
            {
                return mMutex;
            }
        }

        internal DocSourceFileAnalyzer(SyntaxRegion pairStart, SyntaxRegion pairEnd, SyntaxRegion keyword, SyntaxRegion symbol, SyntaxRegion error)
        {
            mKeyword = keyword;
            mPairStart = pairStart;
            mPairEnd = pairEnd;
            mSymbol = symbol;
            mError = error;
            mMutex = new object();
        }

        private string mLastOpenTag;
        private string mLastOpenPair;
        private string mAttributeAtThisLine;
        private int mAttributeAtThisLineStart;
        private List<string> mFilledParams = new List<string>();
        private bool mIsLineEmptyPriorToPosition;
        private int mTagDepth;
        private int mPairDepth;

        internal void reset()
        {
            mLastOpenPair = null;
            mLastOpenTag = null;
            mAttributeAtThisLine = null;
            mAttributeAtThisLineStart = -1;
            mFilledParams.Clear();
            mIsLineEmptyPriorToPosition = false;
            mTagDepth = 0;
            mPairDepth = 0;
        }

        internal void analyze(TextWindow window, int line, int column)
        {
            mWindow = window;
            for (int i = line; i >= 0; i--)
                if (analyzeLine(window, i, i == line ? column : -1))
                    return;
        }

        private bool analyzeLine(TextWindow window, int line, int column)
        {
            XceFileBuffer buffer = window.Text;
            SyntaxHighlighter highlight = window.Highlighter;

            if (line >= buffer.LinesCount)
            {
                if (column >= 0)
                {
                    mIsLineEmptyPriorToPosition = true;
                    return false;
                }
            }

            int ls = buffer.LineStart(line);
            int ll = buffer.LineLength(line);
            bool currentLine;
            if (column == -1)
            {
                currentLine = false;
                column = ll;
            }
            else
            {
                currentLine = true;
                mIsLineEmptyPriorToPosition = true;
                for (int i = 0; i < ll && i < column; i++)
                {
                    if (!char.IsWhiteSpace(buffer[ls + i]))
                    {
                        mIsLineEmptyPriorToPosition = false;
                        break;
                    }
                }
            }

            bool rc;
            rc = highlight.GetFirstRegion(line);
            int keywordStart = 0;
            int keywordLength = 0;
            bool prevKeyword = false;
            mPairCollection.reset();

            while (rc)
            {
                if (highlight.CurrentRegion.StartColumn < column)
                {
                    if (currentLine)
                    {
                        if (highlight.CurrentRegion.Is(mSymbol) && prevKeyword && highlight.CurrentRegion.Length == 1 && buffer[ls + highlight.CurrentRegion.StartColumn] == '=')
                        {
                            mAttributeAtThisLine = buffer.GetRange(ls + keywordStart, keywordLength);
                            mAttributeAtThisLineStart = keywordStart;
                        }

                        if (highlight.CurrentRegion.Is(mKeyword) && mAttributeAtThisLine == null && buffer[ls + highlight.CurrentRegion.StartColumn] == '@')
                        {
                            keywordStart = highlight.CurrentRegion.StartColumn;
                            keywordLength = highlight.CurrentRegion.Length;
                            prevKeyword = true;
                        }
                        else if (highlight.CurrentRegion.Is(mError) && highlight.CurrentRegion.StartColumn > 0 && buffer[ls + highlight.CurrentRegion.StartColumn - 1] == '@')
                        {
                            keywordStart = highlight.CurrentRegion.StartColumn - 1;
                            keywordLength = highlight.CurrentRegion.Length + 1;
                            prevKeyword = true;
                        }
                        else
                            prevKeyword = false;
                    }

                    if (highlight.CurrentRegion.Is(mPairStart))
                    {
                        mPairCollection.add(false, highlight.CurrentRegion.StartColumn, highlight.CurrentRegion.Length);
                    }

                    if (highlight.CurrentRegion.Is(mPairEnd))
                    {
                        mPairCollection.add(true, highlight.CurrentRegion.StartColumn, highlight.CurrentRegion.Length);
                    }
                }
                rc = highlight.GetNextRegion();
            }

            if (prevKeyword)
            {
                mAttributeAtThisLine = buffer.GetRange(ls + keywordStart, keywordLength);
                mAttributeAtThisLineStart = keywordStart;
            }


            for (int i = mPairCollection.Count; i >= 0; i--)
            {
                bool end;
                int start, length;
                if (mPairCollection.get(i, out end, out start, out length))
                {
                    if (!end && buffer[ls + start] == '@')
                    {
                        mTagDepth++;
                        if (mTagDepth == 1)
                        {
                            mLastOpenTag = buffer.GetRange(ls + start, length);
                            return true;
                        }
                    }
                    else if (!end && buffer[ls + start] == '[')
                    {
                        mPairDepth++;
                        if (mPairDepth == 1 && mLastOpenPair == null)
                            mLastOpenPair = buffer.GetRange(ls + start, length);
                    }

                    if (end && buffer[ls + start] == '@')
                    {
                        mTagDepth--;
                    }
                    else if (end && buffer[ls + start] == '[')
                    {
                        mPairDepth--;
                    }
                }
            }
            return false;
        }

        private void preReturnCollection(GenericCodeCompletionItemCollection coll, string preselect)
        {
            if (coll.Count > 0)
            {
                coll.Sort();
                if (preselect != null && preselect.Length > 0)
                {
                    for (int i = 0; i < coll.Count; i++)
                    {
                        if (coll[i].Name.IndexOf(preselect, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        {
                            coll._DefaultIndex = i;
                            coll._Preselection = preselect;
                            break;
                        }
                    }
                }
            }

        }

        internal ICodeCompletionItemCollection getCompletionForAt(string preselect)
        {
            GenericCodeCompletionItemCollection coll = new GenericCodeCompletionItemCollection();

            if (mIsLineEmptyPriorToPosition && mLastOpenTag != null)
            {
                Tag t = mTags[mLastOpenTag];
                if (t != null)
                {
                    coll.Add(new GenericCodeCompletionItem("end"));
                    for (int i = 0; i < t.Count; i++)
                        coll.Add(new GenericCodeCompletionItem(CodeCompletionItemType.Property, t[i].Name.Substring(1), t[i].Name.Substring(1)));
                    for (int i = 0; i < t.Children.Count; i++)
                        coll.Add(new GenericCodeCompletionItem(t.Children[i].Name.Substring(1)));
                }
            }
            else if (mIsLineEmptyPriorToPosition && mLastOpenTag == null)
            {
                Tag t = mTags["@root"];
                if (t != null)
                {
                    for (int i = 0; i < t.Children.Count; i++)
                        coll.Add(new GenericCodeCompletionItem(t.Children[i].Name.Substring(1)));
                }
            }

            preReturnCollection(coll, preselect);
            return coll;
        }

        internal ICodeCompletionItemCollection getCompletionForEq(string preselect, int position)
        {
            GenericCodeCompletionItemCollection coll = new GenericCodeCompletionItemCollection();

            if (mLastOpenTag != null && mAttributeAtThisLine != null && position == mAttributeAtThisLineStart + mAttributeAtThisLine.Length)
            {
                Tag t = mTags[mLastOpenTag];
                if (t != null)
                {
                    if ((t.Name == "@group" || t.Name == "@article" || t.Name == "@class") && mAttributeAtThisLine == "@ingroup")
                    {
                        AddKeysToCollection(mWindow, AddKeysMode.OnlyGroups, coll);
                    }
                    else if (t.Name == "@class" && mAttributeAtThisLine == "@import")
                    {
                        AddKeysToCollection(mWindow, AddKeysMode.OnlyClasses, coll);
                    }
                    else
                    {
                        for (int i = 0; i < t.Count; i++)
                        {
                            if (t[i].Name == mAttributeAtThisLine && t[i].Count > 0)
                                for (int j = 0; j < t[i].Count; j++)
                                    coll.Add(new GenericCodeCompletionItem(t[i][j]));
                        }
                    }
                }
            }
            else
            {
                int line, column, length;
                if (DsUtils.WordUnderCursor(mWindow, mWindow.CursorRow, position, false, out line, out column, out length))
                {
                    if (column > 1 && mWindow[mWindow.CursorRow, column - 1] == '[')
                    {
                        string w = mWindow.Text.GetRange(mWindow.Text.LineStart(line) + column, length);
                        if (w == "link" || w == "clink")
                            AddKeysToCollection(mWindow, AddKeysMode.All, coll);
                    }
                }
            }

            preReturnCollection(coll, preselect);
            return coll;
        }

        internal ICodeCompletionItemCollection getCompletionForBracket(string preselect)
        {
            GenericCodeCompletionItemCollection coll = new GenericCodeCompletionItemCollection();

            bool allowed = false;
            if (mLastOpenTag != null)
            {
                Tag t = mTags[mLastOpenTag];
                if (t != null)
                {
                    if (mAttributeAtThisLine != null)
                    {
                        for (int i = 0; i < t.Count; i++)
                        {
                            if (t[i].Name == mAttributeAtThisLine)
                            {
                                allowed = t[i].CanHaveBbCode;
                                break;
                            }
                        }
                    }
                    else
                    {
                        allowed = t.CanHaveText;
                    }
                }
            }

            if (allowed)
            {
                if (mLastOpenPair != null && mLastOpenPair[0] == '[')
                {
                    BbCode c = mBbCodes.FindTag(mLastOpenPair);
                    if (c != null && c.IsPaired)
                        coll.Add(new GenericCodeCompletionItem(c.CloseTag));
                }
                for (int i = 0; i < mBbCodes.Count; i++)
                    coll.Add(new GenericCodeCompletionItem(mBbCodes[i].OpenTag));
            }

            preReturnCollection(coll, preselect);
            return coll;
        }

        private enum AddKeysMode
        {
            OnlyGroups,
            OnlyClasses,
            All,
        }

        private void AddKeysToCollection(TextWindow window, AddKeysMode mode, GenericCodeCompletionItemCollection coll)
        {
            DsProjectInfo pi = window[DocSourceIntellisenseProvider.PRJ_NAME] as DsProjectInfo;
            if (pi == null)
            {
                return ;
            }

            lock (pi.Mutex)
            {
                DocItem _root = pi.ParserRoot;
                if (_root is RootItem)
                {
                    RootItem root = _root as RootItem;
                    RootItemContent content = root.Content;
                    GenericProjectBrowserItemCollection<object> items = new GenericProjectBrowserItemCollection<object>();
                    foreach (DocItem item in content)
                    {
                        if (item is ArticleItem && mode == AddKeysMode.All)
                        {
                            ArticleItem article = item as ArticleItem;
                            coll.Add(new GenericCodeCompletionItem(article.Key));
                        }
                        else if (item is GroupItem && (mode == AddKeysMode.OnlyGroups || mode == AddKeysMode.All))
                        {
                            GroupItem group = item as GroupItem;
                            coll.Add(new GenericCodeCompletionItem(group.Key));
                        }
                        else if (item is ClassItem && (mode == AddKeysMode.All || mode == AddKeysMode.OnlyClasses))
                        {
                            ClassItem cls = item as ClassItem;
                            coll.Add(new GenericCodeCompletionItem(cls.Key));
                            if (mode == AddKeysMode.All)
                            {
                                MemberList mems = cls.Members;
                                foreach (MemberItem mem in mems)
                                {
                                    string key1 = cls.Key + "." + mem.Key;
                                    coll.Add(new GenericCodeCompletionItem(key1));
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}

