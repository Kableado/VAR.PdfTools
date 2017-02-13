@echo off

:: MSBuild and tools path
if exist "%ProgramFiles%\MSBuild\14.0\bin" set PATH=%ProgramFiles%\MSBuild\14.0\bin;%PATH%
if exist "%ProgramFiles(x86)%\MSBuild\14.0\bin" set PATH=%ProgramFiles(x86)%\MSBuild\14.0\bin;%PATH%

:: NuGet
set nuget="nuget"
if exist "%~dp0..\packages\NuGet.CommandLine.3.4.3\tools\NuGet.exe" set nuget="%~dp0\..\packages\NuGet.CommandLine.3.4.3\tools\NuGet.exe"

:: Release .Net 3.5
Title Building Release .Net 3.5
msbuild VAR.PdfTools.csproj /t:Build /p:Configuration="Release .Net 3.5" /p:Platform="AnyCPU"

:: Release .Net 4.6.1
Title Building Release .Net 4.6.1
msbuild VAR.PdfTools.csproj /t:Build /p:Configuration="Release .Net 4.6.1" /p:Platform="AnyCPU"

:: Packing Nuget
Title Packing Nuget
%nuget% pack VAR.PdfTools.csproj -Verbosity detailed -OutputDir "NuGet" -MSBuildVersion "14.0" -Properties Configuration="Release .Net 4.6.1" -Prop Platform=AnyCPU

title Finished
pause
