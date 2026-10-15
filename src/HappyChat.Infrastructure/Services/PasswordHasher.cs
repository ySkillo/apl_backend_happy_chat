using System.Security.Cryptography;
using HappyChat.Application.Interfaces;

namespace HappyChat.Infrastructure.Services;

/// <summary>
/// Hash de senha com PBKDF2 (SHA-256) e salt aleatório.
/// Formato salvo: iterações.salt.hash (salt e hash em Base64).
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 600_000;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(
            password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);

        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    public bool Verify(string password, string passwordHash)
    {
        var parts = passwordHash.Split('.');
        if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations))
            return false;

        var salt = Convert.FromBase64String(parts[1]);
        var expected = Convert.FromBase64String(parts[2]);

        var actual = Rfc2898DeriveBytes.Pbkdf2(
            password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);

        // Comparação em tempo constante, para não vazar informação pelo tempo de resposta.
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}
