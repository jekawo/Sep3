using System.Security.Claims;
using WebApi.Extensions;

namespace WebApi.Tests.Extensions;

public class ClaimsPrincipalExtensionsTests
{
    private static ClaimsPrincipal PrincipalWith(params Claim[] claims) => new(new ClaimsIdentity(claims));

    [Fact]
    public void GetUserId_ReturnsNameIdentifierClaim()
    {
        var principal = PrincipalWith(new Claim(ClaimTypes.NameIdentifier, "42"));

        Assert.Equal(42, principal.GetUserId());
    }

    [Fact]
    public void GetUserId_ThrowsWithoutClaim() =>
        Assert.Throws<InvalidOperationException>(() => PrincipalWith().GetUserId());

    [Fact]
    public void GetUserId_ThrowsWhenClaimIsNotANumber()
    {
        var principal = PrincipalWith(new Claim(ClaimTypes.NameIdentifier, "alice"));

        Assert.Throws<InvalidOperationException>(() => principal.GetUserId());
    }
}
