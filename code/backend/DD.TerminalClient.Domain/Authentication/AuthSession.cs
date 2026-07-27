using System.Text.Json;

namespace DD.TerminalClient.Domain.Authentication;

// A local, best-effort view of the profile's JWT: the opaque token plus the username and expiry read
// from its payload purely for session UX (whose data this is, and whether renewal is due). Parsing is
// never treated as validation - the server remains the only authority - so a malformed or unexpected
// token yields a session with the raw token and null metadata rather than an error.
public sealed record AuthSession
{
    private const string UsernameClaim = "name";
    private const string ExpiryClaim = "exp";

    // The inclusive Unix-second range DateTimeOffset.FromUnixTimeSeconds accepts; an exp outside it (a
    // corrupt or hostile token) is treated as no expiry so startup routes it to login instead of throwing.
    private static readonly long MinExpirySeconds = DateTimeOffset.MinValue.ToUnixTimeSeconds();
    private static readonly long MaxExpirySeconds = DateTimeOffset.MaxValue.ToUnixTimeSeconds();

    public required string Token { get; init; }

    public string? Username { get; init; }

    public DateTimeOffset? ExpiresAt { get; init; }

    // True when the token is shaped like a JWT this client can act on locally: three dot-separated
    // segments carrying a parseable numeric expiry AND a username. Startup uses this to route an absent
    // or structurally unusable token straight to masked login instead of starting online with a doomed
    // token that renewal can never refresh. The username is required because the same/different-user
    // data-owner guard can only protect the persisted cache and outbox when the token names an owner: a
    // token with no attributable owner would set the owner to null and bypass that guard, so it is
    // unusable for this client. A well-formed token is still verified by the server, so this never
    // substitutes local parsing for server validation.
    public bool HasUsableShape => ExpiresAt is not null && !string.IsNullOrWhiteSpace(Username);

    public static AuthSession FromToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var trimmed = token.Trim();
        var (username, expiresAt) = TryReadClaims(trimmed);
        return new AuthSession
        {
            Token = trimmed,
            Username = username,
            ExpiresAt = expiresAt,
        };
    }

    public bool IsExpired(DateTimeOffset now)
    {
        return ExpiresAt is { } expiry && expiry <= now;
    }

    private static (string? Username, DateTimeOffset? ExpiresAt) TryReadClaims(string token)
    {
        var segments = token.Split('.');
        if (segments.Length != 3)
        {
            return (null, null);
        }

        byte[] payload;
        try
        {
            payload = DecodeBase64Url(segments[1]);
        }
        catch (FormatException)
        {
            return (null, null);
        }

        try
        {
            using var document = JsonDocument.Parse(payload);
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                return (null, null);
            }

            return (ReadUsername(root), ReadExpiry(root));
        }
        catch (JsonException)
        {
            return (null, null);
        }
    }

    private static string? ReadUsername(JsonElement root)
    {
        return root.TryGetProperty(UsernameClaim, out var element)
               && element.ValueKind == JsonValueKind.String
            ? element.GetString()
            : null;
    }

    private static DateTimeOffset? ReadExpiry(JsonElement root)
    {
        return root.TryGetProperty(ExpiryClaim, out var element)
               && element.ValueKind == JsonValueKind.Number
               && element.TryGetInt64(out var seconds)
               && seconds >= MinExpirySeconds && seconds <= MaxExpirySeconds
            ? DateTimeOffset.FromUnixTimeSeconds(seconds)
            : null;
    }

    private static byte[] DecodeBase64Url(string value)
    {
        var normalized = value.Replace('-', '+').Replace('_', '/');
        normalized += (normalized.Length % 4) switch
        {
            2 => "==",
            3 => "=",
            _ => string.Empty,
        };

        return Convert.FromBase64String(normalized);
    }
}
