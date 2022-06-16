using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.search;
using gehtsoft.xce.conio.win;

namespace gehtsoft.xce.editor.command.impl
{
    internal class SearchNextCommand : IEditorCommand
    {
        internal SearchNextCommand()
        {

        }

        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "SearchNext";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            SearchInfo si = application.LastSearchInfo;
            if (si == null)
                return ;
            si.SearchMode = SearchMode.Search;
            SeachController.searchStep(application, si, application.ActiveWindow);
            GC.Collect();
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
            return application.ActiveWindow != null && application.LastSearchInfo != null;
        }
    }
}
