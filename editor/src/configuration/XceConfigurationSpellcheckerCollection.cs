using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.spellcheck;

namespace gehtsoft.xce.editor.configuration
{
    public class XceConfigurationSpellcheckerCollection : IEnumerable<ISpellchecker>
    {
        List<ISpellchecker> mSpellCheckers = new List<ISpellchecker>();
        
        public int Count
        {
            get
            {
                return mSpellCheckers.Count;
            }
        }
        
        public ISpellchecker this[int index]
        {
            get
            {
                return mSpellCheckers[index];
            }
        }
        
        public ISpellchecker this[string title]
        {
            get
            {
                if (title == null)
                    return null;
                foreach (ISpellchecker c in mSpellCheckers)
                {
                    if (c.Name == title)
                        return c;
                }
                return null;
            }
        }
        
        internal void Add(ISpellchecker c)
        {
            mSpellCheckers.Add(c);
        }
        
        public IEnumerator<ISpellchecker> GetEnumerator()
        {
            return mSpellCheckers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mSpellCheckers.GetEnumerator();
        }
        
    }
}
