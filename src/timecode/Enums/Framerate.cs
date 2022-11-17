namespace DotnetTimecode.Enums
{
  /// <summary>
  /// Enums representing different timecode framerates.
  /// </summary>
  public enum Framerate
  {
    /// <summary>
    /// Represents the framerate 23.976 fps.
    /// </summary>
    fps23_976,

    /// <summary>
    /// Represents the framerate 24 fps FILM.
    /// </summary>
    fps24,

    /// <summary>
    /// Represents the framerate 25 fps PAL.
    /// </summary>
    fps25,

    /// <summary>
    /// Represents the framerate 29.97 fps non drop frame.
    /// </summary>
    fps29_97_NDF,

    /// <summary>
    /// Represents the framerate 29.97 fps drop frame.
    /// </summary>
    fps29_97_DF,

    /// <summary>
    /// Represents the framerate 30 fps non drop frame.
    /// </summary>
    fps30,

    /// <summary>
    /// Represents the framerate 47.95 fps.
    /// </summary>
    fps47_95,

    /// <summary>
    /// Represents the framerate 48 fps.
    /// </summary>
    fps48,

    /// <summary>
    /// Represents the framerate 50 fps.
    /// </summary>
    fps50,

    /// <summary>
    /// Represents the framerate 59.94 fps non drop frame.
    /// </summary>
    fps59_94_NDF,

    /// <summary>
    /// Represents the framerate 59.94 fps drop frame.
    /// </summary>
    fps59_94_DF,

    /// <summary>
    /// Represents the framerate 60 fps.
    /// </summary>
    fps60,
  }
}