using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.scintilla;
using gehtsoft.xce.colorer;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (string s in args)
            {
                if (s == "1")
                    test1();
                else if (s == "2")
                    test2();
            }
        }

        static void test1()
        {
            CellBuffer buffer = new CellBuffer();
            buffer.InsertString(0, "Line1\r\n", 0, 7);
            buffer.InsertString(buffer.CharCount, "Line3\r\n", 0, 7);
            buffer.BeginUndoAction();
            buffer.InsertString(buffer.LineStart(1), "Line2\r\n", 0, 7);
            buffer.InsertString(buffer.LineStart(2), "Line2.1\r\n", 0, 9);
            buffer.EndUndoAction();
            buffer.DeleteChars(buffer.LineStart(2), buffer.LineLength(2));
            dump("final", buffer);
            while (buffer.CanUndo())
            {
                buffer.Undo();
                dump("undo", buffer);
            }
            while (buffer.CanRedo())
            {
                buffer.Redo();
                dump("redo", buffer);
            }
            buffer.Dispose();
        }

        static void dump(string title, CellBuffer buffer)
        {
            char[] crlf = "\r\n".ToCharArray();
            Console.WriteLine("{0}", title);
            for (int i = 0; i < buffer.LinesCount; i++)
            {
                string line = buffer.GetRange(buffer.LineStart(i), buffer.LineLength(i)).TrimEnd(crlf);
                Console.WriteLine("{0}:'{1}'", i, line);
            }
        }


        static void test2()
        {
            CellBuffer buffer = new CellBuffer();
            string text = "Line 1\r\nLine 2\r\nLine 3\r\nте2кст\r\nJust 2\r\nText 2";
            bool b = false;
            buffer.InsertString(0, text, 0, text.Length);

            IntPtr a1, a2, a3;
            buffer.GetNativeAccessor(out a1, out a2, out a3);

            Regex regex;
            bool rc;

            Console.WriteLine("r1");
            regex = new Regex("/(\\w+)/");
            rc = regex.Match(a1, a2, a3, 1, buffer.CharCount);
            while (rc)
            {
                for (int i = 1; i < regex.MatchesCount; i++)
                    Console.WriteLine("{0} {1}-{2} '{3}'", i, regex.Start(i), regex.Length(i), regex.Value(i));
                rc = regex.NextMatch();
            }
            regex.Dispose();

            Console.WriteLine("r2");
            regex = new Regex("/^(.+)$/m");
            rc = regex.Match(a1, a2, a3, 1, buffer.CharCount);
            while (rc)
            {
                for (int i = 1; i < regex.MatchesCount; i++)
                    Console.WriteLine("{0} {1}-{2} '{3}'", i, regex.Start(i), regex.Length(i), regex.Value(i));
                rc = regex.NextMatch();
            }
            regex.Dispose();
            Console.WriteLine("r2a");
            regex = new Regex("/^(.+)$/s");
            rc = regex.Match(a1, a2, a3, 0, buffer.CharCount);
            while (rc)
            {
                for (int i = 1; i < regex.MatchesCount; i++)
                    Console.WriteLine("{0} {1}-{2} '{3}'", i, regex.Start(i), regex.Length(i), regex.Value(i));
                rc = regex.NextMatch();
            }
            regex.Dispose();
            Console.WriteLine("r3");
            regex = new Regex("/(2)$+(l)/im");
            rc = regex.Match(a1, a2, a3, 0, buffer.CharCount);
            while (rc)
            {
                for (int i = 1; i < regex.MatchesCount; i++)
                    Console.WriteLine("{0} {1}-{2} '{3}'", i, regex.Start(i), regex.Length(i), regex.Value(i));
                rc = regex.NextMatch();
            }
            regex.Dispose();

            Console.WriteLine("r4");
            regex = new Regex("/\\b(\\d+)\\b/");
            rc = regex.Match(a1, a2, a3, 0, buffer.CharCount);
            while (rc)
            {
                for (int i = 1; i < regex.MatchesCount; i++)
                    Console.WriteLine("{0} {1}-{2} '{3}'", i, regex.Start(i), regex.Length(i), regex.Value(i));
                rc = regex.NextMatch();
            }
            regex.Dispose();

            buffer.Dispose();
        }
    }
}
