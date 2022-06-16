using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.editor.command;

namespace gehtsoft.xce.editor.configuration
{
    internal class MainMenuCommand
    {
        private CommandMenuItem mMenuItem;
        
        /// <summary>
        /// The menu item
        /// </summary>
        internal CommandMenuItem MenuItem
        {
            get
            {
                return mMenuItem;
            }
        }
    
        private IEditorCommand mCommand;
        /// <summary>
        /// The editor command associated with the menu item
        /// </summary>
        internal IEditorCommand Command
        {
            get
            {
                return mCommand;
            }
        }
        
        private string mParameter;
        
        /// <summary>
        /// The parameter associated with the menu command
        /// </summary>
        internal string Parameter
        {
            get
            {
                return mParameter;
            }
        }
        
        internal MainMenuCommand(CommandMenuItem menuitem, IEditorCommand command, string parameter)
        {
            mMenuItem = menuitem;
            mCommand = command;
            mParameter = parameter; 
        }
    }
}
