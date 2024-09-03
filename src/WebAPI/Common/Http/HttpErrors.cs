namespace Cryptic.WebAPI.Common.Http;

public static class HttpErrors
{
    public static readonly CodedError InternalError = new()
    {
        Code = "Cryptic.InternalError",
        Message = "An internal error occurred while processing the request!"
    };
}