using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gehtsoft.xce.conio;

namespace test
{
    class test1
    {
        public class listener : ConsoleInputListener
        {
            private bool mStop;

            public bool Stop
            {
                get
                {
                    return mStop;
                }
            }

            public listener()
            {
                mStop = false;
            }

            private string getSCA(bool shift, bool ctrl, bool alt)
            {
                char[] c = new char[3];
                c[0] = shift ? 'S' : 's';
                c[1] = ctrl ? 'C' : 'c';
                c[2] = alt ? 'A' : 'a';
                return new string(c);
            }

            private string getSCA(bool shift, bool ctrl, bool alt, bool left, bool right)
            {
                char[] c = new char[5];
                c[0] = shift ? 'S' : 's';
                c[1] = ctrl ? 'C' : 'c';
                c[2] = alt ? 'A' : 'a';
                c[3] = left ? 'L' : 'l';
                c[4] = right ? 'R' : 'r';
                return new string(c);
            }


            public void onKeyPressed(int scanCode, char character, bool shift, bool ctrl, bool alt)
            {
                Console.WriteLine("key V: {0} {1} {2} {3}", scanCode, (int)character, character >= 20 ? new string(character, 1) : "", getSCA(shift, ctrl, alt));
            }

            public void onKeyReleased(int scanCode, char character, bool shift, bool ctrl, bool alt)
            {
                Console.WriteLine("key ^: {0} {1} {2} {3}", scanCode, (int)character, character >= 20 ? new string(character, 1) : "", getSCA(shift, ctrl, alt));
                mStop = scanCode == 27;
            }

            public void onMouseMove(int row, int column, bool shift, bool ctrl, bool alt, bool left, bool right)
            {
                Console.WriteLine("mm: {0}:{1} {2}", row, column, getSCA(shift, ctrl, alt, left, right));
            }

            public void onMouseLButtonDown(int row, int column, bool shift, bool ctrl, bool alt)
            {
                Console.WriteLine("lbd: {0}:{1} {2}", row, column, getSCA(shift, ctrl, alt));
            }
            public void onMouseLButtonUp(int row, int column, bool shift, bool ctrl, bool alt)
            {
                Console.WriteLine("lbu: {0}:{1} {2}", row, column, getSCA(shift, ctrl, alt));
            }
            public void onMouseRButtonDown(int row, int column, bool shift, bool ctrl, bool alt)
            {
                Console.WriteLine("rbd: {0}:{1} {2}", row, column, getSCA(shift, ctrl, alt));
            }
            public void onMouseRButtonUp(int row, int column, bool shift, bool ctrl, bool alt)
            {
                Console.WriteLine("rbu: {0}:{1} {2}", row, column, getSCA(shift, ctrl, alt));
            }
            public void onMouseWheelUp(int row, int column, bool shift, bool ctrl, bool alt)
            {
                Console.WriteLine("mvu: {0}:{1} {2}", row, column, getSCA(shift, ctrl, alt));
            }

            public void onMouseWheelDown(int row, int column, bool shift, bool ctrl, bool alt)
            {
                Console.WriteLine("mvd: {0}:{1} {2}", row, column, getSCA(shift, ctrl, alt));
            }
            public void onGetFocus(bool shift, bool ctrl, bool alt)
            {
                Console.WriteLine("gf: {0}", getSCA(shift, ctrl, alt));
            }
            public void onScreenBufferChanged(int rows, int columns)
            {

            }
        }


        static internal void dotest()
        {
            ConsoleInput ci = new ConsoleInput();
            listener li = new listener();

            while (true)
            {
                ci.read(li, -1);
                if (li.Stop)
                    break;
            }
            ci.Dispose();
        }
    }
}
