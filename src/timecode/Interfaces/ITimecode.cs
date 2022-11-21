using DotnetTimecode.Enums;

namespace DotnetTimecode
{
  /// <summary>
  /// Provides an interface for a SMPTE Timecode class.
  /// </summary>
  public interface ITimecode
  {
    /// <summary>
    /// The timecode framerate.
    /// </summary>
    Framerate Framerate { get; }

    /// <summary>
    /// The timecode hour position.
    /// </summary>
    int Hour { get; }

    /// <summary>
    /// The timecode minute position.
    /// </summary>
    int Minute { get; }

    /// <summary>
    /// The timecode second position.
    /// </summary>
    int Second { get; }

    /// <summary>
    /// The timecode frame position.
    /// </summary>
    int Frame { get; }

    /// <summary>
    /// The total amount of frames, where 0 frames represent the timecode 00:00:00:00.
    /// </summary>
    int TotalFrames { get; }

    /// <summary>
    /// Adds frames to the timecode.
    /// <br/><br/>
    /// Positive integer values add frames,
    /// while negative values subtract frames.
    /// </summary>
    /// <param name="framesToAdd">The number of frames to add or remove.</param>
    void AddFrames(int framesToAdd);

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
    /// Converts the timecode object to the target framerate.
    /// </summary>
    /// <param name="destinationFramerate">The target framerate to convert to.</param>
    void ConvertFramerate(Framerate destinationFramerate);

    /// <summary>
    /// Returns the timecode as a string formatted as a SMPTE timecode.
    /// </summary>
    /// <returns>The timecode formatted as a SMPTE timecode</returns>
    string ToString();

    /// <summary>
    /// Returns the timecode as a string formatted as a subtitle timecode,
    /// with millieseconds after the last delimiter.
    /// </summary>
    /// <returns>The timecode formatted as a subtitle timecode</returns>
    string ToSubtitleString();
  }
}