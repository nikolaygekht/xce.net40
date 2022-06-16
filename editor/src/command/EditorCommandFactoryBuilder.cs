using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor.command.impl;

namespace gehtsoft.xce.editor.command
{
    internal class EditorCommandFactoryBuilder
    {
        internal static EditorCommandCollection build()
        {
            EditorCommandCollection commands = new EditorCommandCollection();
            commands.AddCommand(new AssertCommand());
            commands.AddCommand(new ExitCommand());
            commands.AddCommand(new MenuCommand());
            commands.AddCommand(new AssertCommand());
            commands.AddCommand(new NextWindowCommand());
            commands.AddCommand(new CursorCommand());
            commands.AddCommand(new MarkBlockCommand());
            commands.AddCommand(new InsertModeCommand());
            commands.AddCommand(new UndoCommand());
            commands.AddCommand(new RedoCommand());
            commands.AddCommand(new TabCommand());
            commands.AddCommand(new DeleteCommand());
            commands.AddCommand(new DeleteLineCommand());
            commands.AddCommand(new BackspaceCommand());
            commands.AddCommand(new BackspaceWordCommand());
            commands.AddCommand(new DeleteToEndOfLineCommand());
            commands.AddCommand(new DeleteToEndOfWordCommand());
            commands.AddCommand(new EnterCommand());
            commands.AddCommand(new ChooseSpellLanguageCommand());
            commands.AddCommand(new ChooseSpellSuggestCommand());
            commands.AddCommand(new ChangeCodePageCommand());
            commands.AddCommand(new OpenFileCommand());
            commands.AddCommand(new SaveFileCommand());
            commands.AddCommand(new SaveFileAsCommand());
            commands.AddCommand(new CloseFileCommand());
            commands.AddCommand(new DeleteBlockCommand());
            commands.AddCommand(new CopyBlockToClipboardCommand());
            commands.AddCommand(new CutBlockToClipboardCommand());
            commands.AddCommand(new PasteBlockFromClipboardCommand());
            commands.AddCommand(new CopyBlockCommand());
            commands.AddCommand(new MoveBlockCommand());
            commands.AddCommand(new InterwindowCopyBlockCommand());
            commands.AddCommand(new SwitchWindowCommand());
            commands.AddCommand(new CheckPairCommand());
            commands.AddCommand(new SearchCommand());
            commands.AddCommand(new SearchNextCommand());
            commands.AddCommand(new ReplaceCommand());
            commands.AddCommand(new GoToCommand());
            commands.AddCommand(new SetMarkerCommand());
            commands.AddCommand(new GoMarkerCommand());
            commands.AddCommand(new DuplicateLineCommand());
            commands.AddCommand(new CommandListCommand());
            commands.AddCommand(new ExecuteCommand());
            commands.AddCommand(new ReloadCommand());
            return commands;
        }
    }
}
