namespace Cryptic.Core.Common.Types;

public record DeleteAfter
{
    public bool Receipt { get; init; }
    public DateTimeOffset Time { get; init; }
}
