using System;
using System.Threading;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using gehtsoft.xce.editor.application;

namespace gehtsoft.xce.editor
{
    public class StartupStub
    {
        public static void Main(string[] args)
        {
            CtrlCHandler ctrlCHandler = new CtrlCHandler();
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledException);
            Application application;

            try
            {
                application = new Application();
                application.InitApplication(args);
            }
            catch (Exception e)
            {
                logerror("Initialization", e);
                Console.WriteLine("Initialization failed. See editor log for details");
                return ;
            }

            while (true)
            {
                try
                {
                    application.PumpMessages();
                    break;
                }
                catch (Exception e)
                {
                    logerror("Message Loop", e);
                    application.ShowMessage("Unhadled Exception:\r\n" + e.Message, "Error");
                }
            }

            try
            {
                application.ReleaseApplication();
            }
            catch (Exception e)
            {
                logerror("Deinitialization", e);
                Console.WriteLine("Deinitialization failed. See editor log for details");
            }
        }

        internal static void OnUnhandledException(Object sender, UnhandledExceptionEventArgs e)
        {
            logerror("Unhandled appdomain", e.ExceptionObject as Exception);
        }

        static void logerror(string stage, Exception e)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            FileInfo fi = new FileInfo(executingAssembly.Location);
            string appPath = fi.DirectoryName;

            StreamWriter w = new StreamWriter(appPath + "\\exception-log.txt", true, Encoding.UTF8);
            try
            {
                w.WriteLine("---------------------------------------------------------------------");
                w.WriteLine("{0} unhandled exception during {1}", DateTime.Now.ToString("u"), stage);
                w.WriteLine("{0}({1})", e.GetType().ToString(), e.Message);
                w.WriteLine("{0}", e.StackTrace);
            }
            finally
            {
                w.Close();
            }

        }
    }
}