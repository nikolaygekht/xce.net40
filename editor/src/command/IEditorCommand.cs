using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor.application;

namespace gehtsoft.xce.editor.command
{
    /// <summary>
    /// The interface to the editor command
    /// </summary>
    public interface IEditorCommand
    {
        /// <summary>
        /// The command unique identifier
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        void Execute(Application application, string parameter);

        /// <summary>
        /// Get checked status for the menu.
        /// </summary>
        bool IsChecked(Application application, string parameter);

        /// <summary>
        /// Get enabled status for the menu with parameter.
        /// </summary>
        bool IsEnabled(Application application, string parameter);
    }

    /// <summary>
    /// The interface to the dialog command
    /// </summary>
    public interface IDialogCommand
    {
        /// <summary>
        /// Execute the command
        /// </summary>
        void DialogExecute(Application application, XceDialog dialog, string parameter);
    }

    internal static class EditorCommandUtil
    {
        public static bool Equals(IEditorCommand command, string parameter, IEditorCommand otherCommand, string otherParameter)
        {
            if (command == null && otherCommand == null)
            {
                if (parameter == null && otherParameter == null)
                    return true;
                if (parameter != null && otherParameter != null && parameter == otherParameter)
                    return true;
                return false;
            }
            if (command != null && otherCommand != null && command.Name == otherCommand.Name)
            {
                if (parameter == null && otherParameter == null)
                    return true;
                if (parameter != null && otherParameter != null && parameter == otherParameter)
                    return true;
                return false;
            }
            return false;
        }
    }


}
