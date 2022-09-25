using timecode.Enums;

namespace timecode.Helpers
{
  internal static class FramerateValues
  {
    internal static readonly Dictionary<Framerate, decimal> FramerateAsDecimals
    = new Dictionary<Framerate, decimal>
    {
      { Framerate.fps23_976, 23.976m },
      { Framerate.fps24, 24m },
      { Framerate.fps25, 25m },
      { Framerate.fps29_97_DF, 29.97m },
      { Framerate.fps29_97_NDF, 29.97m },
      { Framerate.fps30, 30m },
      { Framerate.fps47_95, 47.95m },
      { Framerate.fps48, 48m },
      { Framerate.fps50, 50m },
      { Framerate.fps59_94_DF, 59.94m },
      { Framerate.fps59_94_NDF, 59.94m },
      { Framerate.fps60, 60m },
    };
  }
}
