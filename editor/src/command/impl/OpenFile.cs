using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.configuration;

namespace gehtsoft.xce.editor.command.impl
{

    internal class XceFileOpenDialog : FileOpenDialog
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

        internal XceFileOpenDialog(string name, Encoding encoding, Application application)
            : base("", name, FileOpenDialog.OpenMode.NewFilePromprt, application.ColorScheme)
        {
            mApplication = application;
            mEncoding = encoding;
            AddCustomButton(0x4000, "< Force &Encoding > ");
        }

        public override void OnItemClick(DialogItem item)
        {
            if (item.ID == 0x4000)
            {
                ChangeCodePageDialog dlg = new ChangeCodePageDialog(mApplication, mEncoding, true);
                if (dlg.DoModal() == Dialog.DialogResultOK)
                    mEncoding = dlg.Encoding;
            }
            base.OnItemClick(item);
        }

        public int DoModal()
        {
            return base.DoModal(mApplication.WindowManager);
        }
    }

    internal class OpenFileCommand : IEditorCommand
    {
        internal OpenFileCommand()
        {

        }

        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "OpenFile";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            Encoding encoding = null;
            if (parameter == null || parameter.Length == 0)
            {
                XceFileOpenDialog dlg = new XceFileOpenDialog("", null, application);

                Profile p = new Profile();
                try
                {
                    p.Load(Path.Combine(application.ApplicationPath, "open-history.ini"));
                    ProfileSection s = p[""];
                    for (int i = 4; i >= 0; i--)
                    {
                        string k = s["path", i, null];
                        if (k != null)
                            dlg.AddLocation(k);
                    }
                }
                catch (Exception )
                {
                }

                if (dlg.DoModal(application.WindowManager) == Dialog.DialogResultOK)
                {
                    parameter = dlg.File;
                    encoding = dlg.Encoding;
                    FileInfo fi = new FileInfo(dlg.File);
                    ProfileSection s = p[""];
                    string d = fi.Directory.FullName;
                    for (int i = 4; i >= 0; i--)
                    {
                        string k = s["path", i, null];
                        if (k != null && k == d)
                            s.Remove("path", i);
                    }
                    s.Add("path", d);
                    if (s.CountOf("path") > 5)
                        s.Remove("path", 0);
                    p.Save(Path.Combine(application.ApplicationPath, "open-history.ini"));
                }
                else
                    return ;
            }
            else
            {
                try
                {
                    FileInfo fi = new FileInfo(parameter);
                    parameter = fi.FullName;
                }
                catch (Exception )
                {
                    application.ShowMessage("File name specified is wrong", "Open File");
                    return ;
                }
            }
            application.OpenFile(parameter, encoding);
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
            return true;
        }
    }
}
