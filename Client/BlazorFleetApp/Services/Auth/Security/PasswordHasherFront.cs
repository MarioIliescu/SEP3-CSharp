using System.Security.Cryptography;
using System.Text;

namespace BlazorFleetApp.Services.Auth.Security;

public class PasswordHasherFront
{
    /// <summary>
    /// Computes a SHA-256 hash of the password
    /// </summary>
    public static string Hash(string password)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));

        using var sha256 = SHA256.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(password);
        byte[] hashBytes = sha256.ComputeHash(bytes);

        return Convert.ToBase64String(hashBytes);
    }
}