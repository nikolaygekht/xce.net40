using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.editor.util;
using gehtsoft.xce.text;

namespace gehtsoft.xce.extension.advnav_commands
{
    internal class FindPreviousWordCommand : FindWordBase, IEditorCommand
    {
        public string Name
        {
            get
            {
                return "FindPreviousWord";
            }
        }        
        
        public bool IsEnabled(Application application, string param)
        {
            int pos;
            if (application.ActiveWindow == null)
                return false;
            if (!CursorToPosition(application.ActiveWindow, out pos))
                return false;
            if (!isWord(application.ActiveWindow.Text, pos))
                return false;
                
            return true;
        }
        
        public bool IsChecked(Application application, string param)
        {
            return false;
        }
        
        public void Execute(Application application, string param)
        {
            if (application.ActiveWindow == null)
                return ;
                
            int i, j, p, ws, wl;
            
            if (!CursorToPosition(application.ActiveWindow, out p))
                return ;
                
            XceFileBuffer b = application.ActiveWindow.Text;
                
            if (!isWord(b, p))
                return ;
                
            getWord(b, p, out ws, out wl);
            
            int l0 = b.Length;
            int ws1, wl1;
            bool f;
            
            for (i = ws - 1; i >= 0; i--)
            {
                if (getWord(b, i, out ws1, out wl1))
                {
                    if (wl1 == wl)
                    {
                        for (j = 0, f = false; j < wl && !f; j++)
                            if (b[ws + j] != b[ws1 + j])
                                f = true;
                        if (!f)
                        {
                            GoPosition(application.ActiveWindow, ws1);
                            break;
                        }
                    }
                    i = ws1 - 1;
                }
            }
        }
    }
}
