OutFile spinst.exe
SetCompressor lzma
Name "Spellset"
Caption "Spellset Setup"

InstallDir "C:\Program Files\Common Files\Microsoft Shared\Proof"

Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

Section ""
    ;files
    SetOutPath $INSTDIR
    File mssp232.dll
    File mssp2_en.lex
    File mspru32.dll
    File mssp_ru.lex

    WriteRegStr HKLM "SOFTWARE\Microsoft\Shared Tools\Proofing Tools\Spelling\1033\Normal" "" ""
    WriteRegStr HKLM "SOFTWARE\Microsoft\Shared Tools\Proofing Tools\Spelling\1033\Normal" "Engine" "$INSTDIR\mssp232.dll"
    WriteRegStr HKLM "SOFTWARE\Microsoft\Shared Tools\Proofing Tools\Spelling\1033\Normal" "Dictionary" "$INSTDIR\mssp2_en.lex"
    WriteRegStr HKLM "SOFTWARE\Microsoft\Shared Tools\Proofing Tools\Spelling\1049\Normal" "" ""
    WriteRegStr HKLM "SOFTWARE\Microsoft\Shared Tools\Proofing Tools\Spelling\1049\Normal" "Engine" "$INSTDIR\mspru32.dll"
    WriteRegStr HKLM "SOFTWARE\Microsoft\Shared Tools\Proofing Tools\Spelling\1049\Normal" "Dictionary" "$INSTDIR\mssp_ru.lex"
SectionEnd

