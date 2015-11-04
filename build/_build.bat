cd %~dp0..\
.nuget\nuget restore "%sln%"
"C:\Program Files (x86)\MSBuild\%_msbuild%\bin\msbuild.exe" "%sln%" /verbosity:%level% /l:"packages\vsSBE.CI.MSBuild.%CIM%\bin\CI.MSBuild.dll" /m:%maxcpu% /t:%target% /p:Configuration=%cfgname% /p:Platform=%platform%