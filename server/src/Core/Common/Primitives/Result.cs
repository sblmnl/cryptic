namespace Cryptic.Core.Common.Primitives;

public abstract record Result<T>
{
    public record Ok(T Value) : Result<T>;
    public record Fail(CodedError Error) : Result<T>;
}
