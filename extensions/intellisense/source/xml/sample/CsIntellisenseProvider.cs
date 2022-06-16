using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using gehtsoft.intellisense.cs;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.configuration;
using gehtsoft.xce.text;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using gehtsoft.xce.intellisense.common;

namespace gehtsoft.intellisense.cs
{
    internal class CsIntellisenseProvider : IIntellisenseProvider
    {
        ParserCollection mParsers = new ParserCollection();
        internal const string DATA_NAME = "csintellisense-file";
        private bool mEnabled;
        private bool mCreateCache;
        private int mTimeout;
        private Application mApplication;

        public bool Initialize(Application application)
        {
            mApplication = application;
            ProfileSection config = application.Configuration["csintellisense"];

            if (config == null)
                mEnabled = false;
            else
            {
                mEnabled = config["enabled", "false"].Trim() == "true";
                mCreateCache = config["create-cache", "false"].Trim() == "true";
                string t = config["parser-timeout", "1000"].Trim();
                if (!Int32.TryParse(t, out mTimeout))
                    mTimeout = 2000;
                if (mTimeout < 250)
                    mTimeout = 250;

            }
            return mEnabled;
        }

        public bool AcceptFile(Application application, TextWindow window)
        {
            FileInfo fi = new FileInfo(window.Text.FileName);
            return (string.Compare(fi.Extension, ".cs", true) == 0);
        }

        public bool Start(Application application, TextWindow window)
        {
             CsParser parser = mParsers.FindParser(window.Text.FileName);
             if (parser == null)
             {
                 CsProject project = CsUtils.FindCsProject(window.Text.FileName);
                 if (project == null)
                     project = CsProject.Default;
                 parser = new CsParser(project, mCreateCache, mTimeout);
                 parser.Start();
                 while (parser.Works && !parser.Parsed);
                 mParsers.Add(parser);
             }
             CsParserFile file = parser.NewFileInEditor(window.Text.FileName, new CsFileSource(window.Text));
             window[DATA_NAME] = file;
             return true;
        }

        public void Stop(Application application, TextWindow window)
        {
            if (window[DATA_NAME] != null)
            {
                CsParserFile file = window[DATA_NAME] as CsParserFile;

                if (file != null)
                {
                    file.Parser.CloseFileInEditor(file);

                    if (!file.Parser.HasEditorFiles())
                    {
                        file.Parser.Stop();
                        mParsers.Remove(file.Parser);
                    }
                }
                window[DATA_NAME] = null;
                GC.Collect();
            }
        }

        public bool CanGetCodeCompletionCollection(Application application, TextWindow window)
        {
            if (application.ActiveWindow == null)
                return false;
            if (application.ActiveWindow[DATA_NAME] == null)
                return false;
            if (application.ActiveWindow[DATA_NAME] as CsParserFile == null)
                return false;
            return true;
        }

        private const string MSG_TITLE = "C# Intellisense";
        private const string MSG_NOT_IN_PROJECT = "The file is not a part of the project";
        private const string MSG_PROJECT_STOPPED = "The parser does not work. Please check parser log for errors";
        private const string MSG_NOT_FOUND = "Declaration is not found";

