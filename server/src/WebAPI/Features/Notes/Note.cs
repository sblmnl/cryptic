namespace Cryptic.WebAPI.Features.Notes;

public record NoteId
{
    public Guid Value { get; }

    private NoteId(Guid value)
    {
        Value = value;
    }

    private NoteId()
    {
        Value = Guid.NewGuid();
    }

    public static NoteId New() => new();
    public static NoteId From(Guid value) => new(value);
}

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
    public NoteId Id { get; }
    public string Content { get; }
    public DeleteAfter DeleteAfter { get; }
    public DateTimeOffset CreatedAt { get; }

    public bool DeleteAfterReading => DeleteAfter == DeleteAfter.Reading || DeleteAfter == DeleteAfter.ReadingNoWarning;
    public bool IsToBeDeleted => !DeleteAfterReading && DateTimeOffset.UtcNow >= CreatedAt + DeleteAfter.ToTimeSpan();
    
    private Note(
        NoteId id,
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
        Id = NoteId.New();
        Content = content;
        DeleteAfter = deleteAfter;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static Note New(
        string content,
        DeleteAfter deleteAfter)
    {
        return new(content, deleteAfter);
    }
}
