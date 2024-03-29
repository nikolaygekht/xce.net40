﻿using System;
using System.Collections.Generic;
using System.Text;

namespace gehtsoft.xce.conio.win
{
    /// <summary>
    /// Popup menu
    /// </summary>
    public class PopupMenu : Window
    {
        #region properties
        /// <summary>
        /// Layout of one menu item in vertical menu layout
        /// </summary>
        class VerticalPopupMenuLayoutItem
        {
            /// <summary>
            /// Offset of the menu item from left corner of menu bar
            /// </summary>
            public int offset;
            /// <summary>
            /// Length of the menu item in columns
            /// </summary>
            public int length;

            internal VerticalPopupMenuLayoutItem(int _offset, int _length)
            {
                offset = _offset;
                length = _length;
            }
        };

        public const int PopupCommandNone = 0;
        public const int PopupCommandEscape = -1;
        internal const int PopupCommandLeft = -2;
        internal const int PopupCommandRight = -3;
        internal const int PretranslatedButtonEscape = -4;


        /// <summary>
        /// Menu content
        /// </summary>
        private PopupMenuItem mMenu;
        /// <summary>
        /// Menu colors
        /// </summary>
        private IColorScheme mColors;
        /// <summary>
        /// Parent menu
        /// </summary>
        private PopupMenu mParent;
        /// <summary>
        /// Menu Bar layout type
        /// </summary>
        private bool mVertical;
        /// <summary>
        /// Layout for vertical bar
        /// </summary>
        private List<VerticalPopupMenuLayoutItem> mLayout;
        /// <summary>
        /// Chosen command
        /// </summary>
        private int mCommand = PopupCommandNone;
        /// <summary>
        /// Currently selected item
        /// </summary>
        private int mCurSel = 0;

        /// <summary>
        /// Chosen command
        /// </summary>
        public int CommandChosen
        {
            get
            {
                return mCommand;
            }
        }
        #endregion

