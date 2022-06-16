using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;


namespace gehtsoft.xce.spellcheck.hunspell
{
    class HunspellMarshal
    {
        [DllImport("Hunspellx86.dll")]
        internal static extern bool HunspellAdd(IntPtr handle, IntPtr word);

        [DllImport("Hunspellx86.dll")]
        internal static extern void HunspellFree(IntPtr handle);

        [DllImport("Hunspellx86.dll")]
        internal static extern bool HunspellSpell(IntPtr handle, IntPtr word);

        [DllImport("Hunspellx86.dll")]
        internal static extern IntPtr HunspellSuggest(IntPtr handle, IntPtr word);

        [DllImport("Hunspellx86.dll")]
        internal static extern IntPtr HunspellInit([MarshalAs(UnmanagedType.LPArray)] byte[] affixData, IntPtr affixDataSize, [MarshalAs(UnmanagedType.LPArray)] byte[] dictionaryData, IntPtr dictionaryDataSize, string key);
    }
}
