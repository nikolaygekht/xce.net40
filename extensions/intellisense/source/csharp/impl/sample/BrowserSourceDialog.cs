using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using ICSharpCode.SharpDevelop.Dom;
using gehtsoft.intellisense.cs;

namespace gehtsoft.xce.extension.csintellisense_impl
{
    internal class BrowserSourceDialog : XceDialog
    {
        internal class BrowserFileInfo
        {
            public string Name;
            public int Line;
            public int Column;

            public BrowserFileInfo(string name, int line, int column)
            {
                Name = name;
                Line = line;
                if (Line < 0)
                    Line = 0;
                Column = column;
                if (Column < 0)
                    Column = 0;
            }
        }
        
        BrowserFileInfo mInfo = null;
        
        internal BrowserFileInfo Selected
        {
            get
            {
                return mInfo;
            }
        }
        
        internal bool SelectionMade
        {
            get
            {
                return mList.CurSel >= 0;
            }
        }
    
        DialogItemSearchingList mList;
        DialogItemButton mOk;
        DialogItemButton mCancel;
    
        internal BrowserSourceDialog(Application application, CsParser parser, string fileName, int cline, int ccolumn) : base(application, "C# Project", true, 20, 60)
        {
            AddItem(mList = new DialogItemSearchingList(0x1000, 0, 0, 17, 58));
            cline++; 
            ccolumn++;
            AddFiles(parser, fileName, cline, ccolumn, null);
            if (mList.CurSel >= 0)
                mList.EnsureVisible(mList.CurSel);
            AddItem(mOk = new DialogItemButton("< &Ok >", DialogResultOK, 17, 0));
            AddItem(mCancel = new DialogItemButton("< &Cancel >", DialogResultCancel, 17, 0));
            CenterButtons(mOk, mCancel);
        }

        internal BrowserSourceDialog(Application application, CsParser parser, string fileName, IEntity entity)
            : base(application, "C# Project", true, 20, 60)
        {
            AddItem(mList = new DialogItemSearchingList(0x1000, 0, 0, 17, 58));
            AddFiles(parser, fileName, 0, 0, entity);
            if (mList.CurSel >= 0)
                mList.EnsureVisible(mList.CurSel);
            AddItem(mOk = new DialogItemButton("< &Ok >", DialogResultOK, 17, 0));
            AddItem(mCancel = new DialogItemButton("< &Cancel >", DialogResultCancel, 17, 0));
            CenterButtons(mOk, mCancel);
        }

        
        private void AddFiles(CsParser parser, string fileName, int cline, int ccolumn, IEntity entity)
        {
            string baseDir;
            string name;
            
            if (parser.Project.IsDefault)
                name = fileName;
            else
                name = parser.Project.Name;
            FileInfo fi = new FileInfo(name);
            baseDir = fi.Directory.FullName;
            
            foreach (CsParserFile file in parser.Files)
            {
                string n = file.Name;
                if (n.StartsWith(baseDir, StringComparison.InvariantCultureIgnoreCase))
                    n = n.Substring(baseDir.Length + 1);
                mList.AddItem(n, new BrowserFileInfo(file.Name, 0, 0));
                bool thisFile = false;
                if (entity == null)
                {
                    if (string.Compare(file.Name, fileName, true) == 0)
                    {
                        thisFile = true;
                        mList.CurSel = mList.Count - 1;
                    }
                }
                foreach (IClass cls in file.CompilationUnit.Classes)
                {
                    AddClass(file.Name, cls, thisFile, cline, ccolumn, entity);
                }
            }
        }
        
        private void AddClass(string file, IClass cls, bool thisFile, int row, int column, IEntity entity)
        {
            mList.AddItem(" class " + cls.FullyQualifiedName, new BrowserFileInfo(file, cls.Region.BeginLine - 1, cls.Region.BeginColumn - 1));
            
            if (thisFile && cls.Region.IsInside(row, column) || cls == entity)
                mList.CurSel = mList.Count - 1;

            foreach (IField field in cls.Fields)
            {
                mList.AddItem("  f:" + field.Name, new BrowserFileInfo(file, field.Region.BeginLine - 1, field.Region.BeginColumn - 1));
                if (thisFile && field.Region.IsInside(row, column) || field == entity)
                    mList.CurSel = mList.Count - 1;
            }
            foreach (IProperty property in cls.Properties)
            {
                mList.AddItem("  p:" + property.Name, new BrowserFileInfo(file, property.Region.BeginLine - 1, property.Region.BeginColumn - 1));
                if (thisFile && property.Region.IsInside(row, column) || property == entity)
                    mList.CurSel = mList.Count - 1;
            }
            foreach (IMethod method in cls.Methods)
            {
                mList.AddItem("  m:" + method.Name, new BrowserFileInfo(file, method.Region.BeginLine - 1, method.Region.BeginColumn - 1));
                if (thisFile && method.Region.IsInside(row, column) || method == entity)
                    mList.CurSel = mList.Count - 1;
            }
            foreach (IEvent evt in cls.Events)
            {
                mList.AddItem("  e:" + evt.Name, new BrowserFileInfo(file, evt.Region.BeginLine - 1, evt.Region.BeginColumn - 1));
                if (thisFile && evt.Region.IsInside(row, column) || evt == entity)
                    mList.CurSel = mList.Count - 1;
            }

            foreach (IClass cls1 in cls.InnerClasses)
                AddClass(file, cls1, thisFile, row, column, entity);
        }
        
        public override void OnSizeChanged()
        {
            base.OnSizeChanged();
            if (Height < 9 || Width < 20)
                move(Row, Column, Math.Max(Height, 9), Math.Max(Width, 20));
            else
            {
                int height, width;
                height = Height;
                width = Width;
                
                mList.move(0, 0, height - 3, width - 2);
                mOk.move(height - 3, 0, 1, mOk.Title.Length);
                mCancel.move(height - 3, 0, 1, mCancel.Title.Length);
                CenterButtons(mOk, mCancel);
                invalidate();
            }
        }

        public override bool OnOK()
        {
            if (mList.CurSel >= 0 && mList.CurSel < mList.Count)
            {
                mInfo = mList[mList.CurSel].UserData as BrowserFileInfo;
                if (mInfo != null)
                    return true;
            }
            mApplication.ShowMessage("Please select a file, class or class member", "C# Project");
            return false;
        }
    }
}
