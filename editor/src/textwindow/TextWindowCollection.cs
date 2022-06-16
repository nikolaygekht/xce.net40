using System;
using System.Collections.Generic;
using System.Text;

namespace gehtsoft.xce.editor.textwindow
{
    public class TextWindowCollection : IEnumerable<TextWindow>
    {
        private List<TextWindow> mWindows = new List<TextWindow>();
        
        public IEnumerator<TextWindow> GetEnumerator()
        {
            return mWindows.GetEnumerator();
        }
        
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return mWindows.GetEnumerator();
        }
        
        public int Count
        {
            get
            {
                return mWindows.Count;
            }
        }
        
        public TextWindow this[int index]
        {
            get
            {
                return mWindows[index];
            }
        }
        
        internal void Add(TextWindow w)
        {
            mWindows.Add(w);
            AssignIds();
        }
        
        public int Find(TextWindow w)
        {
            for (int i = 0; i < mWindows.Count; i++)
                if (object.ReferenceEquals(w, mWindows[i]))
                    return i;
            return -1;
        }
        
        internal void Remove(int index)
        {
            mWindows.RemoveAt(index);
            AssignIds();
        }
        
        private void AssignIds()
        {
            char id;
            int i;
            id = '0';
            for (i = 0; i < mWindows.Count; i++)
            {
                mWindows[i].SetId(id);
                if (id == '9')
                    id = 'A';
                else if (id == 'Z')
                    id = 'a';
                else
                    id++;
            }
        }
        
        
    }
}
