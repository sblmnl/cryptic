using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Cryptic.Web.Server.Tests;

public class ExceptionHandlerMiddlewareTests
{
    private static readonly IServiceProvider ServiceProvider = new ServiceCollection()
        .AddLogging()
        .BuildServiceProvider();

    private readonly ExceptionHandlerMiddleware _middleware;

    public ExceptionHandlerMiddlewareTests()
    {
        _middleware = new ExceptionHandlerMiddleware(NullLogger<ExceptionHandlerMiddleware>.Instance);
    }

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext
        {
            RequestServices = ServiceProvider,
        };
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<JsonElement> ReadResponseBody(DefaultHttpContext context)
    {
        context.Response.Body.Position = 0;
        return await JsonSerializer.DeserializeAsync<JsonElement>(context.Response.Body);
    }

    [Fact]
    public async Task TryHandleAsync_WithBadHttpRequestException_Returns400()
    {
        var context = CreateContext();
        var exception = new BadHttpRequestException("Invalid JSON");

        var handled = await _middleware.TryHandleAsync(context, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

        var body = await ReadResponseBody(context);
        Assert.Equal("failed", body.GetProperty("status").GetString());
    }

    [Fact]
    public async Task TryHandleAsync_WithGenericException_Returns500()
    {
        var context = CreateContext();
        var exception = new InvalidOperationException("Something broke");

        var handled = await _middleware.TryHandleAsync(context, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

        var body = await ReadResponseBody(context);
        Assert.Equal("failed", body.GetProperty("status").GetString());
    }

    [Fact]
    public async Task TryHandleAsync_AlwaysReturnsTrue()
    {
        var context = CreateContext();

        var result1 = await _middleware.TryHandleAsync(context, new BadHttpRequestException("bad"), CancellationToken.None);
        Assert.True(result1);

        context = CreateContext();
        var result2 = await _middleware.TryHandleAsync(context, new Exception("generic"), CancellationToken.None);
        Assert.True(result2);
    }
}