        public ICodeCompletionItemCollection GetCodeCompletionCollection(Application application, TextWindow window, out int wline, out int wcolumn, out int wlength)
        {
            wline = wcolumn = wlength = 0;

            if (application.ActiveWindow == null ||
                application.ActiveWindow[DATA_NAME] == null ||
                application.ActiveWindow[DATA_NAME] as CsParserFile == null)
            {
                application.ShowMessage(MSG_NOT_IN_PROJECT, MSG_TITLE);
                return null;
            }
            CsParserFile file = application.ActiveWindow[DATA_NAME] as CsParserFile;
            if (!file.Parser.Works)
            {
                application.ShowMessage(MSG_PROJECT_STOPPED, MSG_TITLE);
                return null;
            }


            int wordline, wordcolumn, wordlength;
            int row, column;
            bool wordUnderCursor;
            if (CsUtils.WordUnderCursor(application.ActiveWindow, out wordline, out wordcolumn, out wordlength))
            {
                wordUnderCursor = true;
                row = wordline;
                column = wordcolumn + wordlength;
            }
            else
            {
                wordUnderCursor = false;
                row = application.ActiveWindow.CursorRow;
                column = application.ActiveWindow.CursorColumn;
            }

            try
            {
                CsCodeCompletionItemCollection coll = null;
                if (wordUnderCursor)
                {
                    string word = application.ActiveWindow.Text.GetRange(application.ActiveWindow.Text.LineStart(wordline) + wordcolumn, wordlength);
                    if (word == "override")
                        coll = file.Parser.GetOverloadData(file, row + 1, column + 1);
                    else if (word == "base")
                        coll = file.Parser.GetConstructorData(file, row + 1, column + 1);

                    wline = wordline;
                    wcolumn = wordcolumn;
                    wlength = wordlength;
                }
                if (coll == null)
                    coll = file.Parser.GetCompletionData(file, row + 1, column + 1, true);

                return coll;
            }
            finally
            {
                GC.Collect();
            }
        }

        public bool IsShowOnTheFlyDataCharacter(char character)
        {
            return (character == '.');
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
            return char.IsLetterOrDigit(character) || character == '_';
        }

        public bool CanGetOnTheFlyCompletionData(Application application, TextWindow window)
        {
            return CanGetCodeCompletionCollection(application, window);
        }

        public ICodeCompletionItemCollection GetOnTheFlyCompletionData(Application application, TextWindow window, char pressed)
        {
            if (application.ActiveWindow == null ||
                application.ActiveWindow[DATA_NAME] == null ||
                application.ActiveWindow[DATA_NAME] as CsParserFile == null)
            {
                application.ShowMessage(MSG_NOT_IN_PROJECT, MSG_TITLE);
                return null;
            }

            CsParserFile file = application.ActiveWindow[DATA_NAME] as CsParserFile;
            if (!file.Parser.Works)
            {
                application.ShowMessage(MSG_PROJECT_STOPPED, MSG_TITLE);
                return null;
            }

            CsCodeCompletionItemCollection coll = null;
            coll = file.Parser.GetCompletionData(file, mApplication.ActiveWindow.CursorRow + 1, mApplication.ActiveWindow.CursorColumn + 1, true, true);
            return coll;
        }

        public void PostOnTheFlyText(Application application, TextWindow w, int startColumn, int length, ICodeCompletionItem item)
        {
            return ;
        }

        public bool IsShowInsightDataCharacter(char character)
        {
            return (character == '(');
        }

        public bool CanGetInsightDataProvider(Application application, TextWindow window)
        {
            return CanGetCodeCompletionCollection(application, window);
        }

        public IInsightDataProvider GetInsightDataProvider(Application application, TextWindow window, char pressed)
        {
            if (application.ActiveWindow == null ||
                application.ActiveWindow[DATA_NAME] == null ||
                application.ActiveWindow[DATA_NAME] as CsParserFile == null)
            {
                application.ShowMessage(MSG_NOT_IN_PROJECT, MSG_TITLE);
                return null;
            }

            CsParserFile file = application.ActiveWindow[DATA_NAME] as CsParserFile;
            if (!file.Parser.Works)
            {
                application.ShowMessage(MSG_PROJECT_STOPPED, MSG_TITLE);
                return null;
            }

            List<CsInsightData> l = file.Parser.GetListOfInsightMethods(file, application.ActiveWindow.CursorRow + 1, application.ActiveWindow.CursorColumn + 1);
            if (l != null && l.Count > 0)
                return new CsInsightDataProvider(application.ActiveWindow, l, application.ActiveWindow.CursorRow, application.ActiveWindow.CursorColumn);
            else
                return null;
        }

