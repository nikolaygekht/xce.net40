using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.configuration;

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
            }
        }
        
        static bool check(bool condition)
        {
            if (condition)
                Console.WriteLine("ok");
            else
                Console.WriteLine("failed");
            return condition;
        }
        
        static void test1()
        {
            Profile profile = new Profile();
            profile.Load("..\\..\\xce.ini");
            check(profile["common", "menu"].Trim() == "geht");
            check(profile["common", "search", 1].Trim() == "step2");
            profile["common", "search", 2] = "step3";
            profile.Save("xce.ini.bak");
            
        }
    }
}
