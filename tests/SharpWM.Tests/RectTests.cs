using SharpWM.Common;

namespace SharpWM.Tests;
public class RectTests
{
    [Fact]
    public void Right_And_Bottom_AreCorrect()
    {
        var r = new Rect(10, 20, 100, 50);
        Assert.Equal(110, r.Right);
        Assert.Equal(70, r.Bottom);
    }

    [Fact]
    public void IsEmpty_TrueForZeroSize()
    {
        Assert.True(new Rect(0, 0, 0, 0).IsEmpty);
        Assert.True(new Rect(0, 0, -1, 10).IsEmpty);
    }
 
    [Fact]
    public void IsEmpty_FalseForValidRect()
    {
        Assert.False(new Rect(0, 0, 1920, 1080).IsEmpty);
    }
 
    [Fact]
    public void Contains_TrueForPointInside()
    {
        var rect = new Rect(0, 0, 1920, 1080);
 
        Assert.True(rect.Contains(960, 540));
        Assert.True(rect.Contains(0, 0));
    }
 
    [Fact]
    public void Contains_FalseForPointOutside()
    {
        var rect = new Rect(0, 0, 1920, 1080);
 
        Assert.False(rect.Contains(1920, 1080)); // right/bottom sono esclusi
        Assert.False(rect.Contains(-1, 0));
    }
 
    [Fact]
    public void Empty_IsEmpty()
    {
        Assert.True(Rect.Empty.IsEmpty);
    }
 
    [Fact]
    public void Equality_WorksCorrectly()
    {
        var a = new Rect(0, 0, 1920, 1080);
        var b = new Rect(0, 0, 1920, 1080);
        var c = new Rect(0, 0, 2560, 1440);
 
        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
    }
}