using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.editor.configuration;

namespace gehtsoft.xce.editor.application
{
    public class XceDialog : Dialog
    {
        protected Application mApplication;
    
        public XceDialog(Application application, string title, bool sizeable, int height, int width) : base(title, application.ColorScheme, sizeable, height, width)
        {
            mApplication = application;
        }
        
        public int DoModal()
        {
            return base.DoModal(mApplication.WindowManager);
        }

        public override bool PretranslateOnKeyPressed(int scanCode, char character, bool shift, bool ctrl, bool alt)
        {
            bool rc = base.PretranslateOnKeyPressed(scanCode, character, shift, ctrl, alt);
            if (!rc)
            {
                KeyboardShortcut shortcut = mApplication.Keymap.Find(ctrl, alt, shift, scanCode);
                if (shortcut != null)
                {
                    IDialogCommand dlg = shortcut.Command as IDialogCommand;
                    if (dlg != null)
                    {
                        dlg.DialogExecute(mApplication, this, shortcut.Parameter);
                        return true;
                    }
                }
            }
            return rc;
        }
    }
}
