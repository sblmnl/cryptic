namespace Cryptic.Shared.Common.Primitives;

public abstract record Result<T>
{
    public record Success(T Value) : Result<T>;
    public record Failure(Error Error) : Result<T>;
}
