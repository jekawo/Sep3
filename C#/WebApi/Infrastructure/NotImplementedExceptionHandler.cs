using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Infrastructure;

/// <summary>Reports unfinished functionality as 501 Not Implemented instead of 500.</summary>
public class NotImplementedExceptionHandler(IProblemDetailsService problemDetails) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not NotImplementedException)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status501NotImplemented;

        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status501NotImplemented,
                Detail = exception.Message
            }
        });
    }
}
