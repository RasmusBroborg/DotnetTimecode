using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DotnetTimecode.Enums;

namespace DotnetTimecode.Models
{
  /// <summary>
  /// Represents a SubRip timecode, formatted HH:MM:SS,XXX,
  /// where XXX represents millieseconds.
  ///
  /// </summary>
  public class TimecodeSubRip : Timecode
  {
    /// <inheritdoc/>
    public TimecodeSubRip(Framerate framerate) : base(framerate)
    {
    }

    public const string SubRip = @"^(([0-9]){2}:){2}(([0-9]){2})(;|:)([0-9]){2}$";

    /// <inheritdoc/>
    public TimecodeSubRip(TimeSpan timespan, Framerate framerate)
      : base(timespan, framerate)
    {
    }

    /// <summary>
    /// Creates a new Timecode object at a specified timecode position.
    /// </summary>
    /// <param name="timecode">The timecode represented as a string.</param>
    /// <param name="framerate">The timecode framerate.</param>
    public TimecodeSubRip(string timecode, Framerate framerate)
    {
      IsValidSMPTETimecode(timecode, TimecodeRegexPattern);
      ExtractTimecodeValues(timecode,
      out int hour, out int minute, out int second, out int millieseconds);

      _time.AddHours(hour);
      _time.AddMinutes(minute);
      _time.AddSeconds(second);
      _time.AddTicks(millieseconds);
      Framerate = framerate;
    }

    /// <summary>
    /// /// <summary>
    /// Extracts the timecode values from a SubRip timecode represented as a string.
    /// </summary>
    /// <param name="timecode">A string formatted as a timecode.</param>
    /// <param name="hour">The timecode hour.</param>
    /// <param name="minute">The timecode minute.</param>
    /// <param name="second">The timecode second.</param>
    /// <param name="milliesecond">The timecode milliesecond.</param>
    private static void ExtractTimecodeValues(string timecode,
      out int hour, out int minute, out int second, out int milliesecond)
    {
      timecode = timecode.Replace(',', ':');
      string[] timecodeSplit = timecode.Split(":");
      hour = Convert.ToInt32(timecodeSplit[0]);
      minute = Convert.ToInt32(timecodeSplit[1]);
      second = Convert.ToInt32(timecodeSplit[2]);
      milliesecond = Convert.ToInt32(timecodeSplit[3]);
    }

    /// <inheritdoc/>
    public TimecodeSubRip(int hour, int minute, int second, int frame, Framerate framerate)
      : base(hour, minute, second, frame, framerate)
    {
    }

    /// <summary>
    /// Regular expression pattern of a SubRip timecode.
    /// Supports the format "HH:MM:SS,XXX", where XXX represents millieseconds.
    /// </summary>
    public static string TimecodeRegexPattern => @"^(([0-9]){2}:){2}(([0-9]){2})(,)([0-9]){3}$";

    /// <summary>
    /// Returns the timecode as a string formatted as a SubRip timecode.
    /// </summary>
    /// <returns>The timecode formatted as a timecode.</returns>
    public override string ToString()
    {
      return $"{AddZeroPadding(Hour)}" +
        $":{AddZeroPadding(Minute)}" +
        $":{AddZeroPadding(Second)}" +
        $",{AddZeroPadding(Milliesecond, 3)}";
    }
  }
}