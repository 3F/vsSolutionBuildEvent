@echo off
REM # https://github.com/3F/vsSolutionBuildEvent

set "_wdir=%~dp0\tools"
set cimdll=%_wdir%\CI.MSBuild.dll

%_wdir%\hMSBuild %* /l:"%cimdll%" /nologo %__cim_vssbe_xtracmd%