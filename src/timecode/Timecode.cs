using System.Text.RegularExpressions;

using DotnetTimecode.Enums;
using DotnetTimecode.Helpers;

namespace DotnetTimecode
{
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
  public class Timecode
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
  {
    /// <summary>
    /// Regular expression pattern for a timecode. 
    /// Supports the format hh:mm:ss:ff.
    /// </summary>
    public static readonly string RegexPattern = @"(-){0,1}(([0-9]){2}:){3}([0-9]){2}";

    /// <summary>
    /// The timecode hour position, based on the framerate and total frames.
    /// </summary>
    public int Hour { get; private set; }
    /// <summary>
    /// The timecode hour position, based on the framerate and total frames.
    /// </summary>
    public int Minute { get; private set; }
    /// <summary>
    /// The timecode hour position, based on the framerate and total frames.
    /// </summary>
    public int Second { get; private set; }
    /// <summary>
    /// The timecode frame position, based on the framerate and total frames.
    /// </summary>
    public int Frame { get; private set; }

    /// <summary>
    /// The timecodes total amount of frames.
    /// </summary>
    public int TotalFrames { get; private set; }

    /// <summary>
    /// The timecode framerate.
    /// </summary>
    public Framerate Framerate { get; private set; }

    /// <summary>
    /// Returns the timecode as a string formatted hh:mm:ss:ff. 
    /// Does not support negative timelines.
    /// </summary>
    /// <returns>The timecode formatted hh:mm:ss:ff</returns>
    public override string ToString()
    {
      return $"{ZeroPadding(Hour)}:{ZeroPadding(Minute)}:{ZeroPadding(Second)}:{ZeroPadding(Frame)}";
    }

    /// <summary>
    /// Creates a new Timecode object at 00:00:00:00.
    /// </summary>
    /// <param name="framerate">The timecode framerate.</param>
    public Timecode(Framerate framerate)
    {
      TotalFrames = 0;
      Hour = 0;
      Minute = 0;
      Second = 0;
      Frame = 0;
      Framerate = framerate;
    }

    /// <summary>
    /// Creates a new Timecode object.
    /// </summary>
    /// <param name="totalFrames">The timecodes total amount of frames.</param>
    /// <param name="framerate">The timecode framerate.</param>
    public Timecode(int totalFrames, Framerate framerate)
    {
      TotalFrames = totalFrames;
      Framerate = framerate;
      UpdateHoursMinutesSecondsFrames();
    }

    /// <summary>
    /// Creates a new Timecode object.
    /// </summary>
    /// <param name="hour">The timecode hour position.</param>
    /// <param name="minute">The timecode minute position.</param>
    /// <param name="second">The timecode second position.</param>
    /// <param name="frame">The timecode frame position.</param>
    /// <param name="framerate">The timecode framerate.</param>
    public Timecode(int hour, int minute, int second, int frame, Framerate framerate)
    {
      Hour = hour;
      Minute = minute;
      Second = second;
      Frame = frame;
      Framerate = framerate;

      UpdateTotalFrames();
    }

    /// <summary>
    /// Creates a new Timecode object.
    /// </summary>
    /// <param name="timecode">The timecode represented as a string formatted hh:mm:ss:ff.</param>
    /// <param name="framerate">The timecode framerate.</param>
    public Timecode(string timecode, Framerate framerate)
    {
      Regex tcRegex = new Regex(RegexPattern);
      if (!tcRegex.IsMatch(timecode))
        throw new ArgumentException("Invalid timecode format.", nameof(timecode));
      var hhmmssff = timecode.Split(":");

      Hour = Convert.ToInt32(hhmmssff[0]);
      Minute = Convert.ToInt32(hhmmssff[1]);
      Second = Convert.ToInt32(hhmmssff[2]);
      Frame = Convert.ToInt32(hhmmssff[3]);
      Framerate = framerate;

      UpdateTotalFrames();
    }

    /// <summary>
    /// Adds hours to the timecode. 
    /// <br/><br/>
    /// Positive integer values add hours, 
    /// while negative values remove hours.
    /// </summary>
    /// <param name="hours">Number of hours to add or remove.</param>
    public void AddHours(int hours)
    {
      Hour += hours;
      UpdateTotalFrames();
    }


    /// <summary>
    /// Adds minutes to the timecode.
    /// <br/><br/>
    /// Positive integer values add minutes, 
    /// while negative values remove minutes.
    /// </summary>
    /// <param name="minutes">Number of minutes to add to the timecode.</param>
    /// <exception cref="NotImplementedException"></exception>
    public void AddMinutes(int minutes)
    {
      int hoursToAdd = minutes / 60;
      int minutesToAdd = minutes % 60;

      int totMin = Minute + minutesToAdd;

      if (totMin < 0)
      {
        hoursToAdd--;
        minutesToAdd = 60 + minutesToAdd;
      }
      else if (totMin >= 60)
      {
        hoursToAdd++;
        minutesToAdd = minutesToAdd - 60;
      }

      Hour += hoursToAdd;
      Minute += minutesToAdd;

      UpdateTotalFrames();
    }

    /// <summary>
    /// Adds seconds to the timecode. 
    /// <br/><br/>
    /// Positive integer values add seconds, 
    /// while negative values remove seconds.
    /// </summary>
    /// <param name="seconds">Number of seconds to add to the timecode.</param>
    public void AddSeconds(int seconds)
    {
      int hoursToAdd = seconds / 60 / 60;
      int secondsRemainingAfterHoursRemoved = seconds % (60 * 2);
      int minutesToAdd = secondsRemainingAfterHoursRemoved / 60;
      int secondsToAdd = secondsRemainingAfterHoursRemoved % 60;

      int totSec = Second + secondsToAdd;
      if (totSec < 0)
      {
        minutesToAdd--;
        secondsToAdd = 60 + secondsToAdd;
      }
      else if (totSec >= 60)
      {
        minutesToAdd++;
        secondsToAdd = secondsToAdd - 60;
      }

      int totMin = Minute + minutesToAdd;
      if (totMin < 0)
      {
        hoursToAdd--;
        minutesToAdd = 60 + minutesToAdd;
      }
      else if (totMin >= 60)
      {
        hoursToAdd++;
        minutesToAdd = minutesToAdd - 60;
      }

      Second += secondsToAdd;
      Minute += minutesToAdd;
      Hour += hoursToAdd;

      UpdateTotalFrames();
    }

    /// <summary>
    /// Adds frames to the timecode.
    /// <br/><br/>
    /// Positive integer values add frames, 
    /// while negative values remove frames.
    /// </summary>
    /// <param name="frames">Number of frames to add to the timecode.</param>
    public void AddFrames(int frames)
    {
      TotalFrames += frames;
      UpdateHoursMinutesSecondsFrames();
    }

    /// <summary>
    /// Converts the timecode object to the target framerate.
    /// </summary>
    /// <param name="targetFramerate">The target framerate.</param>
    public void ConvertFramerate(Framerate targetFramerate)
    {
      Framerate = targetFramerate;
      UpdateHoursMinutesSecondsFrames();
    }

    /// <summary>
    /// Addition of two Timecodes.
    /// </summary>
    /// <param name="left">The timecode to add to.</param>
    /// <param name="right">The timecode that will be added to the original.</param>
    /// <returns>The sum of the left and right timecode.</returns>
    /// <exception cref="InvalidOperationException">Is Thrown when FrameRates are not equal.</exception>
    public static Timecode operator +(Timecode left, Timecode right)
    {
      if (left.Framerate != right.Framerate)
      {
        throw new InvalidOperationException("It is not possible to calculate the addition between different framerates.");
      }
      return new Timecode(left.TotalFrames + right.TotalFrames, left.Framerate);
    }

    /// <summary>
    /// Subtraction of two Timecodes.
    /// </summary>
    /// <param name="left">The timeCode to subtract from.</param>
    /// <param name="right">The timeCode that will be subtracted.</param>
    /// <returns>The difference between timeCode left and right.</returns>
    /// <exception cref="InvalidOperationException">Is Thrown when FrameRates are not equal.</exception>
    public static Timecode operator -(Timecode left, Timecode right)
    {
      if (left.Framerate != right.Framerate)
      {
        throw new InvalidOperationException("It is not possible to calculate the difference between different framerates.");
      }
      return new Timecode(left.TotalFrames - right.TotalFrames, left.Framerate);
    }

    /// <summary>
    /// Determines whether a timecode is smaller than another timecode.
    /// </summary>
    /// <param name="left">The timecode of which needs to be determined if its smaller.</param>
    /// <param name="right">The timecode which the other will be compared to.</param>
    /// <returns>True if the timecode is smaller than the compared timecode.</returns>
    /// <exception cref="InvalidOperationException">Is Thrown when FrameRates are not equal.</exception>
    public static bool operator <(Timecode left, Timecode right)
    {
      if (left.Framerate != right.Framerate)
      {
        throw new InvalidOperationException("It is not possible to calculate the difference between different framerates. \n" +
          "Use the TotalFrames property for comparison operations between timecodes instead.");
      }
      return left.TotalFrames < right.TotalFrames;
    }

    /// <summary>
    /// Determines whether a timecode is larger than another timecode.
    /// </summary>
    /// <param name="left">The timecode of which needs to be determined if its larger.</param>
    /// <param name="right">The timecode which the other will be compared to.</param>
    /// <returns>whether a timecode is larger than another timecode</returns>
    /// <exception cref="InvalidOperationException">Is Thrown when FrameRates are not equal.</exception>
    public static bool operator >(Timecode left, Timecode right)
    {
      if (left.Framerate != right.Framerate)
      {
        throw new InvalidOperationException("It is not possible to calculate the difference between different framerates. \n" +
          "Use the TotalFrames property for comparison operations between timecodes instead.");
      }
      return left.TotalFrames > right.TotalFrames;
    }

    /// <summary>
    /// Determines whether a timecode is smaller or equal to another timecode.
    /// </summary>
    /// <param name="left">The timecode of which needs to be determined if its smaller.</param>
    /// <param name="right">The timecode which the other will be compared to.</param>
    /// <returns>True if the timecode is smaller than the compared timecode.</returns>
    /// <exception cref="InvalidOperationException">Is Thrown when FrameRates are not equal.</exception>
    public static bool operator <=(Timecode left, Timecode right)
    {
      if (left.Framerate != right.Framerate)
      {
        throw new InvalidOperationException("It is not possible to calculate the difference between different framerates. \n" +
          "Use the TotalFrames property for comparison operations between timecodes instead.");
      }
      return left.TotalFrames <= right.TotalFrames;
    }

    /// <summary>
    /// Determines whether a timecode is larger or equal to another timecode.
    /// </summary>
    /// <param name="left">The timecode of which needs to be determined if its larger.</param>
    /// <param name="right">The timecode which the other will be compared to.</param>
    /// <returns>whether a timecode is larger than another timecode</returns>
    /// <exception cref="InvalidOperationException">Is Thrown when FrameRates are not equal.</exception>
    public static bool operator >=(Timecode left, Timecode right)
    {
      if (left.Framerate != right.Framerate)
      {
        throw new InvalidOperationException("It is not possible to calculate the difference between different framerates. \n" +
          "Use the TotalFrames property for comparison operations between timecodes instead.");
      }
      return left.TotalFrames >= right.TotalFrames;
    }

    /// <summary>
    /// Check whether timecodes are equal.
    /// </summary>
    /// <param name="left">Left hand side timecode.</param>
    /// <param name="right">Right hand side timecode.</param>
    /// <returns>Whether timecodes are equal</returns>
    public static bool operator ==(Timecode left, Timecode right)
    {
      return left.TotalFrames == right.TotalFrames && left.Framerate == right.Framerate;
    }

    /// <summary>
    /// Check whether timecodes are not equal.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns>Whether timecodes are not equal</returns>
    public static bool operator !=(Timecode left, Timecode right)
    {
      return left.TotalFrames != right.TotalFrames && left.Framerate != right.Framerate;
    }

    /// <summary>
    /// Pads the first number position with a 0 if the number is less than two positions long.
    /// </summary>
    /// <returns>A string representation of a number value in the format of ex: "09".</returns>
    private string ZeroPadding(int num) => Math.Abs(num) < 10 ? $"0{num}" : num.ToString();

    /// <summary>
    /// Calculates and sets the TotalFrames property based on Hour, 
    /// Minute, Second, Frame and Framerate properties.
    /// </summary>
    private void UpdateTotalFrames()
    {
      if (Framerate == Framerate.fps29_97_DF || Framerate == Framerate.fps59_94_DF)
      {
        SetTotalFramesUsingDropFrames(Hour, Minute, Second, Frame);
      }
      else
      {
        SetTotalFrames(Hour, Minute, Second, Frame);
      }
    }

    /// <summary>
    /// Calculates and sets the Hour, Minute, Second, Frame based 
    /// on the TotalFrames and Framerate properties.
    /// </summary>
    private void UpdateHoursMinutesSecondsFrames()
    {
      if (Framerate == Framerate.fps29_97_DF || Framerate == Framerate.fps59_94_DF)
      {
        SetHHMMSSFFUsingDropFrames(TotalFrames);
      }
      else
      {
        SetHHMMSSFF(TotalFrames);
      }
    }

    private void SetHHMMSSFF(int totalFrames)
    {
      decimal framerate = FramerateValues.FramerateAsDecimals[Framerate];

      int timeBase = Convert.ToInt32(Math.Round(framerate));

      int remainingFrames = totalFrames;

      int hourFrames = timeBase * 60 * 60;
      int minuteFrames = timeBase * 60;

      Hour = remainingFrames / hourFrames;
      remainingFrames = remainingFrames - (Hour * hourFrames);

      Minute = remainingFrames / minuteFrames;
      remainingFrames = remainingFrames - (Minute * minuteFrames);

      Second = remainingFrames / timeBase;
      Frame = remainingFrames - (Second * timeBase);
    }

    private void SetHHMMSSFFUsingDropFrames(int totalFrames)
    {
      decimal framerate = FramerateValues.FramerateAsDecimals[Framerate];

      int div;
      int mod;

      int dropFrames = Convert.ToInt32(Math.Round(framerate * .066666m));
      int framesPerMinute = Convert.ToInt32((Math.Round(framerate) * 60) - dropFrames);
      int framesPer10Minutes = Convert.ToInt32(Math.Round(framerate * 60 * 10));
      int framesPerHour = Convert.ToInt32(Math.Round(framerate * 60 * 60));
      int framesPer24Hours = framesPerHour * 24;

      totalFrames = totalFrames % framesPer24Hours;

      div = totalFrames / framesPer10Minutes;
      mod = totalFrames % framesPer10Minutes;

      if (mod > dropFrames)
      {
        totalFrames = totalFrames + (dropFrames * 9 * div) + dropFrames * ((mod - dropFrames) / framesPerMinute);
      }
      else
      {
        totalFrames = totalFrames + dropFrames * 9 * div;
      }

      int framerateRounded = Convert.ToInt32((Math.Round(framerate)));
      Frame = totalFrames % framerateRounded;
      Second = (totalFrames / framerateRounded) % 60;
      Minute = ((totalFrames / framerateRounded) / 60) % 60;
      Hour = (((totalFrames / framerateRounded) / 60) / 60);
    }

    private void SetTotalFrames(int hours, int minutes, int seconds, int frames)
    {
      decimal framerate = FramerateValues.FramerateAsDecimals[Framerate];

      int timeBase = Convert.ToInt32(Math.Round(framerate));

      int hourFrames = timeBase * 60 * 60;
      int minuteFrames = timeBase * 60;

      TotalFrames = (hourFrames * hours) + (minuteFrames * minutes) + (timeBase * seconds) + frames;
    }

    private void SetTotalFramesUsingDropFrames(int hours, int minutes, int seconds, int frames)
    {
      decimal framerate = FramerateValues.FramerateAsDecimals[Framerate];

      int dropFrames = Convert.ToInt32((Math.Round(framerate * 0.066666m)));
      int timeBase = Convert.ToInt32(Math.Round(framerate));

      int hourFrames = timeBase * 60 * 60;
      int minuteFrames = timeBase * 60;
      int totalMinutes = (60 * hours) + minutes;
      int totalFrames = ((hourFrames * hours) + (minuteFrames * minutes) + (timeBase * seconds) + frames) - (dropFrames * (totalMinutes - (totalMinutes / 10)));
      TotalFrames = totalFrames;
    }

    #region staticMethods

    /// <summary>
    /// Adds hours to the given time. 
    /// <br/><br/>
    /// Positive integer values add hours, 
    /// while negative values remove hours.
    /// </summary>
    /// <param name="inputString">Time to update.</param>
    /// <param name="hours">Number of hours to add or remove.</param>
    /// <param name="framerate">The timecode framerate.Default rate is fps24</param>
    public static string AddHours(string inputString, int hours, Framerate framerate = Framerate.fps24)
    {
      Timecode timecode = new Timecode(inputString, framerate);
      timecode.AddHours(hours);
      return timecode.ToString();
    }

    /// <summary>
    /// Adds minutes to the given time. 
    /// <br/><br/>
    /// Positive integer values add minutes, 
    /// while negative values remove minutes.
    /// </summary>
    /// <param name="inputString">Time to update.</param>
    /// <param name="minutes">Number of minutes to add or remove.</param>
    /// <param name="framerate">The timecode framerate.Default rate is fps24</param>
    public static string AddMinutes(string inputString, int minutes, Framerate framerate = Framerate.fps24)
    {
      Timecode timecode = new Timecode(inputString, framerate);
      timecode.AddMinutes(minutes);
      return timecode.ToString();
    }

    /// <summary>
    /// Adds seconds to the given time. 
    /// <br/><br/>
    /// Positive integer values add seconds, 
    /// while negative values remove seconds.
    /// </summary>
    /// <param name="inputString">Time to update.</param>
    /// <param name="seconds">Number of seconds to add or remove.</param>
    /// <param name="framerate">The timecode framerate.Default rate is fps24</param>
    public static string AddSeconds(string inputString, int seconds, Framerate framerate = Framerate.fps24)
    {
      Timecode timecode = new Timecode(inputString, framerate);
      timecode.AddSeconds(seconds);
      return timecode.ToString();
    }

    /// <summary>
    /// Adds seconds to the given time. 
    /// <br/><br/>
    /// Positive integer values add frames, 
    /// while negative values remove frames.
    /// </summary>
    /// <param name="inputString">Time to update.</param>
    /// <param name="frames">Number of frames to add or remove.</param>
    /// <param name="framerate">The timecode framerate.Default rate is fps24</param>
    public static string AddFrames(string inputString, int frames, Framerate framerate = Framerate.fps24)
    {
      Timecode timecode = new Timecode(inputString, framerate);
      timecode.AddFrames(frames);
      return timecode.ToString();
    }

    /// <summary>
    /// Converts a timecode string to a timecode string of a different framerate.
    /// </summary>
    /// <param name="originalTimecode">The original timecode, formatted "HH:MM:SS:FF" or "-HH:MM:SS:FF".</param>
    /// <param name="originalFramerate">The original timecode framerate.</param>
    /// <param name="destinationFramerate">The target framerate to convert to.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string ConvertFramerate(string originalTimecode, Framerate originalFramerate, Framerate destinationFramerate)
    {
      // Validate the original timecode format
      Regex tcRegex = new Regex(RegexPattern);
      if (!tcRegex.IsMatch(originalTimecode))
        throw new ArgumentException("Invalid timecode format.", nameof(originalTimecode));
      Timecode timecode = new Timecode(originalTimecode, originalFramerate);
      timecode.ConvertFramerate(destinationFramerate);
      return timecode.ToString();
    }
    #endregion
  }
}
