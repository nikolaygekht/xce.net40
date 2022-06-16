using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.text;


namespace gehtsoft.xce.editor.command.impl
{
    internal class FileSaveAsDialog : FileSaveDialog
    {
        Encoding mEncoding;
        Application mApplication;

        internal Encoding Encoding
        {
            get
            {
                return mEncoding;
            }
        }

        internal FileSaveAsDialog(string name, Encoding encoding, Application application) : base("", name, SaveMode.OverwritePrompt, application.ColorScheme)
        {
            mApplication = application;
            mEncoding = encoding;
            AddCustomButton(0x4000, "< Save &Encoding > ");
        }

        public override void OnItemClick(DialogItem item)
        {
            if (item.ID == 0x4000)
            {
                ChangeCodePageDialog dlg = new ChangeCodePageDialog(mApplication, mEncoding, false);
                if (dlg.DoModal() == Dialog.DialogResultOK)
                {
                    mEncoding = dlg.Encoding;
                }
            }
            base.OnItemClick(item);
        }

        public int DoModal()
        {
            return base.DoModal(mApplication.WindowManager);
        }
    }


    internal class SaveFileAsCommand : IEditorCommand
    {
        internal SaveFileAsCommand()
        {

        }

        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "SaveFileAs";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            TextWindow w = application.ActiveWindow;
            if (w == null)
                return;

            FileSaveAsDialog dlg = new FileSaveAsDialog(w.Text.FileName, w.Text.Encoding, application);
            Encoding encoding = null;

            if (dlg.DoModal() == Dialog.DialogResultOK)
            {
                parameter = dlg.File;
                if (dlg.Encoding.CodePage != w.Text.Encoding.CodePage)
                    encoding = dlg.Encoding;
            }
            else
                return;

                if (!SaveFileCommandUtil.CheckOpenToWrite(application, parameter))
                    return;

                w.FireSaveWindowEvent();

                try
                {
                    if (encoding != null)
                        w.Text.SaveAs(parameter, encoding, !w.FileTypeInfo.IgnoreBOM);
                    else
                        w.Text.SaveAs(parameter, !w.FileTypeInfo.IgnoreBOM);
                }
                catch (Exception e)
                {
                    MessageBox.Show(application.WindowManager, application.ColorScheme,
                                    string.Format("Can't save file {0}\r\n{1}", w.Text.FileName, e.ToString()),
                                    "Error", MessageBoxButtonSet.Ok);
                }
                w.invalidate();
                GC.Collect();
        }

        /// <summary>
        /// Get checked status for the menu.
        /// </summary>
        public bool IsChecked(Application application, string parameter)
        {
            return false;
        }

        /// <summary>
        /// Get enabled status for the menu with parameter.
        /// </summary>
        public bool IsEnabled(Application application, string parameter)
        {
            return application.ActiveWindow != null;
        }
    }
}
