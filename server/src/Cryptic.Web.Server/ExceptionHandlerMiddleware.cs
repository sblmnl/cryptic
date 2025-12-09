using Microsoft.AspNetCore.Diagnostics;

namespace Cryptic.Web.Server;

internal sealed class ExceptionHandlerMiddleware : IExceptionHandler
{
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is BadHttpRequestException)
        {
            var response = HttpResponses.Fail(new BadRequestError(exception.Message), StatusCodes.Status400BadRequest);
            await response.ExecuteAsync(httpContext);
            return true;
        }

        _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);
        await HttpResponses.InternalError.ExecuteAsync(httpContext);
        return true;
    }
}
