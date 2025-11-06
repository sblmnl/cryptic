namespace Cryptic.Core.Common.Errors;

public abstract class IncorrectPasswordError<TResourceId> : CodedError where TResourceId : IEquatable<TResourceId>
{
    protected IncorrectPasswordError(string code, string message, string resourceIdKey, TResourceId resourceIdValue)
        : base(code, message)
    {
        Metadata.Add(resourceIdKey, resourceIdValue);
    }
}
