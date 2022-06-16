using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;

namespace test1
{
    class MyDialog : Dialog
    {
        private const int idLabel = 0x100;
        private const int idList = 0x101;
        private const int idAdd = 0x102;
        private const int idExit = 0x103;

        private DialogItemListBox mListBox;
        private int mLastID = 0;


        public MyDialog() : base("test", ColorScheme.Default, false, 12, 42)
        {
            AddItem(new DialogItemLabel("&List", idLabel, 0, 0));
            mListBox = new DialogItemListBox(idList, 1, 0, 6, 40);
            AddItem(mListBox);
            AddItem(new DialogItemButton("<&Add String>", idAdd, 8, 6));
            AddItem(new DialogItemButton("<&Exit>", idExit, 8, 21));
            AddItem(new DialogItemComboBox("", 0x1000, 9, 5, 30));
        }

        public override void OnItemClick(DialogItem item)
        {
            if (item.ID == idAdd)
            {
                mListBox.AddItem(string.Format("{0}", mLastID++));
                if (mListBox.CurSel < 0)
                    mListBox.CurSel = 0;
            }
            else if (item.ID == idExit)
            {
                EndDialog(Dialog.DialogResultCancel);
            }
            else
                base.OnItemClick(item);
        }
    }


    class Program
    {
        static WindowManager mManager;

        internal static WindowManager WindowManager
        {
            get
            {
                return mManager;
            }
        }

        static void Main(string[] args)
        {
            mManager = new WindowManager(false);
            mManager.FastDrawMode = false;

            MyDialog dlg = new MyDialog();
            dlg.DoModal(mManager);
        }
    }
}
