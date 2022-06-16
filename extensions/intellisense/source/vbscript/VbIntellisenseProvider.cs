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

namespace gehtsoft.intellisense.vbscript
{
    internal class VbIntellisenseProvider : IIntellisenseProvider
    {
        private bool mEnabled;
        private string mTypes;
        private Application mApplication;
        VbKeywordCollection mKeywords;
        VbPropertyDictionary mProperties;
        VbSourceFileAnalyzer mAnalyzer;

        public bool Initialize(Application app)
        {
            mApplication = app;
            ProfileSection config = app.Configuration["vbintellisense"];

            if (config == null)
                mEnabled = false;
            else
            {
                mEnabled = config["enabled", "false"].Trim() == "true";
                mTypes = config["types", "standart"].Trim();
            }

            SyntaxRegion outline = app.ColorerFactory.FindSyntaxRegion("def:Outlined");
            SyntaxRegion comment = app.ColorerFactory.FindSyntaxRegion("def:Comment");
            SyntaxRegion str = app.ColorerFactory.FindSyntaxRegion("def:String");
            SyntaxRegion pair = app.ColorerFactory.FindSyntaxRegion("def:PairStart");

            if (mEnabled)
            {
                mAnalyzer = new VbSourceFileAnalyzer(pair, comment, str, outline);
                mKeywords = TypeFactory.CreateKeywords(mTypes);
                mProperties = TypeFactory.CreateProperties(mTypes);
            }

            return mEnabled;
        }

        public bool AcceptFile(Application application, TextWindow window)
        {
            FileInfo fi = new FileInfo(window.Text.FileName);
            return (string.Compare(fi.Extension, ".vbs", true) == 0);
        }

        public bool Start(Application application, TextWindow window)
        {
             return true;
        }

        public void Stop(Application application, TextWindow window)
        {
        }

        public bool CanGetCodeCompletionCollection(Application application, TextWindow window)
        {
            if (application.ActiveWindow == null)
                return false;
            return true;
        }

        private void AddKeywords(GenericCodeCompletionItemCollection coll)
        {
            bool add;
            int i, j;
            for (i = 0; i < mKeywords.Count; i++)
            {
                VbKeyword keyword = mKeywords[i];
                add = true;
                if (keyword.After != null)
                {
                    if (mAnalyzer.PreviousWord == null || string.Compare(keyword.After, mAnalyzer.PreviousWord, true) != 0)
                        add = false;
                }

                if (add)
                {
                    switch (keyword.Context)
                    {
                    case    VbKeywordContext.Expr:
                            add = mAnalyzer.InExpr;
                            break;
                    case    VbKeywordContext.NotExpr:
                            add = !mAnalyzer.InExpr;
                            break;
                    case    VbKeywordContext.Bol:
                            add = mAnalyzer.AtBeginOfLine;
                            break;
                    case    VbKeywordContext.Inside:
                            {
                                add = false;
                                if (mAnalyzer.InExpr)
                                    break;
                                for (j = 0; j < keyword.Count && !add; j++)
                                    add = mAnalyzer.IsOnTopPair(keyword[j]);
                            }
                            break;
                    case    VbKeywordContext.InsideAndBol:
                            {
                                add = mAnalyzer.AtBeginOfLine;
                                if (add)
                                {
                                    add = false;
                                    for (j = 0; j < keyword.Count && !add; j++)
                                        add = mAnalyzer.IsOnTopPair(keyword[j]);
                                }
                            }
                            break;
                    case    VbKeywordContext.InsideAnywhere:
                            {
                                add = false;
                                if (mAnalyzer.InExpr)
                                    break;
                                for (j = 0; j < keyword.Count && !add; j++)
                                    add = mAnalyzer.HasPair(keyword[j]);
                            }
                            break;
                    case    VbKeywordContext.InsideAnywhereAndBol:
                            {
                                add = mAnalyzer.AtBeginOfLine;
                                if (add)
                                {
                                    add = false;
                                    for (j = 0; j < keyword.Count && !add; j++)
                                        add = mAnalyzer.HasPair(keyword[j]);
                                }
                            }
                            break;
                    }
                }

                if (add)
                    coll.Add(new GenericCodeCompletionItem(keyword.Name));
            }
        }

