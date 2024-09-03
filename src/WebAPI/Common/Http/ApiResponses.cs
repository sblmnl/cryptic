namespace Cryptic.WebAPI.Common.Http;

public static class ApiResponses
{
    public static readonly IResult InternalError =
        Results.Json(ApiResponseBody.InternalError, statusCode: StatusCodes.Status500InternalServerError);
}