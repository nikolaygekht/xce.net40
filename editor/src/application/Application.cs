using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.colorer;
using gehtsoft.xce.configuration;
using gehtsoft.xce.editor.configuration;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.editor.search;
using gehtsoft.xce.text;

namespace gehtsoft.xce.editor.application
{
    /// <summary>
    /// The editor application
    /// </summary>
    public class Application
    {
        private WindowManager mWindowManager;
        /// <summary>
        /// Window manager
        /// </summary>
        public WindowManager WindowManager
        {
            get
            {
                return mWindowManager;
            }
        }

        private ApplicationWindow mAppWindow;
        private FileTypeInfoCollection mFileTypes;
        private TextWindowCollection mTextWindows = new TextWindowCollection();
        private TextWindow mActiveTextWindow;
        private List<IEditorExtension> mExtensions = new List<IEditorExtension>();

        private XceColorScheme mColorScheme;
        /// <summary>
        /// Editor color scheme
        /// </summary>
        public XceColorScheme ColorScheme
        {
            get
            {
                return mColorScheme;
            }
        }

        private string mApplicationPath;
        /// <summary>
        /// Path to the application
        /// </summary>
        public string ApplicationPath
        {
            get
            {
                return mApplicationPath;
            }
        }

        private XceConfiguration mConfiguration;
        /// <summary>
        /// Editor Configuration
        /// </summary>
        public XceConfiguration Configuration
        {
            get
            {
                return mConfiguration;
            }
        }

        private ColorerFactory mColorerFactory;
        /// <summary>
        /// The colorer factory
        /// </summary>
        public ColorerFactory ColorerFactory
        {
            get
            {
                return mColorerFactory;
            }
        }

        private KeyboardShortcutCollection mKeymap;
        internal KeyboardShortcutCollection Keymap
        {
            get
            {
                return mKeymap;
            }
        }

        private MainMenu mMainMenu;
        internal MainMenu MainMenu
        {
            get
            {
                return mMainMenu;
            }
        }

        private EditorCommandCollection mCommands;
        public EditorCommandCollection Commands
        {
            get
            {
                return mCommands;
            }
        }

        internal FileTypeInfoCollection FileTypes
        {
            get
            {
                return mFileTypes;
            }
        }

        /// <summary>
        /// Initializes the application
        /// </summary>
        /// <param name="commandLine">The command line parameters</param>
        public void InitApplication(string[] commandLine)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            FileInfo fi = new FileInfo(executingAssembly.Location);
            mApplicationPath = fi.DirectoryName;
            mConfiguration = new XceConfiguration(Path.Combine(mApplicationPath, "xce.ini"));

            mColorerFactory = new ColorerFactory();
            mColorerFactory.Init(Path.Combine(mApplicationPath, mConfiguration.ColorerPath), "console", mConfiguration.ColorerColorScheme, mConfiguration.ColorerBackParse);
            mColorScheme = new XceColorScheme(mColorerFactory);

            mCommands = EditorCommandFactoryBuilder.build();

            List<string> errors = new List<string>();
            mConfiguration.InitSpellchecker(mApplicationPath, errors);

