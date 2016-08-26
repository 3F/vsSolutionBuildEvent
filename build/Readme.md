A very simple file-helpers to start build, for example:

```
"C:\Program Files (x86)\MSBuild\14.0\bin\msbuild.exe" "vsSolutionBuildEvent.sln" /verbosity:detailed /l:"packages\vsSBE.CI.MSBuild\bin\CI.MSBuild.dll" /m:4 /p:Configuration=Debug /p:Platform="Any CPU"
```

/**[How to build](http://vssbe.r-eg.net/doc/Dev/How%20to%20build/)**