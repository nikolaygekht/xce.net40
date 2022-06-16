using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.text;

namespace gehtsoft.xce.editor.command.impl
{
    internal class SaveFileCommand : IEditorCommand
    {
        IEditorCommand mSaveAsCommand = null;

        internal SaveFileCommand()
        {

        }

        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "SaveFile";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            if (mSaveAsCommand == null)
                mSaveAsCommand = application.Commands["SaveFileAs"];

            TextWindow w = application.ActiveWindow;
            if (w == null)
                return ;
            if (w.Text.FileName.Length == 0)
                mSaveAsCommand.Execute(application, null);
            else
            {
                if (!SaveFileCommandUtil.CheckOpenToWrite(application, w.Text.FileName))
                    return ;
                w.FireSaveWindowEvent();
                try
                {
                    w.Text.Save(!w.FileTypeInfo.IgnoreBOM, null);
                }
                catch (Exception e)
                {
                    MessageBox.Show(application.WindowManager, application.ColorScheme,
                                    string.Format("Can't save file {0}\r\n{1}", w.Text.FileName, e.Message),
                                    "Error", MessageBoxButtonSet.Ok);
                }
                w.invalidate();
                GC.Collect();
            }
        }

        /// <summary>
        /// Get checked status for the menu.
        /// </summary>
        public bool IsChecked(Application application, string parameter)
        {
            return false;
        }

        /// <summary>
        /// Get enabled status for the menu with parameter.
        /// </summary>
        public bool IsEnabled(Application application, string parameter)
        {
            return application.ActiveWindow != null && application.ActiveWindow.Text.FileName.Length != 0;
        }
    }
    
    internal class SaveFileCommandUtil
    {
        internal static bool CheckOpenToWrite(Application app, string fileName)
        {
            if (File.Exists(fileName))
            {
                FileInfo fi = new FileInfo(fileName);
                if (fi.IsReadOnly)
                {
                    MessageBoxButton b = MessageBox.Show(app.WindowManager, app.ColorScheme,
                                                         string.Format("File\r\n{0}\r\nis read only? Make it writeable?", fileName),
                                                         "Save", MessageBoxButtonSet.YesCancel);
                    if (b == MessageBoxButton.Yes)
                    {
                        try
                        {
                            fi.IsReadOnly = false;
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(app.WindowManager, app.ColorScheme, string.Format("Can't change the file access\r\n{0}", e.Message), "Save", MessageBoxButtonSet.Ok);
                            return false;
                        }
                        return true;
                           
                    }
                    else
                        return false;
                }
                else
                    return true;
            }
            else
            {
                FileInfo fi = new FileInfo(fileName);
                if (!fi.Directory.Exists)
                {
                    MessageBoxButton b = MessageBox.Show(app.WindowManager, app.ColorScheme,
                                                         string.Format("Directory\r\n{0}\r\ndoes not exist. Create Directory?", fi.Directory.FullName),
                                                         "Save", MessageBoxButtonSet.YesCancel);
                    if (b == MessageBoxButton.Yes)
                    {
                        try
                        {
                            Directory.CreateDirectory(fi.Directory.FullName);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(app.WindowManager, app.ColorScheme, string.Format("Directory can't be created\r\n{0}", e.Message), "Save", MessageBoxButtonSet.Ok);
                            return false;
                        }
                        return true;
                    }
                    else
                        return false;                                                         
                }
                else
                    return true;
            }
        }
    }
}
