@echo off

REM # Version of used CI.MSBuild
REM # set CIM=1.6.1207

REM # Version of MSBuild tool by default
set _msbuild=14.0

REM # Solution file by defualt
set sln=vsSolutionBuildEvent_2015.sln

REM # Target by default
set target=Rebuild

REM # Maximum number of concurrent processes to use when building
set /a maxcpu=12

REM # Configuration by default
set cfgname=CI_Debug

REM # Platform by default
set platform="Any CPU"

REM # Verbosity level by default: q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic].
set level=detailed