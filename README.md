[![](https://raw.githubusercontent.com/3F/vsSolutionBuildEvent/master/vsSolutionBuildEvent/Resources/Package.png)](https://github.com/3F/vsSolutionBuildEvent) [**vsSolutionBuildEvent**](https://github.com/3F/vsSolutionBuildEvent)

**Control everything: Visual Studio, MSBuild Tools, CI, and more …**

🎛 *Event*-Catcher with variety of advanced *Actions* 

[![Event model](https://3F.github.io/web.vsSBE/doc/Resources/events_model_small.png)](#)

to service projects, libraries, build processes, runtime environment of the Visual Studio, MSBuild Tools, and … 

```r
Copyright (c) 2013-2023  Denis Kuzmin <x-3F@outlook.com> github/3F
```

[ 「 <sub>@</sub> ☕ 」 ](https://3F.github.io/Donation/) [![LGPLv3](https://img.shields.io/badge/LGPLv3-008033.svg)](LICENSE)

[**Download** *latest*](https://github.com/3F/vsSolutionBuildEvent/releases/latest)

* *Archive*: All binaries before *v1.14* via sf [![D2](https://img.shields.io/sourceforge/dt/vssbe.svg)](https://sourceforge.net/projects/vssbe/)
* Page on [VisualStudio Marketplace](https://marketplace.visualstudio.com/items?itemName=GitHub3F.vsSolutionBuildEvent-11615)

[![Build status - master](https://ci.appveyor.com/api/projects/status/l38xn0j2c5an28e1/branch/master?svg=true)](https://ci.appveyor.com/project/3Fs/vssolutionbuildevent/branch/master)
[![vsix](https://img.shields.io/badge/dynamic/xml?color=6C2C7C&label=vsix&query=//text()&url=https://raw.githubusercontent.com/3F/vsSolutionBuildEvent/master/.version)](https://marketplace.visualstudio.com/items?itemName=GitHub3F.vsSolutionBuildEvent-11615)
[![nuget](https://img.shields.io/nuget/v/vsSolutionBuildEvent.svg)](https://www.nuget.org/packages/vsSolutionBuildEvent/)
[![API](https://img.shields.io/badge/dynamic/xml?color=A9C6B7&label=API&query=//text()&url=https://raw.githubusercontent.com/3F/vsSolutionBuildEvent/master/Bridge/.version)](https://3F.github.io/web.vsSBE/Changelist/#api)

[![Build history](https://buildstats.info/appveyor/chart/3Fs/vssolutionbuildevent?buildCount=20&showStats=true)](https://ci.appveyor.com/project/3Fs/vssolutionbuildevent/history)

[![MvsSln](https://img.shields.io/badge/🧩-MvsSln-865FC5)](https://github.com/3F/MvsSln)
[![GetNuTool](https://img.shields.io/badge/🧩-GetNuTool-93C10B)](https://github.com/3F/GetNuTool)
[![SobaScript](https://img.shields.io/badge/🧩-SobaScript-8E5733)](https://github.com/3F/SobaScript)
[![E-MSBuild](https://img.shields.io/badge/🧩-E--MSBuild-C8597A)](https://github.com/3F/E-MSBuild)

## Why vsSolutionBuildEvent ?

Advanced handler of the most **events** from MS Visual Studio & MSBuild tools. Full control and flexible multi-**actions** for basic pre/post events and other additional such as:

* [CommandEvent](https://3F.github.io/web.vsSBE/doc/Events/CommandEvent/), Errors, Warnings, Cancel-Build, Output-Build, Transmitter, Logging

Ability to handle events for all subprojects at once from the whole solution as an Solution-Events or individually for each separately.

Various modes for everything:

* Files Mode, Operation Mode, Interpreter Mode, [Script Mode](https://3F.github.io/web.vsSBE/doc/Modes/Script/), [Targets Mode](https://3F.github.io/web.vsSBE/doc/Modes/Targets/), [C# Mode](https://3F.github.io/web.vsSBE/doc/Modes/CSharp/)

Supports [Advanced MSBuild](https://3F.github.io/web.vsSBE/doc/Scripts/MSBuild/) & [SBE-Scripts engine](https://3F.github.io/web.vsSBE/doc/Scripts/SBE-Scripts/) for your awesome ideas.

Provides CI support (TeamCity, AppVeyor, Azure DevOps, ...), Command-Line mode and lot of other features for convenient work with the builds, tests, versioning, IO operations, and so on. See the documentation.

* [SBE-Scripts](https://3F.github.io/web.vsSBE/doc/Scripts/SBE-Scripts/)
* [MSBuild](https://3F.github.io/web.vsSBE/doc/Scripts/MSBuild/)
* **[Examples](https://3F.github.io/web.vsSBE/doc/Examples/)** *- scripts, solutions, syntax etc.,*
* [Continuous Integration (CI)](https://3F.github.io/web.vsSBE/doc/CI/)
* [Processing modes](https://3F.github.io/web.vsSBE/doc/Modes/)
* [API](https://3F.github.io/web.vsSBE/doc/API/)

[![Scheme of vsSolutionBuildEvent projects](https://3F.github.io/web.vsSBE/doc/Resources/scheme.png)](https://3F.github.io/web.vsSBE/doc/Scheme/)


* [Install & Build](https://3F.github.io/web.vsSBE/doc/Dev/How-to-build/) ([Developer Zone](https://3F.github.io/web.vsSBE/doc/Dev/))
* [Wiki](https://3F.github.io/web.vsSBE/) - read or edit


## Advanced MSBuild 

Through [E-MSBuild](https://github.com/3F/E-MSBuild) engine.

```js
#[$(
    [System.Math]::Exp('$(
        [MSBuild]::Multiply(
            $([System.Math]::Log(10)), 
            4
        ))'
    )
)]
```

```js
$(n = 0)       $(desc = "Hello ")
$(n += 3.14)   $(desc += "from vsSBE !")
$(n += $(n))   $(p1 = " Platform is $(Platform)")
```

```js
$(...)
$(...:project) - from selected project in your solution
$$(...) ... $$(...:project)
```

```js
$(tStart = $([System.DateTime]::Parse("2014/01/01").ToBinary()))
$([System.Guid]::NewGuid())

$([System.TimeSpan]::FromTicks($([MSBuild]::Subtract($(tNow), $(tStart))))
                        .TotalHours.ToString("0"))

$(pdir = $(ProjectDir:project))
$(pdir = $(ProjectDir.Replace('\', '/'):project))
```

## #SobaScript ##

https://github.com/3F/SobaScript -- Extensible Modular Scripting Programming Language.

```js
#["
    #SobaScript in action
"]
#[var v = 1.2.3]
#[$(log = "$(TMP)/v.txt")]

#[($(Configuration) ~= Deb || true)
{
    #[var tBase     = $([System.DateTime]::Parse('2019/08/01').ToBinary())]
    #[var tNow      = $([System.DateTime]::UtcNow.Ticks)]
    #[var revBuild  = #[$(
        [System.TimeSpan]::FromTicks('$(
            [MSBuild]::Subtract(
            $(tNow), 
            $(tBase))
        )')
        .TotalMinutes
        .ToString('0')
    )]]
    
    #[var v = $(v).$([MSBuild]::Modulo($(revBuild), $([System.Math]::Pow(2, 14))))]
}]

#[var v = $([System.String]::Format("v{0}\r\n\t", $(v)))]

#[try
{ 
    #[File write("#[var log]"):> Example #[var v] Generated by a vsSolutionBuildEvent]
    #[IO scall("notepad", "#[var log]")]
}
catch(err, msg)
{
    $(err) - Type of Exception
    $(msg) - Error Message
}]
```

For example, you can even exclude projects from build at runtime:

```js
#[Build projects.find("name").IsBuildable = false]
``` 

Capture data from external utilities:

```js
#[var bSha1 = #[IO sout("git", "rev-parse --short HEAD")]]
```

Work with files and archives:

```js
#[IO copy.file("$(odir)/notes.txt", "$(pDirCIM)bin\\$(cfg)\\", true)]
#[7z pack.files({
            "$(pDirBridge)bin\$(cfg)\Bridge.*.*",
            "CI.MSBuild.dll",
            "CI.MSBuild.pdb",
            "$(pDirCIM)bin\$(cfg)\*.txt"}, "$(odir)CI.MSBuild_v$(numCIM)_[$(branchSha1)][$(netStamp)].zip")]

```

+DTE-commands, +Access to all MSBuild properties on the fly, +Conditional statements and lot of other components:

```js
#[try
{
    #[Box iterate(i = 0; $(i) < 10; i += 1): 
        ...
    ]
}catch{ }]

#[( #[vsSBE events.Pre.item(1).Enabled] || ($(Configuration) == "Release" && $(sysc)) )
{
    #[Build projects.find("name").IsBuildable = false]
}
else
{
    #[var bSha1 = #[IO sout("git", "rev-parse --short HEAD")]]
    ...
}]
```

... [create **new** in 5 minutes](https://3F.github.io/web.vsSBE/doc/Dev/New%20Component/)

## Processing modes

[https://3F.github.io/web.vsSBE/doc/Modes/](https://3F.github.io/web.vsSBE/doc/Modes/)

From simple commands to C# or even msbuild targets:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="4.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">

    <Target Name="Init">
        <!-- ... -->
    </Target>

    <!--
        Additional properties:
            $(ActionName)
            $(BuildType)
            $(EventType)
            $(SupportMSBuild)
            $(SupportSBEScripts)
            $(SolutionActiveCfg)
            $(StartupProject)
    -->
</Project>
```

## CommandEvent (DTE)

You can also use this to catch all command from VS IDE. [Samples:](https://3F.github.io/web.vsSBE/doc/Events/CommandEvent/)

```js
$(lcGuid = #[DTE events.LastCommand.Guid])
$(lcId   = #[DTE events.LastCommand.Id])

#[($(lcGuid) == "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}" && $(lcId) == 1627) {
    #[File scall("notepad", "#[var log]", 30)]
}]
```

![](https://3F.github.io/web.vsSBE/doc/Resources/examples/CommandEvent.gif)

## Automatic Version Numbering

See our [**Wizard** for automatic code generation **or** use any **custom scripts**.](https://3F.github.io/web.vsSBE/doc/Examples/Version/)

![](https://3F.github.io/web.vsSBE/doc/Resources/examples/VersionClass.gif)

## Various environments

You can easily use this with TeamCity, Azure DevOps, AppVeyor, and any other automated environments:

![](https://3F.github.io/web.vsSBE/doc/Resources/ci_example_appveyor.png)

![](https://3F.github.io/web.vsSBE/doc/Resources/CI.MSBuild_example_TC.png)

![](https://3F.github.io/web.vsSBE/doc/Resources/CI.MSBuild_example_console.png)

![](https://3F.github.io/web.vsSBE/doc/Resources/Demo/DemoClient.png)

...

## Solution-wide Build events

Pre-Build / Post-Build events for all projects at once or individually for each separately: [configure what you need.](https://3F.github.io/web.vsSBE/doc/Features/Solution-wide/)

![](https://3F.github.io/web.vsSBE/doc/Resources/examples/obsolete/vbs_ext.jpg)

## Stop build on first error

[Immediately stop](https://3F.github.io/web.vsSBE/doc/Examples/Errors.Stop%20build/) (at the same time) after the first appearance (compared with StopOnFirstBuildError plugin [[?]](https://3F.github.io/web.vsSBE/doc/Examples/Errors.Stop%20build/))

![](https://3F.github.io/web.vsSBE/doc/Resources/examples/stop_build.png)

## Wiki

[Wiki](https://3F.github.io/web.vsSBE/) - Contains help for work with plugins, basic examples, syntax, information for develop, and lot of other...

Feel free to improve any our pages. Click [Edit] button or [Start new here.](https://3F.github.io/web.vsSBE/doc/New/)


## Questions / Bugs / Suggestions / Source Code

Welcome to the new home (*\*since 2017; before, Bitbucket*):

* [https://github.com/3F/vsSolutionBuildEvent](https://github.com/3F/vsSolutionBuildEvent)

## Screenshots

![](https://3F.github.io/web.vsSBE/doc/Resources/Screenshots/UI-State_panel.png)

![](https://3F.github.io/web.vsSBE/doc/Resources/Screenshots/main_v0.12.png)

![](https://3F.github.io/web.vsSBE/doc/Resources/Screenshots/msbuild_prop_code_completion.png)

![](media/scr/Automatic_Version_Numbering.png)

[![Overview.VS2013](https://3F.github.io/web.vsSBE/doc/Resources/examples/overview-youtube.png)](https://youtu.be/FX5GiMX0ulI)

[**[. . .](https://3F.github.io/web.vsSBE/Screenshots/)**]