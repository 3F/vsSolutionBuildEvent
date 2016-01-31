A very simple files-helpers to start build, for example:

```
"C:\Program Files (x86)\MSBuild\12.0\bin\msbuild.exe" "vsSolutionBuildEvent_2013.sln" /verbosity:detailed /l:"packages\vsSBE.CI.MSBuild.1.5.1206\bin\CI.MSBuild.dll" /m:4 /p:Configuration=Debug /p:Platform="Any CPU"
```

/**[How to build](http://vssbe.r-eg.net/doc/Dev/How%20to%20build/)**