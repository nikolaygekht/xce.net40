using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.spellcheck;

namespace test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string assemblyPath = Path.GetFullPath("gehtsoft.xce.spellcheck.csapi.dll");
            ISpellcheckerFactory factory = SpellcheckerFactoryLoader.LoadFactory(assemblyPath, "gehtsoft.xce.spellcheck.csapi.CSAPIFactory");
            ISpellchecker checker = factory.CreateInstance("..\\..\\", "english", "custom_en.dic");
            check(checker, "retrieve");
            check(checker, "coopeation");
            check(checker, "n0taW0rD");
            check(checker, "retreive");
            check(checker, "silmuntoneous");
            check(checker, "gehtsoft");
            check(checker, "ghetsfot");
            factory.Dispose();
        }
        
        private static void check(ISpellchecker checker, string word)
        {
            Console.WriteLine("{0}", word);
            if (checker.Spellcheck(word.ToCharArray(), 0, word.Length))
                Console.WriteLine("  correct");
            else
            {
                Console.WriteLine("  not correct");
                ISpellcheckerSuggestions suggestions = checker.Suggest(word);
                foreach (string s in suggestions)
                    Console.WriteLine("     {0}", s);
            }
        }
    }
}
