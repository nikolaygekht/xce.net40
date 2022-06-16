using System;
using System.Collections.Generic;
using System.Text;

namespace gehtsoft.xce.spellcheck
{
    public interface ISpellcheckerFactory : IDisposable
    {
        ISpellchecker CreateInstance(string dictionaryPath, string dictionary, string customDictionary);
    }
}
