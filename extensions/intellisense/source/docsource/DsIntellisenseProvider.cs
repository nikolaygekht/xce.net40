using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using gehtsoft.intellisense.cs;
using gehtsoft.xce.colorer;
using gehtsoft.xce.text;
using gehtsoft.xce.configuration;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.intellisense.common;
using GehtSoft.DocCreator.Parser;

namespace gehtsoft.intellisense.docsource
{
    internal class DocSourceIntellisenseProvider : IIntellisenseProvider
    {
        DocSourceFileAnalyzer mAnalyzer;
        SyntaxRegion mPairEnd;
        bool mEnabled;
        int mTimeout;
        DsProjectInfoCollection mProjects = new DsProjectInfoCollection();
        internal const string PRJ_NAME = "dsintellisense-project";
        internal const string FILE_NAME = "dsintellisense-file";
        string mLogPath;

        public bool Initialize(Application app)
        {
            mLogPath = Path.Combine(app.ApplicationPath, "dsintellisense.log");

            ProfileSection config = app.Configuration["dsintellisense"];
            if (config == null)
                mEnabled = false;
            else
            {
                mEnabled = config["enabled", "false"].Trim() == "true";
                string t = config["parser-timeout", "1000"].Trim();
                if (!Int32.TryParse(t, out mTimeout))
                    mTimeout = 2000;
                if (mTimeout < 250)
                    mTimeout = 250;

            }

            if (!mEnabled)
                return false;

            SyntaxRegion pairStart = app.ColorerFactory.FindSyntaxRegion("def:PairStart");
            SyntaxRegion pairEnd = app.ColorerFactory.FindSyntaxRegion("def:PairEnd");
            SyntaxRegion keyword = app.ColorerFactory.FindSyntaxRegion("ds:keyword");
            SyntaxRegion symbol = app.ColorerFactory.FindSyntaxRegion("ds:sym");
            SyntaxRegion error = app.ColorerFactory.FindSyntaxRegion("ds:error");
            if (keyword != null && symbol != null && error != null)
            {
                mPairEnd = pairEnd;
                mAnalyzer = new DocSourceFileAnalyzer(pairStart, pairEnd, keyword, symbol, error);
                mEnabled = true;
                return true;
            }
            else
            {
                mEnabled = false;
                return false;
            }
        }

        public bool AcceptFile(Application application, TextWindow window)
        {
            FileInfo fi = new FileInfo(window.Text.FileName);
            return (string.Compare(fi.Extension, ".ds", true) == 0) || (string.Compare(fi.Extension, ".dsi", true) == 0);
        }

        public bool Start(Application application, TextWindow window)
        {
            for (int i = 0; i < mProjects.Count; i++)
            {
                if (mProjects[i] != null && mProjects[i].Project != null)
                {
                    if (mProjects[i].Project.Sources.Contains(window.Text.FileName))
                    {
                        DsFileSource src = new DsFileSource(window);
                        mProjects[i].Sources.Add(src);
                        window[PRJ_NAME] = mProjects[i];
                        window[FILE_NAME] = src;
                        return true;
                    }
                }
            }

            DsProject p = FindProject(window.Text.FileName);
            DsProjectInfo pi = new DsProjectInfo(p, mTimeout, mLogPath);
            DsFileSource src1 = new DsFileSource(window);
            pi.Sources.Add(src1);
            mProjects.Add(pi);
            window[PRJ_NAME] = pi;
            window[FILE_NAME] = src1;
            pi.Start();
            return true;
        }

        public void Stop(Application application, TextWindow window)
        {
            DsProjectInfo pi = window[PRJ_NAME] as DsProjectInfo;
            DsFileSource src = window[FILE_NAME] as DsFileSource;

            if (pi != null && src != null)
            {
                lock (pi.Mutex)
                {
                    if (pi.Sources.Contains(src))
                        pi.Sources.Remove(src);
                    if (pi.Sources.Count == 0)
                    {
                        pi.Stop();
                        if (mProjects.Contains(pi))
                            mProjects.Remove(pi);
                    }
                }
            }

            if (src != null)
                src.Dispose();

            window[PRJ_NAME] = null;
            window[FILE_NAME] = null;
        }

