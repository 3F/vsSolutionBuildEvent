# [SobaScript.Mapper](https://github.com/3F/SobaScript.Mapper)

Mapper for SobaScript components and their nodes. 

**-- #SobaScript** 

Extensible Modular Scripting Programming Language.

https://github.com/3F/SobaScript

[![Build status](https://ci.appveyor.com/api/projects/status/SobaScript.Mapper/branch/master?svg=true)](https://ci.appveyor.com/project/3Fs/SobaScript.Mapper/branch/master)
[![release-src](https://img.shields.io/github/release/3F/SobaScript.Mapper.svg)](https://github.com/3F/SobaScript.Mapper/releases/latest)
[![License](https://img.shields.io/badge/License-MIT-74A5C2.svg)](https://github.com/3F/SobaScript.Mapper/blob/master/License.txt)
[![NuGet package](https://img.shields.io/nuget/v/SobaScript.Mapper.svg)](https://www.nuget.org/packages/SobaScript.Mapper/)
[![Tests](https://img.shields.io/appveyor/tests/3Fs/SobaScript.Mapper/master.svg)](https://ci.appveyor.com/project/3Fs/SobaScript.Mapper/build/tests)

[![Build history](https://buildstats.info/appveyor/chart/3Fs/SobaScript.Mapper?buildCount=20&includeBuildsFromPullRequest=true&showStats=true)](https://ci.appveyor.com/project/3Fs/SobaScript.Mapper/history)

## License

Licensed under the [MIT License](https://github.com/3F/SobaScript.Mapper/blob/master/License.txt)

```
Copyright (c) 2014-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
```

[ [ ☕ Donate ](https://3F.github.com/Donation/) ]

SobaScript.Mapper contributors: https://github.com/3F/SobaScript.Mapper/graphs/contributors

# Mapper

This helps to describe your component for code completion (Intellisense), for generating the documentation files, etc.

## PropertyAttribute

To describe the properties. For example:

```csharp
[Property("propertyName", "Description of the property", CValType.Boolean, CValType.Boolean)]
public string yourLogic()
{
   ...
}
```

```csharp
[Property(
    "IsBuildable", 
    "Gets or Sets whether the project or project item configuration can be built.", 
    "find", 
    "stProjectConf", 
    CValType.Boolean, 
    CValType.Boolean
)]
```


Basic Syntax:

```csharp
[Property(string name, string description, CValType get, CValType set)]
```

```csharp
[Property(string name, string parent, string method, CValType get, CValType set)]
```


Note:

* An optional `parent` argument is used for linking to parent property/method/etc.
* An `method` argument must contain the real method name who implements the parent element (property/method etc.)

## MethodAttribute

To describe the methods/functions. For example:


```csharp
[
    Method
    (
        "call", 
        "Caller of executable files with arguments.", 
        new string[] { "name", "args" }, 
        new string[] { "Executable file", "Arguments" }, 
        CValType.Void, 
        CValType.String, CValType.String
    )
]
protected string stCall(string data, bool stdOut, bool silent)
{
    ...
}
```

Basic Syntax:

```csharp
[Method(string name, string description, CValType ret, params CValType[] args)]
```

```csharp
[Method(string name, string parent, string method, CValType ret, params CValType[] args)]
```

Note:

* An optional `parent` argument is used for linking to parent property/method/etc.
* An `method` argument must contain the real method name who implements the parent element (property/method etc.)


## ComponentAttribute

To describe the new component. For example:

```csharp
[Component("File", "I/O operations")]
public class FileComponent: Component, IComponent
{
    ...
}
```

Basic Syntax:

```csharp
[Component(string name, string description)]
```

### Aliases

```csharp
[Component("Primary", new[]{ "Alias1", "Alias2", "Alias3" }, "description")]
```

## DefinitionAttribute

To describe the any definition. For example:

```csharp
[Definition("(true) { }", "Conditionals statements\n\n(1 > 2) {\n ... \n}")]
public class ConditionComponent: Component, IComponent
{
    ...
}
```

```csharp
[Definition("var name", "Get data from variable the 'name'")]
[Definition("var name = data", "Set the 'data' for variable the 'name'")]
```

Syntax:

```csharp
[Definition(string name, string description)]
```
