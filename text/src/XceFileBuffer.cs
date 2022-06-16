using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.scintilla;

namespace gehtsoft.xce.text
{
    public enum XceFileBufferEndOfLine
    {
        eCr,
        eLf,
        eCrLf,
    }

    public delegate void XceFileBufferChangedDelegate(XceFileBuffer sender, int position, bool major);
    public delegate void XceFileNameChangedDelegate(XceFileBuffer sender);

    /// <summary>
    /// Text file buffer
    /// </summary>
    public class XceFileBuffer : IDisposable
    {
        public class BufferState
        {
            CellBuffer mBuffer;

            internal BufferState(CellBuffer b)
            {
                mBuffer = b;
            }

            public int this[int index]
            {
                get
                {
                    return mBuffer.GetState(index);
                }
                set
                {
                    mBuffer.SetState(index, value);
                }
            }
        }

        private object mMutex = new object();

        internal object Mutex
        {
            get
            {
                return mMutex;
            }
        }

        #region global properties
        readonly static char[] gCrLf = "\r\n".ToCharArray();
        readonly static char[] gCr = "\r".ToCharArray();
        readonly static char[] gLf = "\n".ToCharArray();
        #endregion

        #region event
        /// <summary>
        /// The event that change happened
        /// </summary>
        public event XceFileBufferChangedDelegate OnChanged;
        public event XceFileNameChangedDelegate OnNameChanged;

        private void FireChangedPosition(int position, bool major)
        {
            if (OnChanged != null)
                OnChanged(this, position, major);
        }

        private void FireNameChanged()
        {
            if (OnNameChanged != null)
                OnNameChanged(this);
        }

        private void FireChangedLine(int line, bool major)
        {
            if (line >= 0 && line < mBuffer.LinesCount)
                FireChangedPosition(mBuffer.LineStart(line), major);

        }
        #endregion

        #region properties
        /// <summary>
        /// Character buffer
        /// </summary>
        private CellBuffer mBuffer;

        /// <summary>
        /// Buffer markers
        /// </summary>
        private XceFileBufferMarkers mMarkers;

        /// <summary>
        /// Size of the tabulation in characters
        /// </summary>
        private int mTabSize;

        /// <summary>
        /// Tab expand flag
        /// </summary>
        private bool mExpandTabs;

        /// <summary>
        /// Flag indicating that space must be trimmed
        /// </summary>
        private bool mTrimSpaces;

        /// <summary>
        /// The encoding used
        /// </summary>
        private Encoding mEncoding;

        /// <summary>
        /// The end-of-line mode
        /// </summary>
        XceFileBufferEndOfLine mEolMode;

        /// <summary>
        /// File name
        /// </summary>
        private string mFileName;

        private BufferState mState;

        public BufferState State
        {
            get
            {
                return mState;
            }
        }
        #endregion

