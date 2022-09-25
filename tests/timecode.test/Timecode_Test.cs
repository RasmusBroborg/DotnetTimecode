using FluentAssertions;

namespace timecode.test
{
  public class Timecode_Test
  {
    [Fact]
    public void Construct_With_2997_Drop_Frame_Using_TotalFrames()
    {
      //Variable source
      //https://www.cinelexi.com/tc-calc
      var tenHours2997DropFrame = 1078920;

      var sut = new Timecode(tenHours2997DropFrame, Enums.Framerate.fps29_97_DF);

      sut.ToString().Should().Be("10:00:00:00");
    }

    [Fact]
    public void Construct_With_5994_Drop_Frame_Using_TotalFrames()
    {
      //Variable source
      //https://www.cinelexi.com/tc-calc
      var tenHours5994DropFrame = 2157840;

      var sut = new Timecode(tenHours5994DropFrame, Enums.Framerate.fps59_94_DF);

      sut.ToString().Should().Be("10:00:00:00");
    }
  }
}
