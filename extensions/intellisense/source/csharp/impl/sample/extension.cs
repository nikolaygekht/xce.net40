using System;
using System.IO;
using gehtsoft.xce.configuration;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.extension.csintellisense_impl;
using gehtsoft.intellisense.cs;

namespace gehtsoft.xce.extension
{
    public class csintellisense : IEditorExtension
    {
        private bool mEnabled;
        private bool mCreateCache;
        private int mTimeout;
        private KeyPressedHook mKeyPressedHook;
        private IEditorCommand mInsight;
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
            
            if (mEnabled)
            {
                application.AfterOpenWindowEvent += new AfterOpenWindowHook(AfterOpenWindow);
                application.BeforeCloseWindowEvent += new BeforeCloseWindowHook(BeforeCloseWindow);
            }
            application.Commands.AddCommand(new BrowserCsProjectCommand());
            application.Commands.AddCommand(new GoCsProjectDefinitionCommand());
            application.Commands.AddCommand(new GoCsSuggestionCommand());
            application.Commands.AddCommand(mInsight = new CsForceInsightCommand());

            mApplication.KeyPressedEvent += (mKeyPressedHook = new KeyPressedHook(OnKeyPressed));
            return true;
        }

        internal void OnKeyPressed(int scanCode, char character, bool shift, bool ctrl, bool alt, ref bool handled)
        {
            if (!handled)
            {
                if (!ctrl && !alt && (character == '(' || character == '[') &&
                    mInsight.IsEnabled(mApplication, null) &&
                    (TooltipWindow.Tooltip == null || !(TooltipWindow.Tooltip is PopupHintWindow)))
                    mInsight.Execute(mApplication, null);

                if (!ctrl && !alt && (character == '.') && mInsight.IsEnabled(mApplication, null) && (TooltipWindow.Tooltip == null || !(TooltipWindow.Tooltip is PopupHintWindow)))
                {
                    CsParserFile file = mApplication.ActiveWindow[csintellisense.DATA_NAME] as CsParserFile;
                    if (!file.Parser.Works)
                        return;
                    CsCodeCompletionItemCollection coll = null;
                    coll = file.Parser.GetCompletionData(file, mApplication.ActiveWindow.CursorRow + 1, mApplication.ActiveWindow.CursorColumn + 1, true, true);
                    if (coll != null && coll.Count > 0)
                    {
                        PopupHintWindow w = new PopupHintWindow(mApplication, mApplication.ActiveWindow.CursorRow, mApplication.ActiveWindow.CursorColumn, coll, this);
                        w.create();                        
                    }                    
                }
            }
        }

        
        ParserCollection mParsers = new ParserCollection();
        
        public const string DATA_NAME = "csintellisense-file";
        
        private void BeforeCloseWindow(TextWindow window)
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
            }
        }
        
        private void AfterOpenWindow(TextWindow window)
        {
            FileInfo fi = new FileInfo(window.Text.FileName);
            if (string.Compare(fi.Extension, ".cs", true) == 0)
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
            }
        }
    };
}

