
using System.Text.RegularExpressions;

using FluentAssertions;

namespace DotnetTimecode.test
{
  public class Timecode_Test
  {
    [Fact]
    public void Construct_Using_Only_Framerate()
    {
      var sut = new Timecode(Enums.Framerate.fps24);
      sut.ToString().Should().Be("00:00:00:00");
    }

    [Fact]
    public void Construct_Using_TotalFrames_25fps()
    {
      var tenHoursAsTotalFrames = 900000;
      var sut = new Timecode(tenHoursAsTotalFrames, Enums.Framerate.fps25);
      sut.ToString().Should().Be("10:00:00:00");
    }

    [Fact]
    public void Construct_Using_TotalFrames_Drop_Frame_29_97fps()
    {
      var tenHoursAsTotalFrames = 1078920;
      var sut = new Timecode(tenHoursAsTotalFrames, Enums.Framerate.fps29_97_DF);
      sut.ToString().Should().Be("10:00:00:00");
    }

    [Fact]
    public void Construct_Using_TotalFrames_Drop_Frame_59_94fps()
    {
      var tenHoursAsTotalFrames = 2157840;
      var sut = new Timecode(tenHoursAsTotalFrames, Enums.Framerate.fps59_94_DF);
      sut.ToString().Should().Be("10:00:00:00");
    }

    [Fact]
    public void Construct_Using_hhmmssff_23_976fps()
    {
      var sut = new Timecode(10, 00, 00, 00, Enums.Framerate.fps23_976);
      sut.TotalFrames.Should().Be(864000);
    }

    [Fact]
    public void Construct_Using_hhmmssff_50fps()
    {
      var sut = new Timecode(10, 00, 00, 00, Enums.Framerate.fps50);
      sut.TotalFrames.Should().Be(1800000);
    }

    [Fact]
    public void Construct_Using_hhmmssff_Drop_Frame_59_94fps()
    {
      var sut = new Timecode(10, 00, 00, 00, Enums.Framerate.fps59_94_DF);
      sut.TotalFrames.Should().Be(2157840);
    }

    [Fact]
    public void Construct_Using_String_Input()
    {
      var sut = new Timecode("10:00:00:00", Enums.Framerate.fps24);
      sut.TotalFrames.Should().Be(864000);
    }

