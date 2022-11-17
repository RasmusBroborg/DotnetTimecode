using System.Text.RegularExpressions;

using DotnetTimecode.Enums;
using DotnetTimecode.Helpers;
using DotnetTimecode.Interfaces;

namespace DotnetTimecode
{
  /// <summary>
  /// Represents a SMPTE timecode. Supports the non drop frame format HH:MM:FF:SS
  /// <br/> and drop frame format HH:MM:SS;FF.
  /// </summary>
  public class Timecode : ITimecode, IEquatable<ITimecode>
  {
    /// <summary>
    /// Represents the timecode as a time POCO.
    /// </summary>
    protected readonly DateTime _time = DateTime.MinValue;

    #region Public Properties

    /// <summary>
    /// Regular expression pattern of a timecode.
    /// Supports the format "HH:MM:SS:FF" and "HH:MM:SS;FF".
    /// </summary>
    public static string TimecodeRegexPattern => @"^(([0-9]){2}:){2}(([0-9]){2})(;|:)([0-9]){2}$";

    /// <inheritdoc/>
    public Framerate Framerate { get; set; } = 0;

    /// <inheritdoc/>
    public int FrameCount => GetFrameCount();

    /// <inheritdoc/>

    public int Hour => (int)(_time - DateTime.MinValue).TotalHours;

    /// <inheritdoc/>
    public int Minute => (int)(_time - DateTime.MinValue).TotalMinutes;

    /// <inheritdoc/>
    public int Second => (int)(_time - DateTime.MinValue).TotalMilliseconds;

    /// <inheritdoc/>
    public int Frame => GetFramePosition();

    /// <inheritdoc/>
    public int Milliesecond => (_time - DateTime.MinValue).Milliseconds;

    /// <inheritdoc/>
    public long Ticks => (_time - DateTime.MinValue).Ticks;

    #endregion Public Properties

    #region Constructors

    /// <summary>
    /// Creates a new Timecode object with timecode position 00:00:00:00.
    /// </summary>
    /// <param name="framerate">The timecode framerate.</param>
    public Timecode(Framerate framerate)
    {
      Framerate = framerate;
    }

    /// <summary>
    /// Creates a new Timecode object based on a timespan.
    /// </summary>
    /// <param name="timespan">The timespan to construct a timecode from.</param>
    /// <param name="framerate">The timecode framerate.</param>
    public Timecode(TimeSpan timespan, Framerate framerate)
    {
      _time = Convert.ToDateTime(timespan.ToString());
      Framerate = framerate;
    }

    /// <summary>
    /// Creates a new Timecode object at a specified timecode position.
    /// </summary>
    /// <param name="hour">The timecode hour.</param>
    /// <param name="minute">The timecode minute.</param>
    /// <param name="second">The timecode second.</param>
    /// <param name="frame">The timecode frame.</param>
    /// <param name="framerate">The timecode framerate.</param>
    public Timecode(int hour, int minute, int second, int frame, Framerate framerate)
    {
      Framerate = framerate;
      int framesAsTicks = CalcTicksFromFrame(frame, framerate);

      _time.AddHours(hour);
      _time.AddMinutes(minute);
      _time.AddSeconds(second);
      _time.AddTicks(framesAsTicks);
    }

    /// <summary>
    /// Creates a new Timecode object at a specified timecode position.
    /// </summary>
    /// <param name="timecode">The timecode represented as a string.</param>
    /// <param name="framerate">The timecode framerate.</param>
    public Timecode(string timecode, Framerate framerate)
    {
      ValidateTimecodeString(timecode, TimecodeRegexPattern);

      ExtractTimecodeValues(timecode, out int hour, out int minute, out int second, out int frame);

      _time.AddHours(hour);
      _time.AddMinutes(minute);
      _time.AddSeconds(second);
      long framesAsTicks = CalcTicksFromFrame(frame, framerate);
      _time.AddTicks(framesAsTicks);
      Framerate = framerate;
    }

    #endregion Constructors

    #region Public Methods

