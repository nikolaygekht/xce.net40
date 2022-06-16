using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.text;

namespace gehtsoft.xce.editor.command.impl
{
    internal class MarkerDialog : Dialog
    {
        int mMarker;
                
        internal int Marker
        {
            get
            {
                return mMarker;
            }
        }
        
        internal MarkerDialog(XceFileBufferMarkers markers, bool set, IColorScheme colors) : base("Choose Marker", colors, false, 4, 47)
        {
            DialogItemButton b;
            for (int i = 0; i < 9; i++)
            {
                string t;
                if (markers[i] >= 0)
                    t = String.Format("<({0})>", i + 1);
                else
                    t = String.Format("< {0} >", i + 1);
                AddItem(b = new DialogItemButton(t, 0x1000 + i, 0, i * 5));
                if (!set && markers[i] < 0)
                    b.Enable(false);
            }
            
            AddItem(b = new DialogItemButton("< Cancel >", DialogResultCancel, 1, 7));
            CenterButtons(b);
        }

        public override void OnItemClick(DialogItem item)
        {
            if (item.ID >= 0x1000 && item.ID < 0x1009)
            {
                mMarker = item.ID - 0x1000;
                EndDialog(DialogResultOK);
            }
            else
                base.OnItemClick(item);
        }
    }


    internal class SetMarkerCommand : IEditorCommand
    {
        internal SetMarkerCommand()
        {

        }


        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "SetMarker";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            if (application.ActiveWindow == null)
                return ;
                
            int marker = 0;
            if (parameter == null || parameter.Length == 0 || !Int32.TryParse(parameter, out marker))
            {
                MarkerDialog dlg = new MarkerDialog(application.ActiveWindow.Text.Markers, true, application.ColorScheme);
                if (dlg.DoModal(application.WindowManager) != Dialog.DialogResultOK)
                    return ;
                marker = dlg.Marker;
            }
            if (marker >= 0 && marker < 9)
                application.ActiveWindow.Text.Markers[marker] = application.ActiveWindow.CursorRow;
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
            return application.ActiveWindow != null;
        }
    }

    internal class GoMarkerCommand : IEditorCommand
    {
        internal GoMarkerCommand()
        {

        }


        /// <summary>
        /// The command unique identifier
        /// </summary>
        public string Name
        {
            get
            {
                return "GoMarker";
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public void Execute(Application application, string parameter)
        {
            if (application.ActiveWindow == null)
                return;

            int marker = 0;
            if (parameter == null || parameter.Length == 0 || !Int32.TryParse(parameter, out marker))
            {
                MarkerDialog dlg = new MarkerDialog(application.ActiveWindow.Text.Markers, false, application.ColorScheme);
                if (dlg.DoModal(application.WindowManager) != Dialog.DialogResultOK)
                    return;
                marker = dlg.Marker;
            }
            if (marker >= 0 && marker < 9 && application.ActiveWindow.Text.Markers[marker] >= 0)
            {
                application.ActiveWindow.CursorRow = application.ActiveWindow.Text.Markers[marker];
                application.ActiveWindow.CursorColumn = 0;
                application.ActiveWindow.EnsureCursorVisibleInCenter();
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
            return application.ActiveWindow != null;
        }
    }

}
