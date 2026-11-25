using System.Security.Claims;

namespace WebApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static long GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var value = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

        return long.TryParse(value, out var id)
            ? id
            : throw new InvalidOperationException("No valid user ID claim. Is the endpoint missing [Authorize]?");
    }
}
