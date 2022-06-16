using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;

namespace gehtsoft.xce.extension.csintellisense_impl
{
    class DialogItemSearchingList : DialogItem, IEnumerable<DialogItemListBoxString>
    {
        private List<DialogItemListBoxString> mItems = new List<DialogItemListBoxString>();
        private int mCurSel = -1;
        private int mOffset = 0;
        private bool mInFocus = false;
        private bool mEnabled;
        private string mHighlight = "";
        private int mHighlightBase = -1;
        private bool mCaptureFocusByMouse = true;

        public int Count
        {
            get
            {
                return mItems.Count;
            }
        }
        
        internal bool CaptureFocusByMouse
        {
            get
            {
                return mCaptureFocusByMouse;
            }
            set
            {
                mCaptureFocusByMouse = true;
            }
        }

        public DialogItemListBoxString this[int index]
        {
            get
            {
                return mItems[index];
            }
        }

        public int CurSel
        {
            get
            {
                return mCurSel;
            }
            set
            {
                mCurSel = value;
                mHighlight = "";
                mHighlightBase = -1;
                if (Exists)
                    invalidate();
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

        public void Enable(bool enable)
        {
            if (mEnabled != enable && Exists)
                invalidate();
            mEnabled = enable;
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
                if (CurSel >= mItems.Count)
                    CurSel = -1;
                else if (CurSel > index)
                    CurSel--;
                invalidate();
            }
        }

        public void RemoveAllItems()
        {
            mItems.Clear();
            CurSel = -1;
            mOffset = 0;
            invalidate();
        }


        public void EnsureVisible(int index)
        {
            if (index >= 0 && index < mItems.Count)
            {
                if (mOffset > index)
                    mOffset = index;
                if (index >= mOffset + (Height - 2))
                    mOffset = index - (Height - 3);

                if (Exists)
                    invalidate();
            }
        }

        public DialogItemSearchingList(int id, int row, int column, int height, int width)
            : base(id, row, column, height, width)
        {
            if (height < 3)
                throw new ArgumentException("List box height is too small", "height");
            mEnabled = true;
        }

        public override void OnSetFocus()
        {
            mInFocus = true;
            invalidate();
        }

        public override void OnKillFocus()
        {
            mInFocus = false;
            invalidate();
        }

        public override void OnPaint(Canvas canvas)
        {
            short color;
            if (Enabled)
                color = Dialog.Colors.DialogItemListBoxColor;
            else
                color = Dialog.Colors.DialogItemListBoxColorDisabled;
            canvas.box(0, 0, Height, Width, BoxBorder.SingleBorderBox, color, ' ');

            if (mItems.Count > Height - 2)
            {
                canvas.write(0, Width - 1, '\u25b2');
                canvas.write(Height - 1, Width - 1, '\u25bc');

                canvas.fill(1, Width - 1, Height - 2, 1, '\u2591');
                canvas.write(1 + offsetToThumb(), Width - 1, '\u2592');
            }

            for (int i = mOffset; i < mItems.Count; i++)
            {
                if (i < 0)
                    continue;
                int row = i - mOffset + 1;
                if (row >= Height - 1)
                    break;
                string text = mItems[i].Label;

                canvas.write(row, 1, text, 0, Width - 2);

                if (Enabled && i == CurSel)
                {
                    if (mInFocus)
                        color = Dialog.Colors.DialogItemListBoxSelectionFocused;
                    else
                        color = Dialog.Colors.DialogItemListBoxSelection;
                    canvas.fill(row, 1, 1, Width - 2, color);
                    
                    if (mHighlight.Length > 0 && mHighlightBase >= 0)
                        canvas.fill(row, 1 + mHighlightBase, 1, mHighlight.Length, mInFocus ? Dialog.Colors.DialogItemListBoxSelectionFocusedHighlighted : 
                                                                                              Dialog.Colors.DialogItemListBoxSelectionHighlighted);
                }
            }
        }

        private int thumbToOffset(int thumb)
        {
            if (thumb == 0)
                return 0;
            else if (thumb == Height - 3)
                return mItems.Count - 1;

            double lines = mItems.Count;
            double scrollpos = Height - 2;
            double linesPerScroll = lines / scrollpos;
            return (int)Math.Floor(thumb * linesPerScroll);
        }

        private int offsetToThumb()
        {
            double lines = mItems.Count;
            double scrollpos = Height - 2;
            double linesPerScroll = lines / scrollpos;
            double scroll = CurSel / linesPerScroll;
            return (int)Math.Floor(scroll);
        }

        public override void OnKeyPressed(int scanCode, char character, bool shift, bool ctrl, bool alt)
        {
            if (!Dialog.PretranslateOnKeyPressed(scanCode, character, shift, ctrl, alt))
            {
                if (!ctrl && !alt && !shift && scanCode == ScanCode.UP)
                {
                    if (CurSel > 0)
                    {
                        CurSel--;
                        Dialog.OnItemChanged(this);
                    }
                    EnsureVisible(CurSel);
                    invalidate();
                }
                else if (!ctrl && !alt && !shift && scanCode == ScanCode.DOWN)
                {
                    if (CurSel < mItems.Count - 1)
                    {
                        CurSel++;
                        Dialog.OnItemChanged(this);
                    }
                    EnsureVisible(CurSel);
                    invalidate();
                }
                if (!ctrl && !alt && !shift && scanCode == ScanCode.PRIOR)
                {
                    CurSel -= (Height - 2);
                    if (CurSel < 0)
                        CurSel = 0;
                    Dialog.OnItemChanged(this);
                    EnsureVisible(mCurSel);
                    invalidate();
                }
                else if (!ctrl && !alt && !shift && scanCode == ScanCode.NEXT)
                {
                    CurSel += (Height - 2);
                    if (CurSel >= mItems.Count - 1)
                        CurSel = mItems.Count - 1;
                    Dialog.OnItemChanged(this);
                    EnsureVisible(mCurSel);
                    invalidate();
                }
                else if (!ctrl && !alt && !shift && scanCode == ScanCode.HOME)
                {
                    CurSel = 0;
                    Dialog.OnItemChanged(this);
                    EnsureVisible(mCurSel);
                    invalidate();
                }
                else if (!ctrl && !alt && !shift && scanCode == ScanCode.END)
                {
                    CurSel = mItems.Count - 1;
                    Dialog.OnItemChanged(this);
                    EnsureVisible(mCurSel);
                    invalidate();
                }
                else if (ctrl && !alt && !shift && scanCode == ScanCode.C)
                {
                    if (CurSel >= 0 && CurSel < mItems.Count && mItems[mCurSel].Label != null)
                        TextClipboard.SetText(mItems[mCurSel].Label);
                }
                else if (!ctrl && !alt && (character >= ' '))
                {
                    string highlight = mHighlight + character;
                    mHighlightBase = -1;
                    for (int i = 0; i < mItems.Count; i++)
                    {
                        int idx = mItems[i].Label.IndexOf(highlight, StringComparison.InvariantCultureIgnoreCase);
                        if (idx >= 0)                        
                        {
                            mCurSel = i;
                            EnsureVisible(mCurSel);
                            mHighlight = highlight;
                            mHighlightBase = idx;
                            break;
                        }
                    }
                }
                else if (ctrl && !alt && !shift && scanCode == (int)ScanCode.UP)
                {
                    if (mCurSel > 0)
                    {
                        string highlight = mHighlight;
                        for (int i = mCurSel - 1; i >= 0; i--)
                        {
                            int idx = mItems[i].Label.IndexOf(highlight, StringComparison.InvariantCultureIgnoreCase);
                            if (idx >= 0)
                            {
                                mCurSel = i;
                                EnsureVisible(mCurSel);
                                mHighlight = highlight;
                                mHighlightBase = idx;
                                break;
                            }
                        }
                    }
                }
                else if (ctrl && !alt && !shift && scanCode == (int)ScanCode.DOWN)
                {
                    if (mCurSel < mItems.Count - 1)
                    {
                        string highlight = mHighlight;
                        for (int i = mCurSel + 1; i < mItems.Count; i++)
                        {
                            int idx = mItems[i].Label.IndexOf(highlight, StringComparison.InvariantCultureIgnoreCase);
                            if (idx >= 0)
                            {
                                mCurSel = i;
                                EnsureVisible(mCurSel);
                                mHighlight = highlight;
                                mHighlightBase = idx;
                                break;
                            }
                        }
                    }
                
                }
                else if (!ctrl && !alt && !shift && scanCode == (int)ScanCode.BACK)
                {
                    if (mHighlight.Length > 0)
                    {
                        mHighlight = mHighlight.Substring(0, mHighlight.Length  - 1);
                        if (mHighlight.Length > 0)
                        {
                            mHighlightBase = -1;
                            for (int i = 0; i < mItems.Count; i++)
                            {
                                int idx = mItems[i].Label.IndexOf(mHighlight, StringComparison.InvariantCultureIgnoreCase);
                                if (idx >= 0)
                                {
                                    string t = mHighlight;
                                    mCurSel = i;
                                    EnsureVisible(mCurSel);
                                    mHighlightBase = idx;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            CurSel = 0;
                            EnsureVisible(0);
                        }
                    }
                }
            }
        }

        public override void OnMouseLButtonDown(int row, int column, bool shift, bool ctrl, bool alt)
        {
            if (Enabled)
            {
                bool wasInFocus = false;
                if (!mInFocus)
                {
                    if (mCaptureFocusByMouse)
                    {
                        WindowManager.setFocus(this);
                        invalidate();
                    }
                }
                else
                    wasInFocus = true;

                if (row >= 1 && row < Height - 1 &&
                    column >= 1 && column < Width - 1)
                {
                    int index = mOffset + row - 1;
                    if (index >= 0 && index < mItems.Count && index != mCurSel)
                    {
                        mCurSel = index;
                        Dialog.OnItemChanged(this);
                        EnsureVisible(mCurSel);
                        invalidate();
                    }
                    else if (index >= 0 && index == mCurSel && wasInFocus)
                    {
                        if (Dialog.Exists)
                            Dialog.OnItemClick(this);
                    }
                }

                if (column == Width - 1)
                {
                    if (row == 0)
                        OnKeyPressed(ScanCode.UP, ' ', false, false, false);
                    else if (row == Height - 1 && column == Width - 1)
                        OnKeyPressed(ScanCode.DOWN, ' ', false, false, false);
                    else
                    {
                        int pos = thumbToOffset(row - 1);
                        mCurSel = pos;
                        if (mCurSel >= mItems.Count)
                            mCurSel = mItems.Count - 1;
                        Dialog.OnItemChanged(this);
                        EnsureVisible(mCurSel);
                        invalidate();
                    }
                }
            }
        }

        public override void OnMouseWheelUp(int row, int column, bool shift, bool ctrl, bool alt)
        {
            base.OnMouseWheelUp(row, column, shift, ctrl, alt);
            OnKeyPressed(ScanCode.UP, ' ', false, false, false);
        }

        public override void OnMouseWheelDown(int row, int column, bool shift, bool ctrl, bool alt)
        {
            base.OnMouseWheelUp(row, column, shift, ctrl, alt);
            OnKeyPressed(ScanCode.DOWN, ' ', false, false, false);
        }

        public override void OnSizeChanged()
        {
            base.OnSizeChanged();
            invalidate();
        }
        
        public void setHighlight(int position, string text)
        {
            mHighlightBase = position;
            mHighlight = text;
        }
        
        public void setHighlight(string text)
        {
            mHighlight = text;
            if (mHighlight.Length > 0)
            {
                mHighlight = mHighlight.Substring(0, mHighlight.Length - 1);
                if (mHighlight.Length > 0)
                {
                    mHighlightBase = -1;
                    bool f = false;
                    for (int i = 0; i < mItems.Count; i++)
                    {
                        int idx = mItems[i].Label.IndexOf(mHighlight, StringComparison.InvariantCultureIgnoreCase);
                        if (idx >= 0)
                        {
                            string t = mHighlight;
                            mCurSel = i;
                            EnsureVisible(mCurSel);
                            mHighlightBase = idx;
                            f = true;
                            break;
                        }
                    }
                    if (!f)
                    {
                        mHighlight = "";
                    }
                }
                else
                {
                    CurSel = 0;
                    EnsureVisible(0);
                }
            }
        }
        
    }
}
