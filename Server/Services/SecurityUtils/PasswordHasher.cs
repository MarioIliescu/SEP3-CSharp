namespace FleetWebApi.SecurityUtils;

using System;
using System.Security.Cryptography;
using System.Text;
/// <summary>
/// Password hasher for secure passwords
/// </summary>
public static class PasswordHasher
{
    // Number of iterations for PBKDF2 (high enough for security)
    private const int Iterations = 100_000;

    // Salt size in bytes
    private const int SaltSize = 16;

    // Hash size in bytes
    private const int HashSize = 32;

    /// <summary>
    /// Generates a salted hash from a plaintext password.
    /// Format: {Base64Salt}.{Base64Hash}
    /// </summary>
    /// <param name="password">Plaintext password</param>
    /// <returns>Salted hash string</returns>
    public static string Hash(string password)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));

        // Generate a random salt
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        // Derive the hash
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize
        );

        // Combine salt and hash as Base64
        string saltBase64 = Convert.ToBase64String(salt);
        string hashBase64 = Convert.ToBase64String(hash);

        return $"{saltBase64}.{hashBase64}";
    }

    /// <summary>
    /// Verifies a plaintext password against a stored salted hash.
    /// </summary>
    /// <param name="password">Plaintext password</param>
    /// <param name="storedHash">Stored salted hash from DB</param>
    /// <returns>True if password matches, false otherwise</returns>
    public static bool Verify(string password, string storedHash)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));
        if (storedHash == null) throw new ArgumentNullException(nameof(storedHash));

        // Split stored value into salt and hash
        var parts = storedHash.Split('.', 2);
        if (parts.Length != 2) return false;

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] expectedHash = Convert.FromBase64String(parts[1]);

        // Compute hash of provided password with the stored salt
        byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize
        );

        // Use fixed-time comparison to prevent timing attacks
        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}