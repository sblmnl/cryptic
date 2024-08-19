namespace Cryptic.WebAPI.Common.Extensions;

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