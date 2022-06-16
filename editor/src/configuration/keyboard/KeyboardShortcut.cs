using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;
using gehtsoft.xce.editor.command;

namespace gehtsoft.xce.editor.configuration
{
    /// <summary>
    /// The keyboard shortcut 
    /// </summary>
    internal class KeyboardShortcut
    {
        private bool mControl;
        
        /// <summary>
        /// The state for the control key
        /// </summary>
        public bool Control
        {
            get
            {
                return mControl;
            }
        }

        private bool mShift;

        /// <summary>
        /// The state for the shift key
        /// </summary>
        public bool Shift
        {
            get
            {
                return mShift;
            }
        }

        private bool mAlt;

        /// <summary>
        /// The state for the alt key
        /// </summary>
        public bool Alt
        {
            get
            {
                return mAlt;
            }
        }
        
        private int mScanCode;
        
        /// <summary>
        /// The shortcut code
        /// </summary>
        public int ScanCode
        {
            get
            {
                return mScanCode;
            }
        }
        
        private IEditorCommand mCommand;
        
        /// <summary>
        /// The command associated
        /// </summary>
        public IEditorCommand Command
        {
            get
            {
                return mCommand;
            }
        }
        
        private string mParameter;
        
        /// <summary>
        /// The command parameter 
        /// </summary>
        public string Parameter
        {
            get
            {
                return mParameter;
            }
        }
        
        private string mName = null;
        
        public string Name
        {
            get
            {
                if (mName == null)
                {
                    string name = "";
                    if (mControl)
                        name += "ctrl-";
                    if (mAlt)
                        name += "alt-";
                    if (mShift)
                        name += "shift-";
                    name += ConsoleInput.keyCodeToName(mScanCode);   
                    mName = name;
                }
                return mName;
                
            }
        }
        
        
        internal KeyboardShortcut(bool control, bool alt, bool shift, int scancode, IEditorCommand command, string parameter)
        {
            mControl = control;
            mAlt = alt;
            mShift = shift;
            mScanCode = scancode;
            mCommand = command;
            mParameter = parameter;
        }


    }
}
