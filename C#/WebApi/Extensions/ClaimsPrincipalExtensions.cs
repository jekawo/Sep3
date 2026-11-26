using System.Security.Claims;

namespace WebApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    // The "this" keyword makes this an extension method, so controllers can write User.GetUserId().
    // The id comes from the validated token and never from the request body or URL,
    // which means a caller cannot act as another user by sending a different id.
    public static long GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var value = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

        return long.TryParse(value, out var id)
            ? id
            : throw new InvalidOperationException("No valid user ID claim. Is the endpoint missing [Authorize]?");
    }
}
