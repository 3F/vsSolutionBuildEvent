@echo off

REM # Version of used CI.MSBuild
set cimdll=packages\vsSBE.CI.MSBuild\bin\CI.MSBuild.dll

REM # MSBuild tools
set _msbuild=tools\hMSBuild -notamd64

REM # Solution file by defualt
set sln=vsSolutionBuildEvent.sln

REM # Platform by default
set platform="Any CPU"

REM # Verbosity level by default: q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic].
set level=minimal

set _gnt=tools\gnt

:::: --------

REM # Configuration name without postfix _SDK...
:: DBG == Debug; REL == Release; DCI == CI Debug; RCI == CI Release;

set reltype=%~1

if "%reltype%"=="" (
    set "reltype=DCI"
)

:::: --------

set __p_call=1

:: package restore for SDK-based projects
:: call %_msbuild% -t:restore /p:Configuration=%reltype%_SDK15 /p:Platform="Any CPU"

:: Activate vsSBE

call %_gnt% /p:ngpath="%cd%/packages" /p:ngconfig="%cd%/.gnt/packages.config" || goto err

:: Build

set bnode=%_msbuild% %sln% /m:4 /l:"%cimdll%" /p:Platform=%platform% /v:%level%

call %bnode% /p:Configuration=%reltype%_SDK10 /t:Rebuild || goto err
call %bnode% /p:Configuration=%reltype%_SDK15 /t:Build || goto err

goto ok

:err

echo. Build failed. 1>&2
exit /B 1

:ok
exit /B 0
