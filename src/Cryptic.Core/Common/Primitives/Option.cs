namespace Cryptic.Core.Common.Primitives;

public abstract record Option<T>
{
    public record Some(T Value) : Option<T>;
    public record None : Option<T>;
}
