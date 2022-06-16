using System;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using gehtsoft.xce.configuration;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.script.engine;
using gehtsoft.xce.extension.scriptimpl;

namespace gehtsoft.xce.extension
{
    public class script : IEditorExtension, IDisposable
    {
        private ScriptEngine mEngine = null;
        private SystemImpl mSystem = null;
        private EditorWrapper mWrapper = null;

        ~script()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposal)
        {
            if (mEngine != null)
                mEngine.Dispose();
            mEngine = null;
        }

        public bool Initialize(Application application)
        {
            ProfileSection config = application.Configuration["script"];
            string engine = "VBScript";
            string path = ".\\scripts\\";
            string mask = "*.vbs";

            if (config != null)
            {
                engine = config["engine", "VBscript"].Trim();
                path = config["folder", ".\\scripts\\"].Trim();
                mask = config["file-mask", "*.vbs"].Trim();
            }
            path = Path.Combine(application.ApplicationPath, path);

            mEngine = new ScriptEngine();
            mWrapper = new EditorWrapper(application);
            mSystem = new SystemImpl(application.ApplicationPath);

            try
            {
                mEngine.initialize(engine);
            }
            catch (Exception e)
            {
                Console.WriteLine("Script engine initialization failed\n{0}", e.ToString());
                mEngine.Dispose();
                mEngine = null;
                return false;
            }

            try
            {
                mEngine.addObject("xce", mWrapper, true);
                mEngine.addObject("system", mSystem, true);
            }
            catch (Exception e)
            {
                Console.WriteLine("Script engine initialization failed\n{0}", e.ToString());
                mEngine.Dispose();
                mEngine = null;
                return false;
            }

            string[] scripts = Directory.GetFiles(path, mask);
            foreach (string s in scripts)
            {
                try
                {
                    using (StreamReader r = new StreamReader(s, true))
                    {
                        string code = r.ReadToEnd();
                        mEngine.load(code, s);
                        r.Close();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Script file {1} reading failed\n{0}", e.ToString(), s);

                }
            }

            try
            {
                mEngine.connect();
            }
            catch (Exception e)
            {
                Console.WriteLine("Script engine initialization failed\n{0}", e.ToString());
                mEngine.Dispose();
                mEngine = null;
                return false;
            }

            application.Commands.AddCommand(new ScriptCommand(this));

            return true;
        }

        internal bool Ready
        {
            get
            {
                return mEngine != null;
            }
        }

        internal void Execute(Application application, string script)
        {
            try
            {
                mEngine.invoke(script);
            }
            catch (COMException e)
            {
                application.ShowMessage(string.Format("Script execution failed:\r\n{0:x} {1}", e.ErrorCode, e.Message), "Script Error");
            }
            catch (ScriptEngineException e)
            {
                if (application.ShowYesNoMessage(string.Format("Script execution failed:\r\n{0} ({1}:{2})\r\n{3}\r\nOpen the source code?", e.SourceName, e.Line, e.Column, e.Description), "Script Error"))
                {
                    application.OpenFile(e.SourceName);
                    application.ActiveWindow.CursorRow = e.Line - 1;
                    application.ActiveWindow.CursorColumn = e.Column - 1;
                    application.ActiveWindow.EnsureCursorVisible();
                }
            }
            catch (Exception e)
            {
                application.ShowMessage(string.Format("Script execution failed:\r\n{0}", e.Message), "Script Error");
            }
        }
    };
}

