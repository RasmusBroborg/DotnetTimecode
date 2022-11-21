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

    /// <inheritdoc/>
    public Framerate Framerate { get; set; } = 0;

    /// <inheritdoc/>
    public int FrameCount => CalcFrameCount();

    /// <inheritdoc/>
    public int Hour => Convert.ToInt32((_time - DateTime.MinValue).TotalHours); // TODO: Needs to take drop frames into consideration

    /// <inheritdoc/>
    public int Minute => (_time - DateTime.MinValue).Minutes;

    /// <inheritdoc/>
    public int Second => (_time - DateTime.MinValue).Seconds;

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
      int framesAsMillieseconds = CalcMilliesecondsFromFrame(frame, framerate);

      _time.AddHours(hour);
      _time.AddMinutes(minute);
      _time.AddSeconds(second);
      _time.AddMilliseconds(framesAsMillieseconds);
    }

    /// <summary>
    /// Creates a new Timecode object at a specified timecode position.
    /// </summary>
    /// <param name="timecode">The timecode represented as a string.</param>
    /// <param name="framerate">The timecode framerate.</param>
    public Timecode(string timecode, Framerate framerate)
    {
      if (IsValidSMPTETimecode(timecode))
        ConstructUsingSMPTEString(timecode, framerate);
      if (IsValidSubtitleTimecode(timecode))
        ConstructUsingSubtitleTimecodeString(timecode, framerate);

      ThrowInvalidTimecodeException(timecode);
    }

    private void ConstructUsingSubtitleTimecodeString(
      string timecode, Framerate framerate)
    {
      ExtractTimecodeValues(timecode,
        out int hour, out int minute, out int second, out int milliesecond);

      _time.AddHours(hour);
      _time.AddMinutes(minute);
      _time.AddSeconds(second);
      _time.AddMilliseconds(milliesecond);
      Framerate = framerate;
    }

    private void ConstructUsingSMPTEString(string timecode, Framerate framerate)
    {
      ExtractTimecodeValues(timecode,
        out int hour, out int minute, out int second, out int frame);

      _time.AddHours(hour);
      _time.AddMinutes(minute);
      _time.AddSeconds(second);
      long framesAsMillieseconds = CalcMilliesecondsFromFrame(frame, framerate);
      _time.AddMilliseconds(framesAsMillieseconds);
      Framerate = framerate;
    }

    #endregion Constructors

    #region Public Methods

    /// <inheritdoc/>
    public void AddHours(int hoursToAdd) => _time.AddHours(hoursToAdd);

    /// <inheritdoc/>
    public void AddMinutes(int minutesToAdd) => _time.AddMinutes(minutesToAdd);

    /// <inheritdoc/>
    public void AddSeconds(int secondsToAdd) => _time.AddSeconds(secondsToAdd);

    /// <inheritdoc/>
    public void AddMilliseconds(int milliesecondsToAdd) =>
      _time.AddMilliseconds(milliesecondsToAdd);

    /// <inheritdoc/>
    public void AddFrames(int framesToAdd)
    {
      long milliesecondsToAdd = CalcMilliesecondsFromFrame(framesToAdd, Framerate);
      _time.AddMilliseconds(framesToAdd);
    }

    /// <summary>
    /// Checks whether or not a string is formatted as a SMPTE timecode.
    /// </summary>
    /// <param name="timecode"></param>
    /// <returns>True if the input argument follows the format
    /// HH:MM:SS:FF or HH:MM:SS;FF</returns>
    public static bool IsValidSMPTETimecode(string timecode)
    {
      string SMPTEPattern = @"^(([0-9]){2}:){2}(([0-9]){2})(;|:|,)([0-9]){2}$";
      Regex tcRegex = new Regex(SMPTEPattern);
      return tcRegex.IsMatch(timecode);
    }

    /// <summary>
    /// Checks whether or not a string is formatted as a subtitle timecode.
    /// </summary>
    /// <param name="timecode"></param>
    /// <returns>True if the input argument follows the format
    /// HH:MM:SS,XXX where XXX represents millieseconds </returns>
    public bool IsValidSubtitleTimecode(string timecode)
    {
      string subtitleTimecodePattern = @"^{0,1}(([0-9]){2}:){2}(([0-9]){2})(,)([0-9]){3}$";
      Regex tcRegex = new Regex(subtitleTimecodePattern);
      return tcRegex.IsMatch(timecode);
    }

    private static void ThrowInvalidTimecodeException(string timecode)
      => throw new ArgumentException("Invalid timecode format.", nameof(timecode));

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
      char frameDelimiter = FramerateValues.GetLastDelimiter(Framerate);

      return $"{AddZeroPadding(Hour)}" +
        $":{AddZeroPadding(Minute)}" +
        $":{AddZeroPadding(Second)}" +
        $"{frameDelimiter}{AddZeroPadding(Frame)}";
    }

    /// <summary>
    /// Returns the timecode as a string formatted as a timecode.
    /// </summary>
    /// <returns>The timecode formatted as a timecode</returns>
    public string ToString(TimecodeFormatOption option)
    {
      char frameDelimiter = FramerateValues.GetLastDelimiter(option);

      return $"{AddZeroPadding(Hour)}" +
        $":{AddZeroPadding(Minute)}" +
        $":{AddZeroPadding(Second)}" +
        $"{frameDelimiter}{AddZeroPadding(Frame)}";
    }

    /// <summary>
    /// Returns the timecode as a string formatted as a subtitle timecode,
    /// where the last three digits are converted from frames to millieseconds.
    /// </summary>
    /// <returns>The timecode formatted as a subtitle timecode.<br/><br/>
    /// Ex: 10:00:00,000</returns>
    public string ToSubtitleString()
    {
      return $"{AddZeroPadding(Hour)}" +
        $":{AddZeroPadding(Minute)}" +
        $":{AddZeroPadding(Second)}" +
        $",{AddZeroPadding(Milliesecond)}";
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
    /// <returns>A timecode string with updated hours.</returns>
    /// <exception cref="ArgumentException">Throws if the timecode
    /// argument format is invalid.</exception>
    public static string AddHours(string timecode, Framerate framerate, int hoursToAdd)
    {
      if (!IsValidSMPTETimecode(timecode)) ThrowInvalidTimecodeException(timecode);

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
    /// <returns>A timecode string with updated minutes.</returns>
    /// <exception cref="ArgumentException">Throws if the timecode
    /// argument format is invalid.</exception>
    public static string AddMinutes(string timecode, Framerate framerate, int minutesToAdd)
    {
      if (!IsValidSMPTETimecode(timecode)) ThrowInvalidTimecodeException(timecode);

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
    /// <returns>A timecode string with updated seconds.</returns>
    /// <exception cref="ArgumentException">Throws if the timecode argument
    /// format is invalid.</exception>
    public static string AddSeconds(string timecode, Framerate framerate, int secondsToAdd)
    {
      if (!IsValidSMPTETimecode(timecode)) ThrowInvalidTimecodeException(timecode);

      Timecode timecodeObj = new Timecode(timecode, framerate);
      timecodeObj.AddSeconds(secondsToAdd);
      return timecodeObj.ToString();
    }

    /// <summary>
    /// Adds seconds to the given time.
    /// <br/><br/>
    /// Positive integer values add frames,
    /// while negative values subtract frames.
    /// </summary>
    /// <param name="inputString">Timecode to update, formatted as a timecode.</param>
    /// <param name="frames">Number of frames to add or subtract.</param>
    /// <param name="framerate"></param>
    /// <returns>A timecode with updated frames.</returns>
    /// <exception cref="ArgumentException">Throws if the timecode argument
    /// format is invalid.</exception>
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
      if (!IsValidSMPTETimecode(originalTimecode)) ThrowInvalidTimecodeException(originalTimecode);

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
    /// <param name="rightTimecode">The timecode that will be added to the
    /// original.</param>
    /// <returns>The sum of the left and right timecode.</returns>
    /// <exception cref="InvalidOperationException">Is Thrown when Framerates are
    /// not equal.</exception>
    public static Timecode operator +(Timecode leftTimecode, Timecode rightTimecode)
    {
      if (leftTimecode.Framerate != rightTimecode.Framerate)
      {
        throw new InvalidOperationException(
          "Arithmetic operations between different framerates are invalid.");
      }

      return new Timecode(
        new TimeSpan(leftTimecode.Ticks + rightTimecode.Ticks), leftTimecode.Framerate);
    }

    /// <summary>
    /// Subtraction of two Timecodes.
    /// </summary>
    /// <param name="leftTimecode">The timeCode to subtract from.</param>
    /// <param name="rightTimecode">The timeCode that will be subtracted.</param>
    /// <returns>The difference between timeCode left and right.</returns>
    public static Timecode operator -(Timecode leftTimecode, Timecode rightTimecode)
    {
      return new Timecode(
        new TimeSpan(leftTimecode.Ticks - rightTimecode.Ticks), leftTimecode.Framerate);
    }

    /// <summary>
    /// Determines whether a timecode is smaller than another timecode.
    /// </summary>
    /// <param name="leftTimecode">The timecode of which needs to be determined if its smaller.</param>
    /// <param name="rightTimecode">The timecode which the other will be compared to.</param>
    /// <returns>True if the timecode is smaller than the compared timecode.</returns>
    public static bool operator <(Timecode leftTimecode, Timecode rightTimecode)
    {
      return leftTimecode.Ticks < rightTimecode.Ticks;
    }

    /// <summary>
    /// Determines whether a timecode is larger than another timecode.
    /// </summary>
    /// <param name="leftTimecode">The timecode of which needs to be determined if its larger.</param>
    /// <param name="rightTimecode">The timecode which the other will be compared to.</param>
    /// <returns>whether a timecode is larger than another timecode</returns>
    public static bool operator >(Timecode leftTimecode, Timecode rightTimecode)
    {
      return leftTimecode.Ticks > rightTimecode.Ticks;
    }

    /// <summary>
    /// Determines whether a timecode is smaller or equal to another timecode.
    /// </summary>
    /// <param name="leftTimecode">The timecode of which needs to be determined if its smaller.</param>
    /// <param name="rightTimecode">The timecode which the other will be compared to.</param>
    /// <returns>True if the timecode is smaller than the compared timecode.</returns>
    public static bool operator <=(Timecode leftTimecode, Timecode rightTimecode)
    {
      return leftTimecode.Ticks <= rightTimecode.Ticks;
    }

    /// <summary>
    /// Determines whether a timecode is larger or equal to another timecode.
    /// </summary>
    /// <param name="leftTimecode">The timecode of which needs to be determined if its larger.</param>
    /// <param name="rightTimecode">The timecode which the other will be compared to.</param>
    /// <returns>whether a timecode is larger than another timecode</returns>
    public static bool operator >=(Timecode leftTimecode, Timecode rightTimecode)
    {
      return leftTimecode.Ticks >= rightTimecode.Ticks;
    }

    #endregion Operator Overloads

    #region Private Methods

    private int CalcFrameCount()
    {
      //TimeSpan timecodeDuration = _time - DateTime.MinValue;
      //double timecodeDurationInMilleseconds = timecodeDuration.TotalMilliseconds;
      //decimal frameRateDecimalVal = FramerateValues.FramerateAsDecimals[Framerate];
      //int frameCount = Convert.ToInt32(
      //  Convert.ToDecimal(timecodeDurationInMilleseconds) * frameRateDecimalVal / 1000);
      //return frameCount;

      decimal frameRateDecimalVal = FramerateValues.FramerateAsDecimals[Framerate];

      int dropFrames = Convert.ToInt32(Math.Round(frameRateDecimalVal * 0.066666m));
      int timeBase = Convert.ToInt32(Math.Round(frameRateDecimalVal));

      int hourFrames = timeBase * 60 * 60;
      int minuteFrames = timeBase * 60;
      int totalMinutes = (60 * Hour) + minutes;
      int frameNumber = ((hourFrames * hours) + (minuteFrames * minutes) + (timeBase * seconds) + frames) - (dropFrames * (totalMinutes - (totalMinutes \ 10)));
      return frameNumber;
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
    ///
    /// </summary>
    /// <returns></returns>
    private int GetFramePosition()
    {
      bool isDropFrame = FramerateValues.IsDropFrame(Framerate);
      // TODO: Drop frame logic

      decimal framerateDecVal = FramerateValues.FramerateAsDecimals[Framerate];
      int frame = Convert.ToInt32(Math.Round(framerateDecVal * Milliesecond * 1000));
      return frame;
    }

    /// <summary>
    /// Calculates the number of millieseconds there are in a given frame.
    /// </summary>
    /// <param name="frame">The frame position of a timecode.</param>
    /// <param name="framerate">The timecode framerate.</param>
    /// <returns>A frame value converted to millieseconds.</returns>
    private int CalcMilliesecondsFromFrame(int frame, Framerate framerate)
    {
      // TODO: Revise this as it should take Drop Frames into consideration
      decimal framerateAsDec = FramerateValues.FramerateAsDecimals[framerate];
      int millieseconds = Convert.ToInt32(Math.Round((frame / framerateAsDec) * 1000));
      return millieseconds;
    }

    /// <summary>
    /// Extract timecode values from a timecode represented as a string.
    /// </summary>
    /// <param name="timecode">A string formatted as a timecode.</param>
    /// <param name="hour">The timecode hour.</param>
    /// <param name="minute">The timecode minute.</param>
    /// <param name="second">The timecode second.</param>
    /// <param name="fraction">The timecode fraction.</param>
    private static void ExtractTimecodeValues(string timecode,
      out int hour, out int minute, out int second, out int fraction)
    {
      timecode = timecode.Replace(';', ':');
      timecode = timecode.Replace(',', ':');
      string[] timecodeSplit = timecode.Split(":");
      hour = Convert.ToInt32(timecodeSplit[0]);
      minute = Convert.ToInt32(timecodeSplit[1]);
      second = Convert.ToInt32(timecodeSplit[2]);
      fraction = Convert.ToInt32(timecodeSplit[3]);
    }

    #endregion Private Methods
  }
}