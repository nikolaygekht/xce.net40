using System;
using System.IO;
using gehtsoft.xce.configuration;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.extension.advnav_commands;
using gehtsoft.xce.extension.advnav_condensed;
using gehtsoft.xce.extension.advnav_layout;

namespace gehtsoft.xce.extension
{
    public class advnav : IEditorExtension
    {
        public bool Initialize(Application application)
        {
            application.Commands.AddCommand(new ScreenUpCommand());
            application.Commands.AddCommand(new ScreenDownCommand());
            application.Commands.AddCommand(new ScreenRightCommand());
            application.Commands.AddCommand(new ScreenLeftCommand());
            application.Commands.AddCommand(new ScreenHomeCommand());
            application.Commands.AddCommand(new ScreenEndCommand());
            application.Commands.AddCommand(new ScreenTopCommand());
            application.Commands.AddCommand(new ScreenBottomCommand());

            application.Commands.AddCommand(new NextLineStartingFromZeroCommand());
            application.Commands.AddCommand(new PrevLineStartingFromZeroCommand());

            application.Commands.AddCommand(new FindNextWordCommand());
            application.Commands.AddCommand(new FindPreviousWordCommand());
            application.Commands.AddCommand(new FixLayoutMistypeCommand());

            application.Commands.AddCommand(new CondensedViewCommand());

            application.Commands.AddCommand(new TransformBlockCommand());

            application.Commands.AddCommand(new ICReplace());
            return true;
        }
    }

}

