# [vsSolutionBuildEvent](https://github.com/3F/vsSolutionBuildEvent)

**Event**-Catcher with variety of advanced **Actions** to service projects, libraries, the build processes and processes at runtime from Visual Studio and MSBuild Tools.

**Flexible actions for all ...**

[![Build status - master](https://ci.appveyor.com/api/projects/status/l38xn0j2c5an28e1/branch/master?svg=true)](https://ci.appveyor.com/project/3Fs/vssolutionbuildevent/branch/master)
[![D2](https://img.shields.io/sourceforge/dt/vssbe.svg)](https://sourceforge.net/projects/vssbe/files/latest/download)
[![VSPackage](https://vssbe.r-eg.net/etc/badges/VSPackage.svg)](https://vssbe.r-eg.net/Changelist/#vsix)
[![Tests](https://img.shields.io/appveyor/tests/3Fs/mvssln/master.svg)](https://ci.appveyor.com/project/3Fs/mvssln/build/tests)
[![LGPLv3](https://vssbe.r-eg.net/etc/badges/License.svg)](https://vssbe.r-eg.net/License/)
[![GetNuTool](https://vssbe.r-eg.net/etc/badges/GetNuTool.svg)](https://github.com/3F/GetNuTool)

[![Build history](https://buildstats.info/appveyor/chart/3Fs/hmsbuild?buildCount=15&includeBuildsFromPullRequest=true&showStats=true)](https://ci.appveyor.com/project/3Fs/hmsbuild/history)
[![Event model](https://vssbe.r-eg.net/doc/Resources/events_model_small.png)](https://vssbe.r-eg.net/doc/Scheme/#model-of-events)
[![Short Overview](https://vssbe.r-eg.net/doc/Resources/examples/overview-youtube.png)](https://youtu.be/FX5GiMX0ulI) 

[![nuget vsSBE.CI.MSBuild](https://img.shields.io/nuget/v/vsSBE.CI.MSBuild.svg)](https://www.nuget.org/packages/vsSBE.CI.MSBuild/)
[![CI.MSBuild](https://vssbe.r-eg.net/etc/badges/CI.MSBuild.svg)](https://vssbe.r-eg.net/Changelist/#cim)
[![Devenv](https://vssbe.r-eg.net/etc/badges/Devenv.svg)](https://vssbe.r-eg.net/Changelist/#devenv)
[![Provider](https://vssbe.r-eg.net/etc/badges/Provider.svg)](https://vssbe.r-eg.net/Changelist/#provider)
[![API](https://vssbe.r-eg.net/etc/badges/API.svg)](https://vssbe.r-eg.net/Changelist/#api)

**[Download](https://vssbe.r-eg.net/Downloads/)** (Binaries, Snapshots, Nightly builds, Libraries, ...)

* VisualStudio Marketplace: [Download .vsix](https://visualstudiogallery.msdn.microsoft.com/0d1dbfd7-ed8a-40af-ae39-281bfeca2334)

## License

Licensed under the [MIT License (MIT)](https://github.com/3F/vsSolutionBuildEvent/blob/master/LICENSE)

```
Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
```

[ [ ☕ Donate ](https://3F.github.com/Donation/) ]


## Why vsSolutionBuildEvent ?

Advanced handler of the most events from MS Visual Studio & MSBuild tools. Full control and flexible multi-actions for basic pre/post events and other additional such as:

* [CommandEvent](https://vssbe.r-eg.net/doc/Events/CommandEvent/), Errors, Warnings, Cancel-Build, Output-Build, Transmitter, Logging

Ability to handle events for all subprojects at once from the whole solution as an Solution-Events or individually for each separately.

Various modes for everything:

* Files Mode, Operation Mode, Interpreter Mode, [Script Mode](https://vssbe.r-eg.net/doc/Modes/Script/), [Targets Mode](https://vssbe.r-eg.net/doc/Modes/Targets/), [C# Mode](https://vssbe.r-eg.net/doc/Modes/CSharp/)

Supports [additional MSBuild](https://vssbe.r-eg.net/doc/Scripts/MSBuild/) features & [SBE-Scripts engine](https://vssbe.r-eg.net/doc/Scripts/SBE-Scripts/) for your awesome ideas.

Possible work even **without** Visual Studio. 

Provides also support of the CI-Build Servers (TeamCity, AppVeyor, Azure DevOps, ...), Command-Line mode and lot of other features for the convenience of your work with the build, tests, versioning, IO operations, and so on. See the documentation.

* [SBE-Scripts](https://vssbe.r-eg.net/doc/Scripts/SBE-Scripts/)
* [MSBuild](https://vssbe.r-eg.net/doc/Scripts/MSBuild/)
* **[Examples](https://vssbe.r-eg.net/doc/Examples/)** *- scripts, solutions, syntax etc.,*
* [Continuous Integration (CI)](https://vssbe.r-eg.net/doc/CI/)
* [Processing modes](https://vssbe.r-eg.net/doc/Modes/)
* [API](https://vssbe.r-eg.net/doc/API/)

[![Scheme of vsSolutionBuildEvent projects](https://vssbe.r-eg.net/doc/Resources/scheme.png)](https://vssbe.r-eg.net/doc/Scheme/)


* [Changelist](https://vssbe.r-eg.net/Changelist/)
* [How to build](https://vssbe.r-eg.net/doc/Dev/How%20to%20build/) ([Developer Zone](https://vssbe.r-eg.net/doc/Dev/))
* [Wiki](https://vssbe.r-eg.net/)
* [Public Bug Tracker](https://github.com/3F/vsSolutionBuildEvent/issues) 

