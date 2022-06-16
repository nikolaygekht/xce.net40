using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using ConsoleColor = gehtsoft.xce.conio.ConsoleColor;


namespace test1
{
    class MyModal : Window
    {
        MyWindow mChild;

        public MyModal(ConsoleColor windowColor)
        {
            mChild = new MyWindow(windowColor, true);
        }

        public override void OnCreate()
        {
            Program.WindowManager.create(mChild, this, 0, 0, 0, 0);
            Program.WindowManager.setFocus(mChild);
        }

        public override void OnShow(bool visible)
        {
            mChild.show(visible);
        }

        public override void OnSizeChanged()
        {
            mChild.move(0, 0, Height, Width);
        }




    }
}
