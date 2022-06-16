using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using ConsoleColor = gehtsoft.xce.conio.ConsoleColor;

namespace test1
{
    class Program
    {
        static WindowManager mManager;
        static bool mQuitMessageReceived = false;

        internal static WindowManager WindowManager
        {
            get
            {
                return mManager;
            }
        }

        internal static void PostQuitMessage()
        {
            mQuitMessageReceived = true;
        }

        static void Main(string[] args)
        {
            mManager = new WindowManager(true);
            mManager.FastDrawMode = false;
            mQuitMessageReceived = false;

            MyClient client = new MyClient();
            WindowBorderContainer container = new WindowBorderContainer("test", BoxBorder.SingleBorderBox, new ConsoleColor(0x80), client, true, true);
            mManager.create(container, null, 5, 5, 10, 10);
            container.show(true);

            while (!mQuitMessageReceived)
                mManager.pumpMessage(100);
        }
    }
}
