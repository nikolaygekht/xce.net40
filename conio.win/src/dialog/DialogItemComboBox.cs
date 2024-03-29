﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace gehtsoft.xce.conio.win
{
    public class DialogItemComboBox : DialogItemSingleLineTextBox, IEnumerable<DialogItemListBoxString>
    {
        protected override int EditWidth
        {
            get
            {
                return Width - 1;
            }
        }

        List<DialogItemListBoxString> mItems = new List<DialogItemListBoxString>();

        private int mCurSel = -1;

        public int CurSel
        {
            get
            {
                return mCurSel;
            }
            set
            {
                if (mText.Length > 0)
                    mText.Remove(0, mText.Length);
                mText.Append(mItems[value].Label);
                mOffset = mCaret = 0;
                mCurSel = value;
                if (Dialog != null && Dialog.Exists)
                    Dialog.OnItemChanged(this);
                if (Exists)
                    invalidate();
            }
        }

        public DialogItemComboBox(string text, int id, int row, int column, int width) : base(text, id, row, column, width)
        {
        }

        public int Count
        {
            get
            {
                return mItems.Count;
            }
        }

        public DialogItemListBoxString this[int index]
        {
            get
            {
                return mItems[index];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator<DialogItemListBoxString> IEnumerable<DialogItemListBoxString>.GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        public int AddItem(string item)
        {
            mItems.Add(new DialogItemListBoxString(item, null));
            if (Exists)
                invalidate();
            return mItems.Count - 1;
        }

        public int AddItem(string item, object userData)
        {
            mItems.Add(new DialogItemListBoxString(item, userData));
            if (Exists)
                invalidate();
            return mItems.Count - 1;
        }

        public int AddItem(DialogItemListBoxString item)
        {
            mItems.Add(item);
            if (Exists)
                invalidate();
            return mItems.Count - 1;
        }

        public void RemoveItem(int index)
        {
            if (index >= 0 && index < mItems.Count)
            {
                mItems.RemoveAt(index);
            }
        }

        public void RemoveAllItems()
        {
            mItems.Clear();
            invalidate();
        }

        public override void OnKeyPressed(int scanCode, char character, bool shift, bool ctrl, bool alt)
        {
            if (ctrl && !shift && !alt && scanCode == (int)ScanCode.DOWN)
                ShowCombo();
            else if (!ctrl && !shift && !alt && scanCode == (int)ScanCode.DOWN)
            {
                if (mCurSel >= -1 && mCurSel < mItems.Count - 1)
                    CurSel++;

            }
            else if (!ctrl && !shift && !alt && scanCode == (int)ScanCode.UP)
            {
                if (mCurSel > 0 && mCurSel < mItems.Count)
                    CurSel--;
            }
            else
                base.OnKeyPressed(scanCode, character, shift, ctrl, alt);
        }

        public override void OnMouseLButtonDown(int row, int column, bool shift, bool ctrl, bool alt)
        {
            if (row == 0 && column == Width - 1)
                ShowCombo();
            else
                base.OnMouseLButtonDown(row, column, shift, ctrl, alt);
        }

        public override void OnPaint(Canvas canvas)
        {
            base.OnPaint(canvas);
            canvas.write(0, Width - 1, '\u2193', mInFocus ? Dialog.Colors.DialogItemEditColorFocused : Dialog.Colors.DialogItemEditColor);
        }

        private void ShowCombo()
        {
            int row, column;
            windowToScreen(0, 0, out row, out column);
            row++;
            ModalListBox list = new ModalListBox(row, column, 10, Width, Dialog.Colors);
            foreach (DialogItemListBoxString s in mItems)
                list.AddItem(s);
            if (mCurSel >= 0 && mCurSel < mItems.Count)
                list.CurSel = mCurSel;
            if (list.DoModal(Manager))
            {
                mCurSel = list.CurSel;
                SetText(mItems[list.CurSel].Label);
            }
        }

        override protected void OnTextChangedByUser()
        {
            mCurSel = -1;
        }

    }
}
