call "%~dp0_config"

REM # Configuration
set cfgname=Release_with_revision

REM # Solution file
set sln=vsSolutionBuildEvent_net40.sln

REM # Version of MSBuild tool
set _msbuild=12.0

call "%~dp0_build"