        public bool CanGetProjectBrowserItemCollection(Application application, TextWindow window)
        {
            return CanGetCodeCompletionCollection(application, window);
        }

        public IProjectBrowserItemCollection GetProjectBrowserItemCollection(Application application, TextWindow window, out IProjectBrowserItem curSel)
        {
            curSel = null;
            if (application.ActiveWindow == null ||
                application.ActiveWindow[DATA_NAME] == null ||
                application.ActiveWindow[DATA_NAME] as CsParserFile == null)
            {
                application.ShowMessage(MSG_NOT_IN_PROJECT, MSG_TITLE);
                return null;
            }

            CsParserFile file = application.ActiveWindow[DATA_NAME] as CsParserFile;
            if (!file.Parser.Works)
            {
                application.ShowMessage(MSG_PROJECT_STOPPED, MSG_TITLE);
                return null;
            }

            CsProjectBrowserItem _curSel = null;
            CsProjectBrowserItemCollection collection = CsProjectBrowserItemCollectionFactory.create(file.Parser, window.Text.FileName, window.CursorRow, window.CursorColumn, null, out _curSel);
            curSel = _curSel;
            return collection;
        }

        public IProjectBrowserItemCollection FindProjectBrowserItem(Application application, TextWindow window, out IProjectBrowserItem curSel)
        {
            curSel = null;
            if (application.ActiveWindow == null ||
                application.ActiveWindow[DATA_NAME] == null ||
                application.ActiveWindow[DATA_NAME] as CsParserFile == null)
            {
                application.ShowMessage(MSG_NOT_IN_PROJECT, MSG_TITLE);
                return null;
            }

            CsParserFile file = application.ActiveWindow[DATA_NAME] as CsParserFile;
            if (!file.Parser.Works)
            {
                application.ShowMessage(MSG_PROJECT_STOPPED, MSG_TITLE);
                return null;
            }

            string word;
            int wordline, wordcolumn, wordlength;
            if (!CsUtils.WordUnderCursor(application.ActiveWindow, out wordline, out wordcolumn, out wordlength, out word))
                return null;

            try
            {
                CsCodeCompletionItemCollection coll = file.Parser.GetCompletionData(file, wordline + 1, wordcolumn + 1 + wordlength, true);
                if (coll.Count > 0 && coll.DefaultIndex >= 0)
                {
                    ICsCodeCompletionItem item = coll[coll.DefaultIndex];
                    if (item.Entity != null)
                    {
                        CsProjectBrowserItem _curSel = null;
                        CsProjectBrowserItemCollection collection = CsProjectBrowserItemCollectionFactory.create(file.Parser, window.Text.FileName, window.CursorRow, window.CursorColumn, item.Entity, out _curSel);

                        if (_curSel != null)
                        {
                            curSel = _curSel;
                            return collection;
                        }
                        else if (item.Name == word && item.Entity != null)
                        {
                            StringBuilder description = new StringBuilder("External definition\r\n");
                            CSharpAmbience a = new CSharpAmbience();
                            description.Append(a.Convert(item.Entity));
                            if (item.OverloadsCount > 0)
                            {
                                for (int i = 0; i < item.OverloadsCount && i < 7; i++)
                                {
                                    description.Append("\r\n");
                                    description.Append(a.Convert(item.Overloads[i].Entity));
                                }
                                if (item.OverloadsCount >= 7)
                                    description.Append("\r\n...");


                            }
                            application.ShowMessage(description.ToString(), MSG_TITLE);
                            return null;
                        }
                        else
                        {
                            application.ShowMessage(MSG_NOT_FOUND, MSG_TITLE);
                            return null;
                        }
                    }
                    else
                    {
                        application.ShowMessage(MSG_NOT_FOUND, MSG_TITLE);
                        return null;
                    }
                }
                else
                {
                    application.ShowMessage(MSG_NOT_FOUND, MSG_TITLE);
                    return null;
                }
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}

