using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.editor.application;

namespace gehtsoft.xce.extension.charmap_impl
{
    internal class CharmapDialog : XceDialog
    {
        private readonly UnicodeRangeView mRangeView;
        private readonly DialogItemComboBox mRangesList;
        private readonly DialogItemSingleLineTextBox mCharCode;
        private readonly UnicodeRangeCollection mRanges;
        private readonly DialogItemButton mGo;
        private static int mLastRange = 0;


        internal CharmapDialog(Application application) : base(application, "Charmap", false, 18, 40)
        {
            mRanges = new UnicodeRangeCollection();
            AddItem(mRangeView = new UnicodeRangeView(0x1000, 0, 0, 13));
            AddItem(new DialogItemLabel("&Page:", 0x1001, 13, 0));
            AddItem(mRangesList = new UnicodeRangeComboBox(0x1002, 13, 5, 33));
            foreach (UnicodeRange r in mRanges)
                mRangesList.AddItem(r.Name, r);
            mRangesList.CurSel = mLastRange;
            mRangeView.GoRange(mRanges[mLastRange].From, mRanges[mLastRange].To);
            mRangesList.Readonly = true;
            AddItem(new DialogItemLabel("&Code:", 0x1003, 14, 0));
            AddItem(mCharCode = new DialogItemSingleLineTextBox("", 0x1004, 14, 5, 6));
            mCharCode.Text = ((uint)mRanges[mLastRange].From).ToString("X4");
            AddItem(mGo = new DialogItemButton("< &Go >", 0x1005, 14, 12));
            DialogItemButton ok, cancel;
            AddItem(ok = new DialogItemButton("< Ok >", DialogResultOK, 15, 0));
            AddItem(cancel = new DialogItemButton("< Cancel >", DialogResultCancel, 15, 0));
            CenterButtons(ok, cancel);
        }

        public override void OnItemChanged(DialogItem item)
        {
            base.OnItemChanged(item);
            if (item == mRangesList)
            {
                UnicodeRange r = mRanges[mRangesList.CurSel];
                mRangeView.GoRange(r.From, r.To);
            }
            else if (item == mRangeView)
            {
                mCharCode.Text = ((uint)mRangeView.CurrentChar).ToString("X4");
            }
        }

        public override void OnItemClick(DialogItem item)
        {
            base.OnItemClick(item);
            if (item == mGo)
            {
                int code;
                if (!Int32.TryParse(mCharCode.Text, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out code))
                {
                    mApplication.ShowMessage("Can't parse the code", "Error");
                    return ;
                }


                UnicodeRange r = mRanges[mRangesList.CurSel];
                char c = (char)code;
                if (c < r.From || c > r.To)
                {
                    bool found = false;
                    for (int i = 0; i < mRanges.Count && !found; i++)
                    {
                        r = mRanges[i];
                        if (c >= r.From && c <= r.To)
                        {
                            mRangesList.CurSel = i;
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        mApplication.ShowMessage("Can't find unicode page with the code specified", "Error");
                        return ;
                    }
                }
                mRangeView.CurrentChar = c.IsSpecial() ? ' ' : c;
            }
        }

        private char mSelChar;

        internal char SelChar
        {
            get
            {
                return mSelChar;
            }
        }

        public override bool OnOK()
        {
            if (WindowManager.getFocus() == mCharCode)
            {
                OnItemClick(mGo);
                return false;
            }
            else
            {
                mSelChar = mRangeView.CurrentChar;
                mLastRange = mRangesList.CurSel;
                return base.OnOK();
            }
        }
    }
}

