using System;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;

namespace gehtsoft.xce.extension.charmap_impl
{
    internal class UnicodeRangeView : DialogItem
    {
        internal static int ControlWidth
        {
            get
            {
                return 38;
            }
        }

        private char mCharFrom;
        private char mCharTo;
        private char mCurrentChar;
        private char mTopChar;

        internal char RangeFrom
        {
            get
            {
                return mCharFrom;
            }
        }

        internal char RangeTo
        {
            get
            {
                return mCharTo;
            }
        }

        internal void GoRange(char from, char to)
        {
            if (from >= to)
                throw new ArgumentException("from >= to");
            mCharFrom = from;
            mTopChar = from;
            mCharTo = to;
            CurrentChar = mCharFrom;
            invalidate();
        }

        internal char CurrentChar
        {
            get
            {
                return mCurrentChar;
            }
            set
            {
                if (value >= mCharFrom && value <= mCharTo)
                {
                    mCurrentChar = value;
                    if (mCurrentChar < mTopChar)
                        mTopChar = (char)(mCurrentChar & 0xfff0);
                    if (mCurrentChar >= mTopChar + (Height - 3) * 16)
                        mTopChar = (char)((mCurrentChar - (Height - 4) * 16) & 0xfff0);
                    if (Dialog != null && Dialog.Exists)
                        Dialog.OnItemChanged(this);
                    if (Exists)
                        invalidate();
                }
                else
                {
                    throw new ArgumentOutOfRangeException("value");
                }
            }
        }

        private bool mEnabled;

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

        internal UnicodeRangeView(int id, int row, int column, int height) : base(id, row, column, height, ControlWidth)
        {
            mCharFrom = (char)0x20;
            mCharTo = (char)0xff;
            mEnabled = true;
        }

        private static void WriteHex(char[] buff, char value)
        {
            for (int i = 3; i >= 0; i--)
            {
                char l = (char)(value & 0xf);
                if (l >= 0 && l < 10)
                    buff[i] = (char)(l + '0');
                else
                    buff[i] = (char)(l - 10 + 'A');
                value = (char)(value >> 4);
            }
        }

        static char[] buff = new char[4];

        public override void OnPaint(Canvas canvas)
        {
            base.OnPaint(canvas);
            int i, j;
            char lineBase, currChar;
            canvas.fill(0, 0, Height, Width, ' ', mEnabled ? (mInFocus ? Dialog.Colors.DialogItemListBoxColorFocused : Dialog.Colors.DialogItemListBoxColor) : Dialog.Colors.DialogItemListBoxColorDisabled);
            canvas.box(0, 0, Height, Width, BoxBorder.SingleBorderBox, Dialog.Colors.DialogItemListBoxColor);
            canvas.write(1, 6, "0 1 2 3 4 5 6 7 8 9 A B C D E F");

            for (i = 2; i < Height - 1; i++)
            {
                lineBase = (char)(mTopChar + (i - 2) * 16);
                WriteHex(buff, lineBase);
                canvas.write(i, 1, buff, 0, 4);

                for (j = 0; j < 16; j++)
                {
                    currChar = (char)(lineBase + j);
                    if (currChar >= mCharFrom && currChar <= mCharTo)
                    {
                        char currChar1 = currChar;
                        if (currChar1.IsSpecial())
                            currChar1 = ' ';
                        canvas.write(i, 6 + j * 2, currChar1);
                        if (mEnabled)
                        {
                            if (currChar == mCurrentChar)
                            {
                                if (mInFocus)
                                    canvas.write(i, 6 + j * 2, Dialog.Colors.DialogItemListBoxSelectionFocused);
                                else
                                    canvas.write(i, 6 + j * 2, Dialog.Colors.DialogItemListBoxSelection);
                            }
                        }
                    }
                }
            }
        }

        private bool mInFocus = false;

        public override void OnKillFocus()
        {
            base.OnKillFocus();
            mInFocus = false;
            invalidate();
        }

        public override void OnSetFocus()
        {
            base.OnSetFocus();
            mInFocus = true;
            invalidate();
        }

        public override void OnMouseLButtonDown(int row, int column, bool shift, bool ctrl, bool alt)
        {
            base.OnMouseLButtonDown(row, column, shift, ctrl, alt);
            if (!mInFocus)
                WindowManager.setFocus(this);
            if (row >= 2 && row < Height - 1)
            {
                if (column >= 6 && column % 2 == 0)
                {
                    int curr = mTopChar + (row - 2) * 16 + (column - 6) / 2;
                    if (curr >= mCharFrom && curr <= mCharTo)
                        CurrentChar = (char)curr;
                }
            }
        }

        public override void OnKeyPressed(int scanCode, char character, bool shift, bool ctrl, bool alt)
        {
            if (!Dialog.PretranslateOnKeyPressed(scanCode, character, shift, ctrl, alt))
            {
                if (!shift && !ctrl && !alt)
                {
                    if (scanCode == (int)ScanCode.UP)
                    {
                        if (mCurrentChar >= (char)(mCharFrom + 16))
                            CurrentChar = (char)(mCurrentChar - 16);
                    }
                    else if (scanCode == (int)ScanCode.DOWN)
                    {
                        if (mCurrentChar <= (char)(mCharTo - 16))
                            CurrentChar = (char)(mCurrentChar + 16);
                    }
                    else if (scanCode == (int)ScanCode.LEFT)
                    {
                        if (mCurrentChar > mCharFrom)
                            CurrentChar--;
                    }
                    else if (scanCode == (int)ScanCode.RIGHT)
                    {
                        if (mCurrentChar < mCharTo)
                            CurrentChar++;
                    }
                    else if (scanCode == (int)ScanCode.HOME)
                    {
                        CurrentChar = mCharFrom;
                    }
                    else if (scanCode == (int)ScanCode.END)
                    {
                        CurrentChar = mCharTo;
                    }
                    else if (scanCode == (int)ScanCode.PRIOR)
                    {
                        int currentChar = mCurrentChar - (Height - 3) * 16;
                        if (currentChar < mCharFrom)
                            currentChar = mCharFrom;
                        CurrentChar = (char)currentChar;
                    }
                    else if (scanCode == (int)ScanCode.NEXT)
                    {
                        int currentChar = mCurrentChar + (Height - 3) * 16;
                        if (currentChar > mCharTo)
                            currentChar = mCharTo;
                        CurrentChar = (char)currentChar;
                    }
                }
            }
        }

        public override void OnMouseWheelUp(int row, int column, bool shift, bool ctrl, bool alt)
        {
            base.OnMouseWheelUp(row, column, shift, ctrl, alt);
            OnKeyPressed((int)ScanCode.UP, (char)0, false, false, false);
        }

        public override void OnMouseWheelDown(int row, int column, bool shift, bool ctrl, bool alt)
        {
            base.OnMouseWheelDown(row, column, shift, ctrl, alt);
            OnKeyPressed((int)ScanCode.DOWN, (char)0, false, false, false);
        }
    }
}

