using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using gehtsoft.xce.intellisense.common;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;


namespace gehtsoft.intellisense.cs
{
    internal class CsProjectBrowserItem : IProjectBrowserItem
    {
        private ProjectBrowserItemType mItemType;

        public ProjectBrowserItemType ItemType
        {
            get
            {
                return mItemType;
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

        private string mFileName;
        public string FileName
        {
            get
            {
                return mFileName;
            }
        }

        private int mStartLine;
        public int StartLine
        {
            get
            {
                return mStartLine;
            }
        }

        private int mStartColumn;
        public int StartColumn
        {
            get
            {
                return mStartColumn;
            }
        }

        CsProjectBrowserItemCollection mChildren;
        public IProjectBrowserItemCollection Children
        {
            get
            {
                return mChildren;
            }
        }

        public bool HasChildren
        {
            get
            {
                return mChildren != null && mChildren.Count > 0;
            }
        }

        internal CsProjectBrowserItem(string name, string fullPath)
        {
            mItemType = ProjectBrowserItemType.File;
            mFileName = fullPath;
            mName = name;
            mStartLine = mStartColumn = 0;
        }

        internal CsProjectBrowserItem(string fileName, IClass iclass)
        {
            mFileName = fileName;
            mName = "class " + iclass.FullyQualifiedName;
            mStartLine = iclass.Region.BeginLine - 1;
            mStartColumn = iclass.Region.BeginColumn - 1;
            mItemType = ProjectBrowserItemType.Class;
        }

        internal CsProjectBrowserItem(string fileName, IMember member)
        {
            mFileName = fileName;
            mName = member.Name;
            mStartLine = member.Region.BeginLine - 1;
            mStartColumn = member.Region.BeginColumn - 1;
            if (member is IField)
                mItemType = ProjectBrowserItemType.Field;
            else if (member is IProperty)
                mItemType = ProjectBrowserItemType.Property;
            else if (member is IMethod)
                mItemType = ProjectBrowserItemType.Method;
            else if (member is IEvent)
                mItemType = ProjectBrowserItemType.Event;
            else
                mItemType = ProjectBrowserItemType.Unknown;
        }

        internal void AddChild(CsProjectBrowserItem item)
        {
            if (mChildren == null)
                mChildren = new CsProjectBrowserItemCollection();
            mChildren.Add(item);
        }
    }

    public class CsProjectBrowserItemCollection : IProjectBrowserItemCollection
    {
        private List<CsProjectBrowserItem> mList = new List<CsProjectBrowserItem>();

        IEnumerator<IProjectBrowserItem> IEnumerable<IProjectBrowserItem>.GetEnumerator()
        {
            return new EnumeratorConvertor<CsProjectBrowserItem, IProjectBrowserItem>(mList.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return mList.Count;
            }

        }

        public IProjectBrowserItem this[int index]
        {
            get
            {
                return mList[index];
            }

        }

        internal void Add(CsProjectBrowserItem item)
        {
            mList.Add(item);
        }
    }

    internal class CsProjectBrowserItemCollectionFactory
    {
        internal static CsProjectBrowserItemCollection create(CsParser parser, string currentFile, int currentLine, int currentColumn, IEntity searchEntity, out CsProjectBrowserItem curSel)
        {
            CsProjectBrowserItemCollection collection = new CsProjectBrowserItemCollection();
            curSel = null;

            string baseDir;
            string name;

            if (parser.Project.IsDefault)
                name = currentFile;
            else
                name = parser.Project.Name;
            FileInfo fi = new FileInfo(name);
            baseDir = fi.Directory.FullName;

            currentLine++;
            currentColumn++;

            foreach (CsParserFile file in parser.Files)
            {
                string n = file.Name;
                if (n.StartsWith(baseDir, StringComparison.InvariantCultureIgnoreCase))
                    n = n.Substring(baseDir.Length + 1);
                CsProjectBrowserItem ifile = new CsProjectBrowserItem(n, file.Name);
                collection.Add(ifile);

                if (searchEntity == null)
                {
                    if (string.Compare(file.Name, currentFile, true) == 0)
                        curSel = ifile;
                }

                foreach (IClass iclass in file.CompilationUnit.Classes)
                    add(ifile, iclass, file.Name, curSel == ifile, currentLine, currentColumn, searchEntity, ref curSel);
            }
            return collection;
        }

        private static void add(CsProjectBrowserItem container, IClass iclass, string fileName, bool thisFile, int fileLine, int fileColumn, IEntity searchEntity, ref CsProjectBrowserItem curSel)
        {
            CsProjectBrowserItem cclass = new CsProjectBrowserItem(fileName, iclass);
            container.AddChild(cclass);

            if (thisFile && iclass.Region.IsInside(fileLine, fileColumn) || iclass == searchEntity)
                curSel = cclass;


            foreach (IField field in iclass.Fields)
            {
                CsProjectBrowserItem cc = new CsProjectBrowserItem(fileName, field);
                cclass.AddChild(cc);
                if (thisFile && field.Region.IsInside(fileLine, fileColumn) || searchEntity == field)
                    curSel = cc;
            }
            foreach (IProperty property in iclass.Properties)
            {
                CsProjectBrowserItem cc = new CsProjectBrowserItem(fileName, property);
                cclass.AddChild(cc);
                if (thisFile && (property.Region.IsInside(fileLine, fileColumn) || property.BodyRegion.IsInside(fileLine, fileColumn)) || searchEntity == property)
                    curSel = cc;
            }
            foreach (IMethod method in iclass.Methods)
            {
                CsProjectBrowserItem cc = new CsProjectBrowserItem(fileName, method);
                cclass.AddChild(cc);
                if (thisFile && (method.Region.IsInside(fileLine, fileColumn) || method.BodyRegion.IsInside(fileLine, fileColumn)) || searchEntity == method)
                    curSel = cc;
            }
            foreach (IEvent evt in iclass.Events)
            {
                CsProjectBrowserItem cc = new CsProjectBrowserItem(fileName, evt);
                cclass.AddChild(cc);
                if (thisFile && evt.Region.IsInside(fileLine, fileColumn) || searchEntity == evt)
                    curSel = cc;
            }

            foreach (IClass cls1 in iclass.InnerClasses)
                add(cclass, cls1, fileName, thisFile, fileLine, fileColumn, searchEntity, ref curSel);
        }

    }

}
