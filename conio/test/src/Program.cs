using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (string s in args)
            {
                if (s == "1")
                    test1.dotest();
                if (s == "2")
                    test2.dotest();
                if (s == "3")
                    test3.dotest();

            }
        }
    }
}
