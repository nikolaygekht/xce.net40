using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;

namespace test1
{
    class Command
    {
        private string mId;

        public override string ToString()
        {
            return mId;
        }
        
        public Command(string id)
        {
            mId = id;
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
            
            PopupMenuItem bar = new PopupMenuItem("MainBar");
            PopupMenuItem submenu, submenu1;
            submenu = bar.createPopup("Menu &1");
            submenu.createCommand("Command 1.&1", 11);
            submenu.createCommand("Command 1.&2", 12);
            (submenu[1] as CommandMenuItem).Enabled = false;
            (submenu[1] as CommandMenuItem).Checked = true;
            submenu.createSeparator();
            submenu.createCommand("Command 1.&3", "RightMod", 13);
            submenu = bar.createPopup("Menu &2");
            submenu.createCommand("Command 2.&1", "Ctrl-V", 21);
            submenu.createCommand("Command 2.&2", 22);
            submenu1 = submenu.createPopup("Submenu 2.&3");
            submenu1.createCommand("Command 2.3.&1", 231);
            submenu1.createCommand("Command 2.3.&2", 232);
            submenu.createSeparator();
            bar.createSeparator();
            bar.createCommand("C&ommand 3", 3);            
            PopupMenu menu = new PopupMenu(bar, ColorScheme.Default, true);
            mManager.showPopupMenu(menu, 1, 1);
            Console.WriteLine("{0}", menu.CommandChosen.ToString());
        }
    }
}
