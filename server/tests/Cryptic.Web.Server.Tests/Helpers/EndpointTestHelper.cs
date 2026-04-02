using Microsoft.AspNetCore.Http;

namespace Cryptic.Web.Server.Tests.Helpers;

internal static class EndpointTestHelper
{
    public static HttpContext CreateHttpContext(CancellationToken cancellationToken = default)
    {
        var context = new DefaultHttpContext();
        context.RequestAborted = cancellationToken;
        return context;
    }
}
