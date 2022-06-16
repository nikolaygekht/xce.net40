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

namespace gehtsoft.intellisense.lua
{
    internal class LuaIntellisenseProvider : IIntellisenseProvider
    {
        private bool mEnabled;
        private string mTypes;
        private Application mApplication;
        SyntaxRegion mOutline;
        SyntaxRegion mComment;
        SyntaxRegion mString;
        SyntaxRegion mError;

        LuaKeywordCollection mKeywords;
        LuaPropertyDictionary mProperties;

        public bool Initialize(Application app)
        {
            mApplication = app;
            ProfileSection config = app.Configuration["luaintellisense"];

            if (config == null)
                mEnabled = false;
            else
            {
                mEnabled = config["enabled", "false"].Trim() == "true";
                mTypes = config["types", "standart"].Trim();
            }

            mOutline = app.ColorerFactory.FindSyntaxRegion("lua:Outline");
            mComment = app.ColorerFactory.FindSyntaxRegion("def:Comment");
            mString = app.ColorerFactory.FindSyntaxRegion("def:String");
            mError = app.ColorerFactory.FindSyntaxRegion("def:Error");

            if (mEnabled)
            {
                mKeywords = TypeFactory.CreateKeywords(mTypes);
                mProperties = TypeFactory.CreateProperties(mTypes);
            }

            return mEnabled;
        }

