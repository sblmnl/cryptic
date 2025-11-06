namespace Cryptic.Core.Common.Errors;

public abstract class IncorrectControlTokenError<TResourceId> : CodedError where TResourceId : IEquatable<TResourceId>
{
    protected IncorrectControlTokenError(string code, string message, string resourceIdKey, TResourceId resourceIdValue)
        : base(code, message)
    {
        Metadata.Add(resourceIdKey, resourceIdValue);
    }
}
