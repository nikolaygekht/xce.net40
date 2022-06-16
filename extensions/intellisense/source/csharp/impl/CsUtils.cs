using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using gehtsoft.intellisense.cs;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.text;
using gehtsoft.xce.intellisense.common;

namespace gehtsoft.intellisense.cs
{
    internal class CsUtils : CommonUtils
    {
        internal static CsProject FindCsProject(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            DirectoryInfo dir = fi.Directory;
            return SearchCsprojIn(dir, fileName);
        }

        private static CsProject SearchCsprojIn(DirectoryInfo dir, string fileName)
        {
            string[] files = Directory.GetFiles(dir.FullName, "*.csproj");
            foreach (string prjPath in files)
            {
                try
                {
                    CsProject prj = CsProjectLoader.load(prjPath);
                    if (prj.Sources.IndexOf(fileName) >= 0)
                        return prj;
                }
                catch (Exception )
                {
                }
            }
            if (dir.Parent != null)
                return SearchCsprojIn(dir.Parent, fileName);
            else
                return null;
        }
    }
}
