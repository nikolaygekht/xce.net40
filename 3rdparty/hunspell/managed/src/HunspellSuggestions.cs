using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.spellcheck;

namespace gehtsoft.xce.spellcheck.hunspell
{
    class HunspellSuggestions : ISpellcheckerSuggestions
    {
        List<string> mList = new List<string>();
        
        internal HunspellSuggestions()
        {
        }
        
        internal void Add(string word)
        {
            mList.Add(word);
        }

        public int Count
        {
            get
            {
                return mList.Count;
            }
        }

        public string this[int index]
        {
            get
            {
                return mList[index];
            }
        }
        
        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mList.GetEnumerator();
        }
        
        
        
    }
}
