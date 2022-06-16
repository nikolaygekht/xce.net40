@echo off

IF /i "%1" == "debug" (
    set config=Debug
) ELSE (
    set config=Release
)

msbuild gehtsoft.xce.spellcheck.csproj /p:Configuration="%config%"
