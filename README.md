## What is dotnet-timecode?

Dotnet Timecode is a single class c# library built to create an API for working with SMPTE Timecodes defined by the Society of Motion Picture and Television Engineers in the SMPTE 12M specification.

The library allows the user to construct timecode objects, manipulate timecode values, and convert between the most common framerates, including drop frame framerates such as 29.97DF and 59.95DF.

## How do I get started?

Add a reference to the library, then simply construct your objects.

Examples:
```csharp
var foo = new Timecode(Framerate.fps30);
var bar = new Timecode(10, 00, 00, 00, Framerate.fps59_94_DF);
var baz = new Timecode("10:00:00:00", Framerate.fps24);

foo.ToString() // "00:00:00:00"
bar.AddMinutes(-61).ToString(); // "08:59:00:00"
baz.ConvertFramerate(Framerate.fps25).ToString(); // "09:36:00:00"
```

## Contributions

Feel free to either add issues or create a pull requests through forks.
Use appropriate issue-labels and imperative style commit messages.

## License, etc.

dotnet-timecode is Copyright &copy; 2022 Rasmus Broborg and other contributors under the [MIT license](LICENSE.txt).
