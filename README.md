## What is Dotnet Timecode?

[![CI](https://github.com/RasmusBroborg/dotnet-timecode/actions/workflows/ci.yml/badge.svg)](https://github.com/RasmusBroborg/dotnet-timecode/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/DotnetTimecode.svg)](https://www.nuget.org/packages/DotnetTimecode/)
[![Nuget](https://img.shields.io/nuget/dt/DotnetTimecode.svg)](https://nuget.org/packages/DotnetTimecode)

Dotnet Timecode is a single class c# library built to create an API for working with SMPTE Timecodes defined by the Society of Motion Picture and Television Engineers in the SMPTE 12M specification.

The library allows the user to construct timecode objects, manipulate timecode values, and convert between the most common framerates, including drop frame framerates such as 29.97DF and 59.95DF.

## How do I get started?

Get the latest nuget package using the dotnet CLI

```
dotnet add package DotnetTimecode
```

or using the Nuget Package Manager

```
Install-Package DotnetTimecode
```

Add a reference to the library, then simply construct your objects.

Examples:

```csharp
using DotnetTimecode;
using DotnetTimecode.Enums;

var foo = new Timecode(Framerate.fps30);
var bar = new Timecode(10, 00, 00, 00, Framerate.fps59_94_DF);
var baz = new Timecode("10:00:00:00", Framerate.fps24);

foo.ToString() // "00:00:00:00"
bar.AddMinutes(-61).ToString(); // "08:59:00:00"
baz.ConvertFramerate(Framerate.fps25).ToString(); // "09:36:00:00"
```

## Public endpoints

### Constructors

```csharp
Timecode(Enums.Framerate framerate);
Timecode(int hour, int minute, int second, int frame, Enums.Framerate framerate);
Timecode(string timecode, Enums.Framerate framerate);
```

### Methods

```csharp
string timecodeString = timecodeObj.ToString();
timecodeObj.AddHours(1);
timecodeObj.AddMinutes(1);
timecodeObj.AddSeconds(120);
timecodeObj.AddFrames(1);
timecodeObj.ConvertFramerate(Enums.Framerate targetFramerate);
```

### Properties

```csharp
int hour = timecodeObj.Hour;
int minute = timecodeObj.Minute;
int second = timecodeObj.Second;
int frame = timecodeObj.Frame;
int totalFrames = timecodeObj.TotalFrames;
Enums.Framerate framerate = timecodeObj.Framerate;
string timecodeRegex = Timecode.RegexPattern;
```

### Operator Overloading

```csharp
var tc1 = new Timecode(10, 0, 0, 0, DotnetTimecode.Enums.Framerate.fps23_976);//10:00:00:00
var tc2 = new Timecode(1, 0, 0, 0, DotnetTimecode.Enums.Framerate.fps23_976); //01:00:00:00
Console.WriteLine(tc1 + tc2); //11:00:00:00
Console.WriteLine(tc1 - tc2); //09:00:00:00
Console.WriteLine(tc1 < tc2); // False
Console.WriteLine(tc1 > tc2); // True
Console.WriteLine(tc1 <= tc2); // False
Console.WriteLine(tc1 >= tc2); // True
Console.WriteLine(tc1 == tc2);// False
Console.WriteLine(tc1 != tc2);// False
```

## Contributions

### How to contribute to the project

See [CONTRIBUTING.md](https://github.com/RasmusBroborg/dotnet-timecode/blob/master/CONTRIBUTING.md) and [CODE_OF_CONDUCT.md](https://github.com/RasmusBroborg/dotnet-timecode/blob/master/CODE_OF_CONDUCT.md) for instructions on how to contribute to the project.

## License, etc.

Dotnet Timecode is Copyright &copy; 2022 Rasmus Broborg and other contributors under the [MIT license](LICENSE.txt).
