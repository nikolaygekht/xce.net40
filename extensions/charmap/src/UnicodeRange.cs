using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace gehtsoft.xce.extension.charmap_impl
{
    internal class UnicodeRange
    {
        private string mName;
        private char mFrom;
        private char mTo;
        
        internal string Name
        {
            get
            {
                return mName;
            }
        }
        
        internal char From
        {
            get
            {
                return mFrom;
            }
        }

        internal char To
        {
            get
            {
                return mTo;
            }
        }
        
        internal UnicodeRange(string name, char from, char to)
        {
            mName = name;
            mFrom = from;
            mTo = to;
        }
        
        internal int Length
        {
            get
            {
                return (int)(mTo - mFrom + 1);
            }
        }
    }
    
    internal class UnicodeRangeCollection : IEnumerable<UnicodeRange>
    {
        List<UnicodeRange> mList = new List<UnicodeRange>();
        
        public IEnumerator<UnicodeRange> GetEnumerator()
        {
            return mList.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return mList.GetEnumerator();
        }
        
        internal int Count
        {
            get
            {
                return mList.Count;
            }
        }
        
        internal UnicodeRange this[int index]
        {
            get
            {
                return mList[index];
            }
        }
        
        private void Add(UnicodeRange range)
        {
            mList.Add(range);
        }
        
        internal UnicodeRangeCollection()
        {
            Add(new UnicodeRange("C0 Controls and Basic Latin (Basic Latin)", (char)0x0020, (char)0x007F));
            Add(new UnicodeRange("C1 Controls and Latin-1 Supplement", (char)0x0080, (char)0x00FF));
            Add(new UnicodeRange("Latin Extended-A", (char)0x0100, (char)0x017F));
            Add(new UnicodeRange("Latin Extended-B", (char)0x0180, (char)0x024F));
            Add(new UnicodeRange("IPA Extensions", (char)0x0250, (char)0x02AF));
            Add(new UnicodeRange("Spacing Modifier Letters", (char)0x02B0, (char)0x02FF));
            Add(new UnicodeRange("Combining Diacritical Marks", (char)0x0300, (char)0x036F));
            Add(new UnicodeRange("Greek and Coptic", (char)0x0370, (char)0x03FF));
            Add(new UnicodeRange("Cyrillic", (char)0x0400, (char)0x04FF));
            Add(new UnicodeRange("Cyrillic Supplement", (char)0x0500, (char)0x052F));
            Add(new UnicodeRange("Armenian", (char)0x0530, (char)0x058F));
            Add(new UnicodeRange("Hebrew", (char)0x0590, (char)0x05FF));
            Add(new UnicodeRange("Arabic", (char)0x0600, (char)0x06FF));
            Add(new UnicodeRange("Syriac", (char)0x0700, (char)0x074F));
            Add(new UnicodeRange("Arabic Supplement", (char)0x0750, (char)0x077F));
            Add(new UnicodeRange("Thaana", (char)0x0780, (char)0x07BF));
            Add(new UnicodeRange("NKo", (char)0x07C0, (char)0x07FF));
            Add(new UnicodeRange("Samaritan", (char)0x0800, (char)0x083F));
            Add(new UnicodeRange("Mandaic", (char)0x0840, (char)0x085F));
            Add(new UnicodeRange("Arabic Extended-A", (char)0x08A0, (char)0x08FF));
            Add(new UnicodeRange("Devanagari", (char)0x0900, (char)0x097F));
            Add(new UnicodeRange("Bengali", (char)0x0980, (char)0x09FF));
            Add(new UnicodeRange("Gurmukhi", (char)0x0A00, (char)0x0A7F));
            Add(new UnicodeRange("Gujarati", (char)0x0A80, (char)0x0AFF));
            Add(new UnicodeRange("Oriya", (char)0x0B00, (char)0x0B7F));
            Add(new UnicodeRange("Tamil", (char)0x0B80, (char)0x0BFF));
            Add(new UnicodeRange("Telugu", (char)0x0C00, (char)0x0C7F));
            Add(new UnicodeRange("Kannada", (char)0x0C80, (char)0x0CFF));
            Add(new UnicodeRange("Malayalam", (char)0x0D00, (char)0x0D7F));
            Add(new UnicodeRange("Sinhala", (char)0x0D80, (char)0x0DFF));
            Add(new UnicodeRange("Thai", (char)0x0E00, (char)0x0E7F));
            Add(new UnicodeRange("Lao", (char)0x0E80, (char)0x0EFF));
            Add(new UnicodeRange("Tibetan", (char)0x0F00, (char)0x0FFF));
            Add(new UnicodeRange("Myanmar", (char)0x1000, (char)0x109F));
            Add(new UnicodeRange("Georgian", (char)0x10A0, (char)0x10FF));
            Add(new UnicodeRange("Hangul Jamo", (char)0x1100, (char)0x11FF));
            Add(new UnicodeRange("Ethiopic", (char)0x1200, (char)0x137F));
            Add(new UnicodeRange("Ethiopic Supplement", (char)0x1380, (char)0x139F));
            Add(new UnicodeRange("Cherokee", (char)0x13A0, (char)0x13FF));
            Add(new UnicodeRange("Unified Canadian Aboriginal Syllabics", (char)0x1400, (char)0x167F));
            Add(new UnicodeRange("Ogham", (char)0x1680, (char)0x169F));
            Add(new UnicodeRange("Runic", (char)0x16A0, (char)0x16FF));
            Add(new UnicodeRange("Tagalog", (char)0x1700, (char)0x171F));
            Add(new UnicodeRange("Hanunoo", (char)0x1720, (char)0x173F));
            Add(new UnicodeRange("Buhid", (char)0x1740, (char)0x175F));
            Add(new UnicodeRange("Tagbanwa", (char)0x1760, (char)0x177F));
            Add(new UnicodeRange("Khmer", (char)0x1780, (char)0x17FF));
            Add(new UnicodeRange("Mongolian", (char)0x1800, (char)0x18AF));
            Add(new UnicodeRange("Unified Canadian Aboriginal Syllabics Extended", (char)0x18B0, (char)0x18FF));
            Add(new UnicodeRange("Limbu", (char)0x1900, (char)0x194F));
            Add(new UnicodeRange("Tai Le", (char)0x1950, (char)0x197F));
            Add(new UnicodeRange("Tai Lue", (char)0x1980, (char)0x19DF));
            Add(new UnicodeRange("Khmer Symbols", (char)0x19E0, (char)0x19FF));
            Add(new UnicodeRange("Buginese", (char)0x1A00, (char)0x1A1F));
            Add(new UnicodeRange("Tai Tham", (char)0x1A20, (char)0x1AAF));
            Add(new UnicodeRange("Balinese", (char)0x1B00, (char)0x1B7F));
            Add(new UnicodeRange("Sundanese", (char)0x1B80, (char)0x1BBF));
            Add(new UnicodeRange("Batak", (char)0x1BC0, (char)0x1BFF));
            Add(new UnicodeRange("Lepcha", (char)0x1C00, (char)0x1C4F));
            Add(new UnicodeRange("Ol Chiki", (char)0x1C50, (char)0x1C7F));
            Add(new UnicodeRange("Sundanese Supplement", (char)0x1CC0, (char)0x1CCF));
            Add(new UnicodeRange("Vedic Extensions", (char)0x1CD0, (char)0x1CFF));
            Add(new UnicodeRange("Phonetic Extensions", (char)0x1D00, (char)0x1D7F));
            Add(new UnicodeRange("Phonetic Extensions Supplement", (char)0x1D80, (char)0x1DBF));
            Add(new UnicodeRange("Combining Diacritical Marks Supplement", (char)0x1DC0, (char)0x1DFF));
            Add(new UnicodeRange("Latin extended additional", (char)0x1E00, (char)0x1EFF));
            Add(new UnicodeRange("Greek Extended", (char)0x1F00, (char)0x1FFF));
            Add(new UnicodeRange("General Punctuation", (char)0x2000, (char)0x206F));
            Add(new UnicodeRange("Superscripts and Subscripts", (char)0x2070, (char)0x209F));
            Add(new UnicodeRange("Currency Symbols", (char)0x20A0, (char)0x20CF));
            Add(new UnicodeRange("Combining Diacritical Marks for Symbols", (char)0x20D0, (char)0x20FF));
            Add(new UnicodeRange("Letterlike Symbols", (char)0x2100, (char)0x214F));
            Add(new UnicodeRange("Number Forms", (char)0x2150, (char)0x218F));
            Add(new UnicodeRange("Arrows", (char)0x2190, (char)0x21FF));
            Add(new UnicodeRange("Mathematical Operators", (char)0x2200, (char)0x22FF));
            Add(new UnicodeRange("Miscellaneous Technical", (char)0x2300, (char)0x23FF));
            Add(new UnicodeRange("Control Pictures", (char)0x2400, (char)0x243F));
            Add(new UnicodeRange("Optical Character Recognition", (char)0x2440, (char)0x245F));
            Add(new UnicodeRange("Enclosed Alphanumerics", (char)0x2460, (char)0x24FF));
            Add(new UnicodeRange("Box Drawing", (char)0x2500, (char)0x257F));
            Add(new UnicodeRange("Block Elements", (char)0x2580, (char)0x259F));
            Add(new UnicodeRange("Geometric Shapes", (char)0x25A0, (char)0x25FF));
            Add(new UnicodeRange("Miscellaneous Symbols", (char)0x2600, (char)0x26FF));
            Add(new UnicodeRange("Dingbats", (char)0x2700, (char)0x27BF));
            Add(new UnicodeRange("Miscellaneous Mathematical Symbols-A", (char)0x27C0, (char)0x27EF));
            Add(new UnicodeRange("Supplemental Arrows-A", (char)0x27F0, (char)0x27FF));
            Add(new UnicodeRange("Braille Patterns", (char)0x2800, (char)0x28FF));
            Add(new UnicodeRange("Supplemental Arrows-B", (char)0x2900, (char)0x297F));
            Add(new UnicodeRange("Miscellaneous Mathematical Symbols-B", (char)0x2980, (char)0x29FF));
            Add(new UnicodeRange("Supplemental Mathematical Operators", (char)0x2A00, (char)0x2AFF));
            Add(new UnicodeRange("Miscellaneous Symbols and Arrows", (char)0x2B00, (char)0x2BFF));
            Add(new UnicodeRange("Glagolitic", (char)0x2C00, (char)0x2C5F));
            Add(new UnicodeRange("Latin Extended-C", (char)0x2C60, (char)0x2C7F));
            Add(new UnicodeRange("Coptic", (char)0x2C80, (char)0x2CFF));
            Add(new UnicodeRange("Georgian Supplement", (char)0x2D00, (char)0x2D2F));
            Add(new UnicodeRange("Tifinagh", (char)0x2D30, (char)0x2D7F));
            Add(new UnicodeRange("Ethiopic Extended", (char)0x2D80, (char)0x2DDF));
            Add(new UnicodeRange("Cyrillic Extended-A", (char)0x2DE0, (char)0x2DFF));
            Add(new UnicodeRange("Supplemental Punctuation", (char)0x2E00, (char)0x2E7F));
            Add(new UnicodeRange("CJK Radicals Supplement", (char)0x2E80, (char)0x2EFF));
            Add(new UnicodeRange("Kangxi Radicals", (char)0x2F00, (char)0x2FDF));
            Add(new UnicodeRange("Ideographic Description Characters", (char)0x2FF0, (char)0x2FFF));
            Add(new UnicodeRange("CJK Symbols and Punctuation", (char)0x3000, (char)0x303F));
            Add(new UnicodeRange("Hiragana", (char)0x3040, (char)0x309F));
            Add(new UnicodeRange("Katakana", (char)0x30A0, (char)0x30FF));
            Add(new UnicodeRange("Bopomofo", (char)0x3100, (char)0x312F));
            Add(new UnicodeRange("Hangul Compatibility Jamo", (char)0x3130, (char)0x318F));
            Add(new UnicodeRange("Kanbun", (char)0x3190, (char)0x319F));
            Add(new UnicodeRange("Bopomofo Extended", (char)0x31A0, (char)0x31BF));
            Add(new UnicodeRange("CJK Strokes", (char)0x31C0, (char)0x31EF));
            Add(new UnicodeRange("Katakana Phonetic Extensions", (char)0x31F0, (char)0x31FF));
            Add(new UnicodeRange("Enclosed CJK Letters and Months", (char)0x3200, (char)0x32FF));
            Add(new UnicodeRange("CJK Compatibility", (char)0x3300, (char)0x33FF));
            Add(new UnicodeRange("CJK Unified Ideographs Extension A", (char)0x3400, (char)0x4DBF));
            Add(new UnicodeRange("Yijing Hexagram Symbols", (char)0x4DC0, (char)0x4DFF));
            Add(new UnicodeRange("CJK Unified Ideographs", (char)0x4E00, (char)0x9FFF));
            Add(new UnicodeRange("Yi Syllables", (char)0xA000, (char)0xA48F));
            Add(new UnicodeRange("Yi Radicals", (char)0xA490, (char)0xA4CF));
            Add(new UnicodeRange("Lisu", (char)0xA4D0, (char)0xA4FF));
            Add(new UnicodeRange("Vai", (char)0xA500, (char)0xA63F));
            Add(new UnicodeRange("Cyrillic Extended-B", (char)0xA640, (char)0xA69F));
            Add(new UnicodeRange("Bamum", (char)0xA6A0, (char)0xA6FF));
            Add(new UnicodeRange("Modifier Tone Letters", (char)0xA700, (char)0xA71F));
            Add(new UnicodeRange("Latin Extended-D", (char)0xA720, (char)0xA7FF));
            Add(new UnicodeRange("Syloti Nagri", (char)0xA800, (char)0xA82F));
            Add(new UnicodeRange("Common Indic Number Forms", (char)0xA830, (char)0xA83F));
            Add(new UnicodeRange("Phags-pa", (char)0xA840, (char)0xA87F));
            Add(new UnicodeRange("Saurashtra", (char)0xA880, (char)0xA8DF));
            Add(new UnicodeRange("Devanagari Extended", (char)0xA8E0, (char)0xA8FF));
            Add(new UnicodeRange("Kayah Li", (char)0xA900, (char)0xA92F));
            Add(new UnicodeRange("Rejang", (char)0xA930, (char)0xA95F));
            Add(new UnicodeRange("Hangul Jamo Extended-A", (char)0xA960, (char)0xA97F));
            Add(new UnicodeRange("Javanese", (char)0xA980, (char)0xA9DF));
            Add(new UnicodeRange("Cham", (char)0xAA00, (char)0xAA5F));
            Add(new UnicodeRange("Myanmar Extended-A", (char)0xAA60, (char)0xAA7F));
            Add(new UnicodeRange("Tai Viet", (char)0xAA80, (char)0xAADF));
            Add(new UnicodeRange("Meetei Mayek Extensions", (char)0xAAE0, (char)0xAAFF));
            Add(new UnicodeRange("Ethiopic Extended-A", (char)0xAB00, (char)0xAB2F));
            Add(new UnicodeRange("Meetei Mayek", (char)0xABC0, (char)0xABFF));
            Add(new UnicodeRange("Hangul Syllables", (char)0xAC00, (char)0xD7AF));
            Add(new UnicodeRange("Hangul Jamo Extended-B", (char)0xD7B0, (char)0xD7FF));
            Add(new UnicodeRange("High Surrogates", (char)0xD800, (char)0xDB7F));
            Add(new UnicodeRange("High Private Use Surrogates", (char)0xDB80, (char)0xDBFF));
            Add(new UnicodeRange("Low Surrogates", (char)0xDC00, (char)0xDFFF));
            Add(new UnicodeRange("Private Use Area", (char)0xE000, (char)0xF8FF));
            Add(new UnicodeRange("CJK Compatibility Ideographs", (char)0xF900, (char)0xFAFF));
            Add(new UnicodeRange("Alphabetic Presentation Forms", (char)0xFB00, (char)0xFB4F));
            Add(new UnicodeRange("Arabic Presentation Forms-A", (char)0xFB50, (char)0xFDFF));
            Add(new UnicodeRange("Variation Selectors", (char)0xFE00, (char)0xFE0F));
            Add(new UnicodeRange("Vertical Forms", (char)0xFE10, (char)0xFE1F));
            Add(new UnicodeRange("Combining Half Marks", (char)0xFE20, (char)0xFE2F));
            Add(new UnicodeRange("CJK Compatibility Forms", (char)0xFE30, (char)0xFE4F));
            Add(new UnicodeRange("Small Form Variants", (char)0xFE50, (char)0xFE6F));
            Add(new UnicodeRange("Arabic Presentation Forms-B", (char)0xFE70, (char)0xFEFF));
            Add(new UnicodeRange("Halfwidth and Fullwidth Forms", (char)0xFF00, (char)0xFFEF));
            Add(new UnicodeRange("Specials", (char)0xFFF0, (char)0xFFFF));
        }
    }
}