            ProfileSection common = mConfiguration["common"];
            foreach (ProfileKey key in common)
            {
                if (key.Name == "extension")
                {
                    try
                    {
                        Assembly a = Assembly.LoadFile(Path.Combine(mApplicationPath, key.Value.Trim() + ".dll"));
                        object o = a.CreateInstance(key.Value.Trim());
                        IEditorExtension ex = o as IEditorExtension;
                        if (ex != null)
                        {
                            ex.Initialize(this);
                            mExtensions.Add(ex);
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("warning: {0}", e.Message);
                    }
                }
            }

            ProfileSection keymap = mConfiguration["keymap." + mConfiguration.KeymapName];
            if (keymap == null)
                throw new Exception("The keymap " + mConfiguration.KeymapName + " is not found in the configuration");
            mKeymap = KeyboardShortcutBuilder.build(keymap, mCommands, errors);

            ProfileSection menu = mConfiguration["menu." + mConfiguration.MenuName];
            if (menu == null)
                throw new Exception("The menu " + mConfiguration.MenuName + " is not found in the configuration");
            mMainMenu = MainMenuBuilder.build(menu, mCommands, mKeymap, errors);

            mFileTypes = FileTypeInfoBuilder.build(mConfiguration.Profile, mColorerFactory, errors);

            if (errors.Count > 0)
                foreach (string s in errors)
                    Console.WriteLine("warning: {0}", s);

            //check command line
            int rows = -1, columns = -1;
            List<string> toOpen = new List<string>();
            Encoding forceEncoding = null;
            ConsoleOutputMode outputMode = ConsoleOutputMode.Win32;

            switch (common["output-mode", "win32"])
            {
            case    "win32":
                    outputMode = ConsoleOutputMode.Win32;
                    break;
            case    "conemu":
                    outputMode = ConsoleOutputMode.ConEmu;
                    break;
            case    "vt":
                    outputMode = ConsoleOutputMode.VT;
                    break;
            case    "vttc":
                    outputMode = ConsoleOutputMode.VTTC;
                    break;
            }

            foreach (string s in commandLine)
            {
                if (s[0] == '/' && s[1] == 'r' && s[2] == ':')
                {
                    if (!Int32.TryParse(s.Substring(3), out rows))
                        rows = -1;
                }
                else if (s[0] == '/' && s[1] == 'c' && s[2] == ':')
                {
                    if (!Int32.TryParse(s.Substring(3), out columns))
                        columns = -1;
                }
                else if (s[0] == '/' && s[1] == 'e' && s[2] == ':')
                {
                    string s1 = s.Substring(3);
                    try
                    {
                        forceEncoding = Encoding.GetEncoding(s1);
                    }
                    catch (Exception )
                    {
                        Console.WriteLine("warning: encoding {0} is not found", s1);
                        forceEncoding = null;
                    }
                }
                else if (s == "/autosize")
                {
                    rows = columns = 0;
                }
                else if (s == "/conemu")
                {
                    outputMode = ConsoleOutputMode.ConEmu;
                }
                else if (s == "/vt")
                {
                    outputMode = ConsoleOutputMode.VT;
                }
                else if (s == "/vttc")
                {
                    outputMode = ConsoleOutputMode.VTTC;
                }
                else
                {
                    toOpen.Add(s);
                }
            }

            if (rows >= 0 && columns >= 0)
            {
                mWindowManager = new WindowManager(false, rows, columns, outputMode);
            }
            else
            {
                mWindowManager = new WindowManager(true, outputMode);
            }

            mAppWindow = new ApplicationWindow(this);
            mAppWindow.create();

            if (toOpen.Count == 0)
                toOpen.Add("");

            foreach (string s in toOpen)
            {
                OpenFile(s, forceEncoding);
            }
        }


        public void ReleaseApplication()
        {
            foreach (IEditorExtension e in mExtensions)
            {
                if (e is IDisposable)
                    (e as IDisposable).Dispose();
            }

            mFileTypes.Dispose();

            if (mColorerFactory != null)
            {
                mColorerFactory.Dispose();
                mColorerFactory = null;
            }

            if (mWindowManager != null)
            {
                mWindowManager.Dispose();
                mWindowManager = null;
            }
        }

        private bool mQuitMessage = false;
        private long mLastTimer = DateTime.Now.Ticks;

        MessageBoxButtonInfo[] mExternalWarning = new MessageBoxButtonInfo[] { new MessageBoxButtonInfo("< &Reload >", MessageBoxButton.Ok),
                                                                               new MessageBoxButtonInfo("< &Switch to >", MessageBoxButton.Retry),
                                                                               new MessageBoxButtonInfo("< &Ignore >", MessageBoxButton.Cancel),
                                                                               new MessageBoxButtonInfo("< Ignore &All >", MessageBoxButton.Option1) };

        public void PumpMessages()
        {
            while (!mQuitMessage)
            {
                mWindowManager.pumpMessage(50);
                if (ActiveWindow != null)
                    ActiveWindow.OnIdle();
                try
                {
                    if (IdleEvent != null)
                        IdleEvent();
                }
                catch (Exception )
                {
                }

                long now = DateTime.Now.Ticks;
                if (Math.Abs(now - mLastTimer) > 2500000)
                {
                    mLastTimer = now;
                    if (TimerEvent != null)
                        TimerEvent();

                    foreach (TextWindow w in mTextWindows)
                    {
                        if (w.Text.HasBeenExtenrallyChanged && !w.IgnoreReload)
                        {
                            w.Text.ResetExternallyChanged();
                            FileInfo f = new FileInfo(w.Text.FileName);
                            Encoding e = w.Text.Encoding;
                            TextWindow prev = mActiveTextWindow;
                            ActivateWindow(w);
                            string s = string.Format("File {0}\r\n{1}\r\nhas been externally changed.", f.Name, f.FullName);
                            MessageBoxButton b = MessageBox.Show(mWindowManager, mColorScheme, s, "Warning", mExternalWarning);
                            switch (b)
                            {
                            case    MessageBoxButton.Ok:
                                    CloseWindow(w);
                                    OpenFile(f.FullName, e);
                                    break;
                            case    MessageBoxButton.Retry:
                                    break;
                            case    MessageBoxButton.Ignore:
                                    ActivateWindow(prev);
                                    break;
                            case    MessageBoxButton.Option1:
                                    w.IgnoreReload = true;
                                    ActivateWindow(prev);
                                    break;
                            }
                        }
                        break;
                    }
                }
            }
        }