        private void AddGlobalProperties(GenericCodeCompletionItemCollection coll)
        {
            for (int i = 0; i < mProperties.Count; i++)
            {
                if (mProperties[i].Table == null)
                {
                    if (mProperties[i].IsFunction)
                        coll.Add(new GenericCodeCompletionItem(CodeCompletionItemType.Method, mProperties[i].Name, mProperties[i].Name));
                    else
                        coll.Add(new GenericCodeCompletionItem(CodeCompletionItemType.Property, mProperties[i].Name, mProperties[i].Name));
                }
            }
        }

        internal class MemorizeSuggestion
        {
            internal string Table;
            internal string Key;

            internal MemorizeSuggestion(string table, string key)
            {
                Table = table;
                Key = key;
            }
        }

        private void AddAllProperties(GenericCodeCompletionItemCollection coll, string key)
        {
            AddProperties(coll, null, key);
        }

        private void AddProperties(GenericCodeCompletionItemCollection coll, string table, string key)
        {
            for (int i = 0; i < mProperties.Count; i++)
            {
                if (mProperties[i].Table != null)
                {
                    if (table == null || string.Compare(mProperties[i].Table, table, true) == 0)
                    {
                        MemorizeSuggestion s = null;
                        if (key != null)
                            s = new MemorizeSuggestion(mProperties[i].Table, key);
                        coll.Add(new GenericCodeCompletionItem(mProperties[i].IsFunction ? CodeCompletionItemType.Method : CodeCompletionItemType.Property, mProperties[i].Name + " in " + mProperties[i].Table, mProperties[i].Name, s));
                    }
                }
            }
        }

        private const string VBDICT = "vbscriptintillesensedict";

        public ICodeCompletionItemCollection GetCodeCompletionCollection(Application application, TextWindow window, out int wline, out int wcolumn, out int wlength)
        {
            GenericCodeCompletionItemCollection coll = new GenericCodeCompletionItemCollection();

            wcolumn = wline = wlength = 0;

            lock (mAnalyzer.Mutex)
            {
                mAnalyzer.reset();
                mAnalyzer.analyze(window, window.CursorRow, window.CursorColumn, true);
                AddKeywords(coll);
                if (mAnalyzer.InExpr && (mAnalyzer.WordDivisor == ' ' || mAnalyzer.PreviousWord == null))
                    AddGlobalProperties(coll);
                else if (mAnalyzer.WordDivisor == '.' && mAnalyzer.PreviousWord != null)
                {
                    if (mProperties.IsPredefinedName(mAnalyzer.PreviousWord))
                        AddProperties(coll, mAnalyzer.PreviousWord, null);
                    else
                    {
                        string key = (mAnalyzer.CurrentOutline + "." + mAnalyzer.PreviousWord).ToUpper();
                        Dictionary<string, string> d = window[VBDICT] as Dictionary<string, string>;
                        if (d == null)
                            AddAllProperties(coll, key);
                        else
                        {
                            string table;

                            if (!d.TryGetValue(key, out table))
                                table = null;
                            if (table == null)
                                AddAllProperties(coll, key);
                            else
                                AddProperties(coll, table, key);
                        }
                    }
                }

                if (mAnalyzer.CurrentWord != null)
                {
                    coll._Preselection = mAnalyzer.CurrentWord;
                    wline = window.CursorRow;
                    wcolumn = mAnalyzer.CurrentWordColumn;
                    wlength = mAnalyzer.CurrentWordLength;
                }

            }
            if (coll.Count > 0)
            {
                coll.Sort();
            }
            return coll;
        }

        public bool IsShowOnTheFlyDataCharacter(char character)
        {
            return character == '.';
        }

        public bool IsHideOnTheFlyDataCharacter(char character)
        {
            return (character == ' ' || character == ',' || character == '.' || character == ':' || character == ')' || character == '(' || character == ';' || character == '=' || character == '-' || character == '+' || character == '*' || character == '/');
        }

