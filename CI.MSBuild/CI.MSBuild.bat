@echo off
REM # The documentation: http://vssbe.r-eg.net/doc/CI/CI.MSBuild/

set cimdll=CI.MSBuild.dll

msbuild %* /l:"%~dp0%cimdll%" /nologo