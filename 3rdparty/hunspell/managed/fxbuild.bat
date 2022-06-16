@echo off

IF /i "%1" == "debug" (
    set config=Debug
) ELSE (
    set config=Release
)

msbuild gehtsoft.xce.spellcheck.hunspell.csproj /p:Configuration="%config%"
