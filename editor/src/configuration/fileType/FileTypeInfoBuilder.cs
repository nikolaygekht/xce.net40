using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.configuration;
using gehtsoft.xce.colorer;
using gehtsoft.xce.spellcheck;

namespace gehtsoft.xce.editor.configuration
{
    internal class FileTypeInfoBuilder
    {
        internal static FileTypeInfoCollection build(Profile profile, ColorerFactory colorer, List<string> errors)
        {
            string[] separators = new string[] { ",", ";", " " };
            //find the default encoding
            string defaultEncodingName = profile["common"]["default-encoding", "windows-1252"].Trim();
            Encoding defaultEncoding;
            try
            {
                defaultEncoding = Encoding.GetEncoding(defaultEncodingName);
            }
            catch (Exception e)
            {
                defaultEncoding = Encoding.Default;
                errors.Add(string.Format("Can't find encoding {0} - {1}", defaultEncodingName, e.ToString()));
            }

            string defaultTabString = profile["common"]["default-tab-size", "4"].Trim();
            int defaultTabSize;
            if (!Int32.TryParse(defaultTabString, out defaultTabSize))
                defaultTabSize = 4;

            string t = profile["common"]["default-auto-indent", "true"].Trim();
            bool defaultAutoIndent = !(string.Compare(t, "false", true) == 0);

            FileTypeInfoCollection coll = new FileTypeInfoCollection(defaultEncoding, defaultTabSize, defaultAutoIndent);

            foreach (ProfileSection s in profile)
            {
                if (s.Name != null && string.Compare(s.Name, "filetype", true) == 0)
                {
                    string encodingName, reName;
                    Encoding encoding;
                    Regex re;

                    encodingName = s["encoding", "windows-1252"].Trim();

                    try
                    {
                        encoding = Encoding.GetEncoding(encodingName);
                    }
                    catch (Exception e)
                    {
                        encoding = defaultEncoding;
                        errors.Add(string.Format("Can't find encoding {0} - {1}", encodingName, e.ToString()));
                    }

                    reName = s["mask", ""].Trim();

                    if (reName == "")
                        re = null;
                    else
                    {
                        try
                        {
                            re = new Regex(reName);
                        }
                        catch (Exception e)
                        {
                            re = null;
                            errors.Add(string.Format("Can't compile regex {0} : {1}", reName, e.ToString()));
                        }
                    }

                    t = s["ignore-bom", "true"].Trim();
                    bool ignoreBom = !(string.Compare(t, "false", true) == 0);
                    t = s["trim-eol-space", "true"].Trim();
                    bool trimEolSpace = string.Compare(t, "true", true) == 0;
                    t = s["auto-indent", defaultAutoIndent ? "true" : "false"].Trim();
                    bool autoIndent = string.Compare(t, "true", true) == 0;
                    t = s["ignore-reload", "false"].Trim();
                    bool ignoreReload = string.Compare(t, "true", true) == 0;
                    t = s["eol-mode", "default"].Trim();
                    EOLMode eolMode = EOLMode.ForceWindows;
                    if (t == "force-mac")
                        eolMode = EOLMode.ForceMac;
                    if (t == "force-linux")
                        eolMode = EOLMode.ForceLinux;

                    string tabString = s["tab-size", "-1"].Trim();
                    int tabSize;
                    if (!Int32.TryParse(tabString, out tabSize))
                        tabSize = defaultTabSize;
                    if (tabSize < 1)
                        tabSize = defaultTabSize;

                    FileTypeInfo fi =  new FileTypeInfo(re, encoding, ignoreBom, trimEolSpace, tabSize, autoIndent, ignoreReload, eolMode);

                    string spellcheck = s["spellcheck-regions", ""].Trim();
                    string[] spellcheckregions = spellcheck.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string r1 in spellcheckregions)
                    {
                        if (r1 == "(null)")
                            fi.AddSyntaxRegion(null);
                        else
                        {
                            SyntaxRegion r2 = colorer.FindSyntaxRegion(r1);
                            if (r2 != null)
                                fi.AddSyntaxRegion(r2);
                        }
                    }
                    spellcheck = s["default-spellchecker", ""].Trim();
                    fi.SetDefaultSpellChecker(spellcheck);
                    coll.Add(fi);
                }
            }
            return coll;
        }
    }
}
