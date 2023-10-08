![Image of a non drop frame timecode.](https://github.com/RasmusBroborg/DotnetTimecode/blob/main/assets/DotnetTimecode_asset_2997NDF.png)

# What is Dotnet Timecode?

[![CI](https://github.com/RasmusBroborg/DotnetTimecode/actions/workflows/ci.yml/badge.svg)](https://github.com/RasmusBroborg/DotnetTimecode/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/DotnetTimecode.svg)](https://www.nuget.org/packages/DotnetTimecode/)
[![Nuget](https://img.shields.io/nuget/dt/DotnetTimecode.svg)](https://nuget.org/packages/DotnetTimecode)
[![CodeQL](https://github.com/RasmusBroborg/DotnetTimecode/actions/workflows/codeql.yml/badge.svg)](https://github.com/RasmusBroborg/DotnetTimecode/actions/workflows/codeql.yml)

Dotnet Timecode is a C# class library for working with SMPTE Timecodes defined by the Society of Motion Picture and Television Engineers in the SMPTE 12M specification.

The library allows the user to construct timecode objects, manipulate timecode values, and convert between the most common framerates, including drop frame framerates such as 29.97DF and 59.95DF.

# How do I get started?

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

ITimecode foo = new Timecode(Framerate.fps30);
ITimecode bar = new Timecode(10, 00, 00, 00, Framerate.fps59_94_DF);
ITimecode baz = new Timecode("10:00:00:00", Framerate.fps24);

foo.ToString() // "00:00:00:00"
bar.AddMinutes(-61).ToString(); // "08:59:00;00"
baz.ConvertFramerate(Framerate.fps25).ToString(); // "09:36:00:00"
```

# Public endpoints

### Constructors

```csharp
Timecode(Enums.Framerate framerate);
Timecode(int hour, int minute, int second, int frame, Enums.Framerate framerate);
Timecode(string timecode, Enums.Framerate framerate);
```

### Object methods

```csharp
timecodeObj.AddHours(int hours);
timecodeObj.AddMinutes(int minutes);
timecodeObj.AddSeconds(int seconds);
timecodeObj.AddFrames(int frames);
timecodeObj.ConvertFramerate(Enums.Framerate targetFramerate);
string timecodeString = timecodeObj.ToString();
string subtitleTimecodeString = timecodeObj.ToSubtitleString();
```

### Static methods

```csharp
string timecodeString = Timecode.AddHours(timecodeString, hoursInteger);
string timecodeString = Timecode.AddMinutes(timecodeString, minutesInteger);
string timecodeString = Timecode.AddSeconds(timecodeString, secondsInteger);
string timecodeString = Timecode.AddFrames(timecodeString, framesInteger);
string timecodeString = Timecode.ConvertFramerate(timecodeString, originalFramerateEnum, targetFramerateEnum);
string timecodeString = Timecode.ConvertSMPTETimecodeToSubtitleTimecode(srtTimecodeString, framerateEnum);
string subtitleTimecodeString = Timecode.ConvertSubtitleTimecodeToSMPTETimecode(timecodeString, framerateEnum);
```

### Object Properties

```csharp
int hour = timecodeObj.Hour;
int minute = timecodeObj.Minute;
int second = timecodeObj.Second;
int frame = timecodeObj.Frame;
int totalFrames = timecodeObj.TotalFrames;
Enums.Framerate framerate = timecodeObj.Framerate;
```

### Static Properties

```csharp
string timecodeRegex = Timecode.TimecodeRegexPattern;
string subtitleTimecodeRegex = Timecode.SubtitleTimecodeRegexPattern;
```

### Operator Overloading

```csharp
ITimecode timecodeObj1 = new Timecode(10, 0, 0, 0, Enums.Framerate.fps23_976); // 10:00:00:00
ITimecode timecodeObj2 = new Timecode(1, 0, 0, 0, Enums.Framerate.fps23_976); // 01:00:00:00

timecodeObj1 + timecodeObj2;  // 11:00:00:00
timecodeObj1 - timecodeObj2;  // 09:00:00:00
timecodeObj1 < timecodeObj2;  // False
timecodeObj1 > timecodeObj2;  // True
timecodeObj1 <= timecodeObj2; // False
timecodeObj1 >= timecodeObj2; // True
timecodeObj1 == timecodeObj2; // False
timecodeObj1 != timecodeObj2; // True
```

# Contributions

### How to contribute to the project

See [CONTRIBUTING.md](https://github.com/RasmusBroborg/DotnetTimecode/blob/master/CONTRIBUTING.md) and [CODE_OF_CONDUCT.md](https://github.com/RasmusBroborg/DotnetTimecode/blob/master/CODE_OF_CONDUCT.md) for instructions on how to contribute to the project.

# License

Dotnet Timecode is Copyright &copy; 2022 Rasmus Broborg and other contributors under the [MIT license](LICENSE.txt).
