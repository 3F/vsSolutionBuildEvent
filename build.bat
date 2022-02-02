@echo off
echo Usage: %~nx0 [RCI flag] [configuration name or nothing to build all]
echo DBG == Debug; REL == Release; + _SDK10/15/17
if "%~1"=="RCI" ( set "IsRCI=1" & set "cfg=%~2" ) else ( set "IsRCI=" & set "cfg=%~1" )

::::::::::::::::::::
set __p_call=1
call tools\gnt /p:ngconfig="tools/packages.config" || goto err

set bnode=packages\vsSolutionBuildEvent\cim.cmd -vsw-priority Microsoft.NetCore.Component.SDK /m:7 /v:m /p:Platform="Any CPU"
if not defined cfg (

    call %bnode% /p:Configuration=REL_SDK10 || goto err
    call %bnode% /p:Configuration=REL_SDK15 || goto err
    call %bnode% /p:Configuration=REL_SDK17 || goto err

) else call %bnode% /p:Configuration=%cfg% || goto err

exit /B 0

:err
echo. Build failed. 1>&2
exit /B 1