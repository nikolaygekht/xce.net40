using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace gehtsoft.xce.spellcheck
{
    public class SpellcheckerFactoryLoader
    {
        public static ISpellcheckerFactory LoadFactory(string assembly, string classname)
        {
            Assembly a = Assembly.LoadFile(assembly);
            object o = a.CreateInstance(classname);
            return o as ISpellcheckerFactory;
        }
    }
}
