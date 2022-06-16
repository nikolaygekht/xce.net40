del api\dst\*.* /q /s
rmdir api\dst /q /s
del User\dst\*.* /q /s
rmdir User\dst /q /s
if exist api\prepare\dst del api\prepare\dst\*.* /s /q > nul
if exist api\prepare\dst rmdir api\prepare\dst /s /q > nul
if exist api\prepare\src del api\prepare\src\*.* /s /q > nul
if exist api\prepare\src rmdir api\prepare\src /s /q > nul
del api\prepare\gehtsoft.indicore.xml
