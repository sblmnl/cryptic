namespace Cryptic.WebAPI.Common.Http;

public static class HttpErrors
{
    public static readonly Error InternalError = new Error(
        "Cryptic.InternalError",
        "An internal error occurred while processing the request!");
}