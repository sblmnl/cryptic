using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Cryptic.Web.Server.Tests.Helpers;

internal static class ResultAssertions
{
    private static readonly IServiceProvider ServiceProvider = new ServiceCollection()
        .AddLogging()
        .BuildServiceProvider();

    public static async Task<(int StatusCode, T Body)> ExecuteAndDeserialize<T>(IResult result)
    {
        var context = new DefaultHttpContext
        {
            RequestServices = ServiceProvider,
        };
        var stream = new MemoryStream();
        context.Response.Body = stream;

        await result.ExecuteAsync(context);

        stream.Position = 0;
        var body = await JsonSerializer.DeserializeAsync<T>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });

        return (context.Response.StatusCode, body!);
    }
}
