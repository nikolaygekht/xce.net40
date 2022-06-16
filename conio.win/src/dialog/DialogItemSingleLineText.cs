using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;

namespace gehtsoft.xce.conio.win
{
    public class DialogItemSingleLineTextBox : DialogItem
    {
        protected StringBuilder mText;
        protected int mOffset;
        protected int mCaret;
        protected int mSelectionStart;
        protected int mSelectionEnd;
        protected int mSelectionFirstPosition;
        protected bool mEnabled;
        protected bool mInFocus;
        protected bool mInsertMode;
        protected bool mReadonly;

        public string Text
        {
            get
            {
                return mText.ToString();
            }
            set
            {
                SetText(value);
            }
        }

        public override bool Enabled
        {
            get
            {
                return mEnabled;
            }
        }

        public bool Readonly
        {
            get
            {
                return mReadonly;
            }
            set
            {
                mReadonly = value;
            }
        }

        public override bool IsInputElement
        {
            get
            {
                return true;
            }
        }

        protected virtual int EditWidth
        {
            get
            {
                return Width;
            }
        }


        public DialogItemSingleLineTextBox(string text, int id, int row, int column, int width) : base(id, row, column, 1, width)
        {
            mText = new StringBuilder(text);
            mOffset = 0;
            mCaret = 0;
            mSelectionStart = -1;
            mSelectionEnd = -1;
            mEnabled = true;
            mInsertMode = true;
            mReadonly = false;
        }

        public void Enable(bool enable)
        {
            if (mEnabled != enable && Exists)
                invalidate();
            mEnabled = enable;
        }

        public void SetText(string text)
        {
            mOffset = 0;
            mCaret = 0;
            mSelectionEnd = mSelectionStart = -1;
            mText = new StringBuilder(text);
            if (mInFocus)
                Manager.setCaretPos(this, 0, mCaret - mOffset);
            if (Exists)
                invalidate();
            if (Dialog != null)
                Dialog.OnItemChanged(this);
        }

        public void SetSel(int from, int to)
        {
            mSelectionStart = from;
            mSelectionEnd = to;
            mCaret = to;
            if (EditWidth > 0)
            {
                if (mCaret - mOffset >= EditWidth)
                    mOffset = mCaret - EditWidth + 1;
            }
        }


        public override void OnCreate()
        {
            mInFocus = false;
        }

        public override void OnSetFocus()
        {
            mInFocus = true;
            Manager.setCaretType(mInsertMode ? 12 : 50, true);
            Manager.setCaretPos(this, 0, mCaret - mOffset);
            invalidate();
        }

        public override void OnKillFocus()
        {
            mInFocus = false;
            Manager.setCaretType(12, false);
            invalidate();
        }

        public override void OnPaint(Canvas canvas)
        {
            ConsoleColor color, selColor;

            if (Enabled)
            {
                if (mInFocus)
                {
                    color = Dialog.Colors.DialogItemEditColorFocused;
                    selColor = Dialog.Colors.DialogItemEditSelectionFocused;
                }
                else
                {
                    color = Dialog.Colors.DialogItemEditColor;
                    selColor = Dialog.Colors.DialogItemEditSelection;
                }
            }
            else
            {
                color = Dialog.Colors.DialogItemEditColorDisabled;
                selColor = color;
            }

            canvas.fill(0, 0, 1, EditWidth, ' ', color);
            if (mOffset < mText.Length)
                canvas.write(0, 0, mText, mOffset, mText.Length - mOffset, color);
            if (mSelectionStart >= 0 && Enabled)
                canvas.fill(0, mSelectionStart - mOffset, 1, mSelectionEnd - mSelectionStart, selColor);
        }

        public override void OnKeyPressed(int scanCode, char character, bool shift, bool ctrl, bool alt)
        {
            if (!Dialog.PretranslateOnKeyPressed(scanCode, character, shift, ctrl, alt))
            {
                if (!ctrl && !alt && scanCode == ScanCode.LEFT)
                    left(shift);
                if (!ctrl && !alt && scanCode == ScanCode.RIGHT)
                    right(shift);
                if (!ctrl && !alt && scanCode == ScanCode.HOME)
                    home(shift);
                if (!ctrl && !alt && scanCode == ScanCode.END)
                    end(shift);
                if (!ctrl && !alt && !shift && scanCode == ScanCode.DEL)
                    delete();
                if (ctrl && !alt && !shift && scanCode == ScanCode.DEL)
                    deleteToEnd();
                if (!ctrl && !alt && !shift && scanCode == ScanCode.BACK)
                    backspace();
                if (ctrl && !alt && !shift && scanCode == ScanCode.BACK)
                    backspaceToBegin();
                if (!ctrl && !alt && !shift && scanCode == ScanCode.INSERT)
                    toggleInsert();
                if (ctrl && !alt && !shift && scanCode == ScanCode.X)
                    cut();
                if (ctrl && !alt && !shift && scanCode == ScanCode.C)
                    copy();
                if (ctrl && !alt && !shift && scanCode == ScanCode.V)
                    paste();
                if (ctrl && !alt && !shift && scanCode == ScanCode.INSERT)
                    copy();
                if (!ctrl && !alt && shift && scanCode == ScanCode.INSERT)
                    paste();

                if (!ctrl && !alt && character >= ' ')
                    type(character);
            }
        }



