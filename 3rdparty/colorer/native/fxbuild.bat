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

cd src\colorer
@msbuild colorer_library.sln /p:Configuration="%config%"
cd ..
cd ..

