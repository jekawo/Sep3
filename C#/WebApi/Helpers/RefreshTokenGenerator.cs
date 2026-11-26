using System.Security.Cryptography;
using System.Text;

namespace WebApi.Helpers;

public static class RefreshTokenGenerator
{
    private const int TokenSize = 64;

    // A refresh token is only a long random string. Unlike the JWT it contains no information,
    // so it means nothing without the matching entry in storage.
    public static string Generate() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(TokenSize));

    // Only the hash is stored, so a leaked database does not hand out usable tokens.
    // A fast hash without salt is enough here, unlike for passwords: the token is too random
    // to guess, and the same token must give the same hash so it can be looked up.
    public static string Hash(string token) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
