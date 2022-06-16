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
            
            MessageBoxButton t = MessageBox.Show(mManager, ColorScheme.Default, "Really?", "Question", MessageBoxButtonSet.AbortRetryIgnore);
            MessageBox.Show(mManager, ColorScheme.Default, "You have chosen:\r\n" + t.ToString(), "Answer", MessageBoxButtonSet.Ok);
        }
    }
}
