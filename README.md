# vsSolutionBuildEvent (vsSBE)

##### Flexible actions for all ...

[![Build status - master](https://ci.appveyor.com/api/projects/status/l38xn0j2c5an28e1/branch/master?svg=true)](https://ci.appveyor.com/project/3Fs/vssolutionbuildevent/branch/master)

[![VSPackage](http://vssbe.r-eg.net/etc/badges/VSPackage.svg)](http://vssbe.r-eg.net/Changelist/#vsix) [![Download vsSolutionBuildEvent](https://img.shields.io/sourceforge/dm/vssbe.svg)](https://sourceforge.net/projects/vssbe/files/latest/download) [![Download](https://img.shields.io/sourceforge/dt/vssbe.svg)](https://sourceforge.net/projects/vssbe/files/latest/download) [![LGPLv3](http://vssbe.r-eg.net/etc/badges/License.svg)](http://vssbe.r-eg.net/License/) 


MSBuild & SBE-Scripts engine for advanced usage. 

+Supports the CI /Build Servers, Command-Line mode and many other features for build, tests, debugging, versioning, CI, logging(+logger for msbuild), work with files etc., 

-------
[Download .vsix](http://visualstudiogallery.msdn.microsoft.com/0d1dbfd7-ed8a-40af-ae39-281bfeca2334/referral/118151) (SourceForge.net) :: **[Downloads](http://vssbe.r-eg.net/Downloads/)**                    
![statistic](http://vssbe.sourceforge.net/stat/)

*MS Visual Studio Gallery*:

* [0d1dbfd7-ed8a-40af-ae39-281bfeca2334](http://visualstudiogallery.msdn.microsoft.com/0d1dbfd7-ed8a-40af-ae39-281bfeca2334/)

*Components*:

* [![nuget vsSBE.CI.MSBuild](https://img.shields.io/nuget/v/vsSBE.CI.MSBuild.svg)](https://www.nuget.org/packages/vsSBE.CI.MSBuild/) [![CI.MSBuild](http://vssbe.r-eg.net/etc/badges/CI.MSBuild.svg)](http://vssbe.r-eg.net/Changelist/#cim) [![Devenv](http://vssbe.r-eg.net/etc/badges/Devenv.svg)](http://vssbe.r-eg.net/Changelist/#devenv)  [![Provider](http://vssbe.r-eg.net/etc/badges/Provider.svg)](http://vssbe.r-eg.net/Changelist/#provider) [![API](http://vssbe.r-eg.net/etc/badges/API.svg)](http://vssbe.r-eg.net/Changelist/#api)

-------
[![Short Overview](https://bitbucket.org/3F/vssolutionbuildevent/wiki/Resources/examples/overview-youtube.png)](http://youtu.be/FX5GiMX0ulI) 
[![Event model](http://vssbe.r-eg.net/doc/Resources/events_model.png)](http://vssbe.r-eg.net/doc/Scheme/#model-of-events)

Advanced handler of most events for MS Visual Studio & MSBuild tools. Full control and flexible multi actions for basic events and others additional, such as:

* [CommandEvent](http://vssbe.r-eg.net/doc/Events/CommandEvent/), Errors, Warnings, Cancel-Build, Output-Build, Transmitter, Logging

Ability to handle events for all subprojects at once in solution as Solution-Events or individually for each project.

Available different modes for all of what you want:

* Files Mode, Operation Mode, Interpreter Mode, [Script Mode](http://vssbe.r-eg.net/doc/Modes/Script/), [Targets Mode](http://vssbe.r-eg.net/doc/Modes/Targets/), [C# Mode](http://vssbe.r-eg.net/doc/Modes/CSharp/)

*Can be used without Visual Studio for work through msbuild.exe (Microsoft Build Tools)*


* [Changelist](http://vssbe.r-eg.net/Changelist/)
* **[How to build](http://vssbe.r-eg.net/doc/Dev/How%20to%20build/)** ([Developer Zone](http://vssbe.r-eg.net/doc/Dev/))
* [SBE-Scripts](http://vssbe.r-eg.net/doc/Scripts/SBE-Scripts/)
* [MSBuild](http://vssbe.r-eg.net/doc/Scripts/MSBuild/)
* [DTE-Commands](http://vssbe.r-eg.net/doc/Scripts/DTE-Commands/)
* **[Examples](http://vssbe.r-eg.net/doc/Examples/)** *- scripts, solutions, syntax etc.,*
* [Continuous Integration (CI)](http://vssbe.r-eg.net/doc/CI/)
* [Processing modes](http://vssbe.r-eg.net/doc/Modes/)
* [API](http://vssbe.r-eg.net/doc/API/)

[![Scheme of vsSolutionBuildEvent projects](http://vssbe.r-eg.net/doc/Resources/scheme.png)](http://vssbe.r-eg.net/doc/Scheme/)

-------
* [Wiki & Complete Solutions](http://vssbe.r-eg.net/)
* **[Public Issue Tracker](https://bitbucket.org/3F/vssolutionbuildevent/issues)** - *bug ? suggestions ? write here*


**[Support us: ![](https://bitbucket.org/3F/vssolutionbuildevent/raw/master/vsSolutionBuildEvent/Resources/help-16.png)](http://vssbe.r-eg.net/Donation/)**    [![Donate](https://bitbucket.org/3F/vssolutionbuildevent/raw/master/vsSolutionBuildEvent/Resources/paypal.png)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=P2HRG52AJSA9N&lc=US&item_name=vsSolutionBuildEvent%20%28vsSBE%29%20projects&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donate_SM%2egif%3aNonHosted)