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
}

public record DeleteAfter
{
    public string Text { get; }
    public string Shorthand { get; }

    private DeleteAfter(
        string text,
        string shorthand)
    {
        Text = text;
        Shorthand = shorthand;
    }

    public static DeleteAfter Reading =>
        new("Reading", "r");

    public static DeleteAfter ReadingNoWarning =>
        new("Reading with no warning", "r-nw");

    public static DeleteAfter OneHour =>
        new("1 hour", "1hr");

    public static DeleteAfter OneDay =>
        new("1 day", "1d");

    public static DeleteAfter SevenDays =>
        new("7 days", "7d");

    public static DeleteAfter ThirtyDays =>
        new("30 days", "30d");

    public static bool TryFromShorthand(string shorthand, out DeleteAfter deleteAfter)
    {
        switch (shorthand.ToLower())
        {
            case "r":
                deleteAfter = Reading;
                return true;
            case "r-nw":
                deleteAfter = ReadingNoWarning;
                return true;
            case "1hr":
                deleteAfter = OneHour;
                return true;
            case "1d":
                deleteAfter = OneDay;
                return true;
            case "7d":
                deleteAfter = SevenDays;
                return true;
            case "30d":
                deleteAfter = ThirtyDays;
                return true;
        }

        deleteAfter = default!;
        return false;
    }
}

public class Note
{
    public NoteId Id { get; }
    public string Content { get; }
    public DeleteAfter DeleteAfter { get; }
    public DateTimeOffset CreatedAt { get; }

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
