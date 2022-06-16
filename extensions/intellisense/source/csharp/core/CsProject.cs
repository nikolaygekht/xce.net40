using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace gehtsoft.intellisense.cs
{
    public class CsProjectReference
    {
        private string mName;
        private string mPath;

        public string Name
        {
            get
            {
                return mName;
            }
        }    
        
        public string Path
        {
            get
            {
                return mPath;
            }
        }
        
        internal CsProjectReference(string name, string path)
        {
            mName = name;
            mPath = path;
        }
    }
    
    public class CsProjectReferenceCollection : IEnumerable<CsProjectReference>
    {
        private List<CsProjectReference> mList = new List<CsProjectReference>();
        
        public int Count
        {
            get
            {
                return mList.Count;
            }
        }
        
        public CsProjectReference this[int index]
        {
            get
            {
                return mList[index];
            }
        }
        
        public IEnumerator<CsProjectReference> GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mList.GetEnumerator();
        }
        
        internal void Add(CsProjectReference obj)
        {
            mList.Add(obj);
        }
        
        internal CsProjectReferenceCollection()
        {
        }
    }

    public class CsProjectSource
    {
        private string mPath;
        
        public string Path
        {
            get
            {
                return mPath;
            }
        }
        
        internal CsProjectSource(string path)
        {
            mPath = path;
        }
    }

    public class CsProjectSourceCollection : IEnumerable<CsProjectSource>
    {
        private List<CsProjectSource> mList = new List<CsProjectSource>();

        public int Count
        {
            get
            {
                return mList.Count;
            }
        }

        public CsProjectSource this[int index]
        {
            get
            {
                return mList[index];
            }
        }

        public IEnumerator<CsProjectSource> GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        internal void Add(CsProjectSource obj)
        {
            mList.Add(obj);
        }

        internal CsProjectSourceCollection()
        {
        }
        
        public int IndexOf(string path)
        {
            for (int i = 0; i < mList.Count; i++)
                if (string.Compare(mList[i].Path, path, true) == 0)
                    return i;
            return -1;
        }
    }

    public class CsProject
    {
        private CsProjectReferenceCollection mReferences = new CsProjectReferenceCollection();
        private CsProjectSourceCollection mSourceFiles = new CsProjectSourceCollection();
        
        public CsProjectReferenceCollection References
        {
            get
            {
                return mReferences;
            }
        }
        
        public CsProjectSourceCollection Sources
        {
            get
            {
                return mSourceFiles;
            }
        }
        
        private string mName;
        
        public string Name
        {
            get
            {
                return mName;
            }
        }
        
        internal CsProject(string name)
        {
            mName = name;
        }
        
        static CsProject mDefault;
        
        public static CsProject Default
        {
            get
            {
                return mDefault;
            }
        }
        
        static CsProject()
        {
            mDefault = new CsProject(null);
            Default.References.Add(new CsProjectReference("System", null));
            Default.References.Add(new CsProjectReference("System.Collections", null));
            Default.References.Add(new CsProjectReference("System.Collections.Generic", null));
            Default.References.Add(new CsProjectReference("System.IO", null));
            Default.References.Add(new CsProjectReference("System.Text", null));
            Default.References.Add(new CsProjectReference("System.Xml", null));
        }
        
        public bool IsDefault
        {
            get
            {
                return object.ReferenceEquals(this, mDefault);
            }
        }
    }
    
}
