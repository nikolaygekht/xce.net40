using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gehtsoft.xce.conio;

using ConsoleColor = gehtsoft.xce.conio.ConsoleColor;

namespace test
{
    class test2
    {
        static internal void dotest()
        {
            Console.ReadLine();
            ConsoleOutput output = new ConsoleOutput(true);
            Canvas canvas = new Canvas(output.Rows, output.Columns);

            canvas.fill(0, 0, canvas.Rows, canvas.Columns, ' ', new ConsoleColor(0x03));
            canvas.box(9, 9, 7, 7, BoxBorder.DoubleBorderBox, new ConsoleColor(0x01, ConsoleColor.rgb(255, 174, 201), ConsoleColor.rgb(128, 128, 255)));
            canvas.box(10, 10, 5, 5, BoxBorder.SingleBorderBox, new ConsoleColor(0x04));
            canvas.write(0, -5, "1234567890", new ConsoleColor(0x04));
            canvas.write(0, canvas.Columns - 5, "1234567890", new ConsoleColor(0x04));
            canvas.write(2, 0, "style0", new ConsoleColor(0x03, ConsoleColor.rgb(44, 162, 232), ConsoleColor.rgb(255, 174, 201), 0));
            canvas.write(3, 0, "style1", new ConsoleColor(0x03, ConsoleColor.rgb(0, 80, 232), 0, 1));
            canvas.write(4, 0, "style2", new ConsoleColor(0x03, ConsoleColor.rgb(50, 12, 232), 0, 2));
            canvas.write(5, 0, "style4", new ConsoleColor(0x03, ConsoleColor.rgb(0, 162, 232), 0, 4));
            canvas.write(6, 0, "stylea", new ConsoleColor(0x03, ConsoleColor.rgb(0, 162, 232), 0, 1 + 2 + 4));
            output.paint(canvas, false);
            Console.ReadLine();
        }
    }
}
