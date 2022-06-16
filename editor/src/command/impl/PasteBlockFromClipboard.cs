using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.editor.util;
using gehtsoft.xce.conio;

namespace gehtsoft.xce.editor.command.impl
{
    internal class PasteBlockFromClipboardCommand : IEditorCommand
    {
        internal PasteBlockFromClipboardCommand()
        {

        }

        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "PasteBlockFromClipboard";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            TextWindow w = application.ActiveWindow;
            if (w == null || !TextClipboard.IsTextAvailable(TextClipboardFormat.Text))
                return ;
            string s = TextClipboard.GetText();
            BlockContent block = new BlockContent(s, w.Text.TabSize);
            w.BeforeModify();
            BlockContentProcessor.PutBlock(w, block, w.CursorRow, w.CursorColumn);
            block = null;
            GC.Collect();
            w.EnsureCursorVisible();
            w.invalidate();
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
            return application.ActiveWindow != null && TextClipboard.IsTextAvailable(TextClipboardFormat.Text);
        }
    }
}