        #region initialize/deinitialize
        /// <summary>
        /// Constructor
        /// </summary>
        public XceFileBuffer()
        {
            mBuffer = new CellBuffer();
            mState = new BufferState(mBuffer);
            mMarkers = new XceFileBufferMarkers(mBuffer);
            mEolMode = XceFileBufferEndOfLine.eCrLf;
            mTabSize = 4;
            mExpandTabs = false;
            mTrimSpaces = false;
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~XceFileBuffer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposal
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Internal disposal implementation
        /// </summary>
        /// <param name="disposal"></param>
        protected void Dispose(bool disposal)
        {
          lock (mMutex)
          {
            if (mFileSystemWatcher != null)
            {
                mFileSystemWatcher.EnableRaisingEvents = false;
                mFileSystemWatcher.Dispose();
            }
            mFileSystemWatcher = null;

            if (mBuffer != null)
                mBuffer.Dispose();
            mBuffer = null;
            if (disposal)
                GC.SuppressFinalize(this);
          }
        }
        #endregion

        #region range operations
        /// <summary>
        /// Length of the buffer in characters
        /// </summary>
        public int Length
        {
            get
            {
                lock (mMutex)
                    return mBuffer.CharCount;
            }
        }

        /// <summary>
        /// Gets/Sets a character by position
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public char this[int index]
        {
            get
            {
              lock (mMutex) {
                if (index < 0 || index >= mBuffer.CharCount)
                    throw new ArgumentOutOfRangeException("index");
                return mBuffer[index];
              }
            }
        }

        /// <summary>
        /// Get text range as a buffer
        /// </summary>
        /// <param name="position"></param>
        /// <param name="length"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public int GetRange(int position, int length, char[] buffer, int offset)
        {
            lock (mMutex)
                return mBuffer.GetRange(position, length, buffer, offset);
        }

        /// <summary>
        /// Get text range as a string
        /// </summary>
        /// <param name="positio"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public string GetRange(int position, int length)
        {
            lock (mMutex)
                return mBuffer.GetRange(position, length);
        }

        /// <summary>
        /// Delete the specified range
        /// </summary>
        /// <param name="position"></param>
        /// <param name="length"></param>
        public void DeleteRange(int position, int length)
        {
            lock (mMutex)
            {
                mBuffer.DeleteChars(position, length);
            }
            FireChangedPosition(position, true);
        }

        /// <summary>
        /// Insert a text to range
        /// </summary>
        /// <param name="position"></param>
        /// <param name="length"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        public void InsertRange(int position, char[] buffer, int offset, int length)
        {
            lock (mMutex)
            {
                mBuffer.InsertString(position, buffer, offset, length);
                if (mExpandTabs)
                    ExpandTabsInRange(position, length);
            }
            FireChangedPosition(position, true);
        }

        public void InsertRange(int position, string text)
        {
            lock (mMutex)
            {
                if (text == null)
                    throw new ArgumentNullException("text");
                InsertRange(position, text.ToCharArray(), 0, text.Length);
            }
        }
        #endregion


        #region line accessors and operations
        /// <summary>
        /// Number of text lines
        /// </summary>
        public int LinesCount
        {
            get
            {
                lock (mMutex)
                    return mBuffer.LinesCount;
            }
        }

        /// <summary>
        /// Get start position of the line
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <returns></returns>
        public int LineStart(int lineIndex)
        {
            lock (mMutex)
            {
                if (lineIndex < 0 || lineIndex >= mBuffer.LinesCount)
                    throw new ArgumentOutOfRangeException("lineIndex");
                return mBuffer.LineStart(lineIndex);
            }
        }

        /// <summary>
        /// Get line length without eof
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <returns></returns>
        public int LineLength(int lineIndex)
        {
            lock (mMutex)
                return LineLength(lineIndex, false);
        }

        /// <summary>
        /// Get line length
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <param name="includeEof">true to include eof</param>
        /// <returns></returns>
        public int LineLength(int lineIndex, bool includeEof)
        {
            lock (mMutex)
            {
                if (lineIndex < 0 || lineIndex >= mBuffer.LinesCount)
                    throw new ArgumentOutOfRangeException("lineIndex");
                int length = mBuffer.LineLength(lineIndex);
                int i;
                if (length > 0 && !includeEof)
                {
                    int start = mBuffer.LineStart(lineIndex);
                    for (i = length - 1; i >= 0; i--)
                    {
                        char c = mBuffer[start + i];
                        if (c == '\r' || c == '\n')
                            continue;
                        else
                            break;
                    }
                    length = i + 1;
                }
                return length;
            }

        }

        /// <summary>
        /// Get line index from the position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public int LineFromPosition(int position)
        {
            lock (mMutex)
                return mBuffer.LineFromPosition(position);
        }

        /// <summary>
        /// Append an empty line
        /// </summary>
        public int AppendLine()
        {
            lock (mMutex)
            {
                mBuffer.InsertString(mBuffer.CharCount, gCrLf, 0, 2);
                int l = mBuffer.LinesCount - 1;
                FireChangedLine(l, false);
                return mBuffer.LineStart(l);
            }
        }

        /// <summary>
        /// Inserts a new empty line
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <returns></returns>
        public int InsertLine(int lineIndex)
        {
            lock (mMutex)
            {
                if (lineIndex < 0)
                    throw new IndexOutOfRangeException("lineIndex");

                if (lineIndex >= mBuffer.LinesCount)
                {
                    mBuffer.BeginUndoAction();
                    while (mBuffer.LinesCount <= lineIndex)
                        AppendLine();
                    mBuffer.EndUndoAction();
                    return mBuffer.LineStart(mBuffer.LinesCount - 1);
                }

                int position = mBuffer.LineStart(lineIndex);
                mBuffer.InsertString(position, gCrLf, 0, 2);
                FireChangedPosition(position, false);
                return position;
            }
        }

        /// <summary>
        /// Delete a line
        /// </summary>
        /// <param name="lineIndex"></param>
        public void DeleteLine(int lineIndex)
        {
            int position = 0;
            int length = 0;
            lock (mMutex)
            {
                if (lineIndex < 0)
                    throw new IndexOutOfRangeException("lineIndex");
                if (lineIndex >= mBuffer.LinesCount)
                    return;
                position = mBuffer.LineStart(lineIndex);
                length = mBuffer.LineLength(lineIndex);

                if (length > 0)
                    mBuffer.DeleteChars(position, length);
            }
            if (length > 0)
                FireChangedPosition(position, true);
            return;
        }

        /// <summary>
        /// Insert a text to a line
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <param name="position"></param>
        /// <param name="buffer"></param>
        public void InsertToLine(int lineIndex, int position, string buffer)
        {
            InsertToLine(lineIndex, position, buffer.ToCharArray(), 0, buffer.Length);
        }

        /// <summary>
        /// Inserts a text to the line
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <param name="position"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public int InsertToLine(int lineIndex, int position, char[] buffer, int offset, int length)
        {
            lock (mMutex)
            {
                bool transact = false;

                if (lineIndex < 0)
                    throw new ArgumentOutOfRangeException("lineIndex");
                if (position < 0)
                    throw new ArgumentOutOfRangeException("position");
                if (buffer == null)
                    throw new ArgumentNullException("buffer");
                if (offset < 0 || offset >= buffer.Length)
                    throw new ArgumentOutOfRangeException("offset");
                if (length < 0 || offset + length > buffer.Length)
                    throw new ArgumentOutOfRangeException("length");

                if (lineIndex >= mBuffer.LinesCount)
                {
                    transact = true;
                    mBuffer.BeginUndoAction();
                    while (lineIndex >= mBuffer.LinesCount)
                        AppendLine();
                }

                int lineStart, lineLength;
                lineStart = mBuffer.LineStart(lineIndex);
                lineLength = LineLength(lineIndex, false);
                if (position > lineLength)
                {
                    if (!transact)
                    {
                        transact = true;
                        mBuffer.BeginUndoAction();
                    }
                    mBuffer.InsertChar(lineStart + lineLength, ' ', position - lineLength);
                }

                int olc = mBuffer.LinesCount;
                mBuffer.InsertString(lineStart + position, buffer, offset, length);

                if (mExpandTabs)
                    length = ExpandTabsInRange(lineStart + position, length);

                FireChangedPosition(lineStart, olc != mBuffer.LinesCount);

                if (transact)
                    mBuffer.EndUndoAction();
                return length;
            }
        }

        /// <summary>
        /// Inserts a character to a line
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <param name="position"></param>
        /// <param name="c"></param>
        /// <param name="count"></param>
        public int InsertToLine(int lineIndex, int position, char c, int count)
        {
            lock (mMutex)
            {
                bool transact = false;

                if (lineIndex < 0)
                    throw new ArgumentOutOfRangeException("lineIndex");
                if (position < 0)
                    throw new ArgumentOutOfRangeException("position");
                if (count < 1)
                    throw new ArgumentOutOfRangeException("count");

                if (lineIndex >= mBuffer.LinesCount)
                {
                    transact = true;
                    mBuffer.BeginUndoAction();
                    while (lineIndex >= mBuffer.LinesCount)
                        AppendLine();
                }

                int lineStart, lineLength;
                lineStart = mBuffer.LineStart(lineIndex);
                lineLength = LineLength(lineIndex, false);
                if (position > lineLength)
                {
                    if (!transact)
                    {
                        transact = true;
                        mBuffer.BeginUndoAction();
                    }
                    mBuffer.InsertChar(lineStart + lineLength, ' ', position - lineLength);
                }

                mBuffer.InsertChar(lineStart + position, c, count);

                if (mExpandTabs)
                    count = ExpandTabsInRange(lineStart + position, count);

                FireChangedPosition(lineStart + position, false);

                if (transact)
                    mBuffer.EndUndoAction();

                return count;
            }
        }

        /// <summary>
        /// Delete substring in the line
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <param name="position"></param>
        /// <param name="length"></param>
        public void DeleteFromLine(int lineIndex, int position, int length)
        {
            lock (mMutex)
            {
                if (lineIndex < 0)
                    throw new ArgumentOutOfRangeException("lineIndex");
                if (position < 0)
                    throw new ArgumentOutOfRangeException("position");
                if (lineIndex >= mBuffer.LinesCount)
                    return ;
                int lineStart, lineLength;
                lineStart = mBuffer.LineStart(lineIndex);
                lineLength = LineLength(lineIndex, false);
                if (position >= lineLength)
                    return ;
                if (position + length > lineLength)
                    length = lineLength - position;
                mBuffer.DeleteChars(lineStart + position, length);
                FireChangedPosition(lineStart + position, false);
            }
        }

        /// <summary>
        /// Splits a line at the specified position
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <param name="position"></param>
        public void SplitLine(int lineIndex, int position)
        {
            lock (mMutex)
            {
                if (lineIndex < 0)
                    throw new ArgumentOutOfRangeException("lineIndex");
                if (position < 0)
                    throw new ArgumentOutOfRangeException("position");
                if (lineIndex >= mBuffer.LinesCount)
                    return;
                int lineStart, lineLength;
                lineStart = mBuffer.LineStart(lineIndex);
                lineLength = LineLength(lineIndex, false);
                if (position >= lineLength)
                    position = lineLength;
                mBuffer.InsertString(lineStart + position, gCrLf, 0, 2);
                FireChangedPosition(lineStart, true);
            }
        }

        /// <summary>
        /// Join a line with next one
        /// </summary>
        /// <param name="lineIndex"></param>
        public void JoinWithNext(int lineIndex)
        {
            JoinWithNext(lineIndex, LineLength(lineIndex, false));
        }

        /// <summary>
        /// Join a line with next one at the specified position
        /// </summary>
        /// <param name="lineIndex"></param>
        public void JoinWithNext(int lineIndex, int position)
        {
            lock (mMutex)
            {
                bool transact = false;
                if (lineIndex < 0)
                    throw new ArgumentOutOfRangeException("lineIndex");
                if (position < 0)
                    throw new ArgumentOutOfRangeException("position");
                if (lineIndex >= mBuffer.LinesCount)
                    return;
                int lineStart, lineLength, lineLength1;
                lineStart = mBuffer.LineStart(lineIndex);
                lineLength = LineLength(lineIndex, false);
                lineLength1 = LineLength(lineIndex, true);
                if (position < lineLength)
                    throw new ArgumentOutOfRangeException("position");
                if (position > lineLength)
                {
                    transact = true;
                    mBuffer.BeginUndoAction();
                    mBuffer.InsertChar(lineStart + lineLength, ' ', position - lineLength);
                }
                if (lineLength != lineLength1)
                {
                    mBuffer.DeleteChars(lineStart + lineLength, lineLength1 - lineLength);
                }
                FireChangedPosition(lineStart, true);
                if (transact)
                    mBuffer.EndUndoAction();
            }
        }
        #endregion

        #region tabulation support
        /// <summary>
        /// Gets or sets tabulation mode
        /// </summary>
        public bool ExpandTabs
        {
            get
            {
                return mExpandTabs;
            }
            set
            {
                mExpandTabs = value;
                if (mExpandTabs && mBuffer.CharCount > 0)
                    ExpandTabsInRange(0, mBuffer.CharCount);
            }
        }

        /// <summary>
        /// Tabulation size
        /// </summary>
        public int TabSize
        {
            get
            {
                return mTabSize;
            }
            set
            {
                mTabSize = value;
            }
        }

        /// <summary>
        /// Expands tabulations in the specified range
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="length"></param>
        /// <returns>new size of the range</returns>
        public int ExpandTabsInRange(int startPosition, int length)
        {
            lock (mMutex)
            {
                bool hasTabs = false;
                int end = startPosition + length;
                for (int i = startPosition; i < end; i++)
                {
                    if (mBuffer[i] == '\t')
                    {
                        if (!hasTabs)
                        {
                            mBuffer.BeginUndoAction();
                            hasTabs = true;
                        }
                        int linePosition = i - mBuffer.LineStart(mBuffer.LineFromPosition(i));
                        int tabLength = TabLength(linePosition);
                        mBuffer.DeleteChars(i, 1);
                        mBuffer.InsertChar(i, ' ', tabLength);
                        end += (tabLength - 1);
                    }
                }
                if (hasTabs)
                    mBuffer.EndUndoAction();
                return end - startPosition;
            }
        }

        public int AutoIndentLinesInRange(int startPosition, int length)
        {
            lock (mMutex)
            {
                int end = startPosition + length;
                bool adjustNext = false;
                int adjustLength = 0;
                int line = -1;
                for (int i = startPosition; i < end; i++)
                {
                    if (EolMode == XceFileBufferEndOfLine.eCr)
                    {
                        if (mBuffer[i] == '\r')
                        {
                            adjustNext = true;
                            adjustLength = -1;
                            if (i > 1)
                                line = LineStart(LineFromPosition(i - 1));
                            else
                                line = -1;
                        }
                    }
                    if (EolMode == XceFileBufferEndOfLine.eLf)
                    {
                        if (mBuffer[i] == '\n')
                        {
                            adjustNext = true;
                            adjustLength = -1;
                            if (i > 1)
                                line = LineStart(LineFromPosition(i - 1));
                            else
                                line = -1;
                        }
                    }
                    else if (EolMode == XceFileBufferEndOfLine.eCrLf)
                    {
                        if (mBuffer[i] == '\r' && i + 1 < end && mBuffer[i + 1] == '\n')
                        {
                            adjustNext = true;
                            adjustLength = -1;
                            if (i > 1)
                                line = LineStart(LineFromPosition(i - 1));
                            else
                                line = -1;
                            i++;
                        }
                    }
                    if (adjustNext)
                    {
                        if (adjustLength < 0)
                        {
                            if (line >= 0)
                            {
                                adjustLength = 0;
                                while (this[line + adjustLength] == ' ')
                                    adjustLength++;
                            }
                            line = -1;
                        }
                        else if (adjustLength > 0)
                        {
                            mBuffer.InsertChar(i, ' ', adjustLength);
                            end += adjustLength;
                            adjustNext = false;
                            adjustLength = 0;
                        }
                        else
                            adjustNext = false;

                    }
                }
                return end - startPosition;
            }
        }



        /// <summary>
        /// Get length of the tabulation at the specified line position
        /// </summary>
        /// <param name="linePosition"></param>
        /// <returns></returns>
        public int TabLength(int linePosition)
        {
            return (linePosition / mTabSize + 1) * mTabSize - linePosition;
        }

        #endregion

        #region input/output
        /// <summary>
        /// Gets or sets end-of-line space trimming mode
        /// </summary>
        public bool TrimSpace
        {
            get
            {
                return mTrimSpaces;
            }
            set
            {
                mTrimSpaces = value;
            }
        }

        /// <summary>
        /// Gets or sets end-of-line mode for save
        /// </summary>
        public XceFileBufferEndOfLine EolMode
        {
            get
            {
                return mEolMode;
            }
            set
            {
                mEolMode = value;
            }
        }

        public char[] Eol
        {
            get
            {
                switch (mEolMode)
                {
                case XceFileBufferEndOfLine.eCr:
                    return gCr;
                case XceFileBufferEndOfLine.eLf:
                    return gLf;
                case XceFileBufferEndOfLine.eCrLf:
                    return gCrLf;
                default:
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets a code page for save
        /// </summary>
        public Encoding Encoding
        {
            get
            {
                return mEncoding;
            }
        }

        /// <summary>
        /// Change code page
        /// </summary>
        /// <param name="newCodePage">new code page</param>
        /// <returns>true if there was a error</returns>
        public bool ChangeEncoding(Encoding encoding)
        {
            mEncoding = encoding;
            return mBuffer.SetCP(encoding.CodePage);
        }

        /// <summary>
        /// The name of the file
        /// </summary>
        public string FileName
        {
            get
            {
                return mFileName;
            }
        }

        public void New(Encoding encoding, string file)
        {
            try
            {
                FileInfo fi = new FileInfo(file);
                mFileName = fi.FullName;
            }
            catch (Exception )
            {
                mFileName = "new file";
            }
            FireNameChanged();
            mEncoding = encoding;
            mBuffer.SetCP(encoding.CodePage);
            mBuffer.SetUndoCollection(true);
            mBuffer.SetSavePoint();
        }

        /// <summary>
        /// Load the file
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="file"></param>
        /// <param name="ignoreBOM"></param>
        public void Load(Encoding encoding, string file, bool ignoreBOM)
        {
            FileInfo fi = new FileInfo(file);
            mFileName = fi.FullName;
            FireNameChanged();

            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader r = new BinaryReader(fs))
                {
                    long length = fs.Length;
                    if (length > 100 * 1024 * 1024)
                        length = 100 * 1024 * 1024;

                    Load(encoding, r, ignoreBOM, (int)(length));
                    r.Close();
                }
                fs.Close();
            }

            SetWatcher();
        }

        private void Load(Encoding encoding, BinaryReader content, bool ignoreBOM, int length)
        {
            if (mBuffer.CharCount != 0)
            {
                mBuffer.Dispose();
                mBuffer = new CellBuffer();
                mMarkers = new XceFileBufferMarkers(mBuffer);
            }
            byte[] buff = new byte[length];
            content.Read(buff, 0, length);
            int skip = 0;

            if (!ignoreBOM)
            {
                if (length >= 3 && buff[0] == 0xef && buff[1] == 0xbb && buff[2] == 0xbf)
                {
                    encoding = Encoding.UTF8;
                    skip = 3;
                }
                else if (length >= 4 && buff[0] == 0xff && buff[1] == 0xfe && buff[2] == 0x00 && buff[3] == 0x00)
                {
                    encoding = Encoding.UTF32;
                    skip = 4;
                }
                else if (length >= 2 && buff[0] == 0xff && buff[1] == 0xfe)
                {
                    encoding = Encoding.Unicode;
                    skip = 2;
                }
                else if (length >= 2 && buff[0] == 0xfe && buff[1] == 0xff)
                {
                    encoding = Encoding.BigEndianUnicode;
                    skip = 2;
                }

            }

            mEncoding = encoding;
            mBuffer.SetCP(encoding.CodePage);
            char[] chars = encoding.GetChars(buff, skip, length - skip);
            if (chars.Length > 0)
            {
                mBuffer.SetUndoCollection(false);
                mBuffer.InsertString(0, chars, 0, chars.Length);
                if (mExpandTabs)
                    ExpandTabsInRange(0, mBuffer.CharCount);
                mBuffer.SetUndoCollection(true);
            }
            mBuffer.SetSavePoint();
        }

        /// <summary>
        /// Save the file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="writeBom"></param>
        public void Save(bool writeBom, Encoding encoding)
        {
            lock (mMutex)
            {
                SuspendWatcher();
                using (FileStream fs = new FileStream(mFileName, FileMode.Create, FileAccess.Write))
                {
                    using (BinaryWriter w = new BinaryWriter(fs))
                    {
                        Save(w, encoding, writeBom);
                        w.Close();
                    }
                    fs.Close();
                }
                ResumeWatcher();
            }
        }

        public void SaveAs(string fileName, bool writeBom)
        {
            lock (mMutex)
            {
                SuspendWatcher();
                FileInfo fi = new FileInfo(fileName);
                mFileName = fi.FullName;
                SetWatcher();
                FireNameChanged();
                Save(writeBom, null);
            }
        }

        public void SaveAs(string fileName, Encoding encoding, bool writeBom)
        {
            lock (mMutex)
            {
                SuspendWatcher();
                FileInfo fi = new FileInfo(fileName);
                mFileName = fi.FullName;
                SetWatcher();
                FireNameChanged();
                Save(writeBom, encoding);
            }
        }

        private void Save(BinaryWriter w, Encoding encoding, bool writeBom)
        {
            lock (mMutex)
            {
                int c = LinesCount;

                byte[] eol;
                int eol_length;
                bool newEncoding = false;
                if (encoding == null)
                    encoding = mEncoding;
                else
                {
                    newEncoding = true;
                    BeginUndoTransaction();
                }

                if (writeBom)
                {
                    byte[] bom = encoding.GetPreamble();
                    if (bom != null && bom.Length > 0)
                        w.Write(bom);
                }

                switch (mEolMode)
                {
                case    XceFileBufferEndOfLine.eCr:
                        eol = encoding.GetBytes(gCr, 0, gCr.Length);
                        break;
                case    XceFileBufferEndOfLine.eLf:
                        eol = encoding.GetBytes(gLf, 0, gCr.Length);
                        break;
                default:
                        eol = encoding.GetBytes(gCrLf, 0, gCrLf.Length);
                        break;
                }
                eol_length = eol.Length;

                AutoArray<char> _line = new AutoArray<char>();
                AutoArray<byte> _encoded = new AutoArray<byte>();
                AutoArray<char> _decoded = new AutoArray<char>();

                for (int i = 0; i < c; i++)
                {
                    int position, length;
                    position = mBuffer.LineStart(i);
                    length = mBuffer.LineLength(i);
                    char[] line = _line.Ensure(length);
                    bool hasCrLf = false;
                    if (length > 0)
                        mBuffer.GetRange(position, length, line, 0);
                    //remove cr/lf at the end
                    while (length > 0 && (line[length - 1] == '\r' || line[length - 1] == '\n'))
                    {
                        hasCrLf = true;
                        length--;
                    }

                    //trim spaces at the end
                    if (mTrimSpaces)
                        while (length > 0 && line[length - 1] == ' ')
                            length--;

                    if (length > 0)
                    {
                        int encodedLength = encoding.GetByteCount(line, 0, length);
                        byte[] encoded = _encoded.Ensure(encodedLength);
                        encoding.GetBytes(line, 0, length, encoded, 0);
                        w.Write(encoded, 0, encodedLength);
                        if (newEncoding)
                        {
                            int decodedLength = encoding.GetCharCount(encoded, 0, encodedLength);
                            char[] decoded = _decoded.Ensure(decodedLength);
                            encoding.GetChars(encoded, 0, encodedLength, decoded, 0);
                            mBuffer.DeleteChars(position, length);
                            mBuffer.InsertString(position, decoded, 0, decodedLength);
                        }
                    }
                    if (hasCrLf)
                        w.Write(eol, 0, eol.Length);
                }

                if (newEncoding)
                {
                    EndUndoTransaction();
                    mEncoding = encoding;
                }
                mBuffer.SetSavePoint();
            }
        }
        #endregion

        #region Undo/Redo
        /// <summary>
        /// Check whether can undo
        /// </summary>
        public bool CanUndo
        {
            get
            {
                lock (mMutex)
                    return mBuffer.CanUndo();
            }
        }

        /// <summary>
        /// Check whether can redo
        /// </summary>
        public bool CanRedo
        {
            get
            {
                lock (mMutex)
                    return mBuffer.CanRedo();
            }
        }

        public bool AtSavePoint
        {
            get
            {
                lock (mMutex)
                    return mBuffer.IsSavePoint();
            }
            set
            {
                lock (mMutex)
                    mBuffer.SetSavePoint();
            }
        }

        public int Undo()
        {
            lock (mMutex)
            {
                int rc = mBuffer.Undo();
                FireChangedPosition(0, true);
                return rc;
            }
        }

        public int Redo()
        {
            lock (mMutex)
            {
                int rc = mBuffer.Redo();
                FireChangedPosition(0, true);
                return rc;
            }
        }

        public void BeginUndoTransaction()
        {
            lock (mMutex)
                mBuffer.BeginUndoAction();
        }

        public void EndUndoTransaction()
        {
            lock (mMutex)
                mBuffer.EndUndoAction();
        }

        public void EnableUndo(bool enable)
        {
            lock (mMutex)
                mBuffer.SetUndoCollection(enable);
        }
        #endregion

        #region misc features
        public XceFileBufferMarkers Markers
        {
            get
            {
                return mMarkers;
            }
        }
        #endregion

        #region Internal methods
        internal void GetNativeAccessors(out IntPtr a1, out IntPtr a2, out IntPtr a3)
        {
            mBuffer.GetNativeAccessor(out a1, out a2, out a3);
        }

        public long LastChange
        {
            get
            {
                return mBuffer.LastChange;
            }
        }

        private FileSystemWatcher mFileSystemWatcher = null;
        private bool mHasBeenExternallyChanged = false;

        private void SetWatcher()
        {
            if (mFileSystemWatcher != null)
            {
                mFileSystemWatcher.EnableRaisingEvents = false;
                mFileSystemWatcher.Dispose();
            }
            mFileSystemWatcher = null;
            FileInfo fi = new FileInfo(mFileName);
            mFileSystemWatcher = new FileSystemWatcher(fi.Directory.FullName, fi.Name);
            mFileSystemWatcher.Changed += new FileSystemEventHandler(OnFileExtenallyChanged);
            mHasBeenExternallyChanged = false;
            mFileSystemWatcher.EnableRaisingEvents = true;
        }

        private void OnFileExtenallyChanged(object sender, FileSystemEventArgs args)
        {
            if (string.Compare(mFileName, args.FullPath, true) == 0)
                mHasBeenExternallyChanged = true;
        }

        private void SuspendWatcher()
        {
            if (mFileSystemWatcher != null && mFileSystemWatcher.EnableRaisingEvents)
                mFileSystemWatcher.EnableRaisingEvents = false;
        }

        private void ResumeWatcher()
        {
            if (mFileSystemWatcher != null && !mFileSystemWatcher.EnableRaisingEvents)
                mFileSystemWatcher.EnableRaisingEvents = true;
        }

        public bool HasBeenExtenrallyChanged
        {
            get
            {
                return mHasBeenExternallyChanged;
            }
        }

        public void ResetExternallyChanged()
        {
            mHasBeenExternallyChanged = false;
        }
        #endregion
    }
}
