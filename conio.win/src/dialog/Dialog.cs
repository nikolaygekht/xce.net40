using System;
using System.Collections.Generic;
using System.Text;
using ConsoleColor = gehtsoft.xce.conio.ConsoleColor;

namespace gehtsoft.xce.conio.win
{
    public class Dialog : WindowBorderContainer
    {
        class DialogClientArea : Window
        {
            ConsoleColor mBg;
            Dialog mDlg;
            Window mPreviousFocus;

            internal DialogClientArea(Dialog dlg, ConsoleColor background)
            {
                mBg = background;
                mDlg = dlg;
            }

            public override void OnCreate()
            {
                DialogItem firstInput = null;
                foreach (DialogItem item in mDlg.Items)
                {
                    item.setDialog(mDlg);
                    Manager.create(item, this, item.Row, item.Column, item.Height, item.Width);
                    item.show(true);
                    if (item.IsInputElement && firstInput == null)
                        firstInput = item;
                }
                mPreviousFocus = Manager.getFocus();
                if (firstInput != null)
                    Manager.setFocus(firstInput);
            }

            public override void OnClose()
            {
                Manager.setFocus(mPreviousFocus);
            }

            public override void OnPaint(Canvas canvas)
            {
                canvas.fill(0, 0, Height, Width, ' ', mBg);
            }
        }

        DialogClientArea mClientArea;
        private IColorScheme mColors;
        private List<DialogItem> mItems = new List<DialogItem>();
        int mDialogResultCode = -1;
        public const int DialogResultOK = 0;
        public const int DialogResultCancel = -1;
        private int mHeight, mWidth;

        public int ResultCode
        {
            get
            {
                return mDialogResultCode;
            }
        }

        public IColorScheme Colors
        {
            get
            {
                return mColors;
            }
        }

        public Dialog(string title, IColorScheme colors, bool sizeable, int height, int width) : base(title, BoxBorder.SingleBorderBox, colors.DialogBorder, true, sizeable)
        {
            mColors = colors;
            mClientArea = new DialogClientArea(this, colors.DialogBorder);
            attachClientArea(mClientArea);
            mHeight = height;
            mWidth = width;
        }

        public void AddItem(DialogItem item)
        {
            mItems.Add(item);
            if (Exists)
            {
                item.setDialog(this);
                Manager.create(item, mClientArea, item.Row, item.Column, item.Height, item.Width);
                item.show(true);
            }
        }

        public void AddItemBefore(DialogItem item, DialogItem next)
        {
            int position;

            if (next == null)
                position = 0;
            else
            {
                for (position = 0; position < mItems.Count; position++)
                    if (mItems[position] == next)
                        break;
                if (position == mItems.Count)
                    throw new ArgumentOutOfRangeException("next");
            }
            mItems.Insert(position, item);
            if (Exists)
            {
                item.setDialog(this);
                Manager.create(item, mClientArea, item.Row, item.Column, item.Height, item.Width);
                item.show(true);
            }
        }



        public int DoModal(WindowManager manager)
        {
            int height = manager.ScreenHeight;
            int width = manager.ScreenWidth;

            int row = (height - mHeight) / 2;
            int column = (width - mWidth) / 2;

            manager.createModal(this, row, column, mHeight, mWidth);
            show(true);
            while (Exists)
                manager.pumpMessage(250);

            return mDialogResultCode;
        }

        public void EndDialog(int resultCode)
        {
            mDialogResultCode = resultCode;
            Manager.close(this);
        }

        public IEnumerable<DialogItem> Items
        {
            get
            {
                return mItems;
            }
        }

