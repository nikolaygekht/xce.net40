using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.colorer;

namespace test
{
    class Program
    {
        /// <summary>
        /// Entry point!
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            foreach (string s in args)
            {
                if (s == "1")
                    Test1();
                else if (s == "2")
                    Test2();
            }
        }

        static void Test2()
        {
            Regex regex = new Regex("/(\\w+)/");
            string s = "  abc def ijk";
            bool rc = regex.Match(s);
            
            if (!rc)
                Console.WriteLine("No matches");
            
            while (rc)
            {
                Console.WriteLine("{0} matches", regex.MatchesCount);
                for (int i = 0; i < regex.MatchesCount; i++)
                    Console.WriteLine("{0}-{1} {2}", regex.Start(i), regex.Length(i), regex.Value(i));
                rc = regex.NextMatch();
            }
            regex.Dispose();
        }


        static void Test1()
        {
            DummySource source = new DummySource("..\\..\\src\\DummySource.cs");
            ColorerFactory factory = new ColorerFactory();
            factory.Init("..\\..\\..\\..\\native\\data\\catalog.xml", "console", "black", 2000);

            StyledRegion defText = factory.FindStyledRegion("def:Text");
            Console.WriteLine("{0:x}", defText.ConsoleColor);

            SyntaxRegion textRegion = factory.FindSyntaxRegion("def:Text");
            SyntaxRegion dsTextRegion = factory.FindSyntaxRegion("ds:text");
            SyntaxRegion commentRegion = factory.FindSyntaxRegion("def:Comment");
            SyntaxRegion pairStartRegion = factory.FindSyntaxRegion("def:PairStart");
            Console.WriteLine("{0}", dsTextRegion.Equals(textRegion));
            Console.WriteLine("{0}", dsTextRegion.IsDerivedFrom(textRegion));
            SyntaxHighlighter highlighter = factory.CreateHighlighter(source);
            highlighter.Colorize(0, source.GetLinesCount());
            for (int i = 0; i < source.GetLinesCount(); i++)
            {
                Console.WriteLine("{0:00}:{1}", i, source.GetLine(i));
                bool rc = highlighter.GetFirstRegion(i);
                while (rc)
                {
                    Console.Write("  {0}-{1} {2}", highlighter.CurrentRegion.StartColumn, highlighter.CurrentRegion.Length, highlighter.CurrentRegion.Name);
                    if (highlighter.CurrentRegion.Is(commentRegion))
                        Console.Write(" a comment");
                    if (highlighter.CurrentRegion.Is(pairStartRegion))
                    {
                        SyntaxHighlighterPair pair = highlighter.MatchPair(i, highlighter.CurrentRegion.StartColumn);
                        if (pair != null)
                        {
                            Console.Write(" matched to {0}:{1}-{2}:{3}", pair.Start.Line, pair.Start.StartColumn, pair.End.Line, pair.End.EndColumn);
                            pair.Dispose();
                        }
                    }
                    rc = highlighter.GetNextRegion();
                }
                Console.WriteLine();

            }
            highlighter.Dispose();
            factory.Dispose();

        }
    }
}