        public void PostQuitMessage()
        {
            mQuitMessage = true;
        }

        public void ShowMessage(string message, string title)
        {
            MessageBox.Show(this.mWindowManager, this.mColorScheme, message, title, MessageBoxButtonSet.Ok);
        }

        public bool ShowYesNoMessage(string message, string title)
        {
            return MessageBox.Show(this.mWindowManager, this.mColorScheme, message, title, MessageBoxButtonSet.YesNo) == MessageBoxButton.Yes;
        }

        public void ShowMainMenu()
        {
            mMainMenu.ShowMainMenu(this);
            mAppWindow.invalidate();
        }

        public TextWindow OpenFile(string fileName)
        {
            return OpenFile(fileName, null);
        }

        internal TextWindow OpenFile(string fileName, Encoding forceEncoding)
        {
            foreach (TextWindow win in mTextWindows)
                if (string.Equals(win.Text.FileName, fileName, StringComparison.InvariantCultureIgnoreCase))
                {
                    ActivateWindow(win);
                    return ActiveWindow;
                }


            XceFileBuffer buffer = new XceFileBuffer();
            FileTypeInfo fi = mFileTypes.Find(fileName);
            Encoding e;

            buffer.ExpandTabs = true;
            buffer.TabSize = fi.TabSize;
            buffer.TrimSpace = fi.TrimEolSpace;

            if (forceEncoding == null)
                e = fi.Encoding;
            else
                e = forceEncoding;

            if (fileName.Length != 0)
            {
                if (File.Exists(fileName))
                    buffer.Load(e, fileName, fi.IgnoreBOM);
                else
                    buffer.New(e, fileName);
            }
            else
                buffer.New(e, "");

            TextWindow w = new TextWindow(buffer, this, fi);
            mWindowManager.create(w, mAppWindow, mAppWindow.StatusLineHeight, 0, mAppWindow.Height - mAppWindow.StatusLineHeight, mAppWindow.Width);
            mTextWindows.Add(w);
            ActivateWindow(w);
            if (AfterOpenWindowEvent != null)
                AfterOpenWindowEvent(w);
            return w;
        }

        public void CloseWindow(TextWindow w)
        {
            if (BeforeCloseWindowEvent != null)
                BeforeCloseWindowEvent(w);
            int index = mTextWindows.Find(w);
            if (index >= 0)
            {
                mWindowManager.close(w);
                w.Dispose();
                mTextWindows.Remove(index);
                if (w == mActiveTextWindow)
                {
                    mActiveTextWindow = null;

                    if (mTextWindows.Count > 0)
                    {
                        if (index >= mTextWindows.Count)
                            index = 0;
                        ActivateWindow(mTextWindows[index]);
                    }
                }
            }
        }

        public void ActivateWindow(TextWindow w)
        {
            if (w != mActiveTextWindow)
            {
                if (mActiveTextWindow != null)
                {
                    mActiveTextWindow.show(false);
                    mActiveTextWindow = null;
                }
            }
            mActiveTextWindow = w;
            mActiveTextWindow.show(true);
            mActiveTextWindow.OnActivate();
            mActiveTextWindow.invalidate();
            mAppWindow.invalidate();
        }

        public void repaint()
        {
            mAppWindow.invalidate();
        }

        public TextWindow ActiveWindow
        {
            get
            {
                return mActiveTextWindow;
            }
        }

        public TextWindowCollection TextWindows
        {
            get
            {
                return mTextWindows;
            }
        }

        private SearchInfo mLastSearchInfo = null;

        internal SearchInfo LastSearchInfo
        {
            get
            {
                return mLastSearchInfo;
            }
            set
            {
                mLastSearchInfo = value;
            }
        }


        public event AfterOpenWindowHook AfterOpenWindowEvent;
        public event BeforeCloseWindowHook BeforeCloseWindowEvent;
        public event TimerHook TimerEvent;
        public event KeyPressedHook KeyPressedEvent;
        public event IdleHook IdleEvent;

        internal void FireKeyPressedEvent(int scanCode, char character, bool shift, bool ctrl, bool alt, ref bool handled)
        {
            handled = false;
            if (KeyPressedEvent != null)
                KeyPressedEvent(scanCode, character, shift, ctrl, alt, ref handled);
        }

    }
}
