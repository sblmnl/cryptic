namespace Cryptic.Core.Common.Errors;

public abstract class ResourceNotFoundError<TResourceId> : CodedError where TResourceId : IEquatable<TResourceId>
{
    protected ResourceNotFoundError(string code, string message, string resourceIdKey, TResourceId resourceIdValue)
        : base(code, message)
    {
        Metadata.Add(resourceIdKey, resourceIdValue);
    }
}
