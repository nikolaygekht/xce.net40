using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using gehtsoft.xce.colorer;


namespace gehtsoft.xce.extension.scriptimpl
{
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsDual), Guid("962366D6-1C6F-46CD-A372-5FB86BD1DD4F")]
    public interface ISystem
    {
        string NewGuid();

        string GetApplicationPath();

        string GetFullPath(string path);

        string CombinePaths(string path1, string path2);

        bool FileExists(string path);

        bool DirectoryExists(string path);

        string Environment(string variable);
        string ExpandEnvironmentVariables(string text);

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)]
        object Files(string path, string mask);

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)]
        object Directories(string path);

        string ParentDirectory(string path);

        string DirectoryFromPath(string path);

        string FileNameFromPath(string path);

        string FileExtensionFromPath(string path);

        string GetProfileString(string file, string section, string key);

        string FindInPaths(string pathline, string fileName, string currentPath);

        string ReadFile(string file);

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)]
        object RegexMatch(string regex, string str);

        void WriteFile(string file, string text, string encoding);
        void AppendFile(string file, string text, string encoding);
        void CopyFile(string src, string dst, bool overwrite);
        void MoveFile(string src, string dst, bool overwrite);
        void DeleteFile(string src);
        void CreateDirectory(string path);
        string ExecuteProcess(string application, string commandLine);
        void StartProcess(string application, string commandLine);
    }

    [ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), Guid("EB92A632-CB18-409C-9BD1-CBD2E4CCD949")]
    public class SystemImpl : ISystem
    {
        private string mAppPath;

        public SystemImpl(string appPath)
        {
            mAppPath = appPath;
        }

        public string GetApplicationPath()
        {
            return mAppPath;
        }

        public string NewGuid()
        {
            return System.Guid.NewGuid().ToString();
        }

        public string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }

        public string CombinePaths(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public string Environment(string variable)
        {
            string s = System.Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Process);
            if (s == null)
                return "";
            else
                return s;
        }

        public string ExpandEnvironmentVariables(string variable)
        {
            return System.Environment.ExpandEnvironmentVariables (variable);
        }

        internal class FileNameComparer : IComparer<string>
        {
            public int Compare(string a, string b)
            {
                return string.Compare(a, b, true);
            }
        }

        private FileNameComparer mFileNameComparer = new FileNameComparer();


        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)]
        public object Files(string path, string mask)
        {
            string[] s;
            object[] s1;
            s = Directory.GetFiles(path, mask);
            Array.Sort<string>(s, mFileNameComparer);
            s1 = new object[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                FileInfo fi = new FileInfo(s[i]);
                s1[i] = (object)(fi.Name);
            }
            return s1;
        }

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)]
        public object Directories(string path)
        {
            string[] s = Directory.GetDirectories(path);
            Array.Sort<string>(s, mFileNameComparer);
            object[] s1 = new object[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                DirectoryInfo di = new DirectoryInfo(s[i]);
                s1[i] = (object)(di.Name);
            }
            return s1;
        }

        public string ParentDirectory(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            if (di.Exists)
            {
                di = di.Parent;
                if (di == null)
                    return "";
                else
                    return di.FullName;
            }
            else
                return "";
        }

        public string DirectoryFromPath(string path)
        {
            FileInfo fi = new FileInfo(path);
            return fi.DirectoryName;
        }

        public string FileNameFromPath(string path)
        {
            FileInfo fi = new FileInfo(path);
            return fi.Name;
        }

        public string FileExtensionFromPath(string path)
        {
            FileInfo fi = new FileInfo(path);
            return fi.Extension;
        }

        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringW",
                   SetLastError = true,
                   CharSet = CharSet.Unicode, ExactSpelling = true,
                   CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPrivateProfileString(string lpAppName,
                                                          string lpKeyName,
                                                          string lpDefault,
                                                          string lpReturnString,
                                                          int nSize,
                                                          string lpFilename);


        public string GetProfileString(string file, string section, string key)
        {
            string returnString = new string(' ', 1024);
            GetPrivateProfileString(section, key, "", returnString, 1024, file);
            return returnString.Split('\0')[0];
        }

        public string FindInPaths(string pathline, string fileName, string currentPath)
        {

            string savedCurrentDirectory = System.Environment.CurrentDirectory;
            System.Environment.CurrentDirectory = currentPath;

            try
            {
                string[] paths = pathline.Split(';');
                foreach (string path in paths)
                {
                    try
                    {
                        string path1 = Path.GetFullPath(path);
                        string path2 = Path.Combine(path1, fileName);
                        if (File.Exists(path2))
                            return Path.GetFullPath(path2);
                    }
                    catch (Exception )
                    {
                        continue;
                    }
                }
                return "";
            }
            finally
            {
                System.Environment.CurrentDirectory = savedCurrentDirectory;
            }
        }

        public string ReadFile(string file)
        {
            StreamReader r = new StreamReader(file, true);
            try
            {
                return r.ReadToEnd();
            }
            finally
            {
                r.Close();
            }
        }

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)]
        public object RegexMatch(string regex, string str)
        {
            Regex re = new Regex(regex);
            try
            {
                object[] r;
                if (re.Match(str))
                {
                    r = new object[re.MatchesCount];
                    for (int i = 0; i < re.MatchesCount; i++)
                        r[i] = re.Value(i);
                }
                else
                    r = new object[0];
                return r;
            }
            finally
            {
                if (re != null)
                    re.Dispose();
            }

        }

        public void WriteFile(string file, string text, string encoding)
        {
            StreamWriter w = null;
            try
            {
                w = new StreamWriter(file, false, Encoding.GetEncoding(encoding));
                w.Write(text);
            }
            finally
            {
                if (w != null)
                    w.Close();
            }
        }

        public void AppendFile(string file, string text, string encoding)
        {
            StreamWriter w = null;
            try
            {
                w = new StreamWriter(file, true, Encoding.GetEncoding(encoding));
                w.Write(text);
            }
            finally
            {
                if (w != null)
                    w.Close();
            }

        }

        public void CopyFile(string src, string dst, bool overwrite)
        {
            File.Copy(src, dst, overwrite);
        }

        public void MoveFile(string src, string dst, bool overwrite)
        {
            if (File.Exists(dst))
            {
                if (overwrite)
                    File.Delete(dst);
                else
                    throw new Exception(string.Format("File {0} exists", dst));
            }
            File.Move(src, dst);
        }

        public void DeleteFile(string src)
        {
            if (File.Exists(src))
                File.Delete(src);
            else if (Directory.Exists(src))
                Directory.Delete(src);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        internal class InputReceiver
        {
            StringBuilder b;
            object m;

            internal InputReceiver()
            {
                b = new StringBuilder();
                m = new object();
            }

            internal string Text
            {
                get
                {
                    return b.ToString();
                }
            }

            internal void DataHandler(object sendingProcess, DataReceivedEventArgs errLine)
            {
                lock(m) {
                    if (errLine.Data != null)
                    {
                        if (errLine.Data.Length > 0)
                            b.Append(errLine.Data);
                        b.Append("\r\n");
                    }
                }
            }
        }

        public string ExecuteProcess(string application, string commandLine)
        {
            Process p = new Process();
            p.StartInfo.FileName = application;
            p.StartInfo.Arguments = commandLine;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            InputReceiver r = new InputReceiver();

            p.ErrorDataReceived += new DataReceivedEventHandler(r.DataHandler);
            p.OutputDataReceived += new DataReceivedEventHandler(r.DataHandler);
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.WaitForExit();
            p.Close();
            return r.Text;
        }

        public void StartProcess(string application, string commandLine)
        {
            Process p = new Process();

            p.StartInfo.FileName = application;
            p.StartInfo.Arguments = commandLine;

            p.Start();
        }
    }
}
