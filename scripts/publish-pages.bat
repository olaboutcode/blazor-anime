@echo off
setlocal EnableExtensions

rem Publish the WebAssembly samples into a static site for GitHub Pages.
rem Usage: scripts\publish-pages.bat [output-dir] [base-path]
rem Example: scripts\publish-pages.bat site /blazor-anime

pushd "%~dp0.."
set "ROOT=%CD%"
popd

if "%~1"=="" (
  set "OUT=%ROOT%\site"
) else (
  set "OUT=%~1"
)

if "%~2"=="" (
  set "BASE=/blazor-anime"
) else (
  set "BASE=%~2"
)

if "%BASE:~-1%"=="/" set "BASE=%BASE:~0,-1%"
if not "%BASE:~0,1%"=="/" (
  echo Base path must start with / ^(got: %BASE%^) 1>&2
  exit /b 1
)

set "PYTHON="
where py >nul 2>&1 && set "PYTHON=py -3"
if not defined PYTHON where python >nul 2>&1 && set "PYTHON=python"
if not defined PYTHON where python3 >nul 2>&1 && set "PYTHON=python3"
if not defined PYTHON (
  echo Python is required to finish the Pages site. 1>&2
  exit /b 1
)

set "TMPDIR=%TEMP%\blazor-anime-pages-%RANDOM%%RANDOM%"
mkdir "%TMPDIR%" || exit /b 1

rem CompressionEnabled is the static-web-assets switch. GitHub Pages serves .br/.gz
rem as ordinary files, without a Content-Encoding header, so leave them out.
dotnet publish "%ROOT%\samples\Examples.WebAssembly\Examples.WebAssembly.csproj" --configuration Release -p:CompressionEnabled=false --output "%TMPDIR%\examples"
if errorlevel 1 goto :fail
dotnet publish "%ROOT%\samples\PageTransitions\PageTransitions.csproj" --configuration Release -p:CompressionEnabled=false --output "%TMPDIR%\travel"
if errorlevel 1 goto :fail

if exist "%OUT%" rmdir /s /q "%OUT%"
mkdir "%OUT%\examples" || goto :fail
mkdir "%OUT%\travel" || goto :fail

robocopy "%TMPDIR%\examples\wwwroot" "%OUT%\examples" /E /NFL /NDL /NJH /NJS >nul
if errorlevel 8 goto :fail
robocopy "%TMPDIR%\travel\wwwroot" "%OUT%\travel" /E /NFL /NDL /NJH /NJS >nul
if errorlevel 8 goto :fail

del /s /q "%OUT%\*.br" >nul 2>&1
del /s /q "%OUT%\*.gz" >nul 2>&1
type nul > "%OUT%\.nojekyll"

%PYTHON% "%ROOT%\scripts\publish-pages.py" "%OUT%" "%BASE%" "%ROOT%\pages"
if errorlevel 1 goto :fail

rmdir /s /q "%TMPDIR%"
echo Published %OUT% with base %BASE%
exit /b 0

:fail
if exist "%TMPDIR%" rmdir /s /q "%TMPDIR%"
exit /b 1