        public bool AcceptFile(Application application, TextWindow window)
        {
            FileInfo fi = new FileInfo(window.Text.FileName);
            return (string.Compare(fi.Extension, ".lua", true) == 0);
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

        internal class Context
        {
            internal int line = 0;
            internal string currentWord = null;
            internal int currentWordColumn = 0;
            internal int currentWordLength = 0;
            internal char divisor = ' ';
            internal int previousWordColumn = 0;
            internal int previousWordLength = 0;
            internal string previousWord = null;
            internal char previousWordDivisor = ' ';
        }

        private Context GetContext(TextWindow window, bool allowExtent)
        {
            int wline, wcolumn, wlength;
            int line, column;
            string word;
            Context context = new Context();

            XceFileBuffer b = window.Text;
            context.line = line = window.CursorRow;
            column = window.CursorColumn;

            if (line >= b.LinesCount)
                return context;

            SyntaxHighlighter h = window.Highlighter;
            bool rc;
            bool prevString = false;
            bool noContext = false;
            rc = h.GetFirstRegion(line);
            while (rc && !noContext)
            {
                if (h.CurrentRegion.StartColumn <= column && (h.CurrentRegion.Length == -1 || column <= h.CurrentRegion.EndColumn) &&
                    (h.CurrentRegion.Is(mString) || h.CurrentRegion.Is(mComment) || (h.CurrentRegion.Is(mError) && prevString)))
                    noContext = true;

                prevString = h.CurrentRegion.Is(mString);
                rc = h.GetNextRegion();
            }

            if (noContext)
                return null;

            int ls = b.LineStart(line);
            int ll = b.LineLength(line);

            if (column > ll)
                return context;

            if (CommonUtils.WordUnderCursor(window, line, column, allowExtent, out wline, out wcolumn, out wlength, out word))
            {
                context.currentWord = word;
                context.currentWordColumn = wcolumn;
                context.currentWordLength = wlength;
                if (wcolumn > 0 && (b[ls + wcolumn - 1] == '.' || b[ls + wcolumn - 1] == ':'))
                    context.divisor = b[ls + wcolumn - 1];
            }
            else
            {
                if (column <= ll && column > 0 && (b[ls + column - 1] == '.' || b[ls + column - 1] == ':'))
                {
                    context.currentWordColumn = column;
                    context.currentWordLength = 0;
                    context.divisor = b[ls + column - 1];
                }
            }

            if (context.divisor != ' ')
            {
                if (CommonUtils.WordUnderCursor(window, line, context.currentWordColumn - 2, false, out wline, out wcolumn, out wlength, out word))
                {
                    context.previousWord = word;
                    context.previousWordColumn = wcolumn;
                    context.previousWordLength = wlength;
                    if (wcolumn > 0 && (b[ls + wcolumn - 1] == '.' || b[ls + wcolumn - 1] == ':'))
                        context.previousWordDivisor = b[ls + wcolumn - 1];
                }
            }
            return context;
        }

        private void AddKeywords(GenericCodeCompletionItemCollection coll)
        {
            for (int i = 0; i < mKeywords.Count; i++)
            {
                if (mKeywords[i].IsTable)
                    coll.Add(new GenericCodeCompletionItem(CodeCompletionItemType.Class, mKeywords[i].Name, mKeywords[i].Name));
                else
                    coll.Add(new GenericCodeCompletionItem(mKeywords[i].Name));
            }
            LuaPropertyCollection props = null;
            props = mProperties.FindByTable("_G");
            char div = '.';

            for (int i = 0; i < props.Count; i++)
            {
                LuaProperty p = props[i];
                if (p.Static)
                {
                    string name;
                    if (p.IsFunction)
                        name = string.Format("{2}{3} in {0}", p.Table, div, p.Name, p.Args);
                    else
                        name = string.Format("{2} in {0}", p.Table, div, p.Name);

                    coll.Add(new GenericCodeCompletionItem(p.IsFunction ? CodeCompletionItemType.Method : CodeCompletionItemType.Property, name, p.Name));
                }
            }

        }

        private void AddProperties(GenericCodeCompletionItemCollection coll, string table, bool staticMethods)
        {
            LuaPropertyCollection props = null;
            bool includeG = true;
            if (table != null)
                props = mProperties.FindByTable(table);
            if (props == null)
            {
                includeG = false;
                props = mProperties;
            }

            char div = staticMethods ? '.' : ':';

            for (int i = 0; i < props.Count; i++)
            {
                LuaProperty p = props[i];
                if (p.Table == "_G" && !includeG)
                    continue;
                if (staticMethods == p.Static)
                {
                    string name;
                    if (p.IsFunction)
                        name = string.Format("{2}{3} in {0}", p.Table, div, p.Name, p.Args);
                    else
                        name = string.Format("{2} in {0}", p.Table, div, p.Name);

                    coll.Add(new GenericCodeCompletionItem(p.IsFunction ? CodeCompletionItemType.Method : CodeCompletionItemType.Property, name, p.Name));
                }
            }
        }

        public ICodeCompletionItemCollection GetCodeCompletionCollection(Application application, TextWindow window, out int wline, out int wcolumn, out int wlength)
        {
            GenericCodeCompletionItemCollection coll = new GenericCodeCompletionItemCollection();
            wline = wcolumn = wlength = 0;

            Context context = GetContext(window, true);

            if (context == null)
                return null;

            if (context.previousWord == null)
            {
                //    _ or xxx_
                AddKeywords(coll);
            }
            else if (context.previousWord != null && context.divisor == '.' && context.previousWordDivisor == ' ')
            {
                //   xxx._ or xxx.xxx_
                AddProperties(coll, context.previousWord, true);
            }
            else if (context.previousWord != null && context.divisor == '.' && context.previousWordDivisor != ' ')
            {
                //   xxx.xxx._ or xxx.xxx.xxx._
                AddProperties(coll, null, true);
            }
            else if (context.previousWord != null && context.divisor == ':')
            {
                //   xxx:_ or xxx:xxx_
                AddProperties(coll, null, false);
            }

            if (coll.Count > 0)
            {
                coll.Sort();
                if (context.currentWord != null && context.currentWord.Length > 0)
                {
                    for (int i = 0; i < coll.Count; i++)
                    {
                        if (coll[i].Text.StartsWith(context.currentWord, StringComparison.InvariantCulture))
                        {
                            coll._DefaultIndex = i;
                            coll._Preselection = context.currentWord;
                        }
                    }
                    wline = context.line;
                    wcolumn = context.currentWordColumn;
                    wlength = context.currentWordLength;
                }
            }
            return coll;
        }

        public bool IsShowOnTheFlyDataCharacter(char character)
        {
            return character == ':' || character == '.';
        }

        public bool IsHideOnTheFlyDataCharacter(char character)
        {
            return (character == ' ' || character == ',' || character == '.' || character == ':' || character == ')' || character == '(' || character == ';');
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

            Context context = GetContext(window, false);
            if (context == null)
                return null;

            if (context.currentWord != null && context.previousWord == null)
            {
                //   xxx
                AddProperties(coll, context.currentWord, pressed == '.');
            }
            else if (context.currentWord != null && context.previousWord != null)
            {
                //   xxx.xxx
                AddProperties(coll, null, pressed == '.');
            }

            if (coll.Count > 0)
                coll.Sort();
            return coll;
        }

        public void PostOnTheFlyText(Application application, TextWindow w, int startColumn, int length)
        {
        }

        public bool IsShowInsightDataCharacter(char character)
        {
            return character == '(';
        }

        public bool CanGetInsightDataProvider(Application application, TextWindow window)
        {
            return CanGetCodeCompletionCollection(application, window);
        }

        public IInsightDataProvider GetInsightDataProvider(Application application, TextWindow window, char pressed)
        {
            int row, column;
            row = window.CursorRow;
            column = window.CursorColumn;
            XceFileBuffer text = window.Text;

            if (row >= text.LinesCount)
            {
                return null;
            }

            int ls = text.LineStart(row);
            int ll = text.LineLength(row);

            if (pressed != '(')
            {
                if (column >= ll)
                    return null;
                if (text[ls + column] != '(')
                    return null;
                column--;
            }

            Context context = GetContext(window, false);
            if (context == null)
                return null;

            if (context.currentWord == null)
                return null;

                LuaInsightDataProvider dataProvider = new LuaInsightDataProvider(window, row, column);

            string function = context.currentWord;
            LuaPropertyCollection props = null;
            int i;

            if (context.previousWord == null)
            {
                //add global functions with the name specified
                props = mProperties.FindByTable("_G");
                for (i = 0; i < props.Count; i++)
                {
                    if (props[i].IsFunction && props[i].Name.Equals(function, StringComparison.InvariantCultureIgnoreCase))
                        dataProvider.Add(props[i]);
                }
            }
            else if (context.previousWord != null && context.previousWordDivisor == ' ' && context.divisor == '.' && (props = mProperties.FindByTable(context.previousWord)) != null)
            {
                for (i = 0; i < props.Count; i++)
                {
                    if (props[i].IsFunction && props[i].Name.Equals(function, StringComparison.InvariantCultureIgnoreCase))
                        dataProvider.Add(props[i]);
                }
            }
            else if (context.previousWord != null && context.divisor == '.')
            {
                props = mProperties.FindByName(context.currentWord);
                for (i = 0; i < props.Count; i++)
                {
                    if (props[i].IsFunction && props[i].Static && props[i].Table != "_G" && props[i].Name.Equals(function, StringComparison.InvariantCultureIgnoreCase))
                        dataProvider.Add(props[i]);
                }
            }
            else if (context.previousWord != null && context.divisor == ':')
            {
                props = mProperties.FindByName(context.currentWord);
                for (i = 0; i < props.Count; i++)
                {
                    if (props[i].IsFunction && !props[i].Static && props[i].Table != "_G" &&  props[i].Name.Equals(function, StringComparison.InvariantCultureIgnoreCase))
                        dataProvider.Add(props[i]);
                }
            }
            if (dataProvider.Count > 0)
            {
                dataProvider.Sort();
                return dataProvider;
            }
            else
                return null;
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

