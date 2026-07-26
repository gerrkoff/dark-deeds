using System.Text;
using DD.TerminalClient.Domain.Abstractions;

namespace DD.TerminalClient.Details.Storage;

// Persists the profile's JWT in its own owner-only file, isolated from the rest of the local state so
// a leaked state snapshot never carries credentials. The token is stored verbatim and never parsed,
// logged, or combined with other data here.
public sealed class TokenStore(ApplicationPathProvider paths, string profileName) : ITokenStore
{
    private const string TokenFileName = "token.jwt";

    private readonly string _tokenFilePath =
        Path.Combine(paths.GetProfileStateDirectory(profileName), TokenFileName);

    public string? Load()
    {
        if (!File.Exists(_tokenFilePath))
        {
            return null;
        }

        var token = File.ReadAllText(_tokenFilePath).Trim();
        return token.Length == 0 ? null : token;
    }

    public void Save(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        AtomicFileWriter.WriteAllBytes(_tokenFilePath, Encoding.UTF8.GetBytes(token));
    }

    public void Clear()
    {
        if (File.Exists(_tokenFilePath))
        {
            File.Delete(_tokenFilePath);
        }
    }
}