        private void left(bool select)
        {
            if (select && mSelectionStart == -1)
                mSelectionFirstPosition = mCaret;
            if (mCaret > 0)
            {
                mCaret--;
                if (mCaret < mOffset)
                    mOffset = mCaret;
                Manager.setCaretPos(this, 0, mCaret - mOffset);
                invalidate();

                if (select)
                    updateSelect();
                else
                    mSelectionStart = mSelectionEnd = -1;
            }
        }

        private void right(bool select)
        {
            if (select && mSelectionStart == -1)
                mSelectionFirstPosition = mCaret;

            mCaret++;
            if (mCaret - mOffset >= EditWidth)
                mOffset = mCaret - EditWidth + 1;
            Manager.setCaretPos(this, 0, mCaret - mOffset);
            invalidate();

            if (select)
                updateSelect();
            else
                mSelectionStart = mSelectionEnd = -1;
        }

        private void updateSelect()
        {
            if (mCaret < mSelectionFirstPosition)
            {
                mSelectionStart = mCaret;
                mSelectionEnd = mSelectionFirstPosition;
            }
            else
            {
                mSelectionStart = mSelectionFirstPosition;
                mSelectionEnd = mCaret;
            }
            if (mSelectionStart == mSelectionEnd)
            {
                mSelectionStart = mSelectionEnd = -1;
            }
        }

        private void home(bool select)
        {
            if (select && mSelectionStart == -1)
                mSelectionFirstPosition = mCaret;

            mCaret = 0;
            if (mCaret < mOffset)
                mOffset = 0;

            Manager.setCaretPos(this, 0, mCaret - mOffset);
            invalidate();

            if (select)
                updateSelect();
            else
                mSelectionStart = mSelectionEnd = -1;
        }

        private void end(bool select)
        {
            if (select && mSelectionStart == -1)
                mSelectionFirstPosition = mCaret;

            mCaret = mText.Length;
            if (mCaret - mOffset >= EditWidth)
                mOffset = mCaret - EditWidth + 1;
            Manager.setCaretPos(this, 0, mCaret - mOffset);
            invalidate();

            if (select)
                updateSelect();
            else
                mSelectionStart = mSelectionEnd = -1;
        }

        private void delete()
        {
            if (Readonly)
                return ;
            if (mSelectionStart >= 0)
            {
                bool c = false;
                if (mSelectionEnd >= mText.Length)
                    mSelectionEnd = mText.Length;
                if (mSelectionStart < mSelectionEnd)
                {
                    mText.Remove(mSelectionStart, mSelectionEnd - mSelectionStart);
                    c = true;
                }
                invalidate();
                mCaret = mSelectionStart;
                if (mCaret < mOffset)
                    mOffset = mCaret;
                if (mCaret - mOffset >= EditWidth)
                    mOffset = mCaret - EditWidth + 1;
                Manager.setCaretPos(this, 0, mCaret - mOffset);
                mSelectionStart = mSelectionEnd = -1;
                OnTextChangedByUser();
                if (c && Dialog != null)
                    Dialog.OnItemChanged(this);

            }
            else
            {
                if (mCaret < mText.Length)
                {
                    mText.Remove(mCaret, 1);
                    if (Dialog != null)
                        Dialog.OnItemChanged(this);
                    OnTextChangedByUser();
                }
                invalidate();
            }
            Manager.setCaretPos(this, 0, mCaret - mOffset);
        }

        private void deleteToEnd()
        {
            if (Readonly)
                return;

            if (mCaret < mText.Length)
            {
                mText.Remove(mCaret, mText.Length - mCaret);
                if (Dialog != null)
                    Dialog.OnItemChanged(this);
                OnTextChangedByUser();
            }
            invalidate();
        }

        private void backspace()
        {
            if (Readonly)
                return;

            if (mSelectionStart >= 0)
            {
                if (mSelectionEnd >= mText.Length)
                    mSelectionEnd = mText.Length;
                mText.Remove(mSelectionStart, mSelectionEnd - mSelectionStart);
                invalidate();
                mCaret = mSelectionStart;
                if (mCaret < mOffset)
                    mOffset = mCaret;
                if (mCaret - mOffset >= EditWidth)
                    mOffset = mCaret - EditWidth + 1;
                Manager.setCaretPos(this, 0, mCaret - mOffset);
                mSelectionStart = mSelectionEnd = -1;
                if (Dialog != null)
                    Dialog.OnItemChanged(this);

                OnTextChangedByUser();
            }
            else
            {
                if (mCaret > 0)
                {
                    bool c = false;
                    if (mText.Length > mCaret - 1)
                    {
                        mText.Remove(mCaret - 1, 1);
                        c = true;
                    }
                    mCaret--;
                    if (mCaret < mOffset)
                        mOffset = mCaret;
                    invalidate();
                    if (c && Dialog != null)
                        Dialog.OnItemChanged(this);
                    OnTextChangedByUser();
                }
            }
            Manager.setCaretPos(this, 0, mCaret - mOffset);
        }

