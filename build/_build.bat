cd "%~dp0..\\"
call "%~dp0_packages" "%_msbuild_exe%"
echo.
"%_msbuild_exe%" "%sln%" /verbosity:%level% /l:"packages\vsSBE.CI.MSBuild\bin\CI.MSBuild.dll" /m:%maxcpu% /t:%target% /p:Configuration=%cfgname% /p:Platform=%platform%