        private DsProject FindProject(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            return FindProject(fileName, fi.Directory.FullName);
        }

        private void log(string format, params object[] args)
        {
            StreamWriter w = null;
            try
            {
                w = new StreamWriter(mLogPath, true);
                w.WriteLine("{0} {1}", DateTime.Now.ToString("u"), string.Format(format, args));
                w.Close();
                w = null;
            }
            catch (Exception )
            {
            }
            finally
            {
                if (w != null)
                    w.Close();
                w = null;
            }
        }


        private DsProject FindProject(string fileName, string dir)
        {
            string ini = Path.Combine(dir, "xce-project.ini");
            if (File.Exists(ini))
            {
                try
                {
                    Profile profile = new Profile();
                    profile.Load(ini);
                    if (profile["project", "doc-project"] != null)
                    {
                        string doc = profile["project", "doc-project"];
                        doc = Path.Combine(dir, doc);
                        FileInfo fi = new FileInfo(doc);
                        if (fi.Exists)
                        {
                            DsProject p = new DsProject();
                            p.LoadProject(fi.FullName);
                            if (p.Sources.Contains(fileName))
                                return p;
                        }
                    }
                }
                catch (Exception e)
                {
                    log ("Exception during find the project {0}", e.ToString());
                }
            }
            DirectoryInfo di = new DirectoryInfo(dir);
            if (di.Parent != null)
                return FindProject(fileName, di.Parent.FullName);
            else
                return null;
        }

        public bool CanGetCodeCompletionCollection(Application application, TextWindow window)
        {
            if (application.ActiveWindow == null)
                return false;
            return true;
        }

        public ICodeCompletionItemCollection GetCodeCompletionCollection(Application application, TextWindow window, out int wline, out int wcolumn, out int wlength)
        {
            ICodeCompletionItemCollection coll = null;

            wline = wcolumn = wlength = 0;

            if (window.CursorColumn == 0)       //cannot suggest at the beginning of the line...
                return null;

            if (!DsUtils.WordUnderCursor(window, out wline, out wcolumn, out wlength))
            {
                wline = window.CursorRow;
                wcolumn = window.CursorColumn;
                wlength = 0;
            }

            if (wline >= window.Text.LinesCount)
                return null;

            int ls = window.Text.LineStart(wline);
            int ll = window.Text.LineLength(wline);

            if (wcolumn > ll || wcolumn == 0)
                return null;

            char c = window.Text[ls + wcolumn - 1];
            if (c != '@' && c != '=' && c != '[')
            {
                if (c == '/' && wcolumn > 1 && window.Text[wcolumn - 2] == '[')
                {
                    wcolumn = wcolumn - 1;
                    wlength = wlength + 1;
                }
                else
                    return null;
            }

            string preSel = "";
            if (wlength > 0)
                preSel = window.Text.GetRange(ls + wcolumn, wlength);

            lock (mAnalyzer.Mutex)
            {
                mAnalyzer.reset();
                mAnalyzer.analyze(window, window.CursorRow, wcolumn - 1);
                switch (c)
                {
                    case '@':
                        coll = mAnalyzer.getCompletionForAt(preSel);
                        break;
                    case '=':
                        coll = mAnalyzer.getCompletionForEq(preSel, wcolumn - 1);
                        break;
                    case '[':
                        coll = mAnalyzer.getCompletionForBracket(preSel);
                        break;
                }
            }
            return coll;
        }

        public bool IsShowOnTheFlyDataCharacter(char character)
        {
            return character == '@' || character == '[' || character == '=';
        }

        public bool IsHideOnTheFlyDataCharacter(char character)
        {
            return (char.IsPunctuation(character) && character != '/' && character != '.' && character != '-' && character != '_') || char.IsWhiteSpace(character) || character == '=';
        }

