using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.spellcheck;

namespace gehtsoft.xce.spellcheck.csapi
{
    public class CSAPIFactory : ISpellcheckerFactory
    {
        private Dictionary<string, CSAPIDictionary> mDictionaries = new Dictionary<string, CSAPIDictionary>();

        public ISpellchecker CreateInstance(string dictionaryPath, string dictionary, string customDictionary)
        {
            CSAPIDictionary dict;
            if (!mDictionaries.TryGetValue(dictionary, out dict))
            {
                dict = new CSAPIDictionary(dictionary, dictionaryPath, dictionary, customDictionary);
                mDictionaries[dictionary] = dict;
            }
            return dict;
        }

        public void Dispose()
        {
            foreach (KeyValuePair<string, CSAPIDictionary> pair in mDictionaries)
                pair.Value.Dispose();
            mDictionaries.Clear();
        }
    }
}
