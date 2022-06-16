using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.spellcheck;

namespace gehtsoft.xce.spellcheck.hunspell
{
    public class HunspellFactory : ISpellcheckerFactory
    {
        private Dictionary<string, HunspellDictionary> mDictionaries = new Dictionary<string, HunspellDictionary>();
    
        public ISpellchecker CreateInstance(string dictionaryPath, string dictionary, string customDictionary)
        {
            HunspellDictionary dict;
            if (!mDictionaries.TryGetValue(dictionary, out dict))
            {
                dict = new HunspellDictionary(dictionary, dictionaryPath, dictionary, customDictionary);
                mDictionaries[dictionary] = dict;
            }
            return dict;
        }
        
        public void Dispose()
        {
            foreach (KeyValuePair<string, HunspellDictionary> pair in mDictionaries)
                pair.Value.Dispose();
            mDictionaries.Clear();
        }
    }
}
