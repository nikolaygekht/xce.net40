using System;
using System.Collections.Generic;
using System.Text;

namespace gehtsoft.xce.spellcheck
{
    public interface ISpellchecker 
    {
        string Name
        {
            get;
        }
        bool Spellcheck(string word);
        bool Spellcheck(char[] buffer, int from, int length);
        ISpellcheckerSuggestions Suggest(string word);
        ISpellcheckerSuggestions Suggest(char[] buffer, int from, int length);
    }
}