        private void backspaceToBegin()
        {
            if (Readonly)
                return;

            bool c = false;
            if (mSelectionStart >= 0)
            {
                if (mSelectionEnd >= mText.Length)
                    mSelectionEnd = mText.Length;
                mText.Remove(mSelectionStart, mSelectionEnd - mSelectionStart);
                c = true;
                invalidate();
                mCaret = mSelectionStart;
                if (mCaret < mOffset)
                    mOffset = mCaret;
                if (mCaret - mOffset >= EditWidth)
                    mOffset = mCaret - EditWidth + 1;
                Manager.setCaretPos(this, 0, mCaret - mOffset);
                mSelectionStart = mSelectionEnd = -1;
            }
            if (mCaret > 0)
            {
                mText.Remove(0, mCaret);
                c = true;
                mCaret = 0;
                if (mCaret < mOffset)
                    mOffset = mCaret;
                invalidate();
                if (Dialog != null)
                    Dialog.OnItemChanged(this);

            }
            if (c && Dialog != null)
                Dialog.OnItemChanged(this);
            OnTextChangedByUser();
            Manager.setCaretPos(this, 0, mCaret - mOffset);
        }

        private void toggleInsert()
        {
            mInsertMode = !mInsertMode;
            Manager.setCaretType(mInsertMode ? 12 : 50, true);
        }

        private void type(char c)
        {
            if (Readonly)
                return;

            bool ch = false;
            if (mSelectionStart >= 0)
            {
                ch = true;
                delete();
            }

            if (mCaret >= mText.Length)
            {
                int cc = mCaret - mText.Length + (mInsertMode ? 0 : 1);
                mText.Append(' ', cc);
                ch = true;
            }

            if (mInsertMode)
                mText.Insert(mCaret, c);
            else
                mText[mCaret] = c;
            ch = true;
            mCaret++;
            if (mCaret - mOffset >= EditWidth)
                mOffset = mCaret - EditWidth + 1;
            Manager.setCaretPos(this, 0, mCaret - mOffset);
            invalidate();
            if (ch && Dialog != null)
                Dialog.OnItemChanged(this);
            OnTextChangedByUser();
        }

        private void cut()
        {
            if (Readonly)
                return;

            if (mSelectionStart >= 0)
            {
                copy();
                delete();
            }
        }

        private void copy()
        {
            if (mSelectionStart >= 0)
            {
                if (mSelectionEnd >= mText.Length)
                    mSelectionEnd = mText.Length;
                string selectionText = mText.ToString().Substring(mSelectionStart, mSelectionEnd - mSelectionStart);
                TextClipboard.SetText(selectionText);
            }
        }

        private void paste()
        {
            if (Readonly)
                return;

            string clipText = TextClipboard.GetText(TextClipboardFormat.UnicodeText);
            if (clipText != null)
            {
                if (mSelectionStart >= 0)
                {
                    delete();
                }
                if (mCaret >= mText.Length)
                {
                    int cc = mCaret - mText.Length;
                    mText.Append(' ', cc);
                }
                mText.Insert(mCaret, clipText);
                mCaret += clipText.Length;
                if (mCaret - mOffset >= EditWidth)
                    mOffset = mCaret - EditWidth + 1;
                Manager.setCaretPos(this, 0, mCaret - mOffset);
                invalidate();
                if (Dialog != null)
                    Dialog.OnItemChanged(this);
                OnTextChangedByUser();
            }
        }

        public override void OnMouseLButtonDown(int row, int column, bool shift, bool ctrl, bool alt)
        {
            if (Enabled)
            {
                mSelectionStart = mSelectionEnd = -1;
                mCaret = mOffset + column;
                Manager.setCaretPos(this, 0, mCaret - mOffset);
                if (!mInFocus)
                    Manager.setFocus(this);
                mSelectionFirstPosition = mCaret;
                invalidate();
            }
        }

        public override void OnMouseMove(int row, int column, bool shift, bool ctrl, bool alt, bool leftButton, bool rightButton)
        {
            if (leftButton && mInFocus && Enabled)
            {
                mCaret = mOffset + column;
                Manager.setCaretPos(this, 0, mCaret - mOffset);
                updateSelect();
                invalidate();

            }
        }

        protected virtual void OnTextChangedByUser()
        {
        }
    }
}
