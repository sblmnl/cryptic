using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Features.Notes;

public class DeleteAfterNotFoundException : Exception
{
    public DeleteAfterNotFoundException() : base()
    {
    }
}

public record DeleteAfter
{
    public static readonly DeleteAfter Reading = new("r");
    public static readonly DeleteAfter ReadingNoWarning = new("r!");
    public static readonly DeleteAfter OneHour = new("1hr", TimeSpan.FromHours(1));
    public static readonly DeleteAfter OneDay = new("1d", TimeSpan.FromDays(1));
    public static readonly DeleteAfter SevenDays = new("7d", TimeSpan.FromDays(7));
    public static readonly DeleteAfter ThirtyDays = new("30d", TimeSpan.FromDays(30));
    
    public string Shorthand { get; } = string.Empty;
    public TimeSpan? DeleteIn { get; }

    public DeleteAfter(string shorthand, TimeSpan? deleteIn = null)
    {
        Shorthand = shorthand;
        DeleteIn = deleteIn;
    }
    
    public static IEnumerable<DeleteAfter> GetOptions()
    {
        return new List<DeleteAfter>()
        {
            Reading,
            ReadingNoWarning,
            OneHour,
            OneDay,
            SevenDays,
            ThirtyDays
        };
    }
    
    public static bool TryFromShorthand(
        string shorthand,
        out DeleteAfter? deleteAfter)
    {
        switch (shorthand.ToLower())
        {
            case "r":
                deleteAfter = Reading;
                return true;
            case "r!":
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
            default:
                deleteAfter = null;
                return false;
        }
    }

    public static DeleteAfter FromShorthand(string shorthand)
    {
        return shorthand.ToLower() switch
        {
            "r"   => Reading,
            "r!"  => ReadingNoWarning,
            "1hr" => OneHour,
            "1d"  => OneDay,
            "7d"  => SevenDays,
            "30d" => ThirtyDays,
            _     => throw new DeleteAfterNotFoundException()
        };
    }

    public override string ToString() => Shorthand;
}
