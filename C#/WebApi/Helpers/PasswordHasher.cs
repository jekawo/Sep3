using System.Security.Cryptography;

namespace WebApi.Helpers;

public class PasswordHasher
{
    private const int SaltSize = 32;
    private const int HashSize = 64;

    // PBKDF2 repeats the hashing this many times to make guessing passwords slow.
    // Changing any of these values or the algorithm means existing password hashes no longer match.
    private const int Iterations = 600000;

    public static (byte[] Salt, byte[] Hash) Hash(string password)
    {
        // A random salt per password means two users with the same password get different hashes.
        // The salt is not a secret, but it must be stored to verify the password later.
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA512, HashSize);
        return (salt, hash);
    }

    public static bool Verify(string password, byte[] salt, byte[] expectedHash)
    {
        // A hash cannot be reversed, so we hash the attempt the same way and compare the results.
        var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA512, HashSize);

        // Unlike SequenceEqual, this does not stop at the first difference, so the time it takes
        // does not tell an attacker how close a guess was.
        return CryptographicOperations.FixedTimeEquals(actual, expectedHash);
    }
}