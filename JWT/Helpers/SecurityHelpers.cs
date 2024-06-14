using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace JWT.Helpers;

public static class SecurityHelpers
{
    public static Tuple<string, string> GetHashedPasswordAndSalt(string password)
    {
        var salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password,
            salt,
            KeyDerivationPrf.HMACSHA1,
            10000,
            256 / 8));

        var saltBase64 = Convert.ToBase64String(salt);

        return new Tuple<string, string>(hashed, saltBase64);
    }

    public static string GetHashedPasswordWithSalt(string password, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);

        var currentHashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password,
            saltBytes,
            KeyDerivationPrf.HMACSHA1,
            10000,
            256 / 8));

        return currentHashedPassword;
    }
}