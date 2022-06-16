
    internal enum XmlPosContext
    {
        AfterStartLt,           //cursor is after <
        AfterEndLt,             //cursor is after </
        AfterNsColon,           //cursor is after : in the tag name
        AfterAttributeName,     //the cursor is on or after the attribute name
        InsideStartTag,         //cursor is inside start tag body
        AfterAttrStart,         //cursor is after attr="..."
        InComment,              //cursor is inside the comment
        InCData,                //cursor is inside CData block
        InBody,                 //cursor is inside the tag text part
        Undetected,             //cursor position context is not detected
    }

    internal class XmlPos
    {
        private XmlPosContext mContext;

        internal XmlPosContext Context
        {
            get
            {
                return mContext;
            }
            set
            {
                mContext = value;
            }
        }

        private string mCurrNs;
        /// <summary>
        /// Current namespace.
        ///     Defined for AfterNsColon or InsideStartTag - contains namespace name or "" is there are no namespace
        /// </summary>
        internal string CurrNs
        {
            get
            {
                return mCurrNs;
            }
            set
            {
                mCurrNs = value;
            }
        }

        private string mCurrTag;
        /// <summary>
        /// Current tag.
        ///     Defined InsideStartTag - contains the tag name
        /// </summary>
        internal string CurrTag
        {
            get
            {
                return mCurrTag;
            }
            set
            {
                mCurrTag = value;
            }
        }

        private string mAttributeName;

        internal string AttributeName
        {
            get
            {
                return mAttributeName;
            }
            set
            {
                mAttributeName = value;
            }
        }

        private string mLastOpenTag;
        /// <summary>
        /// The last open tag outside current context for
        ///     all states except inComment, inCData, Undetected
        /// </summary>
        internal string LastOpenTag
        {
            get
            {
                return mLastOpenTag;
            }
            set
            {
                mLastOpenTag = value;
            }
        }

        private int mStart;

        internal int Start
        {
            get
            {
                return mStart;
            }
            set
            {
                mStart = value;
            }
        }

        private int mLength;

        internal int Length
        {
            get
            {
                return mLength;
            }
            set
            {
                mLength = value;
            }
        }

        internal void reset()
        {
            mAttributeName = null;
            mCurrNs = null;
            mCurrTag = null;
            mLastOpenTag = null;
            mContext = XmlPosContext.Undetected;
            mStart = mLength = -1;
        }
    }

        /// <summary>
        /// Get context on the window
        /// </summary>
        /// <param name="window">The text window to get the context</param>
        /// <param name="pos">The context to be filled</param>
        /// <param name="extend">Whether the tag name shall be extended </param>
        internal void GetContext(TextWindow window, XmlPos pos, bool extend)
        {
            //reset the current position
            pos.reset();

            int line = window.CursorRow;
            int column = window.CursorColumn;
            XceFileBuffer buff = window.Text;
            SyntaxHighlighter colorer = window.Highlighter;

            //check the current line

            bool rc = colorer.GetFirstRegion(line);
            while (rc)
            {
                SyntaxHighlighterRegion r = colorer.CurrentRegion;

                //in the current line analyze only the regions which are
                //started before the current position
                if ((extend && r.StartColumn <= column) ||
                    (!extend && r.StartColumn <= column))
                {
                    if ((r.Length < 0 || r.EndColumn > column))
                    {
                        if (r.Is(mXmlCData))
                        {
                            pos.Context = XmlPosContext.InCData;
                            break;
                        }
                        else if (r.Is(mXmlComment))
                        {
                            pos.Context = XmlPosContext.InComment;
                            break;
                        }
                    }
                }
                rc = colorer.GetNextRegion();
            }
        }
