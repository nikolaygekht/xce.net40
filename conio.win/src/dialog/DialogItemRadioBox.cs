﻿using System;
using System.Collections.Generic;
using System.Text;

namespace gehtsoft.xce.conio.win
{
    public class DialogItemRadioBox : DialogItem
    {
        private string mTitle;
        private bool mEnabled;
        private bool mInFocus;
        private char mHotKey;
        private int mHotKeyPosition;
        private bool mChecked;
        private char mCheckMark = '\u2219';
        private bool mGroupStart;

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

        public bool Checked
        {
            get
            {
                return mChecked;
            }
            set
            {
                if (value == true)
                {
                    //uncheck all buttons in the group
                    int count = Dialog.ItemsCount;
                    int groupStart = -1;
                    int thisItem = -1;
                    int i;
                    for (i = 0; i < count; i++)
                    {
                        DialogItem item = Dialog.GetItem(i);
                        if (item is DialogItemRadioBox)
                        {
                            if (object.ReferenceEquals(this, item))
                            {
                                if (groupStart == -1 || this.GroupStart)
                                    groupStart = i;
                                thisItem = i;
                                break;
                            }
                            else if ((item as DialogItemRadioBox).GroupStart)
                            {
                                groupStart = i;
                            }
                        }
                        else
                        {
                            groupStart = -1;
                        }
                    }

                    if (groupStart >= 0 && thisItem >= 0)
                    {
                        for (i = groupStart; i < count; i++)
                        {
                            DialogItem item = Dialog.GetItem(i);
                            if (!(item is DialogItemRadioBox))
                                break;
                            if (i == thisItem)
                                continue;
                            if (i > groupStart && ((item as DialogItemRadioBox).GroupStart))
                                break;
                            (item as DialogItemRadioBox).Checked = false;
                        }
                    }
                }
                mChecked = value;
                if (Exists)
                    invalidate();
            }

        }

        public char CheckMark
        {
            get
            {
                return mCheckMark;
            }
            set
            {
                mCheckMark = value;
            }
        }

        public bool GroupStart
        {
            get
            {
                return mGroupStart;
            }
            set
            {
                mGroupStart = value;
            }
        }

        public DialogItemRadioBox(string title, int id, bool isChecked, int row, int column, int width, bool groupStart)
            : base(id, row, column)
        {
            mHotKeyPosition = StringUtil.processHotKey(ref title);
            if (mHotKeyPosition >= 0)
                mHotKey = title[mHotKeyPosition];
            mChecked = isChecked;
            mTitle = title;
            mEnabled = true;
            mGroupStart = groupStart;
        }

        public DialogItemRadioBox(string title, int id, bool isChecked, int row, int column, bool groupStart)
            : base(id, row, column)
        {
            mHotKeyPosition = StringUtil.processHotKey(ref title);
            if (mHotKeyPosition >= 0)
                mHotKey = title[mHotKeyPosition];
            mTitle = title;
            mChecked = isChecked;
            mEnabled = true;
            setDimesion(1, title.Length + 4);
            mGroupStart = groupStart;
        }

        public void Enable(bool enable)
        {
            if (mEnabled != enable)
                invalidate();
            mEnabled = enable;
        }

        public override void Click()
        {
            if (Enabled)
            {
                if (!Checked)
                {
                    Checked = true;
                    Dialog.OnItemChanged(this);
                }
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
            Manager.setCaretPos(this, 0, 1);
            Dialog.OnItemActivated(this);
            invalidate();
        }

        public override void OnKillFocus()
        {
            mInFocus = false;
            Manager.showCaret(false);
            invalidate();
        }

        public override void OnPaint(Canvas canvas)
        {
            ConsoleColor color;

            if (Enabled)
            {
                if (mInFocus)
                    color = Dialog.Colors.DialogItemCheckBoxColorFocused;
                else
                    color = Dialog.Colors.DialogItemCheckBoxColor;
            }
            else
                color = Dialog.Colors.DialogItemCheckBoxColorDisabled;

            canvas.fill(0, 0, 1, Width, color);
            canvas.write(0, 0, '(');
            if (mChecked)
                canvas.write(0, 1, mCheckMark);
            else
                canvas.write(0, 1, ' ');
            canvas.write(0, 2, ')');
            canvas.write(0, 3, ' ');
            canvas.write(0, 4, mTitle);
            if (Enabled && HasHotKey)
                canvas.write(0, 4 + mHotKeyPosition, mInFocus ? Dialog.Colors.DialogItemCheckBoxHotKeyFocused : Dialog.Colors.DialogItemCheckBoxHotKey);
        }

        public override void OnKeyPressed(int scanCode, char character, bool shift, bool ctrl, bool alt)
        {
            if (!shift && !alt && !ctrl && (scanCode == (int)ScanCode.SPACE))
                Click();
            else
                Dialog.PretranslateOnKeyPressed(scanCode, character, shift, ctrl, alt);
        }

        public override void OnMouseLButtonDown(int row, int column, bool shift, bool ctrl, bool alt)
        {
            if (Enabled)
            {
                Manager.setFocus(this);
                invalidate();
            }
        }

        public override void OnMouseLButtonUp(int row, int column, bool shift, bool ctrl, bool alt)
        {
            if (mInFocus && Enabled)
                Click();
        }

        public override void OnHotkeyActivated()
        {
            Click();
        }

    }
}
