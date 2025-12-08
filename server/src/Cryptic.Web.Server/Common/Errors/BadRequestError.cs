namespace Cryptic.Web.Server.Common.Errors;

public class BadRequestError : CodedError
{
    public const string ErrorCode = "BadRequest";

    public BadRequestError(string message) : base(ErrorCode, message) { }
}
