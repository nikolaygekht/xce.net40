@echo off
if not exist .\dst md dst
if not exist .\dst\img md dst\img
if not exist .\dst\menu md dst\menu
if not exist .\dst\pageImages md dst\pageImages
if not exist .\dst\lua md dst\lua
del dst\*.* /q
del dst\img\*.* /q
del dst\lua\*.* /q
del dst\menu\*.* /q
del dst\pageImages\*.* /q
..\docgen2\bin\docgen project.xml
if %errorlevel% == 0 goto make
goto end
:make
copy img\*.png dst\img\*.* > nul
copy img\*.gif dst\img\*.* > nul
copy html\*.html dst\*.* > nul
copy src\lua\*.* dst\lua\*.* > nul
copy ..\docgen2\template\html\menu\*.* dst\menu
copy ..\docgen2\template\html\pageImages\*.* dst\pageImages
cd dst
if "%ProgramFiles(x86)%" == "" (
"%ProgramFiles%\HTML Help Workshop\hhc.exe" project.hhp
) ELSE (
"%ProgramFiles(x86)%\HTML Help Workshop\hhc.exe" project.hhp
)
cd ..
rem del dst\*.* /q
:end
