@echo off

IF /i "%1" == "debug" (
    set config=Debug
) ELSE (
    IF /i "%1" == "release" (
        set config=Release
    ) ELSE (
        set config=Release
    )
)

msbuild gehtsoft.xce.extension.advnav.csproj /p:Configuration="%config%"
