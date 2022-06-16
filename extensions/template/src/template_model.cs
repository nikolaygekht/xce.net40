using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.colorer;

namespace gehtsoft.xce.extension.template_impl
{
    internal class TemplateStringCollection : IEnumerable<string>
    {
        private List<string> mList = new List<string>();
        
        internal int Count
        {
            get
            {
                return mList.Count;
            }
        }
        
        internal string this[int index]
        {
            get
            {
                return mList[index];
            }
        }
        
        public IEnumerator<string> GetEnumerator()
        {
            return mList.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return mList.GetEnumerator();
        }
        
        internal void Add(string param)
        {
            mList.Add(param);
        }
    }
    


    internal class Template
    {
        private string mName;
        
        internal string Name
        {
            get
            {
                return mName;
            }
        }
    
        private TemplateStringCollection mBody = new TemplateStringCollection();

        internal TemplateStringCollection Body
        {
            get
            {
                return mBody;
            }
        }
        
        private TemplateStringCollection mParams = new TemplateStringCollection();
        
        internal TemplateStringCollection Params
        {
            get
            {
                return mParams;
            }
        }
        
        internal Template(string name)
        {
            mName = name;
        }
    }
    
    internal class TemplateCollection : IEnumerable<Template>
    {
        List<Template> mTemplates = new List<Template>();
        
        public IEnumerator<Template> GetEnumerator()
        {
            return mTemplates.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return mTemplates.GetEnumerator();
        }
        
        internal int Count
        {
            get
            {
                return mTemplates.Count;
            }
        }
        
        internal Template this[int index]
        {
            get
            {
                return mTemplates[index];
            }
        }
        
        internal TemplateCollection()
        {
        }
        
        internal void Add(Template t)
        {
            mTemplates.Add(t);
        }
    }
    
    internal class TemplateFileType : IDisposable
    {
        private string mMask;
        
        internal string Mask
        {
            get
            {
                return mMask;
            }
        }
        
        private Regex mRegex;
        
        internal bool Match(string fileName)
        {
            return mRegex.Match(fileName);
        }
        
        private TemplateCollection mTemplates = new TemplateCollection();
        
        internal TemplateCollection Templates
        {
            get
            {
                return mTemplates;
            }
        }
        
        
        ~TemplateFileType()
        {
            Dispose(false);
        }
        
        public void Dispose()
        {
            Dispose(true);
        }
        
        protected virtual void Dispose(bool disposal)
        {
            if (mRegex != null)
                mRegex.Dispose();
            mRegex = null;
            if (disposal)
                GC.SuppressFinalize(this);
        }
        
        internal TemplateFileType(string mask)
        {
            mMask = mask;
            mRegex = new Regex("/" + mask + "/i");
        }
    }
    
    internal class TemplateFileTypeCollection : IEnumerable<TemplateFileType>, IDisposable
    {
        private List<TemplateFileType> mTypes = new List<TemplateFileType>();
        
        internal int Count
        {
            get
            {
                return mTypes.Count;
            }
        }
        
        internal TemplateFileType this[int index]
        {
            get
            {
                return mTypes[index];
            }
        }
        
        public IEnumerator<TemplateFileType> GetEnumerator()
        {
            return mTypes.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return mTypes.GetEnumerator();
        }
        
        ~TemplateFileTypeCollection()
        {
            Dispose(false);
        }
        
        public void Dispose()
        {
            Dispose(true);
        }
        
        protected virtual void Dispose(bool disposal)
        {
            foreach (TemplateFileType type in mTypes)
                type.Dispose();
            mTypes.Clear();
            if (disposal)
                GC.SuppressFinalize(this);
        }
        
        internal void Add(TemplateFileType type)
        {
            mTypes.Add(type);
        }
        
        internal TemplateFileTypeCollection()
        {
        }
    }
}
