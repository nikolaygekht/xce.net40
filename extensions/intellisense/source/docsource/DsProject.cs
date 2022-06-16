using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using gehtsoft.intellisense.cs;
using gehtsoft.xce.colorer;
using gehtsoft.xce.text;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.configuration;
using gehtsoft.xce.intellisense.common;
using GehtSoft.DocCreator.Parser;

namespace gehtsoft.intellisense.docsource
{

    internal class DsFileSource : IParserSource, IDisposable
    {
        private readonly TextWindow mWindow;
        private readonly XceFileBufferChangedDelegate mDelegate;
        private int mCurrLine;

        internal DsFileSource(TextWindow window)
        {
            mWindow = window;
            mDelegate = new XceFileBufferChangedDelegate(XceFileBufferChangedHandler);
            Changed = true;
            mWindow.Text.OnChanged += mDelegate;
            mCurrLine = 0;
        }

        private void XceFileBufferChangedHandler(XceFileBuffer sender, int position, bool major)
        {
            Changed = true;
        }

        ~DsFileSource()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposal)
        {
            if (mDelegate != null)
            {
                mWindow.Text.OnChanged -= mDelegate;
            }
        }


        public bool Changed { get; set; }

        public string Name => mWindow.Text.FileName;

        public Encoding Encoding => mWindow.Text.Encoding;

        public void Reset()
        {
            mCurrLine = 0;
        }

        public void Stop()
        {
            //nothing to do
        }

        public string ReadLine()
        {
            if (mCurrLine < 0 || mCurrLine >= mWindow.Text.LinesCount)
                return null;
            string s = mWindow.Text.GetRange(mWindow.Text.LineStart(mCurrLine), mWindow.Text.LineLength(mCurrLine));
            mCurrLine++;
            return s;
        }
    }

    internal class DsFileSourceCollection : IEnumerable<DsFileSource>
    {
        private readonly List<DsFileSource> mList = new List<DsFileSource>();

        public IEnumerator<DsFileSource> GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return mList.Count;
            }

        }

        public DsFileSource this[int index]
        {
            get
            {
                return mList[index];
            }

        }

        internal void Add(DsFileSource source)
        {
            mList.Add(source);
        }

        internal void Remove(DsFileSource source)
        {
            mList.Remove(source);
        }

        internal bool Contains(DsFileSource source)
        {
            return mList.IndexOf(source) >= 0;
        }
    }

    internal class DsProjectInfo
    {
        private readonly DsProject mProject;
        private DsFileParser mParser;
        private readonly DsFileSourceCollection mSources = new DsFileSourceCollection();
        private readonly object mMutex = new object();
        private bool mParsed;
        private readonly int mTimeout;
        private readonly Thread mThread;
        private bool mStopRequest, mStopped;
        private readonly object mStopEvent;
        private readonly object mStoppedEvent;
        private readonly string mLogPath;

        internal DsProjectInfo(DsProject project, int timeout, string logPath)
        {
            mLogPath = logPath;
            mProject = project;
            mTimeout = timeout;
            mParsed = (mProject != null);
            mThread = new Thread(this.ParsingThread);
            mThread.IsBackground = true;
            mStopEvent = new object();
            mStoppedEvent = new object();
        }

        private void log(string format, params object[] args)
        {

            StreamWriter w = null;
            try
            {
                w = new StreamWriter(mLogPath, true);
                w.WriteLine("{0} {1}", DateTime.Now.ToString("u"), string.Format(format, args));
                w.Close();
                w = null;
            }
            catch (Exception )
            {
                //suppress exceptions
            }
            finally
            {
                if (w != null)
                    w.Close();
                w = null;
            }
        }

        internal bool Works
        {
            get
            {
                return mThread.IsAlive && !mStopped;
            }
        }

        internal void Start()
        {
            mParser = new DsFileParser();
            mStopRequest = false;
            mThread.Start();
        }

        internal void Stop()
        {
            mStopRequest = true;
            lock (mStopEvent)
                Monitor.PulseAll(mStopEvent);
            int max = 0;
            while (!mStopped && max < 1000)
            {
                lock (mStoppedEvent)
                    Monitor.Wait(mStoppedEvent, 50);
                max += 50;
            }
        }

        internal bool Parsed
        {
            get
            {
                if (mStopped)
                    return false;

                if (!mParsed)
                    return false;

                for (int i = 0; i < mSources.Count; i++)
                    if (mSources[i].Changed)
                        return false;
                return true;
            }
        }

        internal object Mutex
        {
            get
            {
                return mMutex;
            }

        }

        internal DsProject Project
        {
            get
            {
                return mProject;
            }
        }

        internal DsFileSourceCollection Sources
        {
            get
            {
                return mSources;
            }
        }


        internal DocItem ParserRoot
        {
            get
            {
                return mParser.Root;
            }
        }

        private void ParsingThread()
        {
            try
            {
                mStopped = false;

                List<Error> errors = new List<Error>();
                if (mProject != null && mProject.Sources.Count > 0)
                {
                    try
                    {
                        int i, l, j, l1;
                        l = mProject.Sources.Count;
                        for (i = 0; i < l; i++)
                        {
                            DsProjectSource s = mProject.Sources[i];
                            l1 = s.Count;
                            for (j = 0; j < l1; j++)
                            {
                                string file = s[j];
                                FileParserSource src = new FileParserSource(file, s.Encoding);
                                mParser.ParseFile(src, errors, null, null);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        log ("unexpected exception during parsing: {0}", e.ToString());
                    }
                }

                mParsed = true;

                //background job
                while (!mStopRequest)
                {
                    lock (mMutex)
                    {
                        try
                        {
                            errors.Clear();
                            for (int i = 0; i < this.mSources.Count; i++)
                                if (mSources[i].Changed)
                                    mParser.UpdateFile(mSources[i], errors, null);
                        }
                        catch (Exception e)
                        {
                            log ("unexpected exception during parsing: {0}", e.ToString());
                        }
                    }

                    lock (mStopEvent)
                        Monitor.Wait(mStopEvent, mTimeout);
                }
            }
            finally
            {
                mStopped = true;
                lock (mStoppedEvent)
                    Monitor.PulseAll(mStoppedEvent);
            }
        }
    }

    internal class DsProjectInfoCollection : IEnumerable<DsProjectInfo>
    {
        private readonly List<DsProjectInfo> mList = new List<DsProjectInfo>();

        public IEnumerator<DsProjectInfo> GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return mList.Count;
            }

        }

        public DsProjectInfo this[int index]
        {
            get
            {
                return mList[index];
            }

        }

        internal void Add(DsProjectInfo source)
        {
            mList.Add(source);
        }

        internal void Remove(DsProjectInfo source)
        {
            mList.Remove(source);
        }

        internal bool Contains(DsProjectInfo source)
        {
            return mList.IndexOf(source) >= 0;
        }
    }
}