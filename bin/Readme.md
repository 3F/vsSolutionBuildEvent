All final binaries will be here...

Use ['build_x.bat'](../build) file-helpers to start build without Visual Studio like:

```
"msbuild.exe" "vsSolutionBuildEvent.sln" /l:"packages\vsSBE.CI.MSBuild\bin\CI.MSBuild.dll" /m:4 /p:Configuration=Debug
```

*Or click on `Build` - `Build Solution` if you have [installed plugin](https://visualstudiogallery.msdn.microsoft.com/0d1dbfd7-ed8a-40af-ae39-281bfeca2334/)*

/**[How to build](http://vssbe.r-eg.net/doc/Dev/How%20to%20build/)** (the list of requirements etc.)