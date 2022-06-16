using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;

namespace gehtsoft.xce.extension.charmap_impl
{
    class ShowNonLatinCommand : IEditorCommand
    {
        public string Name
        {
            get
            {
                return "ShowNonLatin";
            }
        }

        public bool IsChecked(Application application, string param)
        {
            return false;
        }

        public bool IsEnabled(Application application, string param)
        {
            return application.ActiveWindow != null;
        }

        public void Execute(Application application, string param)
        {
            if (application.ActiveWindow != null)
            {
                if (application.ActiveWindow["show-nonlatin"] == null)
                    application.ActiveWindow["show-nonlatin"] = "y";
                else
                    application.ActiveWindow["show-nonlatin"] = null;
                application.ActiveWindow.invalidate();
            }
        }

        private static gehtsoft.xce.conio.ConsoleColor red = new gehtsoft.xce.conio.ConsoleColor(0x40);

        internal void PainterHook(TextWindow window, Canvas canvas)
        {
            if (window != null && window["show-nonlatin"] != null)
            {

                for (int row = window.TopRow, row1 = 0; row1 < window.Height && row < window.Text.LinesCount; row++, row1++)
                {
                    int s = window.Text.LineStart(row);
                    int l = window.Text.LineLength(row, true);

                    for (int column = window.TopColumn, column1 = 0; column1 < window.Width && column < l; column++, column1++)
                    {
                        if (s + column < window.Text.Length)
                        {
                            char c = window.Text[s + column];
                            if (c > '\u0001' && c < '\u0127')
                                continue;
                            canvas.write(row1, column1, red);
                        }
                    }
                }
            }
        }
    }
}
