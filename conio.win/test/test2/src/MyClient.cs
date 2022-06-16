using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using ConsoleColor = gehtsoft.xce.conio.ConsoleColor;

namespace test1
{
    class MyClient : Window
    {
        public MyClient()
        {
        }

        public override void OnPaint(Canvas canvas)
        {
            canvas.fill(0, 0, Height, Width, ' ', new ConsoleColor(0x70, ConsoleColor.rgb(0, 0, 0), ConsoleColor.rgb(0, 128, 255)));
            canvas.write(0, 0, string.Format("{0}x{1}", Height, Width));
        }

        public override void OnSizeChanged()
        {
            invalidate();
        }
    }
}
