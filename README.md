# vsSolutionBuildEvent (vsSBE)

##### Flexible actions for all ...

[![Build status - master](https://ci.appveyor.com/api/projects/status/l38xn0j2c5an28e1/branch/master?svg=true)](https://ci.appveyor.com/project/3Fs/vssolutionbuildevent/branch/master)

[![VSPackage](https://img.shields.io/badge/VSPackage-v0.12.1-68217A.svg)](http://visualstudiogallery.msdn.microsoft.com/0d1dbfd7-ed8a-40af-ae39-281bfeca2334/) [![Download vsSolutionBuildEvent](https://img.shields.io/sourceforge/dm/vssbe.svg)](https://sourceforge.net/projects/vssbe/files/latest/download) [![Download](https://img.shields.io/sourceforge/dt/vssbe.svg)](https://sourceforge.net/projects/vssbe/files/latest/download) [![LGPLv3](https://img.shields.io/badge/license-LGPLv3-008033.svg?style=flat-square)](https://bitbucket.org/3F/vssolutionbuildevent/raw/master/LICENSE) 


MSBuild & SBE-Scripts engine for advanced usage. 

+Supports the CI /Build Servers, Command-Line mode and many other features for build, tests, debugging, versioning, CI, logging(+logger for msbuild), work with files etc., 

-------
[Download](http://visualstudiogallery.msdn.microsoft.com/0d1dbfd7-ed8a-40af-ae39-281bfeca2334/referral/118151) (SourceForge.net)                    
![statistic](http://vssbe.sourceforge.net/stat/)

*MS Visual Studio Gallery*:

* [0d1dbfd7-ed8a-40af-ae39-281bfeca2334](http://visualstudiogallery.msdn.microsoft.com/0d1dbfd7-ed8a-40af-ae39-281bfeca2334/)

*Components*:

* [![nuget vsSBE.CI.MSBuild](https://img.shields.io/nuget/v/vsSBE.CI.MSBuild.svg)](https://www.nuget.org/packages/vsSBE.CI.MSBuild/) [![CI.MSBuild](https://img.shields.io/badge/CI.MSBuild-v1.2-8080C0.svg?style=flat)](http://sourceforge.net/projects/vssbe/files/CI-Utilities/CI.MSBuild/) [![Devenv](https://img.shields.io/badge/Devenv-v1.2-B5B5D7.svg?style=flat)](http://sourceforge.net/projects/vssbe/files/CI-Utilities/Devenv/)  [![Provider](https://img.shields.io/badge/Provider-v2.1-BAC5C0.svg?style=flat)](https://sourceforge.net/projects/vssbe/files/API/Provider/) [![API](https://img.shields.io/badge/API-v1.2-AFCFBE.svg?style=flat)](https://sourceforge.net/projects/vssbe/files/API/Bridge/)

-------
[![Short Overview](https://bitbucket.org/3F/vssolutionbuildevent/wiki/Resources/examples/overview-youtube.png)](http://youtu.be/FX5GiMX0ulI) 
[![Event model](https://bitbucket.org/3F/vssolutionbuildevent/wiki/Resources/events_model.png)](https://bitbucket.org/3F/vssolutionbuildevent/wiki/scheme#markdown-header-model-of-events)

Advanced handler of most events for MS Visual Studio & MSBuild tools. Full control and flexible multi actions for basic events and others additional, such as:

* [CommandEvent](https://bitbucket.org/3F/vssolutionbuildevent/wiki/Events/CommandEvent), Errors, Warnings, Cancel-Build, Output-Build, Transmitter, Logging

Ability to handle events for all subprojects at once in solution as Solution-Events or individually for each project.

Available different modes for all of what you want:

* Files Mode, Operation Mode, Interpreter Mode, [Script Mode](https://bitbucket.org/3F/vssolutionbuildevent/wiki/Modes/Script), [Targets Mode](https://bitbucket.org/3F/vssolutionbuildevent/wiki/Modes/Targets), [C# Mode](https://bitbucket.org/3F/vssolutionbuildevent/wiki/Modes/CSharp)

*Can be used without Visual Studio for work through msbuild.exe (Microsoft Build Tools)*


* [Changelist](https://bitbucket.org/3F/vssolutionbuildevent/raw/master/changelog.txt)
* **[How to build](https://bitbucket.org/3F/vssolutionbuildevent/wiki/Developer%20Zone/How%20to%20build)** ([Developer Zone](https://bitbucket.org/3F/vssolutionbuildevent/wiki/Developer%20Zone))
* [SBE-Scripts](https://bitbucket.org/3F/vssolutionbuildevent/wiki/Scripts_&_Commands/SBE-Scripts)
* [MSBuild](https://bitbucket.org/3F/vssolutionbuildevent/wiki/Scripts_&_Commands/MSBuild)
* [DTE-Commands](https://bitbucket.org/3F/vssolutionbuildevent/wiki/Scripts_&_Commands/DTE-Commands)
* **[Examples](https://bitbucket.org/3F/vssolutionbuildevent/wiki/Examples)** *- scripts, solutions, syntax etc.,*
* [Continuous Integration (CI)](https://bitbucket.org/3F/vssolutionbuildevent/wiki/CI)
* [Processing modes](https://bitbucket.org/3F/vssolutionbuildevent/wiki/Modes)
* [API](https://bitbucket.org/3F/vssolutionbuildevent/wiki/API)

[![Scheme of vsSolutionBuildEvent projects](https://bitbucket.org/3F/vssolutionbuildevent/wiki/Resources/scheme.png)](https://bitbucket.org/3F/vssolutionbuildevent/wiki/scheme)

-------
* [Wiki & Complete Solutions](https://bitbucket.org/3F/vssolutionbuildevent/wiki/Home)
* **[Public Issue Tracker](https://bitbucket.org/3F/vssolutionbuildevent/issues)** - *bug ? suggestions ? write here*


**[Support us: ![About donation](https://bitbucket.org/3F/vssolutionbuildevent/raw/master/vsSolutionBuildEvent/Resources/help-16.png)](https://bitbucket.org/3F/vssolutionbuildevent/wiki/Donation)**    [![Donate](https://bitbucket.org/3F/vssolutionbuildevent/raw/master/vsSolutionBuildEvent/Resources/paypal.png)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=P2HRG52AJSA9N&lc=US&item_name=vsSolutionBuildEvent%20%28vsSBE%29%20projects&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donate_SM%2egif%3aNonHosted)