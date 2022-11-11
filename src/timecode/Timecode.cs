using System.Text.RegularExpressions;

using DotnetTimecode.Enums;
using DotnetTimecode.Helpers;

namespace DotnetTimecode
{
  /// <summary>
  /// Represents an SMPTE Timecode. Supports the non drop frame formats HH:MM:FF:SS and -HH:MM:FF:SS,
  /// <br/> as well as the drop frame formats HH:MM:SS;FF and -HH:MM:SS;FF.
  /// </summary>
  public class Timecode
  {
    #region Public Properties

    /// <summary>
    /// Regular expression pattern of a timecode following the format. 
    /// Supports the format "HH:MM:SS:FF", "HH:MM:SS;FF".
    /// </summary>
    public const string TimecodeRegexPattern = @"^(-){0,1}(([0-9]){2}:){2}(([0-9]){2})(;|:)([0-9]){2}$";

    /// <summary>
    /// Regular expression pattern of a timecode following the format. 
    /// Supports the format "HH:MM:SS:XXX", where XXX represents millieseconds.
    /// </summary>
    public const string SrtTimecodeRegexPattern = @"^{0,1}(([0-9]){2}:){2}(([0-9]){2})(,)([0-9]){3}$";

    /// <summary> 
    /// The timecode hour position.
    /// </summary> 
    public int Hour { get; private set; } = 0;
    /// <summary> 
    /// The timecode minute position.
    /// </summary> 
    public int Minute { get; private set; } = 0;
    /// <summary> 
    /// The timecode second position.
    /// </summary> 
    public int Second { get; private set; } = 0;
    /// <summary> 
    /// The timecode frame position.
    /// </summary> 
    public int Frame { get; private set; } = 0;
    /// <summary> 
    /// The total amount of frames, where 0 frames represent the timecode 00:00:00:00.
    /// </summary> 
    public int TotalFrames { get; private set; } = 0;
    /// <summary>
    /// The timecode framerate, represented as an enum.
    /// </summary>
    public Framerate Framerate { get; private set; } = 0;

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
    /// Creates a new Timecode object at a specified timecode position. 
    /// </summary> 
    /// <param name="totalFrames">The timecodes total amount of frames.</param> 
    /// <param name="framerate">The timecode framerate.</param> 
    public Timecode(int totalFrames, Framerate framerate)
    {
      TotalFrames = totalFrames;
      Framerate = framerate;
      UpdateTimecodeHoursMinutesSecondsFrames();
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
      Frame = frame;
      Second = second;
      Minute = minute;
      Hour = hour;

      UpdateTimecodeTotalFrames();

      if (HHMMSSFFNeedUpdating()) UpdateTimecodeHoursMinutesSecondsFrames();
    }

    /// <summary> 
    /// Creates a new Timecode object at a specified timecode position. 
    /// </summary> 
    /// <param name="timecode">The timecode represented as a string formatted "HH:MM:SS:FF" or "HH:MM:SS;FF".</param>
    /// <param name="framerate">The timecode framerate.</param> 
    public Timecode(string timecode, Framerate framerate)
    {
      ValidateTimecodeString(timecode);

      string[] timecodeSplit = SplitTimecode(timecode); 

      Framerate = framerate;
      Hour = Convert.ToInt32(timecodeSplit[0]);
      Minute = Convert.ToInt32(timecodeSplit[1]);
      Second = Convert.ToInt32(timecodeSplit[2]);
      Frame = Convert.ToInt32(timecodeSplit[3]);

      UpdateTimecodeTotalFrames();

      if (HHMMSSFFNeedUpdating()) UpdateTimecodeHoursMinutesSecondsFrames();
    }

    /// <summary>
    /// Check if the minute, second, and frame property values are invalid for the target timecode.
    /// </summary>
    /// <returns>True if any of the property values are set to a value which is invalid for the target 
    /// timecode.</returns>
    private bool HHMMSSFFNeedUpdating()
    {
      return Minute >= 60 || Second >= 60 || Frame >= FramerateValues.FramerateAsDecimals[Framerate];
    }

