using System.Text.Json.Serialization;

namespace Cryptic.WebAPI.Common;

public record MetaLink
{
    public string Href { get; init; }
    public string Rel { get; init; }
    public string Method { get; init; }

    public MetaLink(string href, string rel, string method)
    {
        Href = href;
        Rel = rel;
        Method = method;
    }
    
    public MetaLink(Uri href, string rel, string method)
    {
        Href = href.ToString();
        Rel = rel;
        Method = method;
    }
}

public class MetaInformation
{
    public IReadOnlyCollection<MetaLink> Links { get; init; } = new List<MetaLink>();
}

public class ApiResponseBody
{
    public static readonly ApiResponseBody InternalError = new ApiResponseBody
    {
        Data = null,
        Errors = [ HttpErrors.InternalError ]
    };
    
    public string Status => Errors.Any() ? "error" : "ok";
    public object? Data { get; init; } = null;
    public IEnumerable<CodedError> Errors { get; init; } = [];
    public IEnumerable<string> Warnings { get; init; } = [];
    
    [JsonPropertyName("_meta")]
    public MetaInformation Meta { get; init; } = new();
}

public static class ApiResponses
{
    public static readonly IResult InternalError =
        Results.Json(ApiResponseBody.InternalError, statusCode: StatusCodes.Status500InternalServerError);
}

public static class HttpErrors
{
    public static readonly CodedError InternalError = new()
    {
        Code = "Cryptic.InternalError",
        Message = "An internal error occurred while processing the request!"
    };
}

public static class HttpRequestExtensions
{
    public static Uri GetBaseUri(this HttpRequest request)
    {
        var protocol = request.IsHttps ? "https" : "http";

        return new Uri($"{protocol}://{request.Host}{request.Path}");
    }

    public static bool TryGetAuthorization(this HttpRequest request, out string? authorization)
    {
        if (request.Headers.TryGetValue("Authorization", out var headerValue)
            && headerValue is [not null, ..])
        {
            authorization = headerValue[0];
            return true;
        }

        authorization = null;
        return false;
    }
}
