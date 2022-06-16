using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.textwindow;

namespace gehtsoft.xce.editor.configuration
{
    /// <summary>
    /// The main menu object
    /// </summary>
    internal class MainMenu
    {
        private List<MainMenuCommand> mCommands = new List<MainMenuCommand>();
        private PopupMenuItem mBar = null;
        private Stack<PopupMenuItem> mStack = new Stack<PopupMenuItem>();
        private PopupMenuItem mWindowList = null;

        /// <summary>
        /// Constructor. Don't use it directly, use the MainMenuBuilder instead
        /// </summary>
        internal MainMenu()
        {
        }

        /// <summary>
        /// Builder method - add a separator
        /// </summary>
        internal void CreateSeparator()
        {
            mStack.Peek().addItem(new SeparatorMenuItem());
        }

        internal void AddWindowList(string title)
        {
            PopupMenuItem submenu = new PopupMenuItem(title);
            if (mBar == null)
            {
                mBar = submenu;
            }
            else
            {
                mStack.Peek().addItem(submenu);
            }
            mWindowList = submenu;
        }

        /// <summary>
        /// Builder method start a new menu
        /// </summary>
        internal void StartSubmenu(string title)
        {
            PopupMenuItem submenu = new PopupMenuItem(title);
            if (mBar == null)
            {
                mBar = submenu;
                mStack.Push(submenu);
            }
            else
            {
                mStack.Peek().addItem(submenu);
                mStack.Push(submenu);
            }

        }

        /// <summary>
        /// Builder method end a menu
        /// </summary>
        internal void EndSubmenu()
        {
            mStack.Pop();
        }

        /// <summary>
        /// Builder method create a command
        /// </summary>
        internal void CreateCommand(string title, string shortcut, IEditorCommand command, string parameter)
        {
            CommandMenuItem item;
            if (shortcut != null)
                item = new CommandMenuItem(title, shortcut, mCommands.Count + 1);
            else
                item = new CommandMenuItem(title, mCommands.Count + 1);
            mCommands.Add(new MainMenuCommand(item, command, parameter));
            mStack.Peek().addItem(item);
        }

        /// <summary>
        /// Show main menu and execute a command if any chosen
        /// </summary>
        /// <param name="application"></param>
        internal void ShowMainMenu(Application application)
        {
            if (mBar == null)
                return ;

            for (int i = 0; i < mCommands.Count; i++)
            {
                MainMenuCommand command = mCommands[i];
                if (command.Command.IsEnabled(application, command.Parameter))
                    command.MenuItem.Enabled = true;
                else
                    command.MenuItem.Enabled = false;

                if (command.Command.IsChecked(application, command.Parameter))
                    command.MenuItem.Checked = true;
                else
                    command.MenuItem.Checked = false;
            }

            if (mWindowList != null)
            {
                mWindowList.Clear();
                for (int i = 0; i < application.TextWindows.Count; i++)
                {
                    TextWindow w = application.TextWindows[i];
                    string n = w.Text.FileName;
                    if (n.Length > 40)
                    {
                        n = "..." + n.Substring(n.Length - 37);
                    }
                    CommandMenuItem item = new CommandMenuItem("&" + w.Id + ":" + n, 100000 + i);
                    mWindowList.addItem(item);
                    if (w == application.ActiveWindow)
                        item.Checked = true;
                }
            }


            PopupMenu menu = new PopupMenu(mBar, application.ColorScheme, true);
            application.WindowManager.showPopupMenu(menu, 0, 0);
            int index = menu.CommandChosen;
            if (index >= 1 && index <= mCommands.Count)
            {
                MainMenuCommand command = mCommands[index - 1];
                command.Command.Execute(application, command.Parameter);
            }
            else if (index >= 100000 && index - 100000 < application.TextWindows.Count)
            {
                TextWindow w = application.TextWindows[index - 100000];
                if (w != application.ActiveWindow)
                    application.ActivateWindow(w);
            }
        }
    }
}
