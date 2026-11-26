using WebApi.Helpers;

namespace WebApi.Tests.Helpers;

public class PasswordHasherTests
{
    [Fact]
    public void Verify_AcceptsTheHashedPassword()
    {
        var (salt, hash) = PasswordHasher.Hash("Abc12!");

        Assert.True(PasswordHasher.Verify("Abc12!", salt, hash));
    }

    [Theory]
    [InlineData("abc12!")]
    [InlineData("Abc12! ")]
    [InlineData("")]
    public void Verify_RejectsAnyOtherPassword(string attempt)
    {
        var (salt, hash) = PasswordHasher.Hash("Abc12!");

        Assert.False(PasswordHasher.Verify(attempt, salt, hash));
    }

    [Fact]
    public void Hash_UsesANewSaltEveryTime()
    {
        var first = PasswordHasher.Hash("Abc12!");
        var second = PasswordHasher.Hash("Abc12!");

        Assert.NotEqual(first.Salt, second.Salt);
        Assert.NotEqual(first.Hash, second.Hash);
    }
}