        public bool ForwardEnterAtOneTheFlyEnd()
        {
            return true;
        }

        public bool IsOnTheFlyDataCharacter(char character)
        {
            return !IsHideOnTheFlyDataCharacter(character);
        }

        public bool CanGetOnTheFlyCompletionData(Application application, TextWindow window)
        {
            return CanGetCodeCompletionCollection(application, window);
        }

        public ICodeCompletionItemCollection GetOnTheFlyCompletionData(Application application, TextWindow window, char pressed)
        {
            ICodeCompletionItemCollection coll = null;
            lock (mAnalyzer.Mutex)
            {
                mAnalyzer.reset();
                mAnalyzer.analyze(window, window.CursorRow, window.CursorColumn);
                switch (pressed)
                {
                case    '@':
                        coll = mAnalyzer.getCompletionForAt(null);
                        break;
                case    '=':
                        coll = mAnalyzer.getCompletionForEq(null, window.CursorColumn);
                        break;
                case    '[':
                        coll = mAnalyzer.getCompletionForBracket(null);
                        break;
                }
            }
            return coll;
        }

        public void PostOnTheFlyText(Application application, TextWindow w, int startColumn, int length, ICodeCompletionItem item)
        {
            SyntaxHighlighter highlighter = w.Highlighter;
            XceFileBuffer b = w.Text;
            if (w.CursorRow < b.LinesCount)
            {
                int ls = b.LineStart(w.CursorRow);
                bool rc, found = false;
                rc = highlighter.GetFirstRegion(w.CursorRow);
                while (rc && !found)
                {
                    if (highlighter.CurrentRegion.Is(mPairEnd) && highlighter.CurrentRegion.Length == length + 1 &&
                        highlighter.CurrentRegion.StartColumn == startColumn - 1 &&
                        b[ls + highlighter.CurrentRegion.StartColumn] == '@')
                    {
                        found = true;
                    }
                    rc = highlighter.GetNextRegion();
                }

                if (found)
                {
                    SyntaxHighlighterPair pair = highlighter.MatchPair(w.CursorRow, startColumn);
                    if (pair.Start.Line < pair.End.Line)
                    {
                        if (pair.Start.StartColumn < pair.End.StartColumn)
                        {
                            b.DeleteRange(ls + pair.Start.StartColumn, pair.End.StartColumn - pair.Start.StartColumn);
                        }
                        else if (pair.Start.StartColumn > pair.End.StartColumn)
                        {
                            b.InsertToLine(w.CursorRow, startColumn - 1, ' ', pair.Start.StartColumn - pair.End.StartColumn);
                        }
                        w.CursorColumn = pair.Start.StartColumn + pair.End.Length;
                    }
                    else
                    {
                        if (pair.End.StartColumn < pair.Start.StartColumn)
                        {
                            b.DeleteRange(ls + pair.End.StartColumn, pair.Start.StartColumn - pair.End.StartColumn);
                        }
                        else if (pair.End.StartColumn > pair.Start.StartColumn)
                        {
                            b.InsertToLine(w.CursorRow, startColumn - 1, ' ', pair.End.StartColumn - pair.Start.StartColumn);
                        }
                        w.CursorColumn = pair.End.StartColumn + pair.Start.Length;
                    }
                }
            }
        }

        public bool IsShowInsightDataCharacter(char character)
        {
            return false;
        }

        public bool CanGetInsightDataProvider(Application application, TextWindow window)
        {
            return false;
        }

        public IInsightDataProvider GetInsightDataProvider(Application application, TextWindow window, char pressed)
        {
            return null;
        }

        public bool CanGetProjectBrowserItemCollection(Application application, TextWindow window)
        {
            return true;
        }

