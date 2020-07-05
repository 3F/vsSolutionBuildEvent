@echo off

REM https://github.com/3F/vsSolutionBuildEvent/pull/45#issuecomment-506754001
set hMSBuild=-notamd64

set cim=packages\vsSolutionBuildEvent\cim.cmd -vsw-priority Microsoft.NetCore.Component.SDK
set _gnt=tools\gnt

REM # Verbosity level: q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic].
set level=m

REM # Configuration name without postfix _SDK...
:: DBG == Debug; REL == Release; DCI == CI Debug; RCI == CI Release;

set reltype=%~1
if "%reltype%"=="" (
    set "reltype=DCI"
)

:::: --------

set __p_call=1

:: Activate vsSBE

call %_gnt% /p:ngpath="%cd%/packages" /p:ngconfig="%cd%/.gnt/packages.config" || goto err

:: Build

set bnode=%cim% %hMSBuild% vsSolutionBuildEvent.sln /m:7 /p:Platform="Any CPU" /v:%level%

rem call git clean -x -e \.vs -e \.user -d
call %bnode% /p:Configuration=%reltype%_SDK10 || goto err
call %bnode% /p:Configuration=%reltype%_SDK15 || goto err

goto ok

:err

echo. Build failed. 1>&2
exit /B 1

:ok
exit /B 0
