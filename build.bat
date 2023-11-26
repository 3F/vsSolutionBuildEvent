@echo off & echo Usage: %~nx0 [ DBG or REL or RCI [ SDKs numbers ] ]

::::: Default :::::
set SDKs=10,15,17
set "cfg=%~1" & if not defined cfg set "cfg=REL"
:::::

set __p_call=1
call tools\gnt /p:ngconfig="tools/packages.config" || goto err

if "%cfg%"=="RCI" ( set "IsRCI=1" & set "cfg=REL" ) else set "IsRCI="

set "SDK=%~2" & if not defined SDK (
    call :act %cfg% %SDKs% & goto ok
)

:act
set "SDK=%~2" & if not defined SDK goto ok

call packages\vsSolutionBuildEvent\cim.cmd -vsw-priority Microsoft.NetCore.Component.SDK /m:7 /v:m /noautorsp /p:Configuration=%cfg%_SDK%~2 || goto err
shift & goto act

:err
echo. Build failed. 1>&2
exit /B 1

:ok
exit /B 0