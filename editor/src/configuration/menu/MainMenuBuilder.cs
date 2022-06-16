using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.configuration;

//format is
//[menu.name]
//Title=Bar
//  Title=Command
//  Separator
//EndBar

namespace gehtsoft.xce.editor.configuration
{
    internal class MainMenuBuilder
    {
        static Regex mCommand = new Regex("^(\\w+)(\\((.+)\\))?$");

        internal static MainMenu build(ProfileSection section, EditorCommandCollection commands, KeyboardShortcutCollection shortcuts, List<string> errors)
        {
            MainMenu menu = new MainMenu();
            menu.StartSubmenu("mainbar");
            int bars = 0;
            foreach (ProfileKey key in section)
            {
                if (key.IsKey)
                {
                    string title = key.Name;
                    string value = key.Value.Trim();
                    if (value.Equals("bar", StringComparison.InvariantCultureIgnoreCase))
                    {
                        menu.StartSubmenu(title);
                        bars++;
                    }
                    else if (value.Equals("windowlistplaceholder", StringComparison.InvariantCultureIgnoreCase))
                    {
                        menu.AddWindowList(title);
                    }
                    else
                    {
                        Match m = mCommand.Match(value);
                        if (!m.Success)
                        {
                            errors.Add("Can't parse command " + value);
                            continue;
                        }
                        else
                        {
                            IEditorCommand command = null;
                            string parameter = null;
                            string _command = m.Groups[1].Value;
                            command = commands[_command];
                            if (command == null)
                            {
                                errors.Add("Unknown command " + key.Value);
                                continue;
                            }
                            if (m.Groups[3].Value.Length > 0)
                                parameter = m.Groups[3].Value;

                            KeyboardShortcut _shortcut = shortcuts.Find(command, parameter);
                            string shortcut;

                            if (_shortcut != null)
                                shortcut = _shortcut.Name;
                            else
                                shortcut = null;

                            menu.CreateCommand(title, shortcut, command, parameter);
                        }
                    }
                }
                else
                {
                    string q = key.Value.Trim();
                    if (q.Equals("separator", StringComparison.InvariantCultureIgnoreCase))
                    {
                        menu.CreateSeparator();
                    }
                    else if (q.Equals("endbar", StringComparison.InvariantCultureIgnoreCase) && bars > 0)
                    {
                        menu.EndSubmenu();
                        bars--;
                    }
                }
            }
            menu.EndSubmenu();
            return menu;
        }
    }
}
