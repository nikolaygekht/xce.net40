@echo off
IF /i "%1" == "debug" (
@set config=Debug
) ELSE (
    IF /i "%1" == "release" (
        @SET config=Release
       ) ELSE (
        @SET config=Debug
    )
)

@msbuild gehtsoft.xce.scintilla.sln /p:Configuration="%config%"
