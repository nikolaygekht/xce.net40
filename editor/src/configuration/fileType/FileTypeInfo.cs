using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.colorer;

namespace gehtsoft.xce.editor.configuration
{
    public enum EOLMode
    {
        ForceWindows,
        ForceLinux,
        ForceMac,
    }

    public class FileTypeInfo : IDisposable
    {
        private Regex mFileTypeRe;
        private Encoding mEncoding;
        private List<SyntaxRegion> mSpellCheckRegions = new List<SyntaxRegion>();
        private bool mIgnoreBom;
        private bool mTrimEolSpace;
        private bool mAutoIndent;
        private int mTabSize;
        private bool mIgnoreReload;
        private string mDefaultSpellChecker;
        private EOLMode mEOLMode;

        public bool Match(string fileName)
        {
            if (mFileTypeRe == null)
                return false;
            else
                return mFileTypeRe.Match(fileName);
        }

        public Encoding Encoding
        {
            get
            {
                return mEncoding;
            }
        }

        internal FileTypeInfo(Regex re, Encoding enc, bool ignoreBom, bool trimEolSpace, int tabSize, bool autoIndent, bool ignoreReload, EOLMode eofMode)
        {
            mFileTypeRe = re;
            mEncoding = enc;
            mIgnoreBom = ignoreBom;
            mTrimEolSpace = trimEolSpace;
            mTabSize = tabSize;
            mAutoIndent = autoIndent;
            mIgnoreReload = ignoreReload;
            mEOLMode = eofMode;
        }

        internal void AddSyntaxRegion(SyntaxRegion region)
        {
            mSpellCheckRegions.Add(region);
        }

        public bool IsSpellCheckRegion(SyntaxHighlighterRegion region)
        {
            foreach (SyntaxRegion r in mSpellCheckRegions)
                if ((r == null && !region.IsSyntaxRegion) || (r != null && region.Is(r)))
                    return true;
            return false;
        }

        public bool NeedSpellcheck
        {
            get
            {
                return mSpellCheckRegions.Count != 0;
            }
        }

        public bool IgnoreBOM
        {
            get
            {
                return mIgnoreBom;
            }
        }

        public bool IgnoreReload
        {
            get
            {
                return mIgnoreReload;
            }
        }

        public bool TrimEolSpace
        {
            get
            {
                return mTrimEolSpace;
            }
        }

        public int TabSize
        {
            get
            {
                return mTabSize;
            }
        }

        public bool AutoIdent
        {
            get
            {
                return mAutoIndent;
            }
        }

        public string DefaultSpellChecker
        {
            get
            {
                return mDefaultSpellChecker;
            }
        }

        public EOLMode EOLMode
        {
            get
            {
                return mEOLMode;
            }
        }

        internal void SetDefaultSpellChecker(string spellChecker)
        {
            mDefaultSpellChecker = spellChecker;
        }

        protected void Dispose(bool disposal)
        {
            if (mFileTypeRe != null)
                mFileTypeRe.Dispose();
            mFileTypeRe = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FileTypeInfo()
        {
            Dispose(false);
        }
    }


    public class FileTypeInfoCollection : IEnumerable<FileTypeInfo>, IDisposable
    {
        private List<FileTypeInfo> mTypes = new List<FileTypeInfo>();
        FileTypeInfo mNeutralType;

        internal FileTypeInfoCollection(Encoding defaultEncoding, int defaultTabSize, bool autoIndent)
        {
            mNeutralType = new FileTypeInfo(null, defaultEncoding, false, true, defaultTabSize, autoIndent, false, EOLMode.ForceWindows);
        }

        public IEnumerator<FileTypeInfo> GetEnumerator()
        {
            return mTypes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mTypes.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return mTypes.Count;
            }
        }

        public FileTypeInfo this[int index]
        {
            get
            {
                return mTypes[index];
            }
        }

        internal void Add(FileTypeInfo info)
        {
            mTypes.Add(info);
        }

        public FileTypeInfo Find(string fileName)
        {
            foreach (FileTypeInfo type in mTypes)
                if (type.Match(fileName))
                    return type;
            return mNeutralType;
        }

        protected virtual void Dispose(bool disposing)
        {
            foreach (FileTypeInfo t in mTypes)
                t.Dispose();
            mTypes.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FileTypeInfoCollection()
        {
            Dispose(false);
        }

    }
}
