using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.configuration;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.editor.search;
using gehtsoft.xce.editor.util;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.text;
using gehtsoft.xce.spellcheck;
using gehtsoft.xce.colorer;

[assembly: ComVisible(true)]
[assembly: Guid("3B315FDB-846B-43D7-8284-367F11A2FB82")]


namespace gehtsoft.xce.extension.scriptimpl
{
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsDual), Guid("5B1C6EDF-A777-4667-B9FF-74CB474E6817")]
    public interface IEditorWrapper
    {
        bool SaveRequired
        {
            get;
            set;
        }

        int CursorLine
        {
            get;
            set;
        }

        int CursorColumn
        {
            get;
            set;
        }

        int TopLine
        {
            get;
            set;
        }

        int TopColumn
        {
            get;
            set;
        }

        int WindowWidth
        {
            get;
        }

        int WindowHeight
        {
            get;
        }

        string FileName
        {
            get;
        }

        bool InsertMode
        {
            get;
        }

        int BlockMode
        {
            get;
        }

        int BlockStartLine
        {
            get;
        }

        int BlockStartColumn
        {
            get;
        }

        int BlockEndLine
        {
            get;
        }

        int BlockEndColumn
        {
            get;
        }

        int BlockType_None
        {
            get;
        }

        int BlockType_Line
        {
            get;
        }

        int BlockType_Box
        {
            get;
        }

        int BlockType_Stream
        {
            get;
        }

        string CurrentLine
        {
            get;
        }

        int CurrentLineLength
        {
            get;
        }

        int LinesCount
        {
            get;
        }

        string SyntaxRegionName
        {
            get;
        }

        int SyntaxRegionStart
        {
            get;
        }

        int SyntaxRegionLength
        {
            get;
        }

        void MessageBox(string text, string title);
        bool MessageBoxYesNo(string text, string title);
        string Prompt(string label, string initial, int width, string title);

        void ExecuteCommand(string command);
        void ExecuteCommandWithParam(string command, string parameter);
        void Stroke(string text);
        string Line(int lineIndex);
        int LineLength(int lineIndex);
        bool FirstSyntaxRegion(int lineIndex);
        bool NextSyntaxRegion();
        void Search(string re, bool ignoreCase);
        string BlockText
        {
            get;
        }
    }

    [ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), Guid("AE06E68A-37DF-46C6-B4C9-1CA13410F186")]
    public class EditorWrapper : IEditorWrapper
    {
        private Application mApplication;
        private string mSyntaxRegionName;
        private int mSyntaxRegionStart;
        private int mSyntaxRegionLength;

        public bool SaveRequired
        {
            get
            {
                return mApplication.ActiveWindow.SaveRequired;
            }

            set
            {
                mApplication.ActiveWindow.SaveRequired = value;
            }
        }

        internal EditorWrapper(Application application)
        {
            mApplication = application;
        }

        public int CursorLine
        {
            get
            {
                return mApplication.ActiveWindow.CursorRow;
            }
            set
            {
                mApplication.ActiveWindow.CursorRow = value;
                mApplication.ActiveWindow.EnsureCursorVisible();
            }

        }

        public int CursorColumn
        {
            get
            {
                return mApplication.ActiveWindow.CursorColumn;
            }
            set
            {
                mApplication.ActiveWindow.CursorColumn = value;
                mApplication.ActiveWindow.EnsureCursorVisible();
            }
        }

        public int TopLine
        {
            get
            {
                return mApplication.ActiveWindow.TopRow;
            }
            set
            {
                mApplication.ActiveWindow.TopRow = value;
            }
        }

        public int TopColumn
        {
            get
            {
                return mApplication.ActiveWindow.TopColumn;
            }
            set
            {
                mApplication.ActiveWindow.TopColumn = value;
            }
        }

        public int WindowWidth
        {
            get
            {
                return mApplication.ActiveWindow.Width;
            }
        }

        public int WindowHeight
        {
            get
            {
                return mApplication.ActiveWindow.Height;
            }
        }

        public string FileName
        {
            get
            {
                return mApplication.ActiveWindow.Text.FileName;
            }
        }

        public bool InsertMode
        {
            get
            {
                return mApplication.ActiveWindow.InsertMode;
            }
        }

        public int BlockMode
        {
            get
            {
                return (int)mApplication.ActiveWindow.BlockType;
            }
        }

        public int BlockStartLine
        {
            get
            {
                return mApplication.ActiveWindow.BlockLineStart;
            }
        }

        public int BlockStartColumn
        {
            get
            {
                return mApplication.ActiveWindow.BlockColumnStart;
            }
        }

        public int BlockEndLine
        {
            get
            {
                return mApplication.ActiveWindow.BlockLineEnd;
            }
        }

        public int BlockEndColumn
        {
            get
            {
                return mApplication.ActiveWindow.BlockColumnEnd;
            }
        }

        public int BlockType_None
        {
            get
            {
                return (int)TextWindowBlock.None;
            }
        }

        public int BlockType_Line
        {
            get
            {
                return (int)TextWindowBlock.Line;
            }
        }

        public int BlockType_Box
        {
            get
            {
                return (int)TextWindowBlock.Box;
            }

        }

        public int BlockType_Stream
        {
            get
            {
                return (int)TextWindowBlock.Stream;
            }
        }

        public string CurrentLine
        {
            get
            {
                return Line(mApplication.ActiveWindow.CursorRow);
            }
        }

        public int CurrentLineLength
        {
            get
            {
                return LineLength(mApplication.ActiveWindow.CursorRow);
            }
        }

        public int LinesCount
        {
            get
            {
                return mApplication.ActiveWindow.Text.LinesCount;
            }
        }

        public void MessageBox(string text, string title)
        {
            mApplication.ShowMessage(text, title);
        }

        public bool MessageBoxYesNo(string text, string title)
        {
            return mApplication.ShowYesNoMessage(text, title);
        }

        public string Prompt(string label, string initial, int width, string title)
        {
            PromptDialog dlg = new PromptDialog(title, label, initial, width, mApplication.ColorScheme);
            if (dlg.DoModal(mApplication.WindowManager) == Dialog.DialogResultOK)
                return dlg.Value;
            else
                return initial;
        }

        public void ExecuteCommand(string command)
        {
            ExecuteCommandWithParam(command, null);
        }

        public void ExecuteCommandWithParam(string command, string parameter)
        {
            IEditorCommand _command = mApplication.Commands[command];
            if (_command == null)
                throw new Exception("Command " + command + " not found");
            _command.Execute(mApplication, parameter);
        }

        public void Stroke(string text)
        {
            mApplication.ActiveWindow.Stroke(text, 0, text.Length);
        }

        public string Line(int lineIndex)
        {
            if (lineIndex < 0 || lineIndex >= mApplication.ActiveWindow.Text.LinesCount)
                return "";
            return mApplication.ActiveWindow.Text.GetRange(mApplication.ActiveWindow.Text.LineStart(lineIndex), mApplication.ActiveWindow.Text.LineLength(lineIndex, false));
        }

        public int LineLength(int lineIndex)
        {
            if (lineIndex < 0 || lineIndex >= mApplication.ActiveWindow.Text.LinesCount)
                return 0;
            return mApplication.ActiveWindow.Text.LineLength(lineIndex, false);
        }

        public string SyntaxRegionName
        {
            get
            {
                return mSyntaxRegionName;
            }
        }

        public int SyntaxRegionStart
        {
            get
            {
                return mSyntaxRegionStart;
            }

        }

        public int SyntaxRegionLength
        {
            get
            {
                return mSyntaxRegionLength;
            }
        }

        public bool FirstSyntaxRegion(int lineIndex)
        {
            if (mApplication.ActiveWindow == null)
                return false;

            if (mApplication.ActiveWindow.Highlighter == null)
                return false;

            if (mApplication.ActiveWindow.Highlighter.GetFirstRegion(lineIndex) && mApplication.ActiveWindow.Highlighter.CurrentRegion != null)
            {
                if (mApplication.ActiveWindow.Highlighter.CurrentRegion.IsSyntaxRegion)
                    mSyntaxRegionName = mApplication.ActiveWindow.Highlighter.CurrentRegion.Name;
                else
                    mSyntaxRegionName = "(no name)";
                mSyntaxRegionStart = mApplication.ActiveWindow.Highlighter.CurrentRegion.StartColumn;
                mSyntaxRegionLength = mApplication.ActiveWindow.Highlighter.CurrentRegion.Length;
                return true;
            }
            else
                return false;


        }

        public bool NextSyntaxRegion()
        {
            if (mApplication.ActiveWindow == null)
                return false;

            if (mApplication.ActiveWindow.Highlighter == null)
                return false;

            if (mApplication.ActiveWindow.Highlighter.GetNextRegion() && mApplication.ActiveWindow.Highlighter.CurrentRegion != null)
            {
                if (mApplication.ActiveWindow.Highlighter.CurrentRegion.IsSyntaxRegion)
                    mSyntaxRegionName = mApplication.ActiveWindow.Highlighter.CurrentRegion.Name;
                else
                    mSyntaxRegionName = "(no name)";
                mSyntaxRegionStart = mApplication.ActiveWindow.Highlighter.CurrentRegion.StartColumn;
                mSyntaxRegionLength = mApplication.ActiveWindow.Highlighter.CurrentRegion.Length;
                return true;
            }
            else
                return false;
        }

        public void Search(string re, bool ignoreCase)
        {
            XceSearchController.Search(mApplication, re, ignoreCase);
        }

        public string BlockText
        {
            get
            {
                if (mApplication.ActiveWindow == null)
                    return "";
                if (mApplication.ActiveWindow.BlockType == TextWindowBlock.None)
                    return "";
                BlockContent c = new BlockContent(mApplication.ActiveWindow);
                return c.Buffer.GetRange(0, c.Buffer.Length);
            }
        }
    }
}