        private IProjectBrowserItemCollection PrepareProjectBrowserItemCollection(Application application, TextWindow window, out IProjectBrowserItem curSel, string key)
        {
            DsProjectInfo pi = window[PRJ_NAME] as DsProjectInfo;
            if (pi == null)
            {
                curSel = null;
                return null;
            }

            lock (pi.Mutex)
            {
                DocItem _root = pi.ParserRoot;
                if (_root is RootItem)
                {
                    curSel = null;
                    RootItem root = _root as RootItem;
                    RootItemContent content = root.Content;
                    GenericProjectBrowserItemCollection<object> items = new GenericProjectBrowserItemCollection<object>();
                    foreach (DocItem item in content)
                    {
                        if (item is ArticleItem)
                        {
                            ArticleItem article = item as ArticleItem;
                            GenericProjectBrowserItem<object> cur;
                            items.Add(cur = new GenericProjectBrowserItem<object>(ProjectBrowserItemType.Text, article.Key, item.File, item.Line - 1, 0, null));

                            if (key != null)
                            {
                                if (article.Key == key)
                                    curSel = cur;
                            }
                            else
                            if (window.Text.FileName.Equals(item.File, StringComparison.CurrentCultureIgnoreCase) &&
                                window.CursorRow >= item.Line - 1 && window.CursorRow <= item.EndLine - 1)
                            {
                                curSel = cur;
                            }
                        }
                        else if (item is GroupItem)
                        {
                            GroupItem group = item as GroupItem;
                            GenericProjectBrowserItem<object> cur;
                            items.Add(cur = new GenericProjectBrowserItem<object>(ProjectBrowserItemType.Text, group.Key, item.File, item.Line - 1, 0, null));
                            if (key != null)
                            {
                                if (group.Key == key)
                                    curSel = cur;
                            }
                            else
                            if (window.Text.FileName.Equals(item.File, StringComparison.CurrentCultureIgnoreCase) &&
                                window.CursorRow >= item.Line - 1 && window.CursorRow <= item.EndLine - 1)
                            {
                                curSel = cur;
                            }
                        }
                        else if (item is ClassItem)
                        {
                            ClassItem cls = item as ClassItem;
                            GenericProjectBrowserItem<object> cur;
                            items.Add(cur = new GenericProjectBrowserItem<object>(ProjectBrowserItemType.Class, cls.Key, item.File, item.Line - 1, 0, null));
                            if (key != null)
                            {
                                if (cls.Key == key)
                                    curSel = cur;
                            }
                            else
                            if (window.Text.FileName.Equals(item.File, StringComparison.CurrentCultureIgnoreCase) &&
                                window.CursorRow >= item.Line - 1 && window.CursorRow <= item.EndLine - 1)
                            {
                                curSel = cur;
                            }
                            MemberList mems = cls.Members;
                            foreach (MemberItem mem in mems)
                            {
                                string key1 = cls.Key + "." + mem.Key;
                                items.Add(cur = new GenericProjectBrowserItem<object>(ProjectBrowserItemType.Method, key1, mem.File, mem.Line - 1, 0, null));
                                if (key != null)
                                {
                                    if (key1 == key)
                                        curSel = cur;
                                }
                                else
                                if (window.Text.FileName.Equals(item.File, StringComparison.CurrentCultureIgnoreCase) &&
                                    window.CursorRow >= mem.Line - 1 && window.CursorRow <= mem.EndLine - 1)
                                {
                                    curSel = cur;
                                }
                            }
                        }
                    }
                    items.Sort();
                    return items;
                }
                else
                {
                    curSel = null;
                    return null;
                }
            }
        }

        public IProjectBrowserItemCollection GetProjectBrowserItemCollection(Application application, TextWindow window, out IProjectBrowserItem curSel)
        {
            return PrepareProjectBrowserItemCollection(application, window, out curSel, null);
        }

        public IProjectBrowserItemCollection FindProjectBrowserItem(Application application, TextWindow window, out IProjectBrowserItem curSel)
        {
            int line, col, length;
            string word;
            DsUtils.WordUnderCursor(window, out line, out col, out length, out word);
            if (length < 1)
                word = null;

            return PrepareProjectBrowserItemCollection(application, window, out curSel, word);
        }
    }
}

