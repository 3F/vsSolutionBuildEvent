@echo off
echo Usage: %~nx0 [configuration name or nothing to test all]

set "cfg=%~1"
set ciLogger=Appveyor.TestLogger -Version 2.0.0
set tcmd=dotnet test --no-build --no-restore --test-adapter-path:. --logger:Appveyor

::::::::::::::::::::

setlocal
    cd CI.MSBuild.Test
    nuget install %ciLogger%
endlocal
setlocal
    cd vsSolutionBuildEventTest
    nuget install %ciLogger%
endlocal

if not defined cfg (

    call %tcmd% -c REL_SDK10 CI.MSBuild.Test
    call %tcmd% -c REL_SDK15 CI.MSBuild.Test
    call %tcmd% -c REL_SDK17 CI.MSBuild.Test

    call %tcmd% -c REL_SDK10 vsSolutionBuildEventTest
    call %tcmd% -c REL_SDK15 vsSolutionBuildEventTest
    call %tcmd% -c REL_SDK17 vsSolutionBuildEventTest

) else (
    call %tcmd% -c %cfg% CI.MSBuild.Test
    call %tcmd% -c %cfg% vsSolutionBuildEventTest
)

exit /B 0

:err
echo. Build failed. 1>&2
exit /B 1