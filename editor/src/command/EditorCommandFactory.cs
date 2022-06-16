using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace gehtsoft.xce.editor.command
{
    /// <summary>
    /// The collection of the all editor commands available
    /// </summary>
    public class EditorCommandCollection : IEnumerable<IEditorCommand>
    {
        Dictionary<string, IEditorCommand> mDictionary = new Dictionary<string, IEditorCommand>();
        
        internal EditorCommandCollection()
        {
        }
        
        public void AddCommand(IEditorCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (command.Name == null)
                throw new ArgumentNullException("command.Name");
            mDictionary[command.Name] = command;
        }
        
        public IEditorCommand this[string name]
        {
            get
            {
                IEditorCommand command;
                if (mDictionary.TryGetValue(name, out command))
                    return command;
                return null;
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return mDictionary.Values.GetEnumerator();
        }
        
        public IEnumerator<IEditorCommand> GetEnumerator()
        {
            return mDictionary.Values.GetEnumerator();
        }
    }
}
