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
            ConsoleOutputMode mode = ConsoleOutputMode.ConEmu;

            if (args.Length > 0 && args[0] == "conemu")
                mode = ConsoleOutputMode.ConEmu;
            if (args.Length > 0 && args[0] == "win32")
                mode = ConsoleOutputMode.Win32;
            if (args.Length > 0 && args[0] == "vt")
                mode = ConsoleOutputMode.VT;
            if (args.Length > 0 && args[0] == "vttc")
                mode = ConsoleOutputMode.VTTC;

            mManager = new WindowManager(true, mode);
            mManager.FastDrawMode = false;
            mQuitMessageReceived = false;
            MyWindow w1 = new MyWindow(new ConsoleColor(0x30, ConsoleColor.rgb(0, 0, 0), ConsoleColor.rgb(0, 128, 255)), false);
            mManager.create(w1, null, 1, 1, 4, 10);
            w1.show(true);
            MyWindow w2 = new MyWindow(new ConsoleColor(0x40), false);
            mManager.create(w2, null, 5, 5, 4, 30);
            w2.show(true);
            MyWindow w3 = new MyWindow(new ConsoleColor(0x50, ConsoleColor.rgb(0, 0, 0), ConsoleColor.rgb(255, 128, 192)), false);
            mManager.create(w3, w2, 1, 10, 3, 30);
            w3.show(true);

            while (!mQuitMessageReceived)
                mManager.pumpMessage(100);
        }
    }
}
