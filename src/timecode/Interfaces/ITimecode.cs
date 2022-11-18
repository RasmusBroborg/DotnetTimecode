using DotnetTimecode.Enums;

namespace DotnetTimecode.Interfaces
{
  /// <summary>
  /// Defines a generalized SMPTE Timecode model.
  /// </summary>
  public interface ITimecode
  {
    /// <summary>
    /// Gets the timecode framerate.
    /// </summary>
    Framerate Framerate { get; set; }

    /// <summary>
    /// Gets a regex string for a SMPTE Timecode formatted HH:MM:SS:FF and HH:MM:SS;FF
    /// </summary>
    public static readonly string? SrtTimecodeRegexPattern;

    /// <summary>
    /// Gets the hour of the current timecode structure expressed in whole
    /// hours.
    /// </summary>
    int Hour { get; }

    /// <summary>
    /// Gets the minute of the current timecode structure expressed in whole
    /// minutes.
    /// </summary>
    int Minute { get; }

    /// <summary>
    /// Gets the second of the current timecode structure expressed in whole
    /// seconds.
    /// </summary>
    int Second { get; }

    /// <summary>
    /// Gets the milliesecond of the current timecode structure expressed in
    /// whole millieseconds.
    /// </summary>
    int Milliesecond { get; }

    /// <summary>
    /// Gets the number of ticks of the current timecode structure expressed
    /// in whole ticks.
    /// </summary>
    long Ticks { get; }

    /// <summary>
    /// Gets the frame of the current timecode structure expressed in whole
    /// frames.
    /// </summary>
    int Frame { get; }

    /// <summary>
    /// Gets the timecode frame count.
    /// </summary>
    int FrameCount { get; }

    /// <summary>
    /// Adds hours to the timecode.
    /// <br/><br/>
    /// Positive integer values add hours,
    /// while negative values subtract hours.
    /// </summary>
    /// <param name="hoursToAdd">The number of hours to add or remove.</param>
    void AddHours(int hoursToAdd);

    /// <summary>
    /// Adds minutes to the timecode.
    /// <br/><br/>
    /// Positive integer values add minutes,
    /// while negative values subtract minutes.
    /// </summary>
    /// <param name="minutesToAdd">The number of minutes to add to the timecode.</param>
    void AddMinutes(int minutesToAdd);

    /// <summary>
    /// Adds seconds to the timecode.
    /// <br/><br/>
    /// Positive integer values add seconds,
    /// while negative values subtract seconds.
    /// </summary>
    /// <param name="secondsToAdd">The number of seconds to add to the timecode.</param>
    void AddSeconds(int secondsToAdd);

    /// <summary>
    /// Adds frames to the timecode.
    /// <br/><br/>
    /// Positive integer values add frames,
    /// while negative values subtract frames.
    /// </summary>
    /// <param name="framesToAdd">The number of frames to add to the timecode.</param>
    void AddFrames(int framesToAdd);

    /// <summary>
    /// Adds millieseconds to the timecode.
    /// <br/><br/>
    /// Positive integer values add millieseconds,
    /// while negative values subtract millieseconds.
    /// </summary>
    /// <param name="milliesecondsToAdd">The number of millieseconds to add to the timecode.</param>
    void AddMilliseconds(int milliesecondsToAdd);

    /// <summary>
    /// Converts the timecode object to the target framerate.
    /// </summary>
    /// <param name="destinationFramerate">The target framerate to convert to.</param>
    void ConvertFramerate(Framerate destinationFramerate);
  }
}