        virtual public bool PretranslateOnKeyPressed(int scanCode, char character, bool shift, bool ctrl, bool alt)
        {
            if (scanCode == ScanCode.TAB)
            {
                Window focus = Manager.getFocus();
                if (focus is DialogItem &&
                    (focus as DialogItem).Dialog == this)
                {
                    int i, curr = -1;
                    for (i = 0; i < mItems.Count; i++)
                    {
                        if (mItems[i] == focus)
                        {
                            curr = i;
                            break;
                        }
                    }

                    if (curr != -1)
                    {
                        if (!shift && !ctrl &&!alt)
                        {
                            for (i = curr + 1; i != curr; i++)
                            {
                                if (i == mItems.Count)
                                {
                                    i = -1;
                                    continue;
                                }
                                if (mItems[i].IsInputElement && mItems[i].Enabled)
                                {
                                    Manager.setFocus(mItems[i]);
                                    break;
                                }
                            }
                        }
                        else if (shift && !ctrl &&!alt)
                        {
                            for (i = curr - 1; i != curr; i--)
                            {
                                if (i == -1)
                                {
                                    i = mItems.Count;
                                    continue;
                                }

                                if (mItems[i].IsInputElement && mItems[i].Enabled)
                                {
                                    Manager.setFocus(mItems[i]);
                                    break;
                                }
                            }
                        }
                        return true;
                    }
                }
            }
            else if (scanCode == ScanCode.RETURN)
            {
                if (!shift && !ctrl && !alt)
                {
                    //find OK button
                    foreach (DialogItem item in mItems)
                        if (item.ID == Dialog.DialogResultOK)
                        {
                            item.Click();
                            return true;
                        }
                }
                return false;
            }
            else if (scanCode == ScanCode.ESCAPE)
            {
                if (!shift && !ctrl && !alt)
                {
                    //find OK button
                    foreach (DialogItem item in mItems)
                        if (item.ID == Dialog.DialogResultCancel)
                        {
                            item.Click();
                            return true;
                        }
                }
                return false;
            }

            if (!ctrl && alt && character > ' ')
            {
                for (int i = 0; i < mItems.Count; i++)
                {
                    DialogItem item = mItems[i];
                    if (item.HasHotKey && item.Enabled && char.ToUpper(item.HotKey) == char.ToUpper(character))
                    {
                        if (item.IsInputElement)
                        {
                            Manager.setFocus(item);
                            item.OnHotkeyActivated();
                        }
                        else
                        {
                            if (i != mItems.Count - 1)
                            {
                                item = mItems[i + 1];
                                if (item.Enabled && item.IsInputElement)
                                {
                                    Manager.setFocus(item);
                                    item.OnHotkeyActivated();
                                }
                            }

                        }
                    }
                }
            }
            return false;
        }

        public virtual void OnItemClick(DialogItem item)
        {
            if (item is DialogItemButton && item.ID == DialogResultOK)
            {
                if (OnOK())
                    EndDialog(DialogResultOK);
            }

            if (item is DialogItemButton && item.ID == DialogResultCancel)
            {
                if (OnCancel())
                    EndDialog(DialogResultCancel);
            }
            return ;
        }

        public virtual void OnItemActivated(DialogItem item)
        {
            return ;
        }

        public virtual void OnItemChanged(DialogItem item)
        {
            return ;
        }

        public virtual bool OnOK()
        {
            return true;
        }

        public virtual bool OnCancel()
        {
            return true;
        }

        public void CenterButtons(DialogItemButton[] buttons)
        {
            int l0 = 0;
            foreach (DialogItemButton button in buttons)
                l0 += button.Width;
            l0 += buttons.Length - 1;

            int width;
            if (Exists)
                width = Width;
            else
                width = mWidth;

            int baseColumn = (width - 2 - l0) / 2;
            if (baseColumn < 0)
                baseColumn = 0;
            foreach (DialogItemButton button in buttons)
            {
                button.repos(button.Row, baseColumn, 1, button.Width);
                baseColumn += button.Width + 1;
            }
        }

        public void CenterButtons(DialogItemButton b1)
        {
            CenterButtons(new DialogItemButton[] {b1});
        }

        public void CenterButtons(DialogItemButton b1, DialogItemButton b2)
        {
            CenterButtons(new DialogItemButton[] { b1, b2 });
        }

        public void CenterButtons(DialogItemButton b1, DialogItemButton b2, DialogItemButton b3)
        {
            CenterButtons(new DialogItemButton[] { b1, b2, b3 });
        }

        public void CenterButtons(DialogItemButton b1, DialogItemButton b2, DialogItemButton b3, DialogItemButton b4)
        {
            CenterButtons(new DialogItemButton[] { b1, b2, b3, b4 });
        }

        public int ItemsCount
        {
            get
            {
                return mItems.Count;
            }
        }

        public DialogItem GetItem(int index)
        {
            return mItems[index];
        }
    }
}
