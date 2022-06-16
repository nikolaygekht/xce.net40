using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using gehtsoft.xce.spellcheck;

namespace gehtsoft.xce.spellcheck.csapi
{
    class CSAPIDictionary : ISpellchecker, IDisposable
    {
        private string mName;
        private CSAPI mCSAPI;
        private List<string> mUserDictionary = new List<string>();

        public string Name
        {
            get
            {
                return mName;
            }
        }

        internal CSAPIDictionary(string name, string dictionaryPath, string dictionary, string customDictionary)
        {
            mName = name;

            mCSAPI = new CSAPI();
            mCSAPI.open(name);
            
            if (customDictionary != null && customDictionary.Length != 0 && dictionaryPath != null)
            {
                string customDictionaryPath = Path.GetFullPath(dictionaryPath + customDictionary);
                if (File.Exists(customDictionaryPath ))
                {
                    try
                    {
                        using (StreamReader reader = new StreamReader(customDictionaryPath, Encoding.UTF8))
                        {
                            string s;
                            while ((s = reader.ReadLine()) != null)
                            {
                                if (s.Length > 0)
                                    mUserDictionary.Add(s.ToUpper());
                            }
                            reader.Close();
                        }
                    }
                    catch (Exception )
                    {
                    
                    }
                }
            }
        }

        public bool Spellcheck(string word)
        {
            bool rc;
            lock (mCSAPI.Mutex)
                rc = mCSAPI.spell(word);
            if (!rc && mUserDictionary.Count > 0)
            {
                foreach (string s in mUserDictionary)
                    if (s.Equals(word, StringComparison.CurrentCultureIgnoreCase))
                        return true;
                return false;
            }
            return rc;
        }

        public bool Spellcheck(char[] buffer, int from, int length)
        {
            bool rc;
            lock (mCSAPI.Mutex)
                rc = mCSAPI.spell(buffer, from, length);
            if (!rc && mUserDictionary.Count > 0)
            {
                foreach (string s in mUserDictionary)
                {
                    if (s.Length != length)
                        continue;
                    bool rc1 = true;
                    for (int i = 0; i < length; i++)
                    {
                        if (Char.ToLower(buffer[from + i]) != Char.ToLower(s[i]))
                        {
                            rc1 = false;
                            break;
                        }
                    }
                    if (rc1)
                        return true;
                }
                return false;
            }
            return rc;
        }

        public ISpellcheckerSuggestions Suggest(string word)
        {
            lock (mCSAPI.Mutex)
                return mCSAPI.suggest(word);
        }

        public ISpellcheckerSuggestions Suggest(char[] buffer, int from, int length)
        {
            lock (mCSAPI.Mutex)
                return mCSAPI.suggest(buffer, from, length);
        }

        public void Dispose()
        {
            if (mCSAPI != null)
                mCSAPI.close();
            mCSAPI = null;
        }
    }
}