    /// <summary>
    /// Splits a timecode into an array of strings, where each string represents 
    /// a time value such as hour, minute, second, and frame positions.
    /// </summary>
    /// <param name="timecode">A string formatted as a timecode.</param>
    /// <returns>Returns an array of substrings.</returns>
    private static string[] SplitTimecode(string timecode)
    {
      timecode = timecode.Replace(';', ':');
      return timecode.Split(":");
    }

    /// <summary>
    /// Splits an SRT timecode into an array of strings, where each string represents 
    /// a time value such as hour, minute, second, and milliesecond positions.
    /// </summary>
    /// <param name="timecode">A string formatted as a timecode.</param>
    /// <returns>Returns an array of substrings.</returns>
    private static string[] SplitSrtTimecode(string timecode)
    {
      timecode = timecode.Replace(',', ':');
      return timecode.Split(":");
    }

    #endregion Constructors

    #region Public Methods

    /// <summary>
    /// Returns the timecode as a string formatted "HH:MM:SS:FF" and "HH:MM:SS;FF". 
    /// </summary>
    /// <returns>The timecode formatted "HH:MM:SS:FF" and "HH:MM:SS;FF"</returns>
    public override string ToString()
    {
      // Drop frame framerates are formatted HH:MM:SS;FF
      char lastColon = FramerateValues.IsNonDropFrame(Framerate) ? ':' : ';';
      bool isNegativeTimecode = TotalFrames < 0;
      string firstChar = isNegativeTimecode ? "-" : "";

      return $"{firstChar}{AddZeroPadding(Hour)}" +
        $":{AddZeroPadding(Minute)}" +
        $":{AddZeroPadding(Second)}" +
        $"{lastColon}{AddZeroPadding(Frame)}";
    }

    /// <summary> 
    /// Adds hours to the timecode. 
    /// <br/><br/> 
    /// Positive integer values add hours, 
    /// while negative values subtract hours. 
    /// </summary> 
    /// <param name="hoursToAdd">The number of hours to add or remove.</param>
    public void AddHours(int hoursToAdd)
    {
      Hour += hoursToAdd;
      UpdateTimecodeTotalFrames();
    }

    /// <summary> 
    /// Adds minutes to the timecode. 
    /// <br/><br/> 
    /// Positive integer values add minutes,  
    /// while negative values subtract minutes. 
    /// </summary> 
    /// <param name="minutesToAdd">The number of minutes to add to the timecode.</param>
    public void AddMinutes(int minutesToAdd)
    {
      int hoursToAdd = minutesToAdd / 60;
      minutesToAdd = minutesToAdd % 60;
      int totalMinutes = Minute + minutesToAdd;
      int minutesInAnHour = 60;

      if (totalMinutes < 0)
      {
        hoursToAdd--;
        minutesToAdd = minutesToAdd + minutesInAnHour;
      }
      else if (totalMinutes >= minutesInAnHour)
      {
        hoursToAdd++;
        minutesToAdd = minutesToAdd - minutesInAnHour;
      }

      Minute += minutesToAdd;
      Hour += hoursToAdd;

      UpdateTimecodeTotalFrames();
    }

