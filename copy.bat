@echo off
if not exist .\_bin mkdir .\_bin
if not exist .\_bin\colorer mkdir .\_bin\colorer
if not exist .\_bin\hunspell mkdir .\_bin\hunspell
if not exist .\_bin\scripts mkdir .\_bin\scripts
del .\_bin\*.* /s /q
copy .\3rdparty\colorer\managed\bin\gehtsoft.xce.colorer.dll .\_bin
copy .\3rdparty\colorer\managed\bin\gehtsoft.xce.colorer.pdb .\_bin
copy .\3rdparty\scintilla\bin\gehtsoft.xce.scintilla.dll .\_bin
copy .\3rdparty\scintilla\bin\gehtsoft.xce.scintilla.pdb .\_bin
xcopy .\3rdparty\colorer\native\data\*.* .\_bin\colorer /s
copy .\3rdparty\hunspell\data\en_US.aff .\_bin\hunspell
copy .\3rdparty\hunspell\data\en_US.dic .\_bin\hunspell
copy .\3rdparty\hunspell\data\ru_RU.aff .\_bin\hunspell
copy .\3rdparty\hunspell\data\ru_RU.dic .\_bin\hunspell
copy .\3rdparty\hunspell\native\bin\Hunspellx86.dll .\_bin
copy .\3rdparty\hunspell\native\bin\Hunspellx86.pdb .\_bin
copy .\3rdparty\hunspell\managed\bin\gehtsoft.xce.spellcheck.hunspell.dll .\_bin
copy .\3rdparty\hunspell\managed\bin\gehtsoft.xce.spellcheck.hunspell.pdb .\_bin
copy .\conio\bin\gehtsoft.xce.conio.dll .\_bin
copy .\conio\bin\gehtsoft.xce.conio.pdb .\_bin
copy .\conio.win\bin\gehtsoft.xce.conio.win.dll .\_bin
copy .\conio.win\bin\gehtsoft.xce.conio.win.pdb .\_bin
copy .\configuration\bin\gehtsoft.xce.configuration.dll .\_bin
copy .\configuration\bin\gehtsoft.xce.configuration.pdb .\_bin
copy .\spellcheck\interfaces\bin\gehtsoft.xce.spellcheck.dll .\_bin
copy .\spellcheck\interfaces\bin\gehtsoft.xce.spellcheck.pdb .\_bin
copy .\text\bin\gehtsoft.xce.text.dll .\_bin
copy .\text\bin\gehtsoft.xce.text.pdb .\_bin
copy .\editor\bin\gehtsoft.xce.editor.dll .\_bin
copy .\editor\bin\gehtsoft.xce.editor.pdb .\_bin
copy .\app\bin\xce.exe .\_bin
copy .\xce.ini .\_bin
copy .\search-history.ini .\_bin
copy .\app\source\stub.cs .\_bin
copy .\extensions\backup\bin\gehtsoft.xce.extension.backup.dll .\_bin
copy .\extensions\backup\bin\gehtsoft.xce.extension.backup.pdb .\_bin
copy .\extensions\backup\bin\gehtsoft.xce.extension.autosave.dll .\_bin
copy .\extensions\backup\bin\gehtsoft.xce.extension.autosave.pdb .\_bin
copy .\extensions\advnav\bin\gehtsoft.xce.extension.advnav.dll .\_bin
copy .\extensions\advnav\bin\gehtsoft.xce.extension.advnav.pdb .\_bin
copy .\script\engine\bin\gehtsoft.xce.script.engine.dll .\_bin
copy .\script\engine\bin\gehtsoft.xce.script.engine.pdb .\_bin
copy .\script\ext\bin\gehtsoft.xce.extension.script.dll .\_bin
copy .\script\ext\bin\gehtsoft.xce.extension.script.pdb .\_bin
copy .\script\scripts\*.vbs .\_bin\scripts
copy .\extensions\charmap\bin\gehtsoft.xce.extension.charmap.dll .\_bin
copy .\extensions\charmap\bin\gehtsoft.xce.extension.charmap.pdb .\_bin
copy .\extensions\dsformat\bin\gehtsoft.xce.extension.dsformat.dll .\_bin
copy .\extensions\dsformat\bin\gehtsoft.xce.extension.dsformat.pdb .\_bin
copy .\extensions\template\bin\gehtsoft.xce.extension.template.dll .\_bin
copy .\extensions\template\bin\gehtsoft.xce.extension.template.pdb .\_bin
copy .\extensions\template\templates.ini .\_bin
copy .\extensions\intellisense\3rdparty\ICSharpCode.NRefactory.dll .\_bin
copy .\extensions\intellisense\3rdparty\ICSharpCode.SharpDevelop.Dom.dll .\_bin
copy .\extensions\intellisense\3rdparty\log4net.dll .\_bin
copy .\extensions\intellisense\3rdparty\Mono.Cecil.dll .\_bin
copy .\extensions\intellisense\3rdparty\docgen2.parser.dll .\_bin
copy .\extensions\intellisense\bin\gehtsoft.xce.extension.intellisense.dll .\_bin
copy .\extensions\intellisense\bin\gehtsoft.xce.extension.intellisense.pdb .\_bin
