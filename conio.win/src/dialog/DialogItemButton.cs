﻿using System;
using System.Collections.Generic;
using System.Text;

namespace gehtsoft.xce.conio.win
{
    /// <summary>
    /// Dialog item: button
    /// </summary>
    public class DialogItemButton : DialogItem
    {
        private string mTitle;
        private bool mEnabled;
        private bool mInFocus;
        private char mHotKey;
        private int mHotKeyPosition;
        private bool mMouseClicked;

        /// <summary>
        /// button title
        /// </summary>
        public string Title
        {
            get
            {
                return mTitle;
            }
        }

        public override bool Enabled
        {
            get
            {
                return mEnabled;
            }
        }

        public override bool IsInputElement
        {
            get
            {
                return true;
            }
        }

        public override bool HasHotKey
        {
            get
            {
                return mHotKeyPosition >= 0;
            }
        }

        public override char HotKey
        {
            get
            {
                if (!HasHotKey)
                    throw new InvalidOperationException();
                return mHotKey;
            }
        }

        object mUserData;

        public object UserData
        {
            get
            {
                return mUserData;
            }
            set
            {
                mUserData = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title">button title</param>
        /// <param name="id">button identifier</param>
        /// <param name="row">button row</param>
        /// <param name="column">column of left button edge</param>
        /// <param name="width">button width</param>
        public DialogItemButton(string title, int id, int row, int column, int width) : base(id, row, column)
        {
            mHotKeyPosition = StringUtil.processHotKey(ref title);
            if (mHotKeyPosition >= 0)
                mHotKey = title[mHotKeyPosition];
            mTitle = title;
            mEnabled = true;
            mMouseClicked = false;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title">title</param>
        /// <param name="id">identifier</param>
        /// <param name="row">button row</param>
        /// <param name="column">button column</param>
        public DialogItemButton(string title, int id, int row, int column)
            : base(id, row, column)
        {
            mHotKeyPosition = StringUtil.processHotKey(ref title);
            if (mHotKeyPosition >= 0)
                mHotKey = title[mHotKeyPosition];
            mTitle = title;
            mEnabled = true;
            setDimesion(1, title.Length);
        }

        public void Enable(bool enable)
        {
            if (mEnabled != enable)
                invalidate();
            mEnabled = enable;
        }

        public override void Click()
        {
            if (Dialog.Exists && Enabled)
            {
                Dialog.OnItemClick(this);
            }
        }

        public override void OnCreate()
        {
            mInFocus = false;
        }

        public override void OnSetFocus()
        {
            mInFocus = true;
            Manager.showCaret(true);
            Manager.setCaretPos(this, 0, Math.Min(2, mTitle.Length - 1));
            invalidate();
        }

        public override void OnKillFocus()
        {
            mInFocus = false;
            Manager.showCaret(false);
            mMouseClicked = false;
            invalidate();
        }

        public override void OnPaint(Canvas canvas)
        {
            ConsoleColor color;

            if (Enabled)
            {
                if (mInFocus)
                    color = Dialog.Colors.DialogItemButtonColorFocused;
                else
                    color = Dialog.Colors.DialogItemButtonColor;
            }
            else
                color = Dialog.Colors.DialogItemButtonColorDisabled;

            canvas.fill(0, 0, 1, Width, color);
            canvas.write(0, 0, mTitle);
            if (Enabled && HasHotKey)
                canvas.write(0, mHotKeyPosition, mInFocus ? Dialog.Colors.DialogItemButtonHotKeyFocused : Dialog.Colors.DialogItemButtonHotKey);
        }

        public override void OnKeyPressed(int scanCode, char character, bool shift, bool ctrl, bool alt)
        {
            if (!shift && !alt && !ctrl && (scanCode == (int)ScanCode.SPACE || scanCode == ScanCode.RETURN))
                Click();
            else if (!shift && !alt && !ctrl && scanCode == (int)ScanCode.LEFT)
                Dialog.PretranslateOnKeyPressed((int)ScanCode.TAB, (char)0, true, ctrl, alt);
            else if (!shift && !alt && !ctrl && scanCode == (int)ScanCode.RIGHT)
                Dialog.PretranslateOnKeyPressed((int)ScanCode.TAB, (char)0, false, ctrl, alt);
            else
                Dialog.PretranslateOnKeyPressed(scanCode, character, shift, ctrl, alt);
        }

        public override void OnMouseLButtonDown(int row, int column, bool shift, bool ctrl, bool alt)
        {
            if (Enabled)
            {
                Manager.setFocus(this);
                invalidate();
                mMouseClicked = true;
            }
        }

        public override void OnMouseLButtonUp(int row, int column, bool shift, bool ctrl, bool alt)
        {
            if (mInFocus && Enabled && mMouseClicked)
                Click();
        }

        public override void OnHotkeyActivated()
        {
            Click();
        }


    }
}
