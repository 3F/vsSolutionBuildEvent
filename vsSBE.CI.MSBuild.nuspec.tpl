<?xml version="1.0"?>
<package xmlns="http://schemas.microsoft.com/packaging/2011/08/nuspec.xsd">
  <metadata>
    <id>vsSBE.CI.MSBuild</id>
    <title>CI.MSBuild vsSolutionBuildEvent</title>
    <version>%CIM%.%Lib%</version>
    <authors>github.com/3F/vsSolutionBuildEvent</authors>
    <owners>reg</owners>
    <licenseUrl>https://visualstudiogallery.msdn.microsoft.com/0d1dbfd7-ed8a-40af-ae39-281bfeca2334/</licenseUrl>
    <projectUrl>https://visualstudiogallery.msdn.microsoft.com/0d1dbfd7-ed8a-40af-ae39-281bfeca2334/</projectUrl>
    <iconUrl>https://bitbucket.org/3F/vssolutionbuildevent/raw/master/vsSolutionBuildEvent/Resources/Package.png</iconUrl>
    <requireLicenseAcceptance>false</requireLicenseAcceptance>
    <description>Official package of libraries to support the CI /Build Servers for plugin vsSolutionBuildEvent.
    
    * *!* Documentation: http://vssbe.r-eg.net -> http://vssbe.r-eg.net/doc/CI/CI.MSBuild/
    * VS Gallery Page: https://visualstudiogallery.msdn.microsoft.com/0d1dbfd7-ed8a-40af-ae39-281bfeca2334/
    * Source code &amp; Public Issue Tracker: 
        * https://github.com/3F/vsSolutionBuildEvent
        * https://bitbucket.org/3F/vssolutionbuildevent
        
    Package version: a.b.x[.y] -&gt; a &amp; b - CI.MSBuild (v%CIM%) -&gt; x - main library (%Lib% is a v%vsSBE%) -&gt; y - variant of package if exists

    ~~~~~~~~
    Alternative to get it:
    ==========================================
    msbuild gnt.core /p:ngpackages="vsSBE.CI.MSBuild/%CIM%.%Lib%"
    ==========================================
    * https://github.com/3F/GetNuTool
    
    ~10 Kb to get CI.MSBuild in one click (Compiled by GetNuTool): vssbe.r-eg.net/doc/CI/get.CIM.bat 
    </description>
    <releaseNotes>Updated the vsSolutionBuildEvent library: v%vsSBE%</releaseNotes>
    <copyright>Copyright (c) 2013-2016  Denis Kuzmin (reg) [entry.reg@gmail.com]</copyright>
    <tags>Build Tools vsSolutionBuildEvent MSBuild logger automation Version vsSBE Scripts Events CI Solution Projects Versioning AppVeyor TeamCity Build-Server Continuous-Integration Build-automation</tags>
  </metadata>
</package>