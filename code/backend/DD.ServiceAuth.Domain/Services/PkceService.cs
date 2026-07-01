using System.Security.Cryptography;
using System.Text;

namespace DD.ServiceAuth.Domain.Services;

public interface IPkceService
{
    bool Verify(string codeVerifier, string codeChallenge);
}

internal sealed class PkceService : IPkceService
{
    public bool Verify(string codeVerifier, string codeChallenge)
    {
        if (string.IsNullOrEmpty(codeVerifier) || string.IsNullOrEmpty(codeChallenge))
        {
            return false;
        }

        var verifierBytes = Encoding.ASCII.GetBytes(codeVerifier);
        var hash = SHA256.HashData(verifierBytes);
        var computedChallenge = Base64UrlEncode(hash);

        var computedBytes = Encoding.ASCII.GetBytes(computedChallenge);
        var providedBytes = Encoding.ASCII.GetBytes(codeChallenge);
        return CryptographicOperations.FixedTimeEquals(computedBytes, providedBytes);
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
