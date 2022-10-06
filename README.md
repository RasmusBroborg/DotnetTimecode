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
```
Timecode(Enums.Framerate framerate);
Timecode(int hour, int minute, int second, int frame, Enums.Framerate framerate);
Timecode(string timecode, Enums.Framerate framerate);
```

### Object Methods
```
string timecodeString = timecodeObj.ToString();
timecodeObj.AddHours(int hours);
timecodeObj.AddMinutes(int minutes);
timecodeObj.AddSeconds(int seconds);
timecodeObj.AddFrames(int frames);
timecodeObj.ConvertFramerate(Enums.Framerate targetFramerate);
```

### Properties
```
int hour = timecodeObj.Hour;
int minute = timecodeObj.Minute;
int second = timecodeObj.Second;
int frame = timecodeObj.Frame;
int totalFrames = timecodeObj.TotalFrames;
Enums.Framerate framerate = timecodeObj.TotalFrames;
string timecodeRegex = Timecode.RegexPattern;
```

## Contributions

Feel free to [contribute](CONTRIBUTING.md) by either adding an issues or creating pull requests through forks.
Use appropriate issue-labels and imperative style commit messages. Check out the Issues tab for requests and ideas for future implementations or bugfixes.


## License, etc.

Dotnet Timecode is Copyright &copy; 2022 Rasmus Broborg and other contributors under the [MIT license](LICENSE.txt).
