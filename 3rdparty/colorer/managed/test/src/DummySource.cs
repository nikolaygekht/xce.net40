using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using gehtsoft.xce.colorer;

namespace test
{
    /// <summary>
    /// Dummy source
    /// </summary>
    class DummySource : ILineSource
    {
        string mFileName;
        List<string> mStrings;
        
        internal DummySource(string filename)
        {
            mFileName = filename;
            mStrings = new List<string>();
            StreamReader reader = new StreamReader(filename, true);
            string s;
            while ((s = reader.ReadLine()) != null)
                mStrings.Add(s);
            reader.Close();
        }
        
        public string GetFileName()
        {
            return mFileName;
        }
        
        public int GetLinesCount()
        {
            return mStrings.Count;
        }
        
        public int GetLineLength(int index)
        {
            return mStrings[index].Length;
        }
        
        public int GetLine(int line, char[] buffer, int columnFrom, int length)
        {
            mStrings[line].CopyTo(columnFrom, buffer, 0, length);
            return length;
        }
        
        public string GetLine(int line)
        {
            return mStrings[line];
        }
    }
}
