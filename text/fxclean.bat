if exist bin del bin\*.* /s /q > nul
if exist bin rmdir bin /s /q > nul
if exist obj del obj\*.* /s /q > nul
if exist obj rmdir obj /s /q > nul
if exist test\test1\bin del test\test1\bin\*.* /s /q > nul
if exist test\test1\bin rmdir test\test1\bin /s /q > nul
if exist test\test1\obj del test\test1\obj\*.* /s /q > nul
if exist test\test1\obj rmdir test\test1\obj /s /q > nul
del fxbuild.err /q > nul