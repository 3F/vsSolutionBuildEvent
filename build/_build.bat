cd "%~dp0..\\"
call "%~dp0_packages" "C:\Program Files (x86)\MSBuild\%_msbuild%\bin\msbuild.exe"
"C:\Program Files (x86)\MSBuild\%_msbuild%\bin\msbuild.exe" "%sln%" /verbosity:%level% /l:"packages\vsSBE.CI.MSBuild.%CIM%\bin\CI.MSBuild.dll" /m:%maxcpu% /t:%target% /p:Configuration=%cfgname% /p:Platform=%platform%