
using System.Text.RegularExpressions;

using DotnetTimecode.Enums;

using FluentAssertions;

namespace DotnetTimecode.test
{
  public class Timecode_Test
  {
    #region Constructors

    [Theory] // Arrange
    [InlineData("10:00:00:00", Enums.Framerate.fps25, "10:00:00:00")]
    [InlineData("10:00:00;00", Enums.Framerate.fps25, "10:00:00:00")]
    [InlineData("10:00:00:00", Enums.Framerate.fps29_97_DF, "10:00:00;00")]
    [InlineData("10:00:00;00", Enums.Framerate.fps29_97_DF, "10:00:00;00")]
    [InlineData("-10:00:00:00", Enums.Framerate.fps48, "-10:00:00:00")]
    [InlineData("10:00:00;50", Enums.Framerate.fps25, "10:00:02:00")]
    [InlineData("10:09:10:00", Enums.Framerate.fps59_94_DF, "10:09:10;00")]
    [InlineData("-10:00:00;00", Enums.Framerate.fps29_97_DF, "-10:00:00;00")]
    [InlineData("00:00:00;25", Enums.Framerate.fps24, "00:00:01:01")]
    public void Constructor_ConstructUsingValidPositiveStringInput_Succeeds(
      string validTimecodeFormat, Enums.Framerate framerate, string expectedResult)
    {
      // Act
      Timecode result = new Timecode(validTimecodeFormat, framerate);

      // Assert
      result.ToString().Should().Be(expectedResult);
    }

    [Theory] // Arrange
    [InlineData("10:-00:00:00", Enums.Framerate.fps25)]
    [InlineData("foobar", Enums.Framerate.fps25)]
    [InlineData("10:001:00:700", Enums.Framerate.fps30)]
    [InlineData("10.00.00.00", Enums.Framerate.fps29_97_DF)]
    public void Constructor_ConstructUsingIncorrectStringInput_ThrowsException(
      string incorrectTimecodeFormat, Enums.Framerate framerate)
    {
      // Act
      Action act = () => new Timecode(incorrectTimecodeFormat, framerate);

      // Assert
      act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_UsingOnlyFramerate_Succeeds()
    {
      var sut = new Timecode(Enums.Framerate.fps24);
      sut.ToString().Should().Be("00:00:00:00");
    }

    [Fact]
    public void Constructor_UsingTotalFrames25fps_Succeeds()
    {
      var tenHoursAsTotalFrames = 900000;
      var sut = new Timecode(tenHoursAsTotalFrames, Enums.Framerate.fps25);
      sut.ToString().Should().Be("10:00:00:00");
    }

    [Fact]
    public void Constructor_UsingHHMMSSFF23976fps_Succeeds()
    {
      var sut = new Timecode(10, 00, 00, 00, Enums.Framerate.fps23_976);
      sut.TotalFrames.Should().Be(864000);
    }

    [Fact]
    public void Constructor_UsingHHMMSSFF50fps_Succeeds()
    {
      var sut = new Timecode(10, 00, 00, 00, Enums.Framerate.fps50);
      sut.TotalFrames.Should().Be(1800000);
    }

    [Fact]
    public void Constructor_UsingHHMMSSFFDropFrame5994fps_Succeeds()
    {
      var sut = new Timecode(10, 00, 00, 00, Enums.Framerate.fps59_94_DF);
      sut.TotalFrames.Should().Be(2157840);
    }

    [Fact]
    public void Constructor_UsingStringInput_Succeeds()
    {
      var sut = new Timecode("10:00:00:00", Enums.Framerate.fps24);
      sut.TotalFrames.Should().Be(864000);
    }

    [Fact]
    public void Constructor_UsingIncorrectFormatInput_ThrowsException()
    {
      string incorrectTimecodeFormat = "10:a0:00:00";
      Action act = () => new Timecode(incorrectTimecodeFormat, Enums.Framerate.fps24);
      act.Should().Throw<ArgumentException>();
    }
    #endregion

    #region Public Methods

    [Theory]
    [InlineData("10:00:00:00", Framerate.fps23_976, 1, "11:00:00:00")]
    [InlineData("10:00:00:00", Framerate.fps24, 100, "110:00:00:00")]
    [InlineData("10:00:00:00", Framerate.fps25, -1, "09:00:00:00")]
    [InlineData("10:00:00;00", Framerate.fps29_97_DF, -11, "-01:00:00;00")]
    [InlineData("10:00:00:00", Framerate.fps29_97_NDF, -11, "-01:00:00:00")]
    [InlineData("10:00:00:00", Framerate.fps30, -11, "-01:00:00:00")]
    [InlineData("10:00:00:00", Framerate.fps47_95, 100, "110:00:00:00")]
    [InlineData("10:00:00:00", Framerate.fps48, -1, "09:00:00:00")]
    [InlineData("10:00:00:00", Framerate.fps50, -11, "-01:00:00:00")]
    [InlineData("10:00:00;00", Framerate.fps59_94_DF, -11, "-01:00:00;00")]
    [InlineData("10:00:00:00", Framerate.fps59_94_NDF, -11, "-01:00:00:00")]
    [InlineData("10:00:00:00", Framerate.fps60, -11, "-01:00:00:00")]
    public void AddHours_MultipleInputs_ExpectedSuccessfulResults(
      string timecodeStr, Framerate framerate, int hoursToAdd, string expectedResult)
    {
      // Arrange
      Timecode sut = new Timecode(timecodeStr, framerate);

      // Act
      sut.AddHours(hoursToAdd);
      string result = sut.ToString();

      // Assert
      result.ToString().Should().Be(expectedResult);
    }

    [Fact]
    public void ToString_NegativeTimecodes_ToString()
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
    public void AddFrames_RecalculateTimecode_Succeeds()
    {
      var sut = new Timecode("10:00:00:00", Enums.Framerate.fps25);

      sut.AddFrames(50);
      sut.ToString().Should().Be("10:00:02:00");
    }

    [Fact]
    public void AddFrames_Recalculate_Timecode_Succeeds()
    {
      var sut = new Timecode("10:00:00:00", Enums.Framerate.fps25);
      sut.AddFrames(-50);
      sut.ToString().Should().Be("09:59:58:00");
    }

    [Fact]
    public void Convert_Framerate_24fps_To_25fps_Succeeds()
    {
      var sut = new Timecode("10:00:00:00", Enums.Framerate.fps24);
      sut.ConvertFramerate(Enums.Framerate.fps25);
      sut.ToString().Should().Be("09:36:00:00");
    }

    [Fact]
    public void Convert_Framerate_23_976fps_To_59_94fps_Succeeds()
    {
      var sut = new Timecode("10:00:00:00", Enums.Framerate.fps23_976);
      sut.ConvertFramerate(Enums.Framerate.fps59_94_NDF);
      sut.ToString().Should().Be("04:00:00:00");
    }

    [Fact]
    public void Add_61_Minutes_To_Timecode_Succeeds()
    {
      var sut = new Timecode("10:00:00:00", Enums.Framerate.fps23_976);
      sut.AddMinutes(61);
      sut.ToString().Should().Be("11:01:00:00");
    }

    [Fact]
    public void Remove_60_Minutes_From_Timecode_Succeeds()
    {
      var sut = new Timecode("10:00:00:00", Enums.Framerate.fps23_976);
      sut.AddMinutes(-60);
      sut.ToString().Should().Be("09:00:00:00");
    }

    [Fact]
    public void Remove_61_Minutes_From_Timecode_Succeeds()
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
    #endregion

    #region Public Static Methods

    [Theory]
    [InlineData("10:00:00:00", Framerate.fps24, 1, "11:00:00:00")]
    [InlineData("10:00:00:00", Framerate.fps48, 100, "110:00:00:00")]
    [InlineData("10:00:00:00", Framerate.fps60, -1, "09:00:00:00")]
    [InlineData("10:00:00:00", Framerate.fps50, - 11, "-01:00:00:00")]
    [InlineData("10:00:00;00", Framerate.fps29_97_DF, -11, "-01:00:00;00")]
    public void AddHours_StaticMethodTest_ExpectedResults(
      string timecodeStr, Framerate framerate, int hoursToAdd, string expectedResult)
    {
      var sut = Timecode.AddHours(timecodeStr, framerate, hoursToAdd);
      sut.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("10.00.00.00")]
    [InlineData("Not a timecode")]
    [InlineData(" 10:00:00:00 ")]
    [InlineData("+10:00:00:00")]
    public void AddHours_StaticMethodTestWithInvalidInput_ThrowsException(string invalidTimecodeStr)
    {
      Action act = () => Timecode.AddHours(invalidTimecodeStr, Framerate.fps24, 1);
      act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("10:00:00:00", Framerate.fps24, 1, "10:01:00:00")]
    [InlineData("10:00:00:00", Framerate.fps50, 100, "11:40:00:00")]
    [InlineData("10:00:00:00", Framerate.fps24, -1, "09:59:00:00")]
    [InlineData("10:00:00:00", Framerate.fps24, -61, "-08:59:00:00")]
    [InlineData("00:00:00;00", Framerate.fps29_97_DF, -1, "-00:01:00;00")]
    [InlineData("00:00:00;00", Framerate.fps24, -61, "-01:01:00:00")]
    [InlineData("00:00:00;00", Framerate.fps29_97_DF, -121, "-02:01:00;00")]
    public void AddMinutes_StaticMethodTest_ExpectedResults(
      string timecodeStr, Framerate framerate, int minutesToAdd, string expectedResult)
    {
      var sut = Timecode.AddMinutes(timecodeStr, framerate, minutesToAdd);
      sut.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("10.00.00.00")]
    [InlineData("Not a timecode")]
    [InlineData(" 10:00:00:00 ")]
    [InlineData("+10:00:00:00")]
    public void AddMinutes_StaticMethodTestWithInvalidInput_ThrowsException(string invalidTimecodeStr)
    {
      Action act = () => Timecode.AddMinutes(invalidTimecodeStr, Framerate.fps24, 1);
      act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("10:00:00:00", Framerate.fps24, 1, "10:00:01:00")]
    [InlineData("10:00:00:00", Framerate.fps24, 100, "10:01:40:00")]
    [InlineData("10:00:00:00", Framerate.fps24, -1, "09:59:59:00")]
    [InlineData("00:00:00:00", Framerate.fps24, -61, "-00:01:01:00")]
    [InlineData("00:00:00;00", Framerate.fps59_94_DF, -121, "-00:02:01;00")]
    public void AddSeconds_StaticMethodTest_ExpectedResults(
      string timecodeStr, Framerate framerate, int secondsToAdd, string expectedResult)
    {
      var sut = Timecode.AddSeconds(timecodeStr, framerate, secondsToAdd);
      sut.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("10.00.00.00")]
    [InlineData("Not a timecode")]
    [InlineData(" 10:00:00:00 ")]
    [InlineData("+10:00:00:00")]
    public void AddSeconds_StaticMethodTestWithInvalidInput_ThrowsException(string invalidTimecodeStr)
    {
      Action act = () => Timecode.AddSeconds(invalidTimecodeStr, Framerate.fps24, 1);
      act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("10:00:00:00", 24, Framerate.fps23_976, "10:00:01:00")]
    [InlineData("10:00:00:00", 24, Framerate.fps24, "10:00:01:00")]
    [InlineData("10:00:00:00", 25, Framerate.fps25, "10:00:01:00")]
    [InlineData("10:00:00;00", 30, Framerate.fps29_97_DF, "10:00:01;00")]
    [InlineData("10:00:00:00", 30, Framerate.fps29_97_NDF, "10:00:01:00")]
    [InlineData("10:00:00:00", 30, Framerate.fps30, "10:00:01:00")]
    [InlineData("10:00:00:00", 48, Framerate.fps47_95, "10:00:01:00")]
    [InlineData("10:00:00:00", 48, Framerate.fps48, "10:00:01:00")]
    [InlineData("10:00:00:00", 50, Framerate.fps50, "10:00:01:00")]
    [InlineData("10:00:00;00", 60, Framerate.fps59_94_DF, "10:00:01;00")]
    [InlineData("10:00:00:00", 60, Framerate.fps59_94_NDF, "10:00:01:00")]
    [InlineData("10:00:00:00", 60, Framerate.fps60, "10:00:01:00")]
    [InlineData("00:00:00:00", -24, Framerate.fps23_976, "-00:00:01:00")]
    [InlineData("00:00:00:00", -24, Framerate.fps24, "-00:00:01:00")]
    [InlineData("00:00:00:00", -25, Framerate.fps25, "-00:00:01:00")]
    [InlineData("00:00:00;00", -30, Framerate.fps29_97_DF, "-00:00:01;00")]
    [InlineData("00:00:00:00", -30, Framerate.fps29_97_NDF, "-00:00:01:00")]
    [InlineData("00:00:00:00", -30, Framerate.fps30, "-00:00:01:00")]
    [InlineData("00:00:00:00", -48, Framerate.fps47_95, "-00:00:01:00")]
    [InlineData("00:00:00:00", -48, Framerate.fps48, "-00:00:01:00")]
    [InlineData("00:00:00:00", -50, Framerate.fps50, "-00:00:01:00")]
    [InlineData("00:00:00;00", -60, Framerate.fps59_94_DF, "-00:00:01;00")]
    [InlineData("00:00:00:00", -60, Framerate.fps59_94_NDF, "-00:00:01:00")]
    [InlineData("00:00:00:00", -60, Framerate.fps60, "-00:00:01:00")]
    public void AddFrames_StaticMethodTest_ExpectedResults(string timecodeStr, int framesToAdd, Framerate framerate, string expectedResult)
    {
      var sut = Timecode.AddFrames(timecodeStr, framerate, framesToAdd);
      sut.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("10.00.00.00")]
    [InlineData("Not a timecode")]
    [InlineData(" 10:00:00:00 ")]
    [InlineData("+10:00:00:00")]
    public void AddFrames_StaticMethodTestWithInvalidInput_ThrowsException(string invalidTimecodeStr)
    {
      Action act = () => Timecode.AddFrames(invalidTimecodeStr, Enums.Framerate.fps25, 1);
      act.Should().Throw<ArgumentException>();
    }

    // TODO: Write better inputs
    [Fact]
    public void ConvertFramerate_StaticMethodTest_ExpectedResults()
    {
      var result = Timecode.ConvertFramerate("10:00:00:00", Enums.Framerate.fps23_976, Enums.Framerate.fps59_94_NDF);
      result.Should().Be("04:00:00:00");
    }

    [Theory]
    [InlineData("10:01:20:15", Framerate.fps23_976, "10:01:20,626")] // 0.62562562562
    [InlineData("10:01:20:15", Framerate.fps24, "10:01:20,625")] // 0.625
    [InlineData("10:01:20:15", Framerate.fps25, "10:01:20,600")] // 0.600
    [InlineData("10:01:20:15", Framerate.fps29_97_DF, "10:01:20,501")] // 0.5005005005
    [InlineData("10:01:20;15", Framerate.fps29_97_NDF, "10:01:20,501")] // 0.5005005005
    [InlineData("10:01:20:15", Framerate.fps30, "10:01:20,500")] // 0.500
    [InlineData("10:01:20:15", Framerate.fps47_95, "10:01:20,313")] // 0.31282586027
    [InlineData("10:01:20:15", Framerate.fps48, "10:01:20,313")] // 0.3125
    [InlineData("10:01:20;15", Framerate.fps50, "10:01:20,300")] // 0.300
    [InlineData("10:01:20;15", Framerate.fps59_94_DF, "10:01:20,250")] // 0.25025025025
    [InlineData("10:01:20;15", Framerate.fps59_94_NDF, "10:01:20,250")] // 0.25025025025
    [InlineData("10:01:20;15", Framerate.fps60, "10:01:20,250")] // 0.250
    public void ConvertTimecodeToSrtTimecode_MultipleInputs_ExpectedBehaviour(
      string timecodeStr, Framerate originalFramerate, string expectedResult)
    {
      // Act
      var result = Timecode.ConvertTimecodeToSrtTimecode(timecodeStr, originalFramerate);

      // Assert
      result.Should().Be(expectedResult);
    }

    #endregion

    #region Public Static Properties

    [Theory]
    [InlineData("10:00:00:00", true)]
    [InlineData("10:00:00;00", true)]
    [InlineData("", false)]
    [InlineData("10 : 00 : 00 : 00", false)]
    [InlineData("10.00:00;00", false)]
    [InlineData("10:00:00.00", false)]
    [InlineData("1:0:0;0", false)]
    [InlineData("1:00:00;00", false)]
    [InlineData("10:00:00;0", false)]
    [InlineData("aa:aa:aa;aa", false)]
    public void Timecode_Regex_Works(string timecodeStr, bool expectedResult)
    {
      var sut = new Regex(Timecode.TimecodeRegexPattern);
      bool result = sut.Match(timecodeStr).Success;
      result.Should().Be(expectedResult);
    }

    #endregion

    #region Operator Overloads

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
    public void Negative_Timecodes_Check_Addition_Overloading()
    {
      // Arrange
      var t1 = new Timecode(10, 00, 00, 00, Enums.Framerate.fps25);
      var t2 = new Timecode("-20:00:00:00", Enums.Framerate.fps25);
      var expectedResult = "-10:00:00:00";

      // Act
      var timecodeAfterOperation = t1 + t2;
      var result = timecodeAfterOperation.ToString();

      // Assert
      result.Should().Be(expectedResult);
    }

    [Fact]
    public void Negative_Timecodes_Check_Addition_Overloading_DropFrame()
    {
      // Arrange
      var t1 = new Timecode(10, 00, 00, 00, Enums.Framerate.fps59_94_DF);
      var t2 = new Timecode("-20:00:00:00", Enums.Framerate.fps59_94_DF);

      // Act
      var expectedResult = "-10:00:00;00";
      var timecodeAfterOperation = t1 + t2;
      var result = timecodeAfterOperation.ToString();

      // Assert
      t1.ToString().Should().Be("10:00:00;00");
      t2.ToString().Should().Be("-20:00:00;00");
      result.Should().Be(expectedResult);
    }

    #endregion
  }
}
