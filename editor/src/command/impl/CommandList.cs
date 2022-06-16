using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;

namespace gehtsoft.xce.editor.command.impl
{
    internal class CommandListCommand : IEditorCommand
    {
        internal CommandListCommand()
        {
            
        }


        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "CommandList";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            foreach (IEditorCommand command in application.Commands)
            {
                if (command != this)
                {
                    application.ActiveWindow.Stroke(command.Name, 0, command.Name.Length);
                    application.ActiveWindow.Stroke(',');
                }
            }
        }

        /// <summary>
        /// Get checked status for the menu.
        /// </summary>
        public bool IsChecked(Application application, string parameter)
        {
            return false;
        }

        /// <summary>
        /// Get enabled status for the menu with parameter.
        /// </summary>
        public bool IsEnabled(Application application, string parameter)
        {
            return true;
        }
    }
}
