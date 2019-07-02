@echo off

:: To restore packages via GetNuTool
:: https://github.com/NuGet/Home/issues/1521

set _gnt=tools/gnt
set _gntArgs=%*


:found

echo. 
echo. [ Restoring packages. Please wait ... ]
echo. 


:: -
echo.
echo MvsSln ...
call %_gnt% %_gntArgs% /p:wpath="%cd%/MvsSln" /p:ngconfig="packages.config;MvsSlnTest/packages.config" /nologo /v:m /m:4 || goto err

:: -
echo.
echo vsSolutionBuildEvent ...
call %_gnt% %_gntArgs% /p:ngpath="%cd%/packages" /p:ngconfig="%cd%/.gnt/packages.config;%cd%/vsSolutionBuildEvent/packages.config;%cd%/vsSolutionBuildEventTest/packages.config" /nologo /v:m || goto err

goto exit

:err

echo. failed. 1>&2
exit /B 1

:exit
exit /B 0