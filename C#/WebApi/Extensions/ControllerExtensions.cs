using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Extensions;

public static class ControllerExtensions
{
    extension(ControllerBase controller)
    {
        /// <summary>Translates an unsuccessful service result into the matching HTTP response.</summary>
        public IActionResult Failure(ServiceResult result) => result.Status switch
        {
            ServiceStatus.NotFound => controller.Problem(result.Error, statusCode: StatusCodes.Status404NotFound),
            ServiceStatus.Forbidden => controller.Forbid(),
            ServiceStatus.Conflict => controller.Problem(result.Error, statusCode: StatusCodes.Status409Conflict),
            ServiceStatus.Invalid => controller.Problem(result.Error,
                statusCode: StatusCodes.Status422UnprocessableEntity),
            _ => throw new InvalidOperationException($"{result.Status} is not a failure.")
        };
    }
}
