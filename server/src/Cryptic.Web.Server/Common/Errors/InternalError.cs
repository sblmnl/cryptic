namespace Cryptic.Web.Server.Common.Errors;

public class InternalError : CodedError
{
    public const string ErrorCode = "InternalError";
    public const string ErrorMessage = "An error occurred while processing the request!";

    public InternalError() : base(ErrorCode, ErrorMessage) { }
}