        public bool ForwardEnterAtOneTheFlyEnd()
        {
            return false;
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
            GenericCodeCompletionItemCollection coll = new GenericCodeCompletionItemCollection();

            lock (mAnalyzer.Mutex)
            {
                mAnalyzer.reset();
                mAnalyzer.analyze(window, window.CursorRow, window.CursorColumn, false);
                if (mAnalyzer.CurrentWord != null)
                {
                    if (mProperties.IsPredefinedName(mAnalyzer.CurrentWord))
                        AddProperties(coll, mAnalyzer.CurrentWord, null);
                    else
                    {
                        string key = (mAnalyzer.CurrentOutline + "." + mAnalyzer.CurrentWord).ToUpper();
                        Dictionary<string, string> d = window[VBDICT] as Dictionary<string, string>;
                        if (d == null)
                            AddAllProperties(coll, key);
                        else
                        {
                            string table;

                            if (!d.TryGetValue(key, out table))
                                table = null;
                            if (table == null)
                                AddAllProperties(coll, key);
                            else
                                AddProperties(coll, table, key);
                        }
                    }
                }
            }
            if (coll.Count > 0)
            {
                coll.Sort();
            }
            return coll;
        }

        public void PostOnTheFlyText(Application application, TextWindow w, int startColumn, int length, ICodeCompletionItem _item)
        {
            GenericCodeCompletionItem item = _item as GenericCodeCompletionItem;
            if (item != null)
            {
                MemorizeSuggestion s = item.UserData as MemorizeSuggestion;
                if (s != null)
                {
                    Dictionary<string, string> d = w[VBDICT] as Dictionary<string, string>;
                    if (d == null)
                    {
                        d = new Dictionary<string,string>();
                        w[VBDICT] = d;
                    }
                    d[s.Key] = s.Table;
                }
            }
        }

        public bool IsShowInsightDataCharacter(char character)
        {
            return character == '(' || character == ' ';
        }

        public bool CanGetInsightDataProvider(Application application, TextWindow window)
        {
            return true;
        }

        public IInsightDataProvider GetInsightDataProvider(Application application, TextWindow window, char pressed)
        {
            VbInsightDataProvider coll = new VbInsightDataProvider(window, window.CursorRow, window.CursorColumn);
            lock (mAnalyzer.Mutex)
            {
                mAnalyzer.reset();
                mAnalyzer.analyze(window, window.CursorRow, window.CursorColumn, false);
                if (mAnalyzer.CurrentWord == null)
                    return null;
                if (mAnalyzer.WordDivisor == ' ')
                {
                    //add globals
                    for (int i = 0; i < mProperties.Count; i++)
                    {
                        if (mProperties[i].Table == null &&
                            mProperties[i].IsFunction &&
                            string.Compare(mProperties[i].Name, mAnalyzer.CurrentWord, true) == 0)
                        {
                            coll.Add(mProperties[i]);
                        }
                    }
                }
                else if (mAnalyzer.WordDivisor == '.' && mAnalyzer.PreviousWord != null)
                {
                    //add props
                    Dictionary<string, string> d = window[VBDICT] as Dictionary<string, string>;
                    string key = mAnalyzer.CurrentOutline + "." + mAnalyzer.PreviousWord;
                    string table = null;
                    if (mProperties.IsPredefinedName(mAnalyzer.PreviousWord))
                        table = mAnalyzer.PreviousWord;
                    else
                    {
                        if (d != null)
                            if (!d.TryGetValue(key, out table))
                                table = null;
                    }
                    for (int i = 0; i < mProperties.Count; i++)
                    {
                        if (mProperties[i].Table != null &&
                            (table == null || string.Compare(table, mProperties[i].Table, true) == 0) &&
                            mProperties[i].IsFunction &&
                            string.Compare(mProperties[i].Name, mAnalyzer.CurrentWord, true) == 0)
                            coll.Add(mProperties[i]);
                    }
                }
            }
            if (coll.Count > 1)
                coll.Sort();
            return coll;
        }

        public bool CanGetProjectBrowserItemCollection(Application application, TextWindow window)
        {
            return true;
        }

        public IProjectBrowserItemCollection GetProjectBrowserItemCollection(Application application, TextWindow window, out IProjectBrowserItem curSel)
        {
            curSel = null;
            return null;
        }

        public IProjectBrowserItemCollection FindProjectBrowserItem(Application application, TextWindow window, out IProjectBrowserItem curSel)
        {
            curSel = null;
            return null;
        }


    }
}

