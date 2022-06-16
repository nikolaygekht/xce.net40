using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor.command;

namespace gehtsoft.xce.editor.configuration
{

    /// <summary>
    /// Collection of keyboard shortcuts
    /// </summary>
    internal class KeyboardShortcutCollection : IEnumerable<KeyboardShortcut>
    {
        Dictionary<int, KeyboardShortcut> mDictionary = new Dictionary<int, KeyboardShortcut>();
        
        /// <summary>
        /// Constructor
        /// </summary>
        internal KeyboardShortcutCollection()
        {
        
        }        
        
        /// <summary>
        /// Add a new shortcut
        /// </summary>
        internal void Add(KeyboardShortcut shortcut)
        {
            mDictionary[getkey(shortcut.Control, shortcut.Alt, shortcut.Shift, shortcut.ScanCode)] = shortcut;
        }
        
        /// <summary>
        /// Calculated the key for shortcut
        /// </summary>
        /// <param name="control"></param>
        /// <param name="alt"></param>
        /// <param name="shift"></param>
        /// <param name="scancode"></param>
        /// <returns></returns>
        private int getkey(bool control, bool alt, bool shift, int scancode)
        {
            return (scancode & 0xffff) | ((control ? 1 : 0) << 17) |
                    ((alt ? 1 : 0) << 18) | ((shift ? 1 : 0) << 19);
        }
        
        /// <summary>
        /// Finds the key shortcut by keyboard key pressed
        /// </summary>
        /// <returns>null in case there is no association</returns>
        internal KeyboardShortcut Find(bool control, bool alt, bool shift, int scancode)
        {
            KeyboardShortcut shortcut;
            if (!mDictionary.TryGetValue(getkey(control, alt, shift, scancode), out shortcut))
                shortcut = null;
            return shortcut;
        }
        
        /// <summary>
        /// Finds the shortcut by the command
        /// </summary>
        internal KeyboardShortcut Find(IEditorCommand command, string parameter)
        {
            foreach (KeyboardShortcut shortcut in mDictionary.Values)
                if (EditorCommandUtil.Equals(command, parameter, shortcut.Command, shortcut.Parameter))
                    return shortcut;
            return null;
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return mDictionary.Values.GetEnumerator();
        }
        
        public IEnumerator<KeyboardShortcut> GetEnumerator()
        {
            return mDictionary.Values.GetEnumerator();
        }
    }
}
