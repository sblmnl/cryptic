using Microsoft.AspNetCore.Diagnostics;

namespace Cryptic.WebAPI.Common.Middleware;

public class ExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ExceptionHandler> _logger;
    
    public ExceptionHandler(ILogger<ExceptionHandler> logger)
    {
        _logger = logger;
    }
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);


        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(ApiResponseBody.InternalError, cancellationToken);
        
        return true;
    }
}