namespace Cryptic.Web.Server.Common;

public abstract record HttpResponseBody(string Status)
{
    public static HttpResponseBody Ok(object? data = null) => new OkHttpResponseBody(data);
    public static HttpResponseBody Ok<T>(T? data) => new OkHttpResponseBody<T>(data);
    public static HttpResponseBody Fail(ICollection<CodedError> errors) => new FailedHttpResponseBody(errors);
    public static HttpResponseBody Fail(CodedError error) => Fail([error]);

}

public record OkHttpResponseBody(object? Data) : HttpResponseBody("ok");
public record OkHttpResponseBody<T>(T? Data) : HttpResponseBody("ok");
public record FailedHttpResponseBody(ICollection<CodedError> Errors) : HttpResponseBody("failed");
