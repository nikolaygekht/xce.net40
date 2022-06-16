using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.scintilla;

namespace gehtsoft.xce.text
{
    /// <summary>
    /// Markers for the XceFileBuffer
    /// </summary>
    public class XceFileBufferMarkers
    {
        private CellBuffer mBuffer;
        
        public int this[int index]
        {
            get
            {
                if (index < 0 || index > 15)
                    throw new ArgumentOutOfRangeException("index");
                return mBuffer.GetMarker(index);
            }
            set
            {
                if (index < 0 || index > 15)
                    throw new ArgumentOutOfRangeException("index");
                mBuffer.SetMarker(index, value);                
            }
        }
        
        internal XceFileBufferMarkers(CellBuffer buffer)
        {   
            mBuffer = buffer;
        }
    }
}
