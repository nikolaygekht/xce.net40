using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;

namespace gehtsoft.xce.extension.scriptimpl
{
    internal class ScriptCommand : IEditorCommand
    {
        script mScriptExtension;
        
        public ScriptCommand(script scriptExtension)
        {
            mScriptExtension = scriptExtension;
        }
        
        public string Name
        {
            get
            {
                return "Script";
            }
        }
        
        public bool IsEnabled(Application application, string p)
        {
            return mScriptExtension.Ready && p != null;
        }

        public bool IsChecked(Application application, string p)
        {
            return false;
        }
        
        public void Execute(Application application, string p)
        {
            TextWindow tw = application.ActiveWindow;
            tw.Text.BeginUndoTransaction();
            try
            {
                if (mScriptExtension.Ready && p != null)
                    mScriptExtension.Execute(application, p);
                if (application.ActiveWindow != null &&
                    application.ActiveWindow.Exists)
                        application.ActiveWindow.invalidate();
            }
            finally
            {
                if (tw.Exists)
                    tw.Text.EndUndoTransaction();
            }
        }
    }
}
