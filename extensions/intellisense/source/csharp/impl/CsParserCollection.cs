using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using gehtsoft.intellisense.cs;


namespace gehtsoft.intellisense.cs
{
    internal class ParserCollection : IEnumerable<CsParser>
    {
        private List<CsParser> mList = new List<CsParser>();

        internal ParserCollection()
        {
        }

        public int Count
        {
            get
            {
                return mList.Count;
            }
        }

        public CsParser this[int index]
        {
            get
            {
                return mList[index];
            }
        }

        public IEnumerator<CsParser> GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        public CsParser FindParser(string fileName)
        {
            foreach (CsParser parser in mList)
            {
                if (parser.Project.Sources.IndexOf(fileName) >= 0)
                    return parser;
            }
            return null;
        }

        public void Add(CsParser parser)
        {
            mList.Add(parser);
        }

        public void Remove(CsParser parser)
        {
            mList.Remove(parser);
        }
    }
}
