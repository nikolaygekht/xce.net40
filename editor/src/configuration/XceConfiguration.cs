using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.configuration;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.spellcheck;

namespace gehtsoft.xce.editor.configuration
{
    /// <summary>
    /// Editor configuration
    /// </summary>
    public class XceConfiguration
    {
        Profile mProfile;

        internal Profile Profile
        {
            get
            {
                return mProfile;
            }
        }

        internal XceConfiguration(string profileName)
        {
            mProfile = new Profile();
            mProfile.Load(profileName);
            mMenuName = mProfile["common", "menu", "default"].Trim();
            mKeymapName = mProfile["common", "keymap", "default"].Trim();
            mColorerPath = mProfile["common", "colorer-config", ".\\colorer\\catalog.xml"].Trim();
            mColorerColorScheme = mProfile["common", "colorer-color-scheme", "gray"].Trim();
            string s = mProfile["common", "colorer-backparse", "5000"].Trim();
            if (!Int32.TryParse(s, out mColorerBackParse))
                mColorerBackParse = 5000;
            mPersistentBlock = string.Compare("true", mProfile["common", "persistent-blocks", "true"].Trim(), true) == 0;
            mFormattingInBlock = string.Compare("true", mProfile["common", "formatting-in-blocks", "false"].Trim(), true) == 0;

        }

        private string mMenuName;
        /// <summary>
        /// The name of the application menu
        /// </summary>
        public string MenuName
        {
            get
            {
                return mMenuName;
            }
        }

        private string mKeymapName;
        /// <summary>
        /// The name of the application menu
        /// </summary>
        public string KeymapName
        {
            get
            {
                return mKeymapName;
            }
        }

        private string mColorerPath;
        /// <summary>
        /// Path to the colorer's configuration
        /// </summary>
        public string ColorerPath
        {
            get
            {
                return mColorerPath;
            }
        }

        private string mColorerColorScheme;
        /// <summary>
        /// Path to the colorer's configuration
        /// </summary>
        public string ColorerColorScheme
        {
            get
            {
                return mColorerColorScheme;
            }
        }


        private int mColorerBackParse;
        /// <summary>
        /// The number strings for the colorer's backparse
        /// </summary>
        public int ColorerBackParse
        {
            get
            {
                return mColorerBackParse;
            }
        }

        public ProfileSection this[string name]
        {
            get
            {
                return mProfile[name];
            }
        }

        private bool mPersistentBlock;
        public bool PersistentBlock
        {
            get
            {
                return mPersistentBlock;
            }
        }

        XceConfigurationSpellcheckerCollection mSpellCheckers = new XceConfigurationSpellcheckerCollection();
        public XceConfigurationSpellcheckerCollection SpellCheckers
        {
            get
            {
                return mSpellCheckers;
            }
        }

        private bool mFormattingInBlock;
        public bool FormattingInBlock
        {
            get
            {
                return mFormattingInBlock;
            }
        }

        internal void InitSpellchecker(string appPath, List<string> errors)
        {
            string assemblyPath = mProfile["common"]["spell-check-assembly", ""].Trim();
            string assemblyClass = mProfile["common"]["spell-check-class", ""].Trim();
            string dataPath = mProfile["common"]["spell-check-data", ""].Trim();

            if (assemblyPath.Length == 0 || assemblyClass.Length == 0)
                    return ;

            try
            {
                assemblyPath = Path.GetFullPath(Path.Combine(appPath, assemblyPath));
            }
            catch (Exception e)
            {
                errors.Add(string.Format("Can't resolve spellchecker path {0} : {1}", assemblyPath, e.ToString()));
                return ;
            }

            ISpellcheckerFactory factory;

            try
            {
                factory = SpellcheckerFactoryLoader.LoadFactory(assemblyPath, assemblyClass);
                if (factory == null)
                    throw new Exception("unknown");
            }
            catch (Exception e)
            {
                errors.Add(string.Format("Can't load spellchecker class {0} : {1}", assemblyClass, e.ToString()));
                return;
            }

            char[] sep = ", ".ToCharArray();

            for (int i = 0; i < 10; i++)
            {
                string dict = mProfile["common"]["spell-check-dict", i, ""].Trim();
                if (dict.Length == 0)
                    break;
                string[] dicts = dict.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                dict = dicts[0];
                string cdict;
                if (dicts.Length > 1)
                    cdict = dicts[1];
                else
                    cdict = "";
                ISpellchecker speller;
                try
                {
                    speller = factory.CreateInstance(Path.Combine(appPath, dataPath), dict, dict + "_custom");
                    if (speller == null)
                        throw new Exception("unknown");
                }
                catch (Exception e)
                {
                    errors.Add(string.Format("Can't init spellchecker class {0} : {1}", assemblyClass, e.ToString()));
                    return;

                }
                mSpellCheckers.Add(speller);
            }
        }
    }
}
