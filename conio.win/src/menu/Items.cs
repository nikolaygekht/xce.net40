﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace gehtsoft.xce.conio.win
{
    /// <summary>
    /// Base class for menu items
    /// </summary>
    public abstract class MenuItem
    {
        public MenuItem()
        {
        }
    }

    /// <summary>
    /// Menu item separator
    /// </summary>
    public class SeparatorMenuItem : MenuItem
    {
        public SeparatorMenuItem()
        {
        }
    }

    public abstract class TitledMenuItem : MenuItem
    {
        private string mTitle;
        private char mHotKey;
        private int mHotkeyPosition;

        internal TitledMenuItem(string title)
        {
            if (title == null)
                throw new ArgumentNullException("title");

            mHotkeyPosition = StringUtil.processHotKey(ref title);
            if (mHotkeyPosition >= 0)
                mHotKey = title[mHotkeyPosition];
            mTitle = title;
        }

        /// <summary>
        /// Command name
        /// </summary>
        public string Title
        {
            get
            {
                return mTitle;
            }
        }

        public bool HasHotkey
        {
            get
            {
                return mHotkeyPosition >= 0;
            }
        }

        public char HotKey
        {
            get
            {
                if (!HasHotkey)
                    throw new InvalidOperationException();

                return mHotKey;
            }
        }

        public int HotKeyPosition
        {
            get
            {
                return mHotkeyPosition;
            }
        }
    }

    /// <summary>
    /// Menu command
    /// </summary>
    public class CommandMenuItem : TitledMenuItem
    {
        private string mRightSide;
        private int mCommand;
        private bool mEnabled;
        private bool mChecked;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title">The name of the command</param>
        /// <param name="command">The command object</param>
        public CommandMenuItem(string title, int command) : base(title)
        {
            mRightSide = null;
            mCommand = command;
            mEnabled = true;
            mChecked = false;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title">the name of the command</param>
        /// <param name="rightSide">the command comment (for example key shortcut) located at right side</param>
        /// <param name="command">the command object</param>
        public CommandMenuItem(string title, string rightSide, int command) : base(title)
        {
            mRightSide = rightSide;
            mCommand = command;
            mEnabled = true;
            mChecked = false;
        }



        /// <summary>
        /// right-side command comment
        /// </summary>
        public string RightSide
        {
            get
            {
                return mRightSide;
            }
        }

        /// <summary>
        /// command object
        /// </summary>
        public int Command
        {
            get
            {
                return mCommand;
            }
        }

        public bool Checked
        {
            get
            {
                return mChecked;
            }
            set
            {
                mChecked = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return mEnabled;
            }
            set
            {
                mEnabled = value;
            }
        }
    }

    /// <summary>
    /// Popup-menu
    /// </summary>
    public class PopupMenuItem : TitledMenuItem, IEnumerable<MenuItem>
    {
        private List<MenuItem> mItems = new List<MenuItem>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title">the title of the menu</param>
        public PopupMenuItem(string title) : base(title)
        {
        }

        /// <summary>
        /// Number of menu items
        /// </summary>
        public int Count
        {
            get
            {
                return mItems.Count;
            }
        }

        /// <summary>
        /// Menu item by index
        /// </summary>
        /// <param name="index">The zero-based index of the menu item</param>
        public MenuItem this[int index]
        {
            get
            {
                return mItems[index];
            }
        }

        IEnumerator<MenuItem> IEnumerable<MenuItem>.GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        public void addItem(MenuItem item)
        {
            mItems.Add(item);
        }

        public void createCommand(string title, int command)
        {
            addItem(new CommandMenuItem(title, command));
        }

        public void createCommand(string title, string rightside, int command)
        {
            addItem(new CommandMenuItem(title, rightside, command));
        }

        public void createSeparator()
        {
            addItem(new SeparatorMenuItem());
        }

        public PopupMenuItem createPopup(string title)
        {
            PopupMenuItem popup = new PopupMenuItem(title);
            addItem(popup);
            return popup;
        }

        public void Clear()
        {
            mItems.Clear();
        }
    }
}
