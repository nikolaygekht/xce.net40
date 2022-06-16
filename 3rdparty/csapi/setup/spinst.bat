if "%ProgramFiles(x86)" == "" (
"%ProgramFiles%\NSIS\makensis.exe" spinst.nsi
) ELSE (
"%ProgramFiles(x86)%\NSIS\makensis.exe" spinst.nsi
)

