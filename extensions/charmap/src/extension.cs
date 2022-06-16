using System;
using System.IO;
using gehtsoft.xce.configuration;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.extension.charmap_impl;


namespace gehtsoft.xce.extension
{
    public class charmap : IEditorExtension
    {
        AfterPaintWindowHook mPaintHook1;
        AfterPaintWindowHook mPaintHook2;

        public bool Initialize(Application application)
        {
            application.Commands.AddCommand(new SnapshotCommand());
            application.Commands.AddCommand(new CharmapCommand());
            application.Commands.AddCommand(new ShowColorerRegionsCommand());
            ShowWhitespaceCommand cmd1 = new ShowWhitespaceCommand();
            mPaintHook1 = new AfterPaintWindowHook(cmd1.PainterHook);
            application.Commands.AddCommand(cmd1);
            ShowNonLatinCommand cmd2 = new ShowNonLatinCommand();
            mPaintHook2 = new AfterPaintWindowHook(cmd2.PainterHook);
            application.Commands.AddCommand(cmd2);
            application.AfterOpenWindowEvent += new AfterOpenWindowHook(AfterOpenWindow);
            return true;
        }

        private void AfterOpenWindow(TextWindow window)
        {
            window.AfterPaintWindowEvent += mPaintHook1;
            window.AfterPaintWindowEvent += mPaintHook2;
        }

    };
}

