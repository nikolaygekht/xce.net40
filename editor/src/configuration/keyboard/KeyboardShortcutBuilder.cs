using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using gehtsoft.xce.conio;
using gehtsoft.xce.editor.command;
using System.Text.RegularExpressions;
using gehtsoft.xce.configuration;

namespace gehtsoft.xce.editor.configuration
{
    //[keymap.name]
    //ctrl-alt-shift-vk=command

    /// <summary>
    /// Builds shortcut configuration from keymap
    /// </summary>
    internal class KeyboardShortcutBuilder
    {
        static Regex mShortcut = new Regex("^(ctrl-)?(alt-)?(shift-)?(\\S+)$", RegexOptions.IgnoreCase);
        static Regex mCommand = new Regex("^(\\w+)(\\((.+)\\))?$");

        internal static KeyboardShortcutCollection build(ProfileSection keymapSection, EditorCommandCollection commands, List<string> errors)
        {
            KeyboardShortcutCollection keymap = new KeyboardShortcutCollection();
            foreach (ProfileKey key in keymapSection)
            {
                if (key.IsKey)
                {
                    bool control = false;
                    bool alt = false;
                    bool shift = false;
                    int scancode = -1;
                    IEditorCommand command = null;
                    string parameter = null;
                    Match m;

                    m = mShortcut.Match(key.Name);
                    if (!m.Success)
                        errors.Add("Can't parse shortcut " + key.Name);
                    else
                    {
                        if (m.Groups[1].Length > 0)
                            control = true;
                        if (m.Groups[2].Length > 0)
                            alt = true;
                        if (m.Groups[3].Length > 0)
                            shift = true;
                        scancode = ConsoleInput.keyNameToCode(m.Groups[4].Value.ToUpper());
                        if (scancode == -1)
                            errors.Add("Unknown scancode name in shortcut " + key.Name);
                    }
                    
                    m = mCommand.Match(key.Value.Trim());
                    if (!m.Success)
                        errors.Add("Can't parse command " + key.Value);
                    else
                    {
                        string _command = m.Groups[1].Value;
                        command = commands[_command];
                        if (command == null)
                            errors.Add("Unknown command " + key.Value);
                        if (m.Groups[3].Value.Length > 0)
                            parameter = m.Groups[3].Value;
                    }

                    if (scancode > 0 && command != null)
                        keymap.Add(new KeyboardShortcut(control, alt, shift, scancode, command, parameter));
                }
            }
            return keymap;
        }

    }
}
