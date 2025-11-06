namespace Cryptic.Core.Common.Errors;

public abstract class PasswordNotProvidedError<TResourceId> : CodedError where TResourceId : IEquatable<TResourceId>
{
    protected PasswordNotProvidedError(string code, string message, string resourceIdKey, TResourceId resourceIdValue)
        : base(code, message)
    {
        Metadata.Add(resourceIdKey, resourceIdValue);
    }
}
