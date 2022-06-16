using System;
using System.Runtime.InteropServices;
using gehtsoft.xce.script.engine;

[assembly: ComVisible(true)]
[assembly: Guid("158A0FEB-E15C-432D-80EF-E3B6EA22D762")]

namespace test
{
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsDual), Guid("2FC0EFED-6534-4A04-AD24-F47F36924714")]
    public interface IApp
    {
        void Write(string what);
    };

    [ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), Guid("65A22974-5682-4AA5-9754-178DF491B575")]
    public class App : IApp
    {
        public void Write(string what)
        {
            Console.WriteLine("{0}", what);
        }
    };

    class app
    {



        public static void Main(string[] args)
        {
            ScriptEngine engine = new ScriptEngine();
            App app = new App();

            try
            {
                try
                {
                    engine.initialize("xep");
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}", e.ToString());
                }
                try
                {
                    engine.initialize("VBScript");
                    Console.WriteLine("Ok!");
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}", e.ToString());
                }

                try
                {
                    engine.addObject("App", (IApp)app, false);
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}", e.ToString());
                    return ;
                }

                try
                {
                    engine.load("Function Div(a, b)\r\nDiv = a / b\r\nEnd Function\r\nSub Test()\r\nApp.Write(\"Test!\")\r\nEnd Sub\r\nFunction RetInt()\r\nRetInt = 10\r\nEnd Function\r\nFunction RetStr()\r\nRetStr = \"xep\"\r\nEnd Function\r\n", "piece1");
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}", e.ToString());
                    return ;
                }

                try
                {
                    engine.connect();
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}", e.ToString());
                    return ;
                }

                try
                {
                    engine.invoke("Test");
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}", e.ToString());
                }

                try
                {
                    engine.invoke("NoMethod");
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}", e.ToString());
                }

                try
                {
                    object x = null;
                    engine.invoke("RetInt", null, out x);
                    Console.WriteLine("{0} {1}", x.GetType().FullName, x);
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}", e.ToString());
                }

                try
                {
                    object x = null;
                    engine.invoke("RetStr", null, out x);
                    Console.WriteLine("{0} {1}", x.GetType().FullName, x);
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}", e.ToString());
                }

                try
                {
                    object x = null;
                    object[] a = new object[] { 1.0, 2.0 };
                    engine.invoke("Div", a, out x);
                    Console.WriteLine("{0} {1}", x.GetType().FullName, x);
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}", e.ToString());
                }

                try
                {
                    object x = null;
                    object[] a = new object[] { 1.0, 0.0 };
                    engine.invoke("Div", a, out x);
                    Console.WriteLine("{0} {1}", x.GetType().FullName, x);
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}", e.ToString());
                }

            }
            finally
            {
                engine.Dispose();
            }
        }
    }
}

