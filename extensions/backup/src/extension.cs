using System;
using System.IO;
using gehtsoft.xce.configuration;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;

namespace gehtsoft.xce.extension
{
    public class backup : IEditorExtension
    {
        private bool mEnabled;
        private string mBackupPath;
        private int mMaxCopies;

        public bool Initialize(Application application)
        {
            application.AfterOpenWindowEvent += new AfterOpenWindowHook(AfterOpenWindow);

            ProfileSection config = application.Configuration["backup"];
            if (config == null)
                mEnabled = false;
            else
            {
                mBackupPath = config["folder", "c:\\backup"].Trim();
                string t = config["copies", "1"].Trim();
                if (!Int32.TryParse(t, out mMaxCopies))
                    mMaxCopies = 1;
                if (mMaxCopies >= 1)
                    mEnabled = true;
            }

            if (mEnabled)
            {
                try
                {
                    DirectoryInfo info = new DirectoryInfo(mBackupPath);
                    if (!info.Exists)
                        info.Create();
                }
                catch (Exception e)
                {
                    Console.WriteLine("backup init: {0}", e.Message);
                    mEnabled = false;
                }
            }

            return true;
        }

        private void AfterOpenWindow(TextWindow window)
        {
            window.BeforeSaveWindowEvent += new BeforeSaveWindowHook(BeforeSaveWindow);
        }

        private void BeforeSaveWindow(TextWindow window)
        {
            if (mEnabled)
            {
                try
                {
                    FileInfo fi = new FileInfo(window.Text.FileName);
                    if (fi.Exists)
                    {
                        string name = fi.Name;
                        int i;
                        for (i = 0; i < mMaxCopies; i++)
                        {
                            FileInfo fi1 = new FileInfo(Path.Combine(mBackupPath, fi.Name + "." + i.ToString()));
                            if (!fi1.Exists)
                                break;
                        }
                        if (i == mMaxCopies)
                        {
                            for (i = 1; i < mMaxCopies; i++)
                            {
                                File.Copy(Path.Combine(mBackupPath, fi.Name + "." + i.ToString()),
                                          Path.Combine(mBackupPath, fi.Name + "." + (i - 1).ToString()),
                                          true);
                            }
                            i = mMaxCopies - 1;
                        }
                        File.Copy(fi.FullName, Path.Combine(mBackupPath, fi.Name + "." + i.ToString()), true);
                    }
                }
                catch (Exception )
                {
                    return ;
                }
            }
            return ;
        }
    };
}

