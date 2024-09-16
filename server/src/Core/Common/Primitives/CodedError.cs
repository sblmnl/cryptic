namespace Cryptic.Core.Common.Primitives;

public record CodedError
{
    public required string Code { get; init; }
    public required string Message { get; init; }
}