        #region constructor
        /// <summary>
        /// Constructor for subsequent menu
        /// </summary>
        /// <param name="parent">parent menu bar</param>
        /// <param name="menu">menu content</param>
        /// <param name="colors">menu colors</param>
        /// <param name="vertical">flag indicating whether the bar must be vertical or horizonal</param>
        internal PopupMenu(PopupMenu parent, PopupMenuItem menu, IColorScheme colors, bool vertical)
        {
            if (menu == null)
                throw new ArgumentNullException("menu");
            mMenu = menu;
            mColors = colors;
            mVertical = vertical;
            mParent = parent;
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="menu">menu content</param>
        /// <param name="defaultColor">default menu item color</param>
        /// <param name="defaultSelectedColor">default menu item selected color</param>
        /// <param name="vertical">flag indicating whether the bar must be vertical or horizonal</param>
        public PopupMenu(PopupMenuItem menu, IColorScheme colors, bool vertical)
        {
            if (menu == null)
                throw new ArgumentNullException("menu");
            mMenu = menu;
            mColors = colors;
            mVertical = vertical;
            mParent = null;
        }

        /// <summary>
        /// Calculate the size of the menu bar and layout of vertical menu bars
        /// </summary>
        /// <param name="height">[output] menu bar height in rows</param>
        /// <param name="width">[output] menu bar width in rows</param>
        internal void doLayout(out int height, out int width)
        {
            if (mVertical)
            {
                mLayout = new List<VerticalPopupMenuLayoutItem>();
                width = 1;
                foreach (MenuItem item in mMenu)
                {
                    if (item is CommandMenuItem)
                    {
                        CommandMenuItem cmd = item as CommandMenuItem;
                        mLayout.Add(new VerticalPopupMenuLayoutItem(width, cmd.Title.Length));
                        width += cmd.Title.Length + 1;
                    }
                    else if (item is PopupMenuItem)
                    {
                        PopupMenuItem cmd = item as PopupMenuItem;
                        mLayout.Add(new VerticalPopupMenuLayoutItem(width, cmd.Title.Length));
                        width += cmd.Title.Length + 1;
                    }
                    else if (item is SeparatorMenuItem)
                    {
                        mLayout.Add(new VerticalPopupMenuLayoutItem(width, 2));
                        width += 2;
                    }
                }
                height = 3;
            }
            else
            {
                int leftWidth, rightWidth;
                leftWidth = 0;
                rightWidth = 0;

                foreach (MenuItem item in mMenu)
                {
                    if (item is CommandMenuItem)
                    {
                        CommandMenuItem cmd = item as CommandMenuItem;
                        if (cmd.Title.Length > leftWidth)
                             leftWidth = cmd.Title.Length;
                        if (cmd.RightSide != null)
                            if (cmd.RightSide.Length > rightWidth)
                                rightWidth = cmd.RightSide.Length;
                    }
                    else if (item is PopupMenuItem)
                    {
                        PopupMenuItem cmd = item as PopupMenuItem;
                        if (cmd.Title.Length > leftWidth)
                            leftWidth = cmd.Title.Length;
                        if (rightWidth < 1)
                            rightWidth = 1;
                    }
                }
                width = 3 + leftWidth;
                if (rightWidth > 0)
                    width = width + rightWidth + 1;
                height = mMenu.Count + 2;
            }
        }
        #endregion

        #region activities
        public override void OnCreate()
        {
            Manager.showCaret(false);
            Manager.captureMouse(this);
        }

        public override void OnClose()
        {
            Manager.releaseMouse(this);
        }

        /// <summary>
        /// go to the previous item
        /// </summary>
        private void left()
        {
            mCurSel--;
            if (mCurSel < 0)
                mCurSel = mMenu.Count - 1;
            while (mCurSel >= 0 && mMenu[mCurSel] is SeparatorMenuItem)
                mCurSel--;
            invalidate();
        }

        /// <summary>
        /// go to the next item
        /// </summary>
        private void right()
        {
            mCurSel++;
            while (mCurSel < mMenu.Count && mMenu[mCurSel] is SeparatorMenuItem)
                mCurSel++;
            if (mCurSel >= mMenu.Count)
            {
                mCurSel = 0;
                while (mCurSel < mMenu.Count && mMenu[mCurSel] is SeparatorMenuItem)
                    mCurSel++;
            }
            invalidate();
        }

        /// <summary>
        /// choose item
        /// </summary>
        /// <param name="onlypopup">if the flag is true, only submenus are processed</param>
        /// <returns></returns>
        private bool select(bool onlypopup)
        {
            if (mCurSel >= 0 && mCurSel < mMenu.Count)
            {
                MenuItem item = mMenu[mCurSel];
                if (item is CommandMenuItem && !onlypopup)
                {
                    CommandMenuItem menuItem = (item as CommandMenuItem);
                    if (menuItem.Enabled)
                    {
                        mCommand = (item as CommandMenuItem).Command;
                        Manager.close(this);
                    }
                    return true;
                }
                else if (item is PopupMenuItem)
                {
                    PopupMenuItem subMenu = item as PopupMenuItem;
                    PopupMenu menu = new PopupMenu(this, subMenu, mColors, false);
                    int row, column;
                    if (mVertical)
                    {
                        column = mLayout[mCurSel].offset;
                        row = Row + Height;
                    }
                    else
                    {
                        column = Column + Width;
                        row = Row + mCurSel + 1;
                    }
                    Manager.releaseMouse(this);
                    Manager.showPopupMenu(menu, row, column);
                    Manager.captureMouse(this);

                    if (menu.CommandChosen == PopupCommandEscape)
                    {
                        //just do nothing
                    }
                    else if (menu.CommandChosen == PretranslatedButtonEscape)
                    {
                        OnMouseLButtonDown(mPretranslatedRow, mPretranslatedColumn, false, false, false);
                    }
                    else if (menu.CommandChosen == PopupCommandLeft)
                    {
                        if (mVertical)
                        {
                            left();
                            select(true);
                        }
                    }
                    else if (menu.CommandChosen == PopupCommandRight)
                    {
                        if (mVertical)
                        {
                            right();
                            select(true);
                        }
                    }
                    else if (menu.CommandChosen != PopupCommandNone)
                    {
                        mCommand = menu.CommandChosen;
                        Manager.close(this);
                    }
                    return true;
                }
            }
            return false;
        }

        public override void OnKeyPressed(int scanCode, char character, bool shift, bool ctrl, bool alt)
        {
            if (!shift && !ctrl && !alt)
            {
                if (scanCode == ScanCode.RIGHT)
                {
                    if (mVertical)
                        right();
                    else
                    {
                        if (!select(true) && mParent != null)
                        {
                            mCommand = PopupCommandRight;
                            Manager.close(this);
                        }
                    }
                    return;
                }
                else if (scanCode == ScanCode.LEFT)
                {
                    if (mVertical)
                        left();
                    else
                    {
                        if (mParent != null)
                        {
                            mCommand = PopupCommandLeft;
                            Manager.close(this);
                        }
                    }
                    return;
                }
                else if (scanCode == ScanCode.DOWN)
                {
                    if (mVertical)
                        select(true);
                    else
                        right();
                    return ;
                }
                else if (scanCode == ScanCode.UP)
                {
                    if (!mVertical)
                        left();
                    return;
                }
                else if (scanCode == ScanCode.HOME)
                {
                    mCurSel = 0;
                    while (mCurSel < mMenu.Count && mMenu[mCurSel] is SeparatorMenuItem)
                        mCurSel++;
                    invalidate();
                    return;
                }
                else if (scanCode == ScanCode.END)
                {
                    mCurSel = mMenu.Count - 1;
                    while (mCurSel >= 0 && mMenu[mCurSel] is SeparatorMenuItem)
                        mCurSel--;
                    invalidate();
                    return;
                }
                else if (scanCode == ScanCode.ESCAPE)
                {
                    mCommand = PopupCommandEscape;
                    Manager.close(this);
                    return;
                }
                else if (scanCode == ScanCode.RETURN)
                {
                    select(false);
                    return;
                }
            }
            if (!ctrl && character > 0)
            {
                for (int i = 0; i < mMenu.Count; i++)
                {
                    MenuItem item = mMenu[i];
                    if (item is TitledMenuItem)
                    {
                        TitledMenuItem t_item = item as TitledMenuItem;
                        if (t_item.HasHotkey && t_item.HotKey == character)
                        {
                            mCurSel = i;
                            select(false);
                            break;
                        }
                    }
                }
            }
        }

        public override void OnMouseLButtonDown(int row, int column, bool shift, bool ctrl, bool alt)
        {
            int winRow, winColumn;
            if (screenToWindow(row, column, out winRow, out winColumn))
            {
                int newSel = -1;
                if (mVertical)
                {
                    for (int i = 0; i < mLayout.Count; i++)
                    {
                        VerticalPopupMenuLayoutItem item = mLayout[i];
                        if (winColumn >= item.offset && winColumn < item.offset + item.length)
                        {
                            newSel = i;
                            break;
                        }
                    }
                }
                else
                {
                    winRow = winRow - 1;
                    if (winRow >= 0 && winRow < mMenu.Count)
                    {
                        MenuItem item = mMenu[winRow];
                        if (!(item is SeparatorMenuItem))
                            newSel = winRow;
                    }
                }

                if (newSel >= 0)
                {
                    if (newSel == mCurSel)
                        select(false);
                    else
                    {
                        mCurSel = newSel;
                        invalidate();
                    }
                }
            }

            if (mParent != null)
                if (mParent.PreTranslateLButtonDown(row, column))
                {
                    mCommand = PretranslatedButtonEscape;
                    Manager.close(this);
                }
        }

        internal int mPretranslatedRow, mPretranslatedColumn;

        internal bool PreTranslateLButtonDown(int row, int column)
        {
            bool rc;
            int t1, t2;
            if (screenToWindow(row, column, out t1, out t2))
            {
                rc = true;
            }
            else if (mParent != null)
            {
                rc = mParent.PreTranslateLButtonDown(row, column);
            }
            else
                rc = false;
            if (rc)
            {
                mPretranslatedRow = row;
                mPretranslatedColumn = column;
            }
            return rc;
        }

        public override void OnMouseRButtonDown(int row, int column, bool shift, bool ctrl, bool alt)
        {
            mCommand = PopupCommandEscape;
            Manager.close(this);
        }

        public override void OnPaint(Canvas canvas)
        {
            canvas.box(0, 0, Height, Width, BoxBorder.SingleBorderBox, mColors.MenuItem, ' ');
            if (mVertical)
            {
                for (int i = 0; i < mMenu.Count; i++)
                {
                    VerticalPopupMenuLayoutItem layout = mLayout[i];
                    MenuItem item = mMenu[i];
                    ConsoleColor color;
                    string title;
                    int highlight = -1;

                    if (item is CommandMenuItem)
                    {
                        CommandMenuItem cmd = item as CommandMenuItem;
                        title = cmd.Title;
                        if (mCurSel == i)
                        {
                            if (cmd.Enabled)
                                color = mColors.MenuItemSelected;
                            else
                                color = mColors.MenuDisabledItemSelected;
                        }
                        else
                        {
                            if (cmd.Enabled)
                                color = mColors.MenuItem;
                            else
                                color = mColors.MenuDisabledItem;
                        }

                        if (cmd.HasHotkey)
                            highlight = cmd.HotKeyPosition;
                    }
                    else if (item is PopupMenuItem)
                    {
                        PopupMenuItem cmd = item as PopupMenuItem;
                        title = cmd.Title;
                        if (mCurSel == i)
                            color = mColors.MenuItemSelected;
                        else
                            color = mColors.MenuItem;

                        if (cmd.HasHotkey)
                            highlight = cmd.HotKeyPosition;
                    }
                    else if (item is SeparatorMenuItem)
                    {
                        color = mColors.MenuItem;
                        title = "\u2502";
                    }
                    else
                    {
                        color = mColors.MenuItem;
                        title = "";
                    }
                    canvas.write(1, layout.offset, title, color);
                    if (highlight >= 0)
                    {
                        if (mCurSel == i)
                            canvas.write(1, layout.offset + highlight, mColors.MenuHotKeySelected);
                        else
                            canvas.write(1, layout.offset + highlight, mColors.MenuHotKey);
                    }
                }
            }
            else
            {
                for (int i = 0; i < mMenu.Count; i++)
                {
                    MenuItem item = mMenu[i];

                    ConsoleColor color;

                    if (item is CommandMenuItem)
                    {
                        CommandMenuItem cmd = item as CommandMenuItem;
                        if (mCurSel == i)
                        {
                            if (cmd.Enabled)
                                color = mColors.MenuItemSelected;
                            else
                                color = mColors.MenuDisabledItemSelected;
                        }
                        else
                        {
                            if (cmd.Enabled)
                                color = mColors.MenuItem;
                            else
                                color = mColors.MenuDisabledItem;
                        }
                        canvas.fill(i + 1, 1, 1, Width - 2, color);
                        canvas.write(i + 1, 2, cmd.Title);
                        if (cmd.HasHotkey)
                        {
                            if (mCurSel == i)
                                canvas.write(i + 1, 2 + cmd.HotKeyPosition, mColors.MenuHotKeySelected);
                            else
                                canvas.write(i + 1, 2 + cmd.HotKeyPosition, mColors.MenuHotKey);
                        }

                        if (cmd.RightSide != null)
                            canvas.write(i + 1, Width - (cmd.RightSide.Length + 1), cmd.RightSide);
                        if (cmd.Checked)
                            canvas.write(i + 1, 1, '\u221a');
                    }
                    else if (item is PopupMenuItem)
                    {
                        PopupMenuItem cmd = item as PopupMenuItem;
                        string title = cmd.Title;
                        if (mCurSel == i)
                            color = mColors.MenuItemSelected;
                        else
                            color = mColors.MenuItem;
                        canvas.fill(i + 1, 1, 1, Width - 2, color);
                        canvas.write(i + 1, 2, cmd.Title);
                        canvas.write(i + 1, Width - 2, '\u25ba');
                        if (cmd.HasHotkey)
                        {
                            if (mCurSel == i)
                                canvas.write(i + 1, 2 + cmd.HotKeyPosition, mColors.MenuHotKeySelected);
                            else
                                canvas.write(i + 1, 2 + cmd.HotKeyPosition, mColors.MenuHotKey);
                        }
                    }
                    else if (item is SeparatorMenuItem)
                    {
                        canvas.fill(i + 1, 1, 1, Width - 2, '\u2500');
                        canvas.write(i + 1, 0, '\u251c');
                        canvas.write(i + 1, Width - 1, '\u2524');
                    }
                }
            }
        }
        #endregion
    }
}
