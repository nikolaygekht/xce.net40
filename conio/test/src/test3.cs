using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gehtsoft.xce.conio;

namespace test
{

    public class test3_reader : ConsoleInputListener
    {
        public bool stop = false;

        public void onKeyPressed(int scanCode, char character, bool shift, bool ctrl, bool alt)
        {
            if (character >= ' ')
                Console.WriteLine("{0:x}({1}) '{2}'", scanCode, ConsoleInput.keyCodeToName(scanCode), character);
            else
                Console.WriteLine("{0:x}({1})", scanCode, ConsoleInput.keyCodeToName(scanCode));
            if (scanCode == 0x1b)
                stop = true;
        }
        public void onKeyReleased(int scanCode, char character, bool shift, bool ctrl, bool alt)
        {
        }
        public void onMouseMove(int row, int column, bool shift, bool ctrl, bool alt, bool lb, bool rb)
        {
        }
        public void onMouseLButtonDown(int row, int column, bool shift, bool ctrl, bool alt)
        {
        }
        public void onMouseLButtonUp(int row, int column, bool shift, bool ctrl, bool alt)
        {
        }
        public void onMouseRButtonDown(int row, int column, bool shift, bool ctrl, bool alt)
        {
        }
        public void onMouseRButtonUp(int row, int column, bool shift, bool ctrl, bool alt)
        {
        }
        public void onMouseWheelUp(int row, int column, bool shift, bool ctrl, bool alt)
        {
        }
        public void onMouseWheelDown(int row, int column, bool shift, bool ctrl, bool alt)
        {
        }
        public void onGetFocus(bool shift, bool ctrl, bool alt)
        {
        }
        public void onScreenBufferChanged(int rows, int columns)
        {
        }
    };



    class test3
    {
        static internal void dotest()
        {
            ConsoleInput input = new ConsoleInput();
            test3_reader reader = new test3_reader();

            while (!reader.stop)
                input.read(reader, -1);

            input.Dispose();
        }
    }
}
