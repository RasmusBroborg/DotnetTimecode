using timecode.Enums;

namespace timecode
{
  public class Timecode
  {
    /// <summary>
    /// The timecode hour position, based on the framerate.
    /// </summary>
    public int Hour
    {
      get
      {
        throw new NotImplementedException();
      }
      set
      {
        throw new NotImplementedException();
      }
    }
    /// <summary>
    /// The timecode minute position, based on the framerate.
    /// </summary>
    public int Minute
    {
      get
      {
        throw new NotImplementedException();
      }
      set
      {
        throw new NotImplementedException();
      }
    }
    /// <summary>
    /// The timecode second position, based on the framerate.
    /// </summary>
    public int Second
    {
      get
      {
        throw new NotImplementedException();
      }
      set
      {
        throw new NotImplementedException();
      }
    }
    /// <summary>
    /// The timecode frame position, based on the framerate after hours,
    /// minutes and seconds have been subtracted from the total frames.
    /// </summary>
    public int Frame
    {
      get
      {
        throw new NotImplementedException();
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// The timecodes total amount of frames.
    /// </summary>
    public int TotalFrames { get; set; }

    /// <summary>
    /// The timecode framerate.
    /// </summary>
    public Framerate Framerate { get; set; }

    /// <summary>
    /// Returns the timecode as a string formatted 00:00:00:00. 
    /// Does not support negative timelines.
    /// </summary>
    /// <returns>The timecode formatted 00:00:00:00</returns>
    public override string ToString()
    {
      return $"{ZeroPadding(Hour)}:{ZeroPadding(Minute)}:{ZeroPadding(Second)}:{ZeroPadding(Frame)}";
    }

    /// <summary>
    /// Creates a new Timecode object.
    /// </summary>
    /// <param name="totalFrames">The total amount of frames.</param>
    /// <param name="framerate">The timecode framerate.</param>
    public Timecode(int totalFrames, Framerate framerate)
    {
      throw new NotImplementedException();
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
      throw new NotImplementedException();
    }

    /// <summary>
    /// Creates a new Timecode object.
    /// </summary>
    /// <param name="timecode">The timecode represented as a string formatted 00:00:00:00.</param>
    /// <param name="framerate">The timecode framerate.</param>
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
    }

    /// <summary>
    /// Adds minutes to the timecode. Negative numbers removes minutes from the timecode.
    /// </summary>
    /// <param name="minutes">Number of minutes to add to the timecode.</param>
    /// <exception cref="NotImplementedException"></exception>
    public void AddMinutes(int minutes)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Adds seconds to the timecode. Negative numbers removes seconds from the timecode.
    /// </summary>
    /// <param name="seconds">Number of seconds to add to the timecode.</param>
    public void AddSeconds(int seconds)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Adds frames to the timecode. Negative numbers removes seconds from the timecode.
    /// </summary>
    /// <param name="frames">Number of frames to add to the timecode.</param>
    public void AddFrames(int frames)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Pads the first number position with a 0 if the number is less than two positions long.
    /// </summary>
    /// <returns>A string representation of a number value in the format of ex: "09" or "100".</returns>
    private string ZeroPadding(int num) => num < 10 ? $"0{num}" : num.ToString();
  }
}
