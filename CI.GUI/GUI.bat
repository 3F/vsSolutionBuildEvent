@echo off
REM # https://github.com/3F/vsSolutionBuildEvent

set "_wdir=%~dp0\tools"
set cigui=%_wdir%\GUI.exe

set argin=%*
if not defined argin (
    rem \packages\vsSolutionBuildEvent\
    %cigui% ..\..
) else (
    %cigui% %*
)