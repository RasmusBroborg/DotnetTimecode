﻿using DotnetTimecode.Enums;

namespace DotnetTimecode.Helpers
{
  /// <summary>
  /// Provides methods related to framerate values.
  /// </summary>
  internal static class FramerateValues
  {
    /// <summary>
    /// Provides a conversion table for framerate enums as their decimal values.
    /// </summary>
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

    /// <summary>
    /// Returns true if the framerate is a non drop frame framerate.
    /// </summary>
    /// <param name="framerate">The target framerate.</param>
    /// <returns>True if the framerate is a non drop frame framerate.</returns>
    internal static bool IsNonDropFrame(Framerate framerate)
    {
      return !(framerate == Framerate.fps29_97_DF || framerate == Framerate.fps59_94_DF);
    }

    /// <summary>
    /// Gets the last delimiter of a framerate. Is either ':' or ';' based on if the framerate
    /// is Drop Frame or Non Drop Frame.
    /// </summary>
    /// <param name="framerate"></param>
    /// <returns>Either ':' if NDF or ';' if DF.</returns>
    internal static char GetLastDelimiter(Framerate framerate)
    {
      return IsNonDropFrame(framerate) ? ':' : ';';
    }
  }
}