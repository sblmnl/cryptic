namespace Cryptic.Core.Tests.Common;

public class DeleteAfterTests
{
    [Fact]
    public void ToTimeSpan_Viewing_ReturnsOneDay()
    {
        Assert.Equal(TimeSpan.FromDays(1), DeleteAfter.Viewing.ToTimeSpan());
    }

    [Fact]
    public void ToTimeSpan_OneHour_ReturnsOneHour()
    {
        Assert.Equal(TimeSpan.FromHours(1), DeleteAfter.OneHour.ToTimeSpan());
    }

    [Fact]
    public void ToTimeSpan_OneDay_ReturnsOneDay()
    {
        Assert.Equal(TimeSpan.FromDays(1), DeleteAfter.OneDay.ToTimeSpan());
    }

    [Fact]
    public void ToTimeSpan_OneWeek_ReturnsSevenDays()
    {
        Assert.Equal(TimeSpan.FromDays(7), DeleteAfter.OneWeek.ToTimeSpan());
    }

    [Fact]
    public void ToTimeSpan_UnknownValue_ThrowsInvalidOperationException()
    {
        var unknown = (DeleteAfter)99;
        Assert.Throws<InvalidOperationException>(() => unknown.ToTimeSpan());
    }
}