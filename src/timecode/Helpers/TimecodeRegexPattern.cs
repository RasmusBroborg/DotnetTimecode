using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetTimecode.Helpers
{
  /// <summary>
  /// Static class containing regular expression pattern of different timecodes.
  /// </summary>
  public static class TimecodeRegexPattern
  {
    public const string SMPTE = @"^(([0-9]){2}:){2}(([0-9]){2})(;|:)([0-9]){2}$";
    public const string SubRip = @"^(([0-9]){2}:){2}(([0-9]){2})(;|:)([0-9]){2}$";
  }
}