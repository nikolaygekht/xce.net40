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
            string assemblyPath = Path.GetFullPath("gehtsoft.xce.spellcheck.hunspell.dll");
            ISpellcheckerFactory factory = SpellcheckerFactoryLoader.LoadFactory(assemblyPath, "gehtsoft.xce.spellcheck.hunspell.HunspellFactory");
            ISpellchecker checker = factory.CreateInstance("..\\..\\..\\data\\", "en_US", "custom_en");
            Console.WriteLine("{0}", checker.Name);
            check(checker, "retrieve");
            check(checker, "coopeation");
            check(checker, "retreive");
            check(checker, "silmuntoneous");
            check(checker, "gehtsoft");
            check(checker, "ghetsfot");
            factory.Dispose();
        }

        private static void check(ISpellchecker checker, string word)
        {
            Console.WriteLine("{0}", word);
            if (checker.Spellcheck(word))
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
