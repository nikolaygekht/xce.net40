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

            FileDialog dlg = new FileDialog("test", ".", "test.txt", ColorScheme.Default, 10, 30);
            if (dlg.DoModal(mManager) == Dialog.DialogResultOK)
            {
                MessageBox.Show(mManager, ColorScheme.Default, dlg.File, "Chosen", MessageBoxButtonSet.Ok);
            }
        }
    }
}
