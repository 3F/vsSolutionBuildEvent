@echo off
REM # The documentation: http://vssbe.r-eg.net/doc/CI/CI.MSBuild/

set cimdll=CI.MSBuild.dll

hMSBuild %* /l:"%~dp0%cimdll%" /nologo