@echo off

REM https://github.com/3F/vsSolutionBuildEvent/pull/45#issuecomment-506754001
set hMSBuild=-notamd64

set cim=packages\vsSolutionBuildEvent\cim.cmd -vsw-priority Microsoft.NetCore.Component.SDK
set __p_call=1

call tools\gnt /p:ngconfig="tools/packages.config" || goto err

:: Build
set bnode=%cim% %hMSBuild% /m:7 /p:Platform="Any CPU" /v:m

:: DBG == Debug; REL == Release; RCI == CI Release; + _SDK10/15
set cfg=%~1
if not defined cfg (
    call %bnode% /p:Configuration=REL_SDK10 || goto err
    call %bnode% /p:Configuration=REL_SDK15 || goto err

) else call %bnode% /p:Configuration=%cfg% || goto err

exit /B 0

:err
echo. Build failed. 1>&2
exit /B 1