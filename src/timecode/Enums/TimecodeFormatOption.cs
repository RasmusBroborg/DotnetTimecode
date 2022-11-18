using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetTimecode.Enums
{
  /// <summary>
  /// Defines different options for timecode formats.
  /// </summary>
  public enum TimecodeFormatOption
  {
    /// <summary>
    /// Sets the last delimiter to a colon. <br/>
    /// <br/>
    /// Ex: 10:00:00:00
    /// </summary>
    ColonFrameDelimiter,

    /// <summary>
    /// Sets the frame delimiter to a semicolon. <br/>
    /// <br/>
    /// Ex: 10:00:00;00
    /// </summary>
    SemicolonFrameDelimiter,

    /// <summary>
    /// Sets the frame delimiter to a comma. <br/>
    /// <br/>
    /// Ex: 10:00:00,00
    /// </summary>
    CommaFrameDelimiter
  }
}