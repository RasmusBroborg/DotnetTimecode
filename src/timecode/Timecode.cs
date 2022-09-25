using timecode.Enums;
using timecode.Helpers;

namespace timecode
{
  public class Timecode
  {
    /// <summary>
    /// Regular expression pattern for a timecode. 
    /// Supports the format hh:mm:ss:ff.
    /// </summary>
    public static string RegexPattern = @"(([0-9]){2}:){3}([0-9]){2}";

    /// <summary>
    /// The timecode hour position, based on the framerate.
    /// </summary>
    public int Hour { get; private set; }
    /// <summary>
    /// The timecode minute position, based on the framerate.
    /// </summary>
    public int Minute { get; private set; }
    /// <summary>
    /// The timecode second position, based on the framerate.
    /// </summary>
    public int Second { get; private set; }
    /// <summary>
    /// The timecode frame position, based on the framerate after hours,
    /// minutes and seconds have been subtracted from the total frames.
    /// </summary>
    public int Frame { get; private set; }

    /// <summary>
    /// The timecodes total amount of frames.
    /// </summary>
    public int TotalFrames { get; set; }

    /// <summary>
    /// The timecode framerate.
    /// </summary>
    public Framerate Framerate { get; set; }

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
    /// Creates a new Timecode object.
    /// </summary>
    /// <param name="totalFrames">The total amount of frames.</param>
    /// <param name="framerate">The timecode framerate.</param>
    /// <param name="dropFrame">Decides if the timecode should drop frames.</param>
    public Timecode(int totalFrames, Framerate framerate)
    {
      TotalFrames = totalFrames;
      Framerate = framerate;
      CalcTimecode();
    }

    /// <summary>
    /// Creates a new Timecode object.
    /// </summary>
    /// <param name="hour">The timecode hour position.</param>
    /// <param name="minute">The timecode minute position.</param>
    /// <param name="second">The timecode second position.</param>
    /// <param name="frame">The timecode frame position.</param>
    /// <param name="framerate">The timecode framerate.</param>
    /// <param name="dropFrame">Decides if the timecode should drop frames.</param>
    public Timecode(int hour, int minute, int second, int frame, Framerate framerate)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Creates a new Timecode object.
    /// </summary>
    /// <param name="timecode">The timecode represented as a string formatted hh:mm:ss:ff.</param>
    /// <param name="framerate">The timecode framerate.</param>
    /// <param name="dropFrame">Decides if the timecode should drop frames.</param>
    public Timecode(string timecode, Framerate framerate)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Adds hours to the timecode. Negative numbers removes hours from the timecode.
    /// </summary>
    /// <param name="hours">Number of hours to add to the timecode.</param>
    public void AddHours(int hours)
    {
      throw new NotImplementedException();
      CalcTimecode();
    }

    /// <summary>
    /// Adds minutes to the timecode. Negative numbers removes minutes from the timecode.
    /// </summary>
    /// <param name="minutes">Number of minutes to add to the timecode.</param>
    /// <exception cref="NotImplementedException"></exception>
    public void AddMinutes(int minutes)
    {
      throw new NotImplementedException();
      CalcTimecode();
    }

    /// <summary>
    /// Adds seconds to the timecode. Negative numbers removes seconds from the timecode.
    /// </summary>
    /// <param name="seconds">Number of seconds to add to the timecode.</param>
    public void AddSeconds(int seconds)
    {
      throw new NotImplementedException();
      CalcTimecode();
    }

    /// <summary>
    /// Adds frames to the timecode. Negative numbers removes seconds from the timecode.
    /// </summary>
    /// <param name="frames">Number of frames to add to the timecode.</param>
    public void AddFrames(int frames)
    {
      throw new NotImplementedException();
      CalcTimecode();
    }

    /// <summary>
    /// Pads the first number position with a 0 if the number is less than two positions long.
    /// </summary>
    /// <returns>A string representation of a number value in the format of ex: "09".</returns>
    private string ZeroPadding(int num) => num < 10 ? $"0{num}" : num.ToString();

    private void CalcTimecode()
    {
      if (Framerate == Framerate.fps29_97_DF || Framerate == Framerate.fps59_94_DF)
      {
        CalcTotalFramesToDropFrame(TotalFrames);
      }
      else
      {
      }
    }

    private void CalcTotalFramesToDropFrame(int totalFrames)
    {
      decimal framerate = FramerateValues.FramerateAsDecimals[Framerate];

      int div;
      int mod;

      int dropFrames = Convert.ToInt32(Math.Round(framerate * .066666m));
      int framesPerMinute = Convert.ToInt32((Math.Round(framerate) * 60) - dropFrames);
      int framesPer10Minutes = Convert.ToInt32(Math.Round(framerate * 60 * 10));
      int framesPerHour = Convert.ToInt32(Math.Round(framerate * 60 * 60));
      int framesPer24Hours = framesPerHour * 24;

      while (totalFrames < 0)
      {
        totalFrames = framesPer24Hours + totalFrames;
      }

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

      int frRound = Convert.ToInt32((Math.Round(framerate)));
      Frame = totalFrames % frRound;
      Second = (totalFrames / frRound) % 60;
      Minute = ((totalFrames / frRound) / 60) % 60;
      Hour = (((totalFrames / frRound) / 60) / 60);
    }
  }
}
