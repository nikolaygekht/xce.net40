using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;

namespace gehtsoft.intellisense.cs
{
    public interface ICsParserFileSource
    {
        TextReader CreateReader();
        
        bool PositionToLine(int position, out int line, out int column);
        int LineToPosition(int line, int column);
        
        string ReadTo(int length);
        string ReadTo(int line, int column);
        
        bool Changed
        {
            get;
            set;
        }
    }
    
    internal class CsParserFileSource : ICsParserFileSource
    {
        private string mPath;
        private bool mChanged;
        
        internal CsParserFileSource(string path)
        {
            mPath = path;
            mChanged = true;
        }
        
        public TextReader CreateReader()
        {
            return new StreamReader(mPath, true);
        }

        public string ReadTo(int length)
        {
            return "";
        }

        public string ReadTo(int line, int column)
        {
            return "";
        }

        public bool PositionToLine(int position, out int line, out int column)
        {
            line = column = 0;
            return false;
        }

        public int LineToPosition(int line, int column)
        {
            return 0;
        }
        
        public bool Changed
        {
            get
            {
                return mChanged;
            }
            set
            {
                mChanged = value;
            }
        }
    }

    public class CsParserFile 
    {
        private string mName;
        
        public string Name
        {
            get
            {
                return mName;
            }
        }
        
        private ICompilationUnit mCompilationUnit;
        
        internal ICompilationUnit CompilationUnit
        {
            get
            {
                return mCompilationUnit;
            }
            set
            {
                mCompilationUnit = value;
                mParserInformation.SetCompilationUnit(value);
            }
        }

        private ParseInformation mParserInformation = new ParseInformation();
        
        internal ParseInformation ParseInformaiton
        {
            get
            {
                return mParserInformation;
            }
        }
        
        ICsParserFileSource mSource;
        
        internal ICsParserFileSource Source
        {
            get
            {
                return mSource;
            }
        }
        
        public IList<IClass> GetClasses()
        {
            if (mCompilationUnit != null)
                return mCompilationUnit.Classes;
            else
                return null;
        }
        
        private CsParser mParser;
        
        public CsParser Parser
        {
            get
            {
                return mParser;
            }
        }
        
        internal CsParserFile(CsParser parser, string name, ICsParserFileSource source)
        {
            mCompilationUnit = null;
            mName = name;
            mSource = source;
            mParser = parser;
        }
    }
    
    public class CsParserFileCollection : IEnumerable<CsParserFile>
    {
        private List<CsParserFile> mList = new List<CsParserFile>();
        private object mMutex = new object();
        
        public object Mutex
        {
            get
            {
                return mMutex;
            }
        }
        
        public int Count
        {
            get
            {
                lock (mMutex)
                    return mList.Count;
            }
        }
        
        public CsParserFile this[int index]
        {
            get
            {
                lock (mMutex)
                    return mList[index];
            }
        }
        
        public IEnumerator<CsParserFile> GetEnumerator()
        {
            return mList.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return mList.GetEnumerator();
        }
        
        public int IndexOf(CsParserFile file)
        {
            lock (mMutex)
            {
                for (int i = 0; i < mList.Count; i++)
                    if (object.ReferenceEquals(file, mList[i]))
                        return i;
                return -1;
            }
        }
        
        public int IndexOf(string path)
        {
            lock (mMutex)
            {
                for (int i = 0; i < mList.Count; i++)
                    if (string.Compare(path, mList[i].Name, true) == 0)
                        return i;
                return -1;
            }
        }
       
        
        internal void Add(CsParserFile file)
        {
            lock (mMutex)
                mList.Add(file);
        }

        internal void Remove(int index)
        {
            lock (mMutex)
                mList.RemoveAt(index);
        }
    }
}
