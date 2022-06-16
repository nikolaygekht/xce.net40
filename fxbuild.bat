@echo off

cd conio
call fxbuild.bat %1
if not %errorlevel% == 0 (goto error)
cd %~dp0

cd conio.win
call fxbuild.bat %1
cd %~dp0

cd spellcheck\interfaces
call fxbuild.bat %1
cd %~dp0

cd configuration
call fxbuild.bat %1
cd %~dp0

cd 3rdparty\colorer\native
call fxbuild.bat %1
cd %~dp0

cd 3rdparty\colorer\managed
call fxbuild.bat %1
cd %~dp0

cd 3rdparty\hunspell\managed
call fxbuild.bat %1
cd %~dp0

cd 3rdparty\hunspell\native
call fxbuild.bat %1
cd %~dp0

cd 3rdparty\scintilla
call fxbuild.bat %1
cd %~dp0

cd text
call fxbuild.bat %1
cd %~dp0

cd editor
call fxbuild.bat %1
cd %~dp0

cd extensions\backup
call fxbuild.bat %1
cd %~dp0

cd extensions\autosave
call fxbuild.bat %1
cd %~dp0

cd extensions\charmap
call fxbuild.bat %1
cd %~dp0

cd extensions\advnav
call fxbuild.bat %1
cd %~dp0

cd extensions\dsformat
call fxbuild.bat %1
cd %~dp0

cd extensions\template
call fxbuild.bat %1
cd %~dp0

cd extensions\intellisense
call fxbuild.bat %1
cd %~dp0

cd script\engine
call fxbuild.bat %1
cd %~dp0

cd script\ext
call fxbuild.bat %1
cd %~dp0

cd app
call fxbuild.bat %1
cd %~dp0

goto ok
echo Build finished!

:error
echo Build failed!

:ok
echo Stop