    /// <inheritdoc/>
    public void AddHours(int hoursToAdd)
    {
      _time.AddHours(hoursToAdd);
    }

    /// <inheritdoc/>
    public void AddMinutes(int minutesToAdd)
    {
      _time.AddMinutes(minutesToAdd);
    }

    /// <inheritdoc/>
    public void AddSeconds(int secondsToAdd)
    {
      _time.AddSeconds(secondsToAdd);
    }

    /// <inheritdoc/>
    public void AddMilliseconds(int milliesecondsToAdd)
    {
      _time.AddMilliseconds(milliesecondsToAdd);
    }

    /// <inheritdoc/>
    public void AddFrames(int framesToAdd)
    {
      long ticksToAdd = CalcTicksFromFrame(framesToAdd, Framerate);
      _time.AddTicks(framesToAdd);
    }

    /// <inheritdoc/>
    public void ConvertFramerate(Framerate destinationFramerate)
    {
      // TODO: Recalculate timecode to new framerate

      // create diff ticks between two timecodes

      throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the timecode as a string formatted as a timecode.
    /// </summary>
    /// <returns>The timecode formatted as a timecode</returns>
    public override string ToString()
    {
      // Drop frame framerates are formatted HH:MM:SS;FF
      char lastColon = FramerateValues.IsNonDropFrame(Framerate) ? ':' : ';';

      return $"{AddZeroPadding(Hour)}" +
        $":{AddZeroPadding(Minute)}" +
        $":{AddZeroPadding(Second)}" +
        $"{lastColon}{AddZeroPadding(Frame)}";
    }

    /// <inheritdoc/>
    public virtual bool Equals(ITimecode? other)
    {
      return Ticks == other?.Ticks && Framerate == other.Framerate;
    }

    #endregion Public Methods

    #region Public Static Methods

    /// <summary>
    /// Adds hours to the given timecode.
    /// <br/><br/>
    /// Positive integer values add hours,
    /// while negative values subtract hours.
    /// </summary>
    /// <param name="timecode">Timecode to update, formatted as a timecode.</param>
    /// <param name="framerate">The timecode framerate.</param>
    /// <param name="hoursToAdd">Number of hours to add or subtract.</param>
    public static string AddHours(string timecode, Framerate framerate, int hoursToAdd)
    {
      ValidateTimecodeString(timecode, TimecodeRegexPattern);
      Timecode timecodeObj = new Timecode(timecode, framerate);
      timecodeObj.AddHours(hoursToAdd);
      return timecodeObj.ToString();
    }

    /// <summary>
    /// Adds minutes to the given timecode.
    /// <br/><br/>
    /// Positive integer values add minutes,
    /// while negative values subtract minutes.
    /// </summary>
    /// <param name="timecode">Timecode to update, formatted as a timecode.</param>
    /// <param name="framerate">The timecode framerate.</param>
    /// <param name="minutesToAdd">Number of minutes to add or subtract.</param>
    public static string AddMinutes(string timecode, Framerate framerate, int minutesToAdd)
    {
      ValidateTimecodeString(timecode, TimecodeRegexPattern);
      Timecode timecodeObj = new Timecode(timecode, framerate);
      timecodeObj.AddMinutes(minutesToAdd);
      return timecodeObj.ToString();
    }

    /// <summary>
    /// Adds seconds to the given timecode.
    /// <br/><br/>
    /// Positive integer values add seconds,
    /// while negative values subtract seconds.
    /// </summary>
    /// <param name="timecode">Timecode to update, formatted as a timecode.</param>
    /// <param name="secondsToAdd">Number of seconds to add or subtract.</param>
    public static string AddSeconds(string timecode, Framerate framerate, int secondsToAdd)
    {
      ValidateTimecodeString(timecode, TimecodeRegexPattern);
      Timecode timecodeObj = new Timecode(timecode, framerate);
      timecodeObj.AddSeconds(secondsToAdd);
      return timecodeObj.ToString();
    }

    /// <summary>
    /// /// <summary>
    /// Adds seconds to the given time.
    /// <br/><br/>
    /// Positive integer values add frames,
    /// while negative values subtract frames.
    /// </summary>
    /// <param name="inputString">Timecode to update, formatted as a timecode.</param>
    /// <param name="frames">Number of frames to add or subtract.</param>
    /// <param name="framerate"></param>
    /// <returns></returns>
    public static string AddFrames(string inputString, Framerate framerate, int frames)
    {
      Timecode timecode = new Timecode(inputString, framerate);
      timecode.AddFrames(frames);
      return timecode.ToString();
    }

    /// <summary>
    /// Converts a timecode string to a timecode string of a different framerate.
    /// </summary>
    /// <param name="originalTimecode">The original timecode, formatted as a timecode.</param>
    /// <param name="originalFramerate">The original timecode framerate.</param>
    /// <param name="destinationFramerate">The target framerate to convert to.</param>
    /// <returns>A string formatted as a timecode.</returns>
    /// <exception cref="ArgumentException">Throws if the input timecode format is invalid.</exception>
    public static string ConvertFramerate(
      string originalTimecode, Framerate originalFramerate, Framerate destinationFramerate)
    {
      // Validate the original timecode format
      ValidateTimecodeString(originalTimecode, TimecodeRegexPattern);

      Timecode timecode = new Timecode(originalTimecode, originalFramerate);
      timecode.ConvertFramerate(destinationFramerate);
      return timecode.ToString();
    }

    #endregion Public Static Methods

    #region Operator Overloads

    /// <summary>
    /// Addition of two Timecodes.
    /// </summary>
    /// <param name="leftTimecode">The timecode to add to.</param>
    /// <param name="rightTimecode">The timecode that will be added to the original.</param>
    /// <returns>The sum of the left and right timecode.</returns>
    /// <exception cref="InvalidOperationException">Is Thrown when Framerates are not equal.</exception>
    public static Timecode operator +(Timecode leftTimecode, Timecode rightTimecode)
    {
      if (leftTimecode.Framerate != rightTimecode.Framerate)
      {
        throw new InvalidOperationException("Arithmetic operations between different framerates are invalid.");
      }

      return new Timecode(new TimeSpan(leftTimecode.Ticks + rightTimecode.Ticks), leftTimecode.Framerate);
    }

    /// <summary>
    /// Subtraction of two Timecodes.
    /// </summary>
    /// <param name="leftTimecode">The timeCode to subtract from.</param>
    /// <param name="rightTimecode">The timeCode that will be subtracted.</param>
    /// <returns>The difference between timeCode left and right.</returns>
    /// <exception cref="InvalidOperationException">Is Thrown when FrameRates are not equal.</exception>
    public static Timecode operator -(Timecode leftTimecode, Timecode rightTimecode)
    {
      if (leftTimecode.Framerate != rightTimecode.Framerate)
      {
        throw new InvalidOperationException("Arithmetic operations between different framerates are invalid.");
      }
      return new Timecode(new TimeSpan(leftTimecode.Ticks - rightTimecode.Ticks), leftTimecode.Framerate);
    }

    /// <summary>
    /// Determines whether a timecode is smaller than another timecode.
    /// </summary>
    /// <param name="leftTimecode">The timecode of which needs to be determined if its smaller.</param>
    /// <param name="rightTimecode">The timecode which the other will be compared to.</param>
    /// <returns>True if the timecode is smaller than the compared timecode.</returns>
    /// <exception cref="InvalidOperationException">Is Thrown when FrameRates are not equal.</exception>
    public static bool operator <(Timecode leftTimecode, Timecode rightTimecode)
    {
      if (leftTimecode.Framerate != rightTimecode.Framerate)
      {
        throw new InvalidOperationException("Size comparison operations between different framerates are invalid. " +
          "\n Use the TotalFrames property for comparison operations between timecodes instead.");
      }
      return leftTimecode.Ticks < rightTimecode.Ticks;
    }

    /// <summary>
    /// Determines whether a timecode is larger than another timecode.
    /// </summary>
    /// <param name="leftTimecode">The timecode of which needs to be determined if its larger.</param>
    /// <param name="rightTimecode">The timecode which the other will be compared to.</param>
    /// <returns>whether a timecode is larger than another timecode</returns>
    /// <exception cref="InvalidOperationException">Is Thrown when FrameRates are not equal.</exception>
    public static bool operator >(Timecode leftTimecode, Timecode rightTimecode)
    {
      if (leftTimecode.Framerate != rightTimecode.Framerate)
      {
        throw new InvalidOperationException("Size comparison operations between different framerates are invalid. " +
          "\n Use the TotalFrames property for comparison operations between timecodes instead.");
      }
      return leftTimecode.Ticks > rightTimecode.Ticks;
    }

    /// <summary>
    /// Determines whether a timecode is smaller or equal to another timecode.
    /// </summary>
    /// <param name="leftTimecode">The timecode of which needs to be determined if its smaller.</param>
    /// <param name="rightTimecode">The timecode which the other will be compared to.</param>
    /// <returns>True if the timecode is smaller than the compared timecode.</returns>
    /// <exception cref="InvalidOperationException">Is Thrown when FrameRates are not equal.</exception>
    public static bool operator <=(Timecode leftTimecode, Timecode rightTimecode)
    {
      if (leftTimecode.Framerate != rightTimecode.Framerate)
      {
        throw new InvalidOperationException("Size comparison operations between different framerates are invalid. " +
          "\n Use the TotalFrames property for comparison operations between timecodes instead.");
      }
      return leftTimecode.Ticks <= rightTimecode.Ticks;
    }

    /// <summary>
    /// Determines whether a timecode is larger or equal to another timecode.
    /// </summary>
    /// <param name="leftTimecode">The timecode of which needs to be determined if its larger.</param>
    /// <param name="rightTimecode">The timecode which the other will be compared to.</param>
    /// <returns>whether a timecode is larger than another timecode</returns>
    /// <exception cref="InvalidOperationException">Is Thrown when FrameRates are not equal.</exception>
    public static bool operator >=(Timecode leftTimecode, Timecode rightTimecode)
    {
      if (leftTimecode.Framerate != rightTimecode.Framerate)
      {
        throw new InvalidOperationException("Size comparison operations between different framerates are invalid. " +
          "\n Use the TotalFrames property for comparison operations between timecodes instead.");
      }
      return leftTimecode.Ticks >= rightTimecode.Ticks;
    }

    #endregion Operator Overloads

    #region Private Methods

    private int GetFrameCount()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Pads the first number position with a 0 if the number is less than two positions long.
    /// </summary>
    /// <param name="num">The number to add padding to.</param>
    /// <param name="totalNumberOfCharacters">The result string number of characters.</param>
    /// <returns>A padded string representation of the number.</returns>
    private static string AddZeroPadding(int num, int totalNumberOfCharacters = 2)
    {
      int numAbs = Math.Abs(num);
      return numAbs.ToString().PadLeft(totalNumberOfCharacters, '0');
    }

    /// <summary>
    /// Calculates the totalframes DF based on the hours, minutes, seconds and frames.
    /// </summary>
    /// <param name="hours">The timecode hour.</param>
    /// <param name="minutes">The timecode minute.</param>
    /// <param name="seconds">The timecode second.</param>
    /// <param name="frames">The timecode frame.</param>
    /// <param name="framerate">The timecode framerate.</param>
    /// <returns>The totalframes DF</returns>
    private int CalcTotalFramesDF(int hours, int minutes, int seconds, int frames, Framerate framerate)
    {
      decimal framerateDecimalValue = FramerateValues.FramerateAsDecimals[framerate];

      int dropFrames = Convert.ToInt32(Math.Round(framerateDecimalValue * 0.066666m));
      int timeBase = Convert.ToInt32(Math.Round(framerateDecimalValue));
      int secondsPerMinute = 60;
      int minutesPerHour = 60;
      int hourFrames = timeBase * secondsPerMinute * minutesPerHour;
      int minuteFrames = timeBase * secondsPerMinute;
      int totalMinutes = minutesPerHour * hours + minutes;
      int totalFrames = hourFrames * hours + minuteFrames * minutes +
      timeBase * seconds + frames - dropFrames * (totalMinutes - totalMinutes / 10);
      return totalFrames;
    }

    /// <summary>
    /// Calculates the totalframes DF based on the hours, minutes, seconds and frames.
    /// </summary>
    /// <param name="hours">The timecode hour.</param>
    /// <param name="minutes">The timecode minute.</param>
    /// <param name="seconds">The timecode second.</param>
    /// <param name="frames">The timecode frame.</param>
    /// <param name="framerate">The timecode framerate.</param>
    /// <returns>The totalframes NDF</returns>
    private int CalcTotalFramesNDF(int hours, int minutes, int seconds, int frames, Framerate framerate)
    {
      decimal framerateDecimalValue = FramerateValues.FramerateAsDecimals[framerate];
      int timeBase = Convert.ToInt32(Math.Round(framerateDecimalValue));
      int secondsPerMinute = 60;
      int minutesPerHour = 60;
      int hourFrames = timeBase * secondsPerMinute * minutesPerHour;
      int minuteFrames = timeBase * secondsPerMinute;
      int totalFrames = hourFrames * hours + minuteFrames * minutes + timeBase * seconds + frames;
      return totalFrames;
    }

    /// <summary>
    /// Validates the format of a string representing a timecode.<br/>
    /// Throws an exception if the timecode string is invalid.
    /// </summary>
    protected static void ValidateTimecodeString(string timecode, string regexPattern)
    {
      Regex tcRegex = new Regex(regexPattern);
      if (!tcRegex.IsMatch(timecode))
        throw new ArgumentException("Invalid timecode format.", nameof(timecode));
    }

    // TODO!!
    protected int GetFramePosition()
    {
      throw new NotImplementedException();
    }

    // TODO!!
    protected int CalcTicksFromFrame(int frame, Framerate framerate)
    {
      int d;
      int m;

      int dropFrames = round(framerate * .066666);
      int framesPerHour = round(framerate * 60 * 60);
      int framesPer24Hours = framesPerHour * 24;
      int framesPer10Minutes = round(framerate * 60 * 10);
      int framesPerMinute = (round(framerate) * 60) - dropFrames;

      while (framenumber < 0)
      {
        framenumber = framesPer24Hours + framenumber;
      }

      framenumber = framenumber % framesPer24Hours;

      d = framenumber \ framesPer10Minutes;
      m = framenumber % framesPer10Minutes

if (m > dropFrames)
      {
        framenumber = framenumber + (dropFrames * 9 * d) + dropFrames * ((m - dropFrames) \ framesPerMinute);
      }
      else
      {
        framenumber = framenumber + dropFrames * 9 * d;
      }

      int frRound = round(framerate);
      int frames = framenumber % frRound;
      int seconds = (framenumber \ frRound) % 60;
      int minutes = ((framenumber \ frRound) \ 60) % 60;
      int hours = (((framenumber \ frRound) \ 60) \ 60);
    }

    /// <summary>
    /// Extract timecode values from a SMPTE timecode represented as a string.
    /// </summary>
    /// <param name="timecode">A string formatted as a timecode.</param>
    /// <param name="hour">The timecode hour.</param>
    /// <param name="minute">The timecode minute.</param>
    /// <param name="second">The timecode second.</param>
    /// <param name="frame">The timecode frame.</param>
    private static void ExtractTimecodeValues(string timecode,
      out int hour, out int minute, out int second, out int frame)
    {
      timecode = timecode.Replace(';', ':');
      string[] timecodeSplit = timecode.Split(":");
      hour = Convert.ToInt32(timecodeSplit[0]);
      minute = Convert.ToInt32(timecodeSplit[1]);
      second = Convert.ToInt32(timecodeSplit[2]);
      frame = Convert.ToInt32(timecodeSplit[3]);
    }

    #endregion Private Methods
  }
}