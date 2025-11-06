namespace Cryptic.Core.Common.Errors;

public class CodedError : IError
{
    public string Code { get; }
    public string Message { get; }
    public Dictionary<string, object> Metadata { get; } = [];
    public List<IError> Reasons { get; } = [];

    protected CodedError(string code, string message, Dictionary<string, object> metadata, List<IError> reasons)
    {
        Code = code;
        Message = message;
        Metadata = metadata;
        Reasons = reasons;
    }

    protected CodedError(string code, string message)
    {
        Code = code;
        Message = message;
    }
}