    /// <summary> 
    /// Adds seconds to the timecode.
    /// <br/><br/> 
    /// Positive integer values add seconds,  
    /// while negative values subtract seconds. 
    /// </summary> 
    /// <param name="secondsToAdd">The number of seconds to add to the timecode.</param> 
    public void AddSeconds(int secondsToAdd)
    {
      int hoursToBeAdded = secondsToAdd / 60 / 60;
      int secondsRemainingAfterHoursRemoved = secondsToAdd - (hoursToBeAdded * 60 * 60);
      int minutesToBeAdded = secondsRemainingAfterHoursRemoved / 60;
      int secondsToBeAdded = secondsRemainingAfterHoursRemoved % 60;
      int totalSeconds = Second + secondsToBeAdded;
      if (totalSeconds < 0)
      {
        minutesToBeAdded--;
        secondsToBeAdded = 60 + secondsToBeAdded;
      }
      else if (totalSeconds >= 60)
      {
        minutesToBeAdded++;
        secondsToBeAdded = secondsToBeAdded - 60;
      }
      int totalMinutes = Minute + minutesToBeAdded;
      if (totalMinutes < 0)
      {
        hoursToBeAdded--;
        minutesToBeAdded = minutesToBeAdded + 60;
      }
      else if (totalMinutes >= 60)
      {
        hoursToBeAdded++;
        minutesToBeAdded = minutesToBeAdded - 60;
      }

      Second += secondsToBeAdded;
      Minute += minutesToBeAdded;
      Hour += hoursToBeAdded;

      UpdateTimecodeTotalFrames();
    }

    /// <summary> 
    /// Adds frames to the timecode. 
    /// <br/><br/> 
    /// Positive integer values add frames,  
    /// while negative values remove frames. 
    /// </summary> 
    /// <param name="framesToAdd">The number of frames to add to the timecode.</param> 
    public void AddFrames(int framesToAdd)
    {
      TotalFrames += framesToAdd;
      UpdateTimecodeHoursMinutesSecondsFrames();
    }

    /// <summary> 
    /// Converts the timecode object to the target framerate. 
    /// </summary> 
    /// <param name="destinationFramerate">The target framerate to convert to.</param> 
    public void ConvertFramerate(Framerate destinationFramerate)
    {
      Framerate = destinationFramerate;
      UpdateTimecodeHoursMinutesSecondsFrames();
    }

    #endregion Public Methods

    #region Public Static Methods

    /// <summary>
    /// Adds hours to the given time. 
    /// <br/><br/>
    /// Positive integer values add hours, 
    /// while negative values subtract hours.
    /// </summary>
    /// <param name="timecode">Timecode to update, formatted "HH:MM:SS:FF" and "HH:MM:SS;FF".</param>
    /// <param name="framerate">The timecode framerate.</param>
    /// <param name="hoursToAdd">Number of hours to add or subtract.</param>
    public static string AddHours(string timecode, Framerate framerate, int hoursToAdd)
    {
      ValidateTimecodeString(timecode);
      Timecode timecodeObj = new Timecode(timecode, framerate);
      timecodeObj.AddHours(hoursToAdd);
      return timecodeObj.ToString();
    }

    /// <summary>
    /// Adds minutes to the given time. 
    /// <br/><br/>
    /// Positive integer values add minutes, 
    /// while negative values subtract minutes.
    /// </summary>
    /// <param name="timecode">Timecode to update, formatted "HH:MM:SS:FF" and "HH:MM:SS;FF".</param>
    /// <param name="framerate">The timecode framerate.</param>
    /// <param name="minutesToAdd">Number of minutes to add or subtract.</param>
    public static string AddMinutes(string timecode, Framerate framerate, int minutesToAdd)
    {
      ValidateTimecodeString(timecode);
      Timecode timecodeObj = new Timecode(timecode, framerate);
      timecodeObj.AddMinutes(minutesToAdd);
      return timecodeObj.ToString();
    }

