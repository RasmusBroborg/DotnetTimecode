using timecode.Enums;

namespace timecode
{
  public class Timecode
  {
    public int Hour { get; set; }
    public int Minute { get; set; }
    public int Second { get; set; }
    public int Frame { get; set; }
    public int TotalFrames { get; set; }
    public Framerate Framerate { get; set; }

    public override string ToString()
    {
      return $"{ZeroPadding(Hour)}:{ZeroPadding(Minute)}:{ZeroPadding(Second)}:{ZeroPadding(Frame)}";
    }

    /// <summary>
    /// Pads the first number position with a 0 if the number is less than two positions long.
    /// </summary>
    /// <returns>A string representation of a number value in the format of ex: "09" or "100".</returns>
    private string ZeroPadding(int num) => num < 10 ? $"0{num}" : num.ToString();
  }
}
