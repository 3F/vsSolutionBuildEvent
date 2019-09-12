@echo off

:: To restore packages via GetNuTool
:: https://github.com/NuGet/Home/issues/1521

set _gnt=tools/gnt
set _gntArgs=%*
set _pkgdir=packages


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
call %_gnt% %_gntArgs% /p:wpath="%cd%" /p:ngpath="%_pkgdir%" /p:ngconfig=".gnt/packages.config;vsSolutionBuildEvent/packages.config;vsSolutionBuildEventTest/packages.config" /nologo /v:m || goto err

goto exit

:err

echo. failed. 1>&2
exit /B 1

:exit
echo . > %cd%\%_pkgdir%\__checked
exit /B 0