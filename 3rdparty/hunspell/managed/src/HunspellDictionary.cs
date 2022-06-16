using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using gehtsoft.xce.spellcheck;

namespace gehtsoft.xce.spellcheck.hunspell
{
    class HunspellDictionary : ISpellchecker, IDisposable
    {
        IntPtr mNativeHandle;
        private string mName;
        IntPtr mNativeBuffer = IntPtr.Zero;
        int mNativeBufferSize = 0;

        public string Name
        {
            get
            {
                return mName;
            }
        }

        internal HunspellDictionary(string name, string dictionaryPath, string dictionary, string customDictionary)
        {
            mName = name;

            string affPath = Path.GetFullPath(dictionaryPath + dictionary + ".aff");
            if (!File.Exists(affPath))
                throw new FileNotFoundException(affPath);
            string dicPath = Path.GetFullPath(dictionaryPath + dictionary + ".dic");
            if (!File.Exists(affPath))
                throw new FileNotFoundException(affPath);

            byte[] affixData;
            using (FileStream stream = File.OpenRead(affPath))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    affixData = reader.ReadBytes((int)stream.Length);
                }
            }

            byte[] dictionaryData;
            using (FileStream stream = File.OpenRead(dicPath))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    dictionaryData = reader.ReadBytes((int)stream.Length);
                }
            }

            mNativeHandle = HunspellMarshal.HunspellInit(affixData, new IntPtr(affixData.Length), dictionaryData, new IntPtr(dictionaryData.Length), null);

            if (mNativeHandle != null)
            {
                string customPath = Path.GetFullPath(dictionaryPath + customDictionary + ".cdic");
                if (File.Exists(customPath))
                {
                    using (StreamReader stream = new StreamReader(customPath, Encoding.UTF8))
                    {
                        string s;
                        while ((s = stream.ReadLine()) != null)
                        {
                            s = s.Trim();
                            if (s.Length != 0)
                                HunspellMarshal.HunspellAdd(mNativeHandle, Word(s));
                        }
                    }
                }
            }
        }

        public bool Spellcheck(string word)
        {
            return Spellcheck(word.ToCharArray(), 0, word.Length);
        }

        public bool Spellcheck(char[] buffer, int from, int length)
        {
            if (mNativeHandle == IntPtr.Zero)
                throw new InvalidOperationException();
            return HunspellMarshal.HunspellSpell(mNativeHandle, Word(buffer, from, length));
        }

        public ISpellcheckerSuggestions Suggest(string word)
        {
            return Suggest(word.ToCharArray(), 0, word.Length);
        }

        public ISpellcheckerSuggestions Suggest(char[] buffer, int from, int length)
        {
            if (mNativeHandle == IntPtr.Zero)
                throw new InvalidOperationException();
            HunspellSuggestions suggestions = new HunspellSuggestions();
            IntPtr strings = HunspellMarshal.HunspellSuggest(mNativeHandle, Word(buffer, from, length));
            int stringCount = 0;
            IntPtr currentString = Marshal.ReadIntPtr(strings, stringCount * IntPtr.Size);

            while (currentString != IntPtr.Zero)
            {
                ++stringCount;
                suggestions.Add(Marshal.PtrToStringUni(currentString));
                currentString = Marshal.ReadIntPtr(strings, stringCount * IntPtr.Size);
            }

            return suggestions;
        }

        public void Dispose()
        {
            if (mNativeHandle != IntPtr.Zero)
            {
                HunspellMarshal.HunspellFree(mNativeHandle);
                mNativeHandle = IntPtr.Zero;
            }
            if (mNativeBuffer != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(mNativeBuffer);
                mNativeBuffer = IntPtr.Zero;
            }
            return ;
        }

        private IntPtr Word(char[] buffer, int offset, int length)
        {
            if (mNativeBuffer == IntPtr.Zero || mNativeBufferSize == 0)
            {
                int newlength = (length + 1) * 2;
                newlength = (newlength / 1024 + 1) * 1024;
                if (mNativeBuffer != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(mNativeBuffer);
                mNativeBuffer = Marshal.AllocCoTaskMem(newlength);
                mNativeBufferSize = newlength;
            }
            Marshal.Copy(buffer, offset, mNativeBuffer, length);
            Marshal.WriteInt16(mNativeBuffer, length * 2, 0);
            return mNativeBuffer;
        }

        private IntPtr Word(string value)
        {
            return Word(value.ToCharArray(), 0, value.Length);
        }

    }
}
