namespace Cryptic.Core.Common;

public enum DeleteAfter
{
    Viewing = 0,
    OneHour = 1,
    OneDay = 2,
    OneWeek = 3,
}

public static class DeleteAfterExtensions
{
    public static TimeSpan ToTimeSpan(this DeleteAfter deleteAfter)
    {
        return deleteAfter switch
        {
            DeleteAfter.Viewing => TimeSpan.FromDays(1),
            DeleteAfter.OneHour => TimeSpan.FromHours(1),
            DeleteAfter.OneDay => TimeSpan.FromDays(1),
            DeleteAfter.OneWeek => TimeSpan.FromDays(7),
            _ => throw new InvalidOperationException("Unknown delete after!"),
        };
    }
}
