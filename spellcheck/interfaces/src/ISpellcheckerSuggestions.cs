using System;
using System.Collections.Generic;
using System.Text;

namespace gehtsoft.xce.spellcheck
{
    public interface ISpellcheckerSuggestions : IEnumerable<string>
    {
        int Count
        {
            get;
        }
        
        string this[int index]
        {
            get;
        }
    }
}