    [Fact]
    public void Construct_Using_Incorrect_Format_Input()
    {
      string incorrectTimecodeFormat = "10:a0:00:00";
      Action act = () => new Timecode(incorrectTimecodeFormat, Enums.Framerate.fps24);
      act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Add_Frames_And_Recalculate_Timecode()
    {
      var sut = new Timecode("10:00:00:00", Enums.Framerate.fps25);

      sut.AddFrames(50);
      sut.ToString().Should().Be("10:00:02:00");
    }

    [Fact]
    public void Remove_Frames_And_Recalculate_Timecode()
    {
      var sut = new Timecode("10:00:00:00", Enums.Framerate.fps25);
      sut.AddFrames(-50);
      sut.ToString().Should().Be("09:59:58:00");
    }

    [Fact]
    public void Convert_Framerate_24fps_To_25fps()
    {
      var sut = new Timecode("10:00:00:00", Enums.Framerate.fps24);
      sut.ConvertFramerate(Enums.Framerate.fps25);
      sut.ToString().Should().Be("09:36:00:00");
    }

    [Fact]
    public void Convert_Framerate_23_976fps_To_59_94fps()
    {
      var sut = new Timecode("10:00:00:00", Enums.Framerate.fps23_976);
      sut.ConvertFramerate(Enums.Framerate.fps59_94_NDF);
      sut.ToString().Should().Be("04:00:00:00");
    }

    [Fact]
    public void Add_61_Minutes_To_Timecode()
    {
      var sut = new Timecode("10:00:00:00", Enums.Framerate.fps23_976);
      sut.AddMinutes(61);
      sut.ToString().Should().Be("11:01:00:00");
    }

    [Fact]
    public void Remove_60_Minutes_From_Timecode()
    {
      var sut = new Timecode("10:00:00:00", Enums.Framerate.fps23_976);
      sut.AddMinutes(-60);
      sut.ToString().Should().Be("09:00:00:00");
    }

    [Fact]
    public void Remove_61_Minutes_From_Timecode()
    {
      var sut = new Timecode("10:00:00:00", Enums.Framerate.fps23_976);
      sut.AddMinutes(-61);
      sut.ToString().Should().Be("08:59:00:00");
    }

    [Fact]
    public void Remove_119_Minutes_From_Timecode()
    {
      var sut = new Timecode("10:00:00:00", Enums.Framerate.fps23_976);
      sut.AddMinutes(-119);
      sut.ToString().Should().Be("08:01:00:00");
    }

    [Fact]
    public void Add_1_Hour_To_Timecode()
    {
      var sut = new Timecode("10:00:00:00", Enums.Framerate.fps23_976);
      sut.AddHours(1);
      sut.ToString().Should().Be("11:00:00:00");
    }

    [Fact]
    public void Remove_1_Hour_From_Timecode()
    {
      var sut = new Timecode("10:00:00:00", Enums.Framerate.fps23_976);
      sut.AddHours(-1);
      sut.ToString().Should().Be("09:00:00:00");
    }

    [Fact]
    public void Add_1_Second_To_Timecode()
    {
      var sut = new Timecode("10:00:00:00", Enums.Framerate.fps23_976);
      sut.AddSeconds(1);
      sut.ToString().Should().Be("10:00:01:00");
    }

    [Fact]
    public void Remove_1_Second_From_Timecode()
    {
      var sut = new Timecode("10:00:00:00", Enums.Framerate.fps23_976);
      sut.AddSeconds(-1);
      sut.ToString().Should().Be("09:59:59:00");
    }

    [Fact]
    public void Add_60_Seconds_To_Timecode()
    {
      var sut = new Timecode("10:00:00:00", Enums.Framerate.fps23_976);
      sut.AddSeconds(60);
      sut.ToString().Should().Be("10:01:00:00");
    }

    [Fact]
    public void Remove_60_Seconds_From_Timecode()
    {
      var sut = new Timecode("10:00:00:00", Enums.Framerate.fps23_976);
      sut.AddSeconds(-60);
      sut.ToString().Should().Be("09:59:00:00");
    }

    [Fact]
    public void Timecode_Regex_Works()
    {
      var sut = new Regex(Timecode.RegexPattern);
      sut.Match("10:00:00:00").Success.Should().Be(true);
      sut.Match("10:a0:00:00").Success.Should().Be(false);
      sut.Match("10:000:00:00").Success.Should().Be(false);
    }

    [Fact]
    public void Timecode_Subtracts_ResultIsSubtracted()
    {
      // Arrange
      var t1 = new Timecode(11, 11, 11, 0, Enums.Framerate.fps29_97_NDF);
      var t2 = new Timecode(1, 1, 1, 0, Enums.Framerate.fps29_97_NDF);

      // Act
      var result = t1 - t2;

      // Assert
      Assert.Equal(10, result.Hour);
      Assert.Equal(10, result.Minute);
      Assert.Equal(10, result.Second);
    }

    [Fact]
    public void Timecode_Add_AddsTimecode()
    {
      // Arrange
      var t1 = new Timecode(11, 11, 11, 0, Enums.Framerate.fps29_97_NDF);
      var t2 = new Timecode(1, 1, 1, 0, Enums.Framerate.fps29_97_NDF);

      // Act
      var result = t1 + t2;

      // Assert
      Assert.Equal(12, result.Hour);
      Assert.Equal(12, result.Minute);
      Assert.Equal(12, result.Second);
    }

    [Fact]
    public void Timecode_Add_DifferentFrameRate_ThrowsException()
    {
      // Arrange
      var t1 = new Timecode(11, 11, 11, 0, Enums.Framerate.fps29_97_NDF);
      var t2 = new Timecode(1, 1, 1, 0, Enums.Framerate.fps50);

      // Assert
      Assert.Throws<InvalidOperationException>(() => t1 + t2);
    }

    [Fact]
    public void Timecode_Equals_OtherTimeCode()
    {
      // Arrange
      var t1 = new Timecode(11, 11, 11, 0, Enums.Framerate.fps29_97_NDF);
      var t2 = t1;

      // Act
      var result = t1 == t2;

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void Timecode_NotEquals_OtherTimeCode()
    {
      // Arrange
      var t1 = new Timecode(11, 11, 11, 0, Enums.Framerate.fps29_97_NDF);
      var t2 = t1;

      // Act
      var result = t1 != t2;

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void Timecode_SmallerThan_OtherTimeCode()
    {
      // Arrange
      var t1 = new Timecode(11, 11, 11, 0, Enums.Framerate.fps29_97_NDF);
      var t2 = new Timecode(12, 12, 12, 0, Enums.Framerate.fps29_97_NDF);

      // Act
      var result = t1 < t2;

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void Timecode_NotSmallerThan_OtherTimeCode()
    {
      // Arrange
      var t1 = new Timecode(11, 11, 11, 0, Enums.Framerate.fps29_97_NDF);
      var t2 = new Timecode(10, 11, 12, 0, Enums.Framerate.fps29_97_NDF);

      // Act
      var result = t1 < t2;

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void Timecode_LargerThan_OtherTimeCode()
    {
      // Arrange
      var t1 = new Timecode(11, 11, 11, 0, Enums.Framerate.fps29_97_NDF);
      var t2 = new Timecode(10, 11, 12, 0, Enums.Framerate.fps29_97_NDF);

      // Act
      var result = t1 > t2;

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void Timecode_NotLargerThan_OtherTimeCode()
    {
      // Arrange
      var t1 = new Timecode(11, 11, 11, 0, Enums.Framerate.fps29_97_NDF);
      var t2 = new Timecode(12, 11, 12, 0, Enums.Framerate.fps29_97_NDF);

      // Act
      var result = t1 > t2;

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void Timecode_LargerOrEqual_OtherTimeCode()
    {
      // Arrange
      var t1 = new Timecode("10:00:00:00", Enums.Framerate.fps29_97_NDF);
      var t2 = new Timecode("10:00:00:00", Enums.Framerate.fps29_97_NDF);

      // Act
      var result = t1 >= t2;

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void Timecode_LessOrEqual_OtherTimeCode()
    {
      // Arrange
      var t1 = new Timecode("10:00:00:00", Enums.Framerate.fps29_97_NDF);
      var t2 = new Timecode("10:00:00:00", Enums.Framerate.fps29_97_NDF);

      // Act
      var result = t1 <= t2;

      // Assert
      result.Should().BeTrue();
    }

    [Fact]
    public void LessThan_Timecodes_With_Different_Framerates()
    {
      // Arrange
      var t1 = new Timecode(10, 0, 0, 0, Enums.Framerate.fps25);
      var t2 = new Timecode(10, 0, 0, 0, Enums.Framerate.fps24);

      // Act
      Action comparison = () => { var res = t1 > t2; };

      // Assert
      comparison.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void MoreThan_Timecodes_With_Different_Framerates()
    {
      // Arrange
      var t1 = new Timecode(10, 0, 0, 0, Enums.Framerate.fps25);
      var t2 = new Timecode(10, 0, 0, 0, Enums.Framerate.fps24);

      // Act
      Action comparison = () => { var res = t1 < t2; };

      // Assert
      comparison.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Negative_Timecodes_ToString()
    {
      // Arrange
      var t1 = new Timecode(-10, 10, 10, 10, Enums.Framerate.fps25);
      var t2 = new Timecode("-10:10:10:10", Enums.Framerate.fps25);

      // Act
      var expectedResult = "-10:10:10:10";
      var t1Result = t1.ToString();
      var t2Result = t2.ToString();

      // Assert
      t1Result.Should().Be(expectedResult);
      t2Result.Should().Be(expectedResult);
    }

    [Fact]
    public void Negative_Timecodes_ArithmeticOperations()
    {
      // Arrange
      var timecode = new Timecode("-10:00:00:00", Enums.Framerate.fps25);

      // Act
      timecode.AddHours(8);
      timecode.AddMinutes(60);
      timecode.AddSeconds(3600);

      // Assert
      timecode.Hour.Should().Be(0);
      timecode.Minute.Should().Be(0);
      timecode.Second.Should().Be(0);
      timecode.Frame.Should().Be(0);
      timecode.TotalFrames.Should().Be(0);
      timecode.Framerate.Should().Be(Enums.Framerate.fps25);
    }
  }
}
