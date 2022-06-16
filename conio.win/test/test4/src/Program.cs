using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;

namespace test1
{
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

            Dialog dlg = new Dialog("test", ColorScheme.Default, false, 10, 28);
            dlg.AddItem(new DialogItemCheckBox("check &1", 100, true, 0, 0));
            dlg.AddItem(new DialogItemCheckBox("check &2", 101, false, 1, 0));
            DialogItemCheckBox cb = new DialogItemCheckBox("check &3", 104, false, 2, 0);
            cb.Enable(false);
            dlg.AddItem(cb);
            dlg.AddItem(new DialogItemLabel("&label 1", 102, 3, 0));
            dlg.AddItem(new DialogItemSingleLineTextBox("123456", 105, 4, 0, 26));
            dlg.AddItem(new DialogItemRadioBox("&A", 200, true, 5, 0, true));
            dlg.AddItem(new DialogItemRadioBox("&B", 201, false, 5, 6, false));
            dlg.AddItem(new DialogItemRadioBox("&C", 202, false, 5, 12, false));
            dlg.AddItem(new DialogItemRadioBox("&D", 202, false, 6, 0, true));
            dlg.AddItem(new DialogItemRadioBox("&E", 203, true, 6, 6, false));
            dlg.AddItem(new DialogItemRadioBox("&F", 204, false, 6, 12, false));
            dlg.AddItem(new DialogItemButton("b&utton 1", Dialog.DialogResultOK, 7, 0));
            dlg.AddItem(new DialogItemButton("button 2", Dialog.DialogResultCancel, 7, 14));
            dlg.DoModal(mManager);

        }
    }
}
