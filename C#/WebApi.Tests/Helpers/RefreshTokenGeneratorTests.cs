using WebApi.Helpers;

namespace WebApi.Tests.Helpers;

public class RefreshTokenGeneratorTests
{
    [Fact]
    public void Generate_ReturnsADifferentTokenEveryTime() =>
        Assert.NotEqual(RefreshTokenGenerator.Generate(), RefreshTokenGenerator.Generate());

    [Fact]
    public void Generate_ReturnsAtLeast32RandomBytes() =>
        Assert.True(Convert.FromBase64String(RefreshTokenGenerator.Generate()).Length >= 32);

    [Fact]
    public void Hash_IsTheSameForTheSameToken() =>
        Assert.Equal(RefreshTokenGenerator.Hash("token"), RefreshTokenGenerator.Hash("token"));

    [Fact]
    public void Hash_DiffersBetweenTokens() =>
        Assert.NotEqual(RefreshTokenGenerator.Hash("token"), RefreshTokenGenerator.Hash("Token"));

    [Fact]
    public void Hash_DoesNotContainTheToken()
    {
        var token = RefreshTokenGenerator.Generate();

        Assert.NotEqual(token, RefreshTokenGenerator.Hash(token));
    }
}
