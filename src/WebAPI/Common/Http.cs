using System.Web;

namespace WebAPI.Common;

public record Link(string Href, string Rel, string Method);

public class HttpResponseBody
{
    public object? Data { get; }
    public IReadOnlyCollection<Error>? Errors { get; } = new List<Error>();
    public IReadOnlyCollection<Link>? Links { get; } = new List<Link>();
    
    public HttpResponseBody() { }

    public HttpResponseBody(object? data, IReadOnlyCollection<Link>? links = default)
    {
        Data = data;
        Links = links ?? new List<Link>();
    }

    public HttpResponseBody(IReadOnlyCollection<Error> errors, IReadOnlyCollection<Link>? links = default)
    {
        Errors = errors;
        Links = links ?? new List<Link>();
    }
    
    public HttpResponseBody(Error error, IReadOnlyCollection<Link>? links = default)
    {
        Errors = new List<Error> { error };
        Links = links ?? new List<Link>();
    }
}

public static class HttpExtensions
{
    public static Uri GetBaseUri(this HttpRequest request)
    {
        var protocol = request.IsHttps ? "https" : "http";

        return new Uri($"{protocol}://{request.Host}{request.Path}");
    }

    public static Uri WithQueryParam(this Uri uri, string name, string value = "")
    {
        var queryParams = HttpUtility.ParseQueryString(uri.Query);
        queryParams.Add(name, value);
        
        var builder = new UriBuilder(uri)
        {
            Query = queryParams.ToString()
        };

        return builder.Uri;
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
