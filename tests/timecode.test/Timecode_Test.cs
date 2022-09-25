
using FluentAssertions;

namespace timecode.test
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
  }
}
