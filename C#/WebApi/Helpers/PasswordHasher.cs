using System.Security.Cryptography;

namespace WebApi.Helpers;

public class PasswordHasher
{
    private const int SaltSize = 32;
    private const int HashSize = 64;
    private const int Iterations = 600000;

    public static (byte[] Salt, byte[] Hash) Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA512, HashSize);
        return (salt, hash);
    }

    public static bool Verify(string password, byte[] salt, byte[] expectedHash)
    {
        var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA512, HashSize);
        return CryptographicOperations.FixedTimeEquals(actual, expectedHash);
    }
}