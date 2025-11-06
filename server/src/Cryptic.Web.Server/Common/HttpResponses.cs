namespace Cryptic.Web.Server.Common;

public static class HttpResponses
{
    public static readonly IResult InternalError =
        Results.Json(
            HttpResponseBody.Fail(HttpErrors.InternalError),
            statusCode: StatusCodes.Status500InternalServerError);

    public static IResult Ok(object? data = null, int statusCode = StatusCodes.Status200OK) =>
        Results.Json(HttpResponseBody.Ok(data), statusCode: statusCode);

    public static IResult Ok(int statusCode) => Ok(null, statusCode);

    public static IResult Fail(CodedError error, int statusCode) =>
        Results.Json(HttpResponseBody.Fail(error), statusCode: statusCode);

    public static IResult Fail(ICollection<CodedError> errors, int statusCode) =>
        Results.Json(HttpResponseBody.Fail(errors), statusCode: statusCode);
}
