@echo off

REM # Version of used CI.MSBuild
REM # set CIM=1.6.1207

REM # Version of MSBuild tool by default
REM # set _msbuild=14.0

REM # MSBuild tools
set _msbuild_exe=%~dp0..\\tools\\msbuild.bat

REM # Solution file by defualt
set sln=vsSolutionBuildEvent.sln

REM # Target by default
set target=Rebuild

REM # Maximum number of concurrent processes to use when building
set /a maxcpu=8

REM # Configuration by default
set cfgname=CI_Debug

REM # Platform by default
set platform="Any CPU"

REM # Verbosity level by default: q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic].
set level=normal