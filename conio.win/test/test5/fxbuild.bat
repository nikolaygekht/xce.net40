@echo off

IF /i "%1" == "debug" (
    set config=Debug
) ELSE (
    set config=Release
)

msbuild test2.csproj /p:Configuration="%config%"
