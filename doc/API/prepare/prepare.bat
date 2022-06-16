rem @echo off
if exist dst del dst\*.* /q /s > nul
if exist src del src\*.* /q /s > nul
rem SET ARGS=..\..\..\conio\bin\gehtsoft.xce.conio.dll ..\..\..\conio.win\bin\gehtsoft.xce.conio.win.dll /out:gehtsoft.xce.xml
rem SET ARGS..\..\text\bin\Debug\gehtsoft.xce.text.dll /out:gehtsoft.xce.xml
rem SET ARGS=..\..\3rdparty\colorer\managed\bin\gehtsoft.xce.colorer.dll /out:gehtsoft.xce.xml
rem SET ARGS=..\..\3rdparty\pcre\managed\bin\gehtsoft.xce.pcre.dll /out:gehtsoft.xce.xml
rem SET ARGS=..\..\configuration\bin\gehtsoft.xce.configuration.dll /out:gehtsoft.xce.xml
rem SET ARGS=..\..\spellcheck\interfaces\bin\gehtsoft.xce.spellcheck.dll /out:gehtsoft.xce.xml
rem SET ARGS=..\..\spellcheck\interfaces\bin\gehtsoft.xce.text.dll /out:gehtsoft.xce.xml
rem SET ARGS=..\..\text\bin\gehtsoft.xce.text.dll /out:gehtsoft.xce.xml
rem SET ARGS=..\..\editor\bin\gehtsoft.xce.editor.dll /out:gehtsoft.xce.xml
rem SET ARGS=..\..\..\script\ext\bin\gehtsoft.xce.extension.script.dll /out:gehtsoft.xce.xml
SET ARGS=..\..\..\conio\bin\gehtsoft.xce.conio.dll ..\..\..\conio.win\bin\gehtsoft.xce.conio.win.dll ..\..\..\3rdparty\colorer\managed\bin\gehtsoft.xce.colorer.dll ..\..\..\text\bin\gehtsoft.xce.text.dll ..\..\..\editor\bin\gehtsoft.xce.editor.dll ..\..\..\script\ext\bin\gehtsoft.xce.extension.script.dll /out:gehtsoft.xce.xml

if "%ProgramFiles(x86)" == "" (
"%ProgramFiles%\Sandcastle\ProductionTools\MrefBuilder.exe" %ARGS%
) ELSE (
"%ProgramFiles(x86)%\Sandcastle\ProductionTools\MrefBuilder.exe" %ARGS%
)
if not exist src mkdir src
..\..\docgen2\bin\docgen prepare.xml
rem copy null.ds src
rem if not exist dst mkdir dst
rem ..\..\..\..\docgen2\bin\docgen test.xml

