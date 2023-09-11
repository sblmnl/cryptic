namespace Cryptic.WebAPI.Features.Notes;

public enum DeleteAfter
{
    Reading,
    ReadingNoWarning,
    OneHour,
    OneDay,
    SevenDays,
    ThirtyDays
}

public static class DeleteAfterExtensions
{
    public static TimeSpan ToTimeSpan(this DeleteAfter deleteAfter)
    {
        return deleteAfter switch
        {
              DeleteAfter.OneHour => TimeSpan.FromHours(1),
              DeleteAfter.OneDay => TimeSpan.FromDays(1),
              DeleteAfter.SevenDays => TimeSpan.FromDays(7),
              DeleteAfter.ThirtyDays => TimeSpan.FromDays(30),
            _ => TimeSpan.MinValue
        };
    }
}

public class Note
{
    public Guid Id { get; }
    public string Content { get; }
    public DeleteAfter DeleteAfter { get; }
    public DateTimeOffset CreatedAt { get; }

    public bool DeleteAfterReading => DeleteAfter == DeleteAfter.Reading || DeleteAfter == DeleteAfter.ReadingNoWarning;
    public bool IsToBeDeleted => !DeleteAfterReading && DateTimeOffset.UtcNow >= CreatedAt + DeleteAfter.ToTimeSpan();
    
    private Note(
        Guid id,
        string content,
        DeleteAfter deleteAfter,
        DateTimeOffset createdAt)
    {
        Id = id;
        Content = content;
        DeleteAfter = deleteAfter;
        CreatedAt = createdAt;
    }

    private Note(
        string content,
        DeleteAfter deleteAfter)
    {
        Id = Guid.NewGuid();
        Content = content;
        DeleteAfter = deleteAfter;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    private Note() {}

    public static Note New(
        string content,
        DeleteAfter deleteAfter)
    {
        return new(content, deleteAfter);
    }
}
