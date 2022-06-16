using System;
using System.IO;
using gehtsoft.xce.configuration;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;

namespace gehtsoft.xce.extension
{
    public class autosave : IEditorExtension
    {
        private bool mEnabled;
        Application mApplication;
        private long mTimeout;

        public bool Initialize(Application application)
        {
            ProfileSection config = application.Configuration["autosave"];
            if (config == null)
                mEnabled = false;
            else
            {
                string t = config["timeout", "600"].Trim();
                if (!Int64.TryParse(t, out mTimeout))
                    mTimeout = 0;
                mTimeout *= 10000000;
                if (mTimeout > 0)
                    mEnabled = true;
            }

            if (mEnabled)
            {
                mApplication = application;
                application.TimerEvent += new TimerHook(OnTimer);
            }

            return true;
        }

        private void OnTimer()
        {
            foreach (TextWindow w in mApplication.TextWindows)
            {
                if (!w.Text.AtSavePoint &&
                    (DateTime.Now.Ticks - w.Text.LastChange > mTimeout) &&
                    w.Text.FileName != null &&
                    w.Text.FileName.Length > 0)
                {
                    FileInfo fi = new FileInfo(w.Text.FileName);
                    if (!fi.IsReadOnly)
                    {
                        w.FireSaveWindowEvent();
                        w.Text.Save(!w.FileTypeInfo1.IgnoreBOM, null);
                        if (w == mApplication.ActiveWindow)
                            mApplication.repaint();
                    }
                }
            }
        }

    }
}

