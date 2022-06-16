using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace gehtsoft.xce.spellcheck.csapi
{
    /// <summary>
    /// Native CS API 
    /// </summary>
    internal class CSAPI
    {
        #region Declarations
        internal struct SpellInputBufferType
        {
            internal short cch;                  //  total characters in buffer area
            internal short cMdr;                 //  count of MDR's specified in lrgMdr
            internal short cUdr;                 //  count of UDR's specified in lrgUdr - should not reference Exclusion UDR's
            internal short wSpellState;          //  state relative to previous spellCheck() call
            internal IntPtr lrgch;               //  pointer to buffer area of text to be spell checked
            internal IntPtr lrgMdr;              //  list of main dictionaries to use when spelling the buffer
            internal IntPtr lrgUdr;              //  list of user dictionaries to use when spelling the buffer - should not reference Exclusion UDR's
            
            internal void clear()
            {
                cch = 0;
                cMdr = 0;
                cUdr = 0;
                wSpellState = 0;
                lrgch = IntPtr.Zero;
                lrgMdr = IntPtr.Zero;
                lrgUdr = IntPtr.Zero;
            }
        };

        internal struct SpellReturnBufferType
        {
            internal short ichError;             //  position in the SIB
            internal short cchError;             //  length of error "word" in SIB
            internal short scrs;                 //  spell check return status
            internal short csz;                  //  count of sz's put in buffer
            internal short cchMac;               //  current total of chars in buffer
            internal short cch;                  //  total space in lrgch. Set by App.
            internal IntPtr lrgsz;               //  pointer to alternatives, null delimited
            internal IntPtr lrgbRating;          //  pointer to rating value for each suggestion returned
            internal short cbRate;               //  number of elements in lrgbRating
            
            internal void clear()
            {
                ichError = 0;
                cchError = 0;
                scrs = 0;
                csz = 0;
                cchMac = 0;
                cch = 0;
                lrgsz = IntPtr.Zero;
                lrgbRating = IntPtr.Zero;
                cbRate = 0;
            }
        };

        internal struct MdrType 
        {
            internal IntPtr Mdr;
            internal IntPtr Lid;
            internal IntPtr Udr;
        };

        internal struct  WizSpecCharsType 
        {
            byte bIgnore;
            byte bHyphenHard;
            byte bHyphenSoft;
            byte bHyphenNonBreaking;
            byte bEmDash;
            byte bEnDash;
            byte bEllipsis;
            byte rgLineBreak1;
            byte rgLineBreak2;
            byte rgParaBreak1;
            byte rgParaBreak2;
            
            internal static WizSpecCharsType Create()
            {
                WizSpecCharsType n = new WizSpecCharsType();
                n.bIgnore = 0;
                n.bHyphenHard = 45;
                n.bHyphenSoft = 31;
                n.bHyphenNonBreaking = 30;
                n.bEmDash = 151;
                n.bEnDash = 150;
                n.bEllipsis = 133;
                n.rgLineBreak1 = 11;
                n.rgLineBreak2 = 10;
                n.rgParaBreak1 = 13;
                n.rgParaBreak2 = 10;
                return n;
            }
         };

        [Flags]
        internal enum fss : ushort
        {
            fssNoStateInfo  = 0,
            fssIsContinued  = 1,
            fssStartsSentence  = 2,
            fssIsEditedChange = 4,
        };

        //Ram Cache User Dictionary Reference
        internal enum udr
        {
            udrChangeOnce = 0xFFFC,       //UDR reserved reference for Change Once List
            udrChangeAlways = 0xFFFD,     //UDR reserved reference for Change Always List
            udrIgnoreAlways = 0xFFFE,     //UDR reserved reference for Ignore Always List
        };

        //Spell ID Engine's
        internal enum sid
        {
            sidSA = 1,                      //SoftArt
            sidHM = 2                       //Houghton-Mifflin
        };

        //Spell Check Command Definitions
        internal enum scc
        {
            sccVerifyWord  = 1,
            sccVerifyBuffer = 2,
            sccSuggest = 3,
            sccSuggestMore = 4,
            sccHyphInfo = 5,
            sccWildcard = 6,
            sccAnagram = 7
        };

        //Spell Check return status identifiers
        internal enum scrs
        {
            scrsNoErrors = 0,               //all buffers processed
            scrsUnknownInputWord = 1,       //unknown word
            scrsNoMoreSuggestions = 8,      //no more suggestions
            scrsNoSentenceStartCap = 10,    //start of sentence was not captialized
            scrsRepeatWord = 11             //repeat word found
        };

        //Spell Options Bitfield Definitions
        internal enum so
        {
            soFindUncappedSentences = 0x10,
            soFindRepeatWord  = 0x40
        };

        //Common Speller
        internal enum sc
        {
            csChange = 1,
            csCancel = 2,
            csDone = 3,
            csError =  4
        };

        //Common Speller Return Status
        internal enum csrs
        {
            csrsChange = 1
        };
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void SpellVerDelegate(out int piSpellVersion, out int piSpellEngineType, out int piSpellFunctionality);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int SpellVerifyMdrDelegate ([MarshalAs(UnmanagedType.LPStr)] string szMainDictionary, int iLidExpected, out int piLanguageIdentifier);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int SpellInitDelegate(out int piSpellIdentifier, ref WizSpecCharsType pWcs);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int SpellOpenMdrDelegate (int iSpellIdentifier, [MarshalAs(UnmanagedType.LPStr)] string szMainDictionary, [MarshalAs(UnmanagedType.LPStr)] string ExclusionDictionary, int iCreateUdrFlag, int iCache, int iLidExpected, ref MdrType pMainDictionaryReference);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int SpellOpenUdrDelegate (int iSpellIdentifier, [MarshalAs(UnmanagedType.LPStr)] string szUserDictionary, int createUdrFlag, int udrPropertyType, int udrReferenceId, int udrReadOnlyFlag);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int SpellOptionsDelegate (int iSpellIdentifier, int iSpellOption);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int SpellCheckDelegate (int iSpellIdentifier, int iSpellCheckCommand, ref SpellInputBufferType pInput, ref SpellReturnBufferType pOutput);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int SpellAddChangeUdrDelegate (int iSpellIdentifier, int iudrReferenceId , [MarshalAs(UnmanagedType.LPStr)] string szStringToReplace, [MarshalAs(UnmanagedType.LPStr)]string szStringToReplaceWith);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int SpellTerminateDelegate (int iSpellIdentifier, int ifForce);
        
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32.dll")]
        internal static extern IntPtr LoadLibrary(string fileName);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);
        [DllImport("kernel32.dll")]
        private static extern int lstrlenA(int buffer);

        #endregion
        
        #region Methods
        private IntPtr mModule = IntPtr.Zero;
        private int mSpellSession = -1;
        private MdrType mMdr = new MdrType();
        private Encoding mDictionaryEncoding;
        private SpellVerDelegate mSpellVer;
        private SpellVerifyMdrDelegate mSpellVerifyMdr;
        private SpellInitDelegate mSpellInit;
        private SpellOpenMdrDelegate mSpellOpenMdr;
        private SpellOpenUdrDelegate mSpellOpenUdr;
        private SpellOptionsDelegate mSpellOptions;
        private SpellCheckDelegate mSpellCheck;
        private SpellAddChangeUdrDelegate mSpellAddChangeUdr;
        private SpellTerminateDelegate mSpellTerminate;
        private SpellInputBufferType mInput;
        private SpellReturnBufferType mOutput;
        private byte[] mBuffer = null;
        private byte[] mBuffer1 = new byte[4096];
        private byte[] mBuffer2 = new byte[4096];
        IntPtr[] mMdrArray = new IntPtr[1];
        private object mMutex = new object();
        #endregion    
        
        internal object Mutex
        {
            get
            {
                return mMutex;
            }
        }    
        
        internal void open(string dictionary)
        {
            close();
        
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");
            int lid = 0;
            if (dictionary.Equals("english", StringComparison.InvariantCultureIgnoreCase))
            {
                lid = 1033;
                mDictionaryEncoding = Encoding.GetEncoding(1252);
            }
            else if (dictionary.Equals("russian", StringComparison.InvariantCultureIgnoreCase))
            {
                lid = 1049;
                mDictionaryEncoding = Encoding.GetEncoding(1251);
            }
            else
                throw new ArgumentOutOfRangeException("dictionary");
            
            string key = String.Format("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Shared Tools\\Proofing Tools\\Spelling\\{0}\\Normal", lid);
            string path = Registry.GetValue(key, "Engine", null) as string;
            if (path == null)
                throw new Exception("CSAPI for the language chosen is not installed");
            string dictFile = Registry.GetValue(key, "Dictionary", null) as string;
            if (dictFile == null)
                throw new Exception("CSAPI for the language chosen is not installed");
                            
            mModule = LoadLibrary(path);
            if (mModule == IntPtr.Zero)
                throw new Exception("CSAPI module cannot be loaded");

            mSpellInit = (SpellInitDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModule, "SpellInit"), typeof(SpellInitDelegate));
            mSpellOpenMdr = (SpellOpenMdrDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModule, "SpellOpenMdr"), typeof(SpellOpenMdrDelegate));
            mSpellCheck = (SpellCheckDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModule, "SpellCheck"), typeof(SpellCheckDelegate));
            mSpellTerminate = (SpellTerminateDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModule, "SpellTerminate"), typeof(SpellTerminateDelegate));

            
            WizSpecCharsType wsct = WizSpecCharsType.Create();
            mSpellInit(out mSpellSession, ref wsct);

            mMdr.Lid = IntPtr.Zero;
            mMdr.Mdr = IntPtr.Zero;
            mMdr.Udr = IntPtr.Zero;

            mSpellOpenMdr(mSpellSession, dictFile, null, 0, 0, lid, ref mMdr);
            
            mMdrArray[0] = mMdr.Mdr;
        }        
        
        internal void close()
        {
            if (mSpellSession != -1)
                mSpellTerminate(mSpellSession, 1);
            mSpellSession = -1;
            if (mModule != IntPtr.Zero)
                FreeLibrary(mModule);
            mModule = IntPtr.Zero;
        }
        
        internal bool spell(string word)
        {
            int length = mDictionaryEncoding.GetByteCount(word);
            if (mBuffer == null || mBuffer.Length < length)
                mBuffer = new byte[((length / 128) + 1) * 128];
            mDictionaryEncoding.GetBytes(word, 0, word.Length, mBuffer, 0);
            return spellBuffer(length);
        }
        
        internal bool spell(char[] buffer, int index, int _length)
        {
            int length = mDictionaryEncoding.GetByteCount(buffer, index, _length);
            if (mBuffer == null || mBuffer.Length < length)
                mBuffer = new byte[((length / 128) + 1) * 128];
            mDictionaryEncoding.GetBytes(buffer, index, _length, mBuffer, 0);
            return spellBuffer(length);
        }
        
        private bool spellBuffer(int length)
        {
            mInput.clear();
            mOutput.clear();
            
            GCHandle gch = GCHandle.Alloc(mBuffer);
            GCHandle gch1 = GCHandle.Alloc(mMdrArray);
            GCHandle gch3 = GCHandle.Alloc(mBuffer1);
            GCHandle gch4 = GCHandle.Alloc(mBuffer2);
            try
            {
                mInput.cMdr = 1;
                mInput.cUdr = 0;
                mInput.cch = (short)length;
                mInput.lrgch = Marshal.UnsafeAddrOfPinnedArrayElement(mBuffer, 0);
                mInput.lrgMdr = Marshal.UnsafeAddrOfPinnedArrayElement(mMdrArray, 0);
                mInput.lrgUdr = Marshal.UnsafeAddrOfPinnedArrayElement(mMdrArray, 0);
                
                mOutput.cch = 4096;
                mOutput.lrgsz = Marshal.UnsafeAddrOfPinnedArrayElement(mBuffer1, 0);
                mOutput.cbRate = 4096;
                mOutput.lrgbRating = Marshal.UnsafeAddrOfPinnedArrayElement(mBuffer2, 0);
                
                mSpellCheck(mSpellSession, (int)scc.sccVerifyWord, ref mInput, ref mOutput);
                
                return mOutput.scrs == 0;
            }
            finally
            {
                gch.Free();
                gch1.Free();
                gch3.Free();
                gch4.Free();
            }
        }

        internal CSAPISuggestions suggest(string word)
        {
            int length = mDictionaryEncoding.GetByteCount(word);
            if (mBuffer == null || mBuffer.Length < length)
                mBuffer = new byte[((length / 128) + 1) * 128];
            mDictionaryEncoding.GetBytes(word, 0, word.Length, mBuffer, 0);
            return suggestBuffer(length);
        }

        internal CSAPISuggestions suggest(char[] buffer, int index, int _length)
        {
            int length = mDictionaryEncoding.GetByteCount(buffer, index, _length);
            if (mBuffer == null || mBuffer.Length < length)
                mBuffer = new byte[((length / 128) + 1) * 128];
            mDictionaryEncoding.GetBytes(buffer, index, _length, mBuffer, 0);
            return suggestBuffer(length);
        }


        private CSAPISuggestions suggestBuffer(int length)
        {
            CSAPISuggestions suggestions = new CSAPISuggestions();
        
            mInput.clear();
            mOutput.clear();

            GCHandle gch = GCHandle.Alloc(mBuffer);
            GCHandle gch1 = GCHandle.Alloc(mMdrArray);
            GCHandle gch3 = GCHandle.Alloc(mBuffer1);
            GCHandle gch4 = GCHandle.Alloc(mBuffer2);
            try
            {
                mInput.cMdr = 1;
                mInput.cUdr = 0;
                mInput.cch = (short)length;
                mInput.lrgch = Marshal.UnsafeAddrOfPinnedArrayElement(mBuffer, 0);
                mInput.lrgMdr = Marshal.UnsafeAddrOfPinnedArrayElement(mMdrArray, 0);
                mInput.lrgUdr = Marshal.UnsafeAddrOfPinnedArrayElement(mMdrArray, 0);

                mOutput.cch = 4096;
                mOutput.lrgsz = Marshal.UnsafeAddrOfPinnedArrayElement(mBuffer1, 0);
                mOutput.cbRate = 4096;
                mOutput.lrgbRating = Marshal.UnsafeAddrOfPinnedArrayElement(mBuffer2, 0);

                mSpellCheck(mSpellSession, (int)scc.sccSuggest, ref mInput, ref mOutput);
                
                while (mOutput.scrs == 0)
                {
                    int buff = mOutput.lrgsz.ToInt32();
                    int offset = 0;
                    int curr_length;
                    
                    while (offset < mOutput.csz)
                    {
                        curr_length = lstrlenA(buff + offset);
                        if (curr_length <= 0)
                            break;
                        suggestions.Add(mDictionaryEncoding.GetString(mBuffer1, offset, curr_length));
                        offset += curr_length + 1;
                    }
                    mSpellCheck(mSpellSession, (int)scc.sccSuggestMore, ref mInput, ref mOutput);
                }
                return suggestions;
            }
            finally
            {
                gch.Free();
                gch1.Free();
                gch3.Free();
                gch4.Free();
            }
        }

    }
}