    /// <summary>
    /// Adds seconds to the given time. 
    /// <br/><br/>
    /// Positive integer values add seconds, 
    /// while negative values subtract seconds.
    /// </summary>
    /// <param name="timecode">Timecode to update, formatted "HH:MM:SS:FF" and "HH:MM:SS;FF".</param>
    /// <param name="secondsToAdd">Number of seconds to add or subtract.</param>
    public static string AddSeconds(string timecode, Framerate framerate, int secondsToAdd)
    {
      ValidateTimecodeString(timecode);
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
    /// <param name="inputString">Timecode to update, formatted "HH:MM:SS:FF" and "HH:MM:SS;FF".</param>
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
    /// <param name="originalTimecode">The original timecode, formatted "HH:MM:SS:FF" and "HH:MM:SS;FF".</param>
    /// <param name="originalFramerate">The original timecode framerate.</param>
    /// <param name="destinationFramerate">The target framerate to convert to.</param>
    /// <returns>A string formatted as a timecode.</returns>
    /// <exception cref="ArgumentException">Throws if the input timecode format is invalid.</exception>
    public static string ConvertFramerate(
      string originalTimecode, Framerate originalFramerate, Framerate destinationFramerate)
    {
      // Validate the original timecode format
      ValidateTimecodeString(originalTimecode);

      Timecode timecode = new Timecode(originalTimecode, originalFramerate);
      timecode.ConvertFramerate(destinationFramerate);
      return timecode.ToString();
    }

    /// <summary>
    /// Converts a timecode string formatted HH:MM:SS:FF to an srt timecode string formatted HH:MM:SS:XXX,
    /// where XXX represents millieseconds.
    /// </summary>
    /// <param name="timecode">The original timecode to convert from.</param>
    /// <param name="framerate">The original timecode framerate.</param>
    /// <returns>An srt timecode string.</returns>
    public static string ConvertTimecodeToSrtTimecode(string timecode, Framerate framerate)
    {
      ValidateTimecodeString(timecode);

      string[] timecodeSplit = SplitTimecode(timecode); 

      int hour = Convert.ToInt32(timecodeSplit[0]);
      int minute = Convert.ToInt32(timecodeSplit[1]);
      int second = Convert.ToInt32(timecodeSplit[2]);
      int frame = Convert.ToInt32(timecodeSplit[3]);
      decimal framerateDecimalValue = FramerateValues.FramerateAsDecimals[framerate];

      decimal milliesecond = (frame / framerateDecimalValue) * 1000;
      decimal milliesecondRounded = Math.Round(milliesecond, 0, MidpointRounding.AwayFromZero);
      string milliesecondRoundedAsStr = milliesecondRounded.ToString();

      char milliesecondFirstDigit = milliesecondRoundedAsStr[0];
      char milliesecondSecondDigit = milliesecondRoundedAsStr.Length >= 2 ? milliesecondRoundedAsStr[1] : '0';
      char milliesecondThirdDigit = milliesecondRoundedAsStr.Length >= 3 ? milliesecondRoundedAsStr[2] : '0';

      var milliesecondThreeDigits = $"{milliesecondFirstDigit}{milliesecondSecondDigit}{milliesecondThirdDigit}";

      return $"{AddZeroPadding(hour)}:{AddZeroPadding(minute)}:{AddZeroPadding(second)},{milliesecondThreeDigits}";
    }

    /// <summary>
    /// Converts an srt timecode string formatted HH:MM:SS:XXX,
    /// where XXX represents millieseconds, to a timecode string formatted HH:MM:SS:FF (NDF) or HH:MM:SS;FF (DF).
    /// </summary>
    /// <param name="srtTimecode">The original srt timecode to convert from.</param>
    /// <param name="framerate">The target framerate.</param>
    /// <returns></returns>
    public static string ConvertSrtTimecodeToTimecode(string srtTimecode, Framerate framerate)
    {
      ValidateSrtTimecodeString(srtTimecode);

      string[] timecodeSplit = SplitSrtTimecode(srtTimecode);

      int hour = Convert.ToInt32(timecodeSplit[0]);
      int minute = Convert.ToInt32(timecodeSplit[1]);
      int second = Convert.ToInt32(timecodeSplit[2]);
      int millieseconds = Convert.ToInt32(timecodeSplit[3]);

      decimal framerateDecimalValue = FramerateValues.FramerateAsDecimals[framerate];

      decimal frameAsDec = (millieseconds * framerateDecimalValue) / 1000;

      decimal frameRounded = Math.Round(frameAsDec, 0, MidpointRounding.AwayFromZero);
      int frame = (int)frameRounded;
      char lastDelimiter = FramerateValues.GetLastDelimiter(framerate);

      return $"{AddZeroPadding(hour)}:{AddZeroPadding(minute)}:{AddZeroPadding(second)}{lastDelimiter}{AddZeroPadding(frame)}";
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
      return new Timecode(leftTimecode.TotalFrames + rightTimecode.TotalFrames, leftTimecode.Framerate);
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
      return new Timecode(leftTimecode.TotalFrames - rightTimecode.TotalFrames, leftTimecode.Framerate);
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
      return leftTimecode.TotalFrames < rightTimecode.TotalFrames;
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
      return leftTimecode.TotalFrames > rightTimecode.TotalFrames;
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
      return leftTimecode.TotalFrames <= rightTimecode.TotalFrames;
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
      return leftTimecode.TotalFrames >= rightTimecode.TotalFrames;
    }

    #endregion Operator Overloads

    #region Private Methods

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
    /// Calculates and sets the TotalFrames property based on Hour,  
    /// Minute, Second, Frame and Framerate properties. 
    /// </summary> 
    private void UpdateTimecodeTotalFrames()
    {
      if (FramerateValues.IsNonDropFrame(Framerate))
      {
        SetTotalFramesNDF(Hour, Minute, Second, Frame, Framerate);
      }
      else
      {
        SetTotalFramesDF(Hour, Minute, Second, Frame, Framerate);
      }
    }

    /// <summary> 
    /// Calculates and sets the Hour, Minute, Second, Frame based  
    /// on the TotalFrames and Framerate properties. 
    /// </summary>
    private void UpdateTimecodeHoursMinutesSecondsFrames()
    {
      if (FramerateValues.IsNonDropFrame(Framerate))
      {
        SetHourMinuteSecondFrameNDF(TotalFrames, Framerate);
      }
      else
      {
        SetHourMinuteSecondFrameDF(TotalFrames, Framerate);
      }
    }

    /// <summary>
    /// Sets the property values for Hour, Minute, Second and Frame based on the TotalFrames property. Non Drop Frame.
    /// </summary>
    /// <param name="totalFrames">The TotalFrames property.</param>
    /// <param name="framerate">The Framerate property.</param>
    private void SetHourMinuteSecondFrameNDF(int totalFrames, Framerate framerate)
    {
      decimal framerateDecimalValue = FramerateValues.FramerateAsDecimals[framerate];

      int timeBase = Convert.ToInt32(Math.Round(framerateDecimalValue));

      int remainingFrames = totalFrames;
      int secondsPerMinute = 60;
      int minutesPerHour = 60;

      int framesPerHour = timeBase * minutesPerHour * secondsPerMinute;
      int framesPerMinute = timeBase * secondsPerMinute;

      Hour = remainingFrames / framesPerHour;
      remainingFrames = remainingFrames - (Hour * framesPerHour);

      Minute = remainingFrames / framesPerMinute;
      remainingFrames = remainingFrames - (Minute * framesPerMinute);

      Second = remainingFrames / timeBase;
      Frame = remainingFrames - (Second * timeBase);
    }

    /// <summary>
    /// Sets the property values for Hour, Minute, Second and Frame based on the TotalFrames property. Drop Frame.
    /// </summary>
    /// <param name="totalFrames">The TotalFrames property.</param>
    /// <param name="framerate">The Framerate property.</param>
    private void SetHourMinuteSecondFrameDF(int totalFrames, Framerate framerate)
    {
      decimal framerateDecimalValue = FramerateValues.FramerateAsDecimals[framerate];
      int secondsPerMinute = 60;
      int minutesPerHour = 60;
      int dropFramesVal = Convert.ToInt32(Math.Round(framerateDecimalValue * .066666m));
      int framesPerMinute = Convert.ToInt32((Math.Round(framerateDecimalValue) * secondsPerMinute) - dropFramesVal);
      int framesPer10Minutes = Convert.ToInt32(Math.Round(framerateDecimalValue * secondsPerMinute * 10));
      int framesPerHour = Convert.ToInt32(Math.Round(framerateDecimalValue * secondsPerMinute * minutesPerHour));
      int framesPer24Hours = framesPerHour * 24;
      totalFrames = totalFrames % framesPer24Hours;
      int div = totalFrames / framesPer10Minutes;
      int mod = totalFrames % framesPer10Minutes;
      if (dropFramesVal < mod)
      {
        totalFrames = totalFrames + (dropFramesVal * 9 * div) + dropFramesVal * ((mod - dropFramesVal) / framesPerMinute);
      }
      else
      {
        totalFrames = totalFrames + dropFramesVal * 9 * div;
      }
      int framerateRounded = Convert.ToInt32((Math.Round(framerateDecimalValue)));
      Frame = totalFrames % framerateRounded;
      Second = (totalFrames / framerateRounded) % secondsPerMinute;
      Minute = ((totalFrames / framerateRounded) / secondsPerMinute) % minutesPerHour;
      Hour = (((totalFrames / framerateRounded) / secondsPerMinute) / minutesPerHour);
    }

    /// <summary>
    /// Sets the property value for TotalFrames property. Non Drop Frame.
    /// </summary>
    /// <param name="hours">The timecode hour.</param>
    /// <param name="minutes">The timecode minute.</param>
    /// <param name="seconds">The timecode second.</param>
    /// <param name="frames">The timecode frame.</param>
    /// <param name="framerate">The timecode framerate.</param>
    private void SetTotalFramesNDF(int hours, int minutes, int seconds, int frames, Framerate framerate)
    {
      TotalFrames = CalcTotalFramesNDF(hours, minutes, seconds, frames, framerate);
    }

    /// <summary>
    /// Sets the property value for TotalFrames property. Drop Frame.
    /// </summary>
    /// <param name="hours">The timecode hour.</param>
    /// <param name="minutes">The timecode minute.</param>
    /// <param name="seconds">The timecode second.</param>
    /// <param name="frames">The timecode frame.</param>
    /// <param name="framerate">The timecode framerate.</param>
    private void SetTotalFramesDF(int hours, int minutes, int seconds, int frames, Framerate framerate)
    {
      TotalFrames = CalcTotalFramesDF(hours, minutes, seconds, frames, framerate);
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

      int dropFrames = Convert.ToInt32((Math.Round(framerateDecimalValue * 0.066666m)));
      int timeBase = Convert.ToInt32(Math.Round(framerateDecimalValue));
      int secondsPerMinute = 60;
      int minutesPerHour = 60;
      int hourFrames = timeBase * secondsPerMinute * minutesPerHour;
      int minuteFrames = timeBase * secondsPerMinute;
      int totalMinutes = (minutesPerHour * hours) + minutes;
      int totalFrames = ((hourFrames * hours) + (minuteFrames * minutes) +
      (timeBase * seconds) + frames) - (dropFrames * (totalMinutes - (totalMinutes / 10)));
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
      int totalFrames = (hourFrames * hours) + (minuteFrames * minutes) + (timeBase * seconds) + frames;
      return totalFrames;
    }


    /// <summary>
    /// Validates the format of a string representing a timecode formatted "HH:MM:SS:FF" and "HH:MM:SS;FF".<br/>
    /// Throws an exception if the timecode string is invalid.
    /// </summary>
    private static void ValidateTimecodeString(string timecode)
    {
      Regex tcRegex = new Regex(TimecodeRegexPattern);
      if (!tcRegex.IsMatch(timecode))
        throw new ArgumentException("Invalid timecode format.", nameof(timecode));
    }

    /// <summary>
    /// Validates the format of a string representing an srt timecode formatted "HH:MM:SS:XXX".<br/>
    /// Throws an exception if the timecode string is invalid.
    /// </summary>
    private static void ValidateSrtTimecodeString(string timecode)
    {
      Regex tcRegex = new Regex(SrtTimecodeRegexPattern);
      if (!tcRegex.IsMatch(timecode))
        throw new ArgumentException("Invalid timecode format.", nameof(timecode));
    }

    #endregion Private Methods
  }
}
