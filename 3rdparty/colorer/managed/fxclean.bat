if exist bin del bin\*.* /s /q > nul
if exist bin rmdir bin /s /q > nul
if exist obj del obj\*.* /s /q > nul
if exist obj rmdir obj /s /q > nul
if exist test\bin del test\bin\*.* /s /q > nul
if exist test\bin rmdir test\bin /s /q > nul
if exist test\obj del test\obj\*.* /s /q > nul
if exist test\obj rmdir test\obj /s /q > nul
del fxbuild.err /q > nul
