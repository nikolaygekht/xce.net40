using System;
using System.Collections.Generic;
using System.IO;
using gehtsoft.xce.configuration;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.extension.intellisense_impl;
using gehtsoft.xce.intellisense.common;

namespace gehtsoft.xce.extension
{
    public class intellisense : IEditorExtension
    {
        List<IIntellisenseProvider> mProviders;
        private Application mApplication;
        private ForceInsightCommand mInsight;
        private IEditorCommand mSuggestion;

        public bool Initialize(Application application)
        {
            mApplication = application;
            mProviders = IntellisenseFactory.CreateProviders(application);

            application.AfterOpenWindowEvent += new AfterOpenWindowHook(AfterOpenWindow);
            application.BeforeCloseWindowEvent += new BeforeCloseWindowHook(BeforeCloseWindow);
            mApplication.KeyPressedEvent += new KeyPressedHook(OnKeyPressed);


            application.Commands.AddCommand(new BrowseProjectCommand());
            application.Commands.AddCommand(new GoDefinitionCommand());
            application.Commands.AddCommand(mSuggestion = new SuggestionCommand());
            application.Commands.AddCommand(mInsight = new ForceInsightCommand());
            return true;
        }

        internal void OnKeyPressed(int scanCode, char character, bool shift, bool ctrl, bool alt, ref bool handled)
        {
            if (!handled)
            {
                if (mApplication.ActiveWindow != null && mApplication.ActiveWindow[DATA_NAME] != null)
                {
                    IIntellisenseProvider p = mApplication.ActiveWindow[DATA_NAME] as IIntellisenseProvider;
                    if (p == null)
                        return ;

                    if (!ctrl && !alt && p.IsShowInsightDataCharacter(character) &&
                        mInsight.IsEnabled(mApplication, null) &&
                        (TooltipWindow.Tooltip == null || !(TooltipWindow.Tooltip is PopupHintWindow)))
                    {
                        mInsight.Execute(mApplication, character);
                    }
                    else if (!ctrl && !alt && p.IsShowOnTheFlyDataCharacter(character) &&
                             mSuggestion.IsEnabled(mApplication, null) &&
                             (TooltipWindow.Tooltip == null || !(TooltipWindow.Tooltip is PopupHintWindow)))
                    {
                        ICodeCompletionItemCollection coll = p.GetOnTheFlyCompletionData(mApplication, mApplication.ActiveWindow, character);
                        if (coll != null && coll.Count > 0)
                        {
                            PopupHintWindow w = new PopupHintWindow(mApplication, mApplication.ActiveWindow.CursorRow, mApplication.ActiveWindow.CursorColumn, coll, p, this);
                            w.create();
                        }
                    }
                }
            }
        }


        public const string DATA_NAME = "intellisense-provider";

        private void BeforeCloseWindow(TextWindow window)
        {
            if (window[DATA_NAME] != null)
            {
                IIntellisenseProvider p = window[DATA_NAME] as IIntellisenseProvider;
                if (p != null)
                    p.Stop(mApplication, window);
                window[DATA_NAME] = null;
            }
        }

        private void AfterOpenWindow(TextWindow window)
        {
            foreach (IIntellisenseProvider p in mProviders)
            {
                if (p.AcceptFile(mApplication, window))
                    if (p.Start(mApplication, window))
                    {
                        window[DATA_NAME] = p;
                        break;
                    }
            }
        }
    };
}

