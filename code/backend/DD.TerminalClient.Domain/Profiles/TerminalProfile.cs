namespace DD.TerminalClient.Domain.Profiles;

// A named connection target for the terminal client: a validated profile name that is safe to use as
// a single filesystem path segment, plus a base URI normalized to exactly one trailing slash.
// Construct only through Create, which enforces both invariants, so any TerminalProfile instance is
// always safe to combine into per-profile storage paths and to use as an HTTP base address.
public sealed record TerminalProfile
{
    private static readonly char[] ForbiddenNameChars =
        Path.GetInvalidFileNameChars().Append('/').Append('\\').Distinct().ToArray();

    private TerminalProfile(string name, Uri baseUri)
    {
        Name = name;
        BaseUri = baseUri;
    }

    public string Name { get; }

    public Uri BaseUri { get; }

    public static TerminalProfile Create(string name, string baseUrl)
    {
        return new TerminalProfile(ValidateName(name), NormalizeBaseUri(baseUrl));
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Profile name must not be empty.", nameof(name));
        }

        if (name is "." or ".." || name.Contains("..", StringComparison.Ordinal))
        {
            throw new ArgumentException(
                $"Profile name '{name}' must not contain path traversal.", nameof(name));
        }

        if (name.Any(char.IsControl) || name.IndexOfAny(ForbiddenNameChars) >= 0)
        {
            throw new ArgumentException(
                $"Profile name '{name}' contains path separators or invalid filename characters.",
                nameof(name));
        }

        return name;
    }

    private static Uri NormalizeBaseUri(string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new ArgumentException("Base URL must not be empty.", nameof(baseUrl));
        }

        if (!Uri.TryCreate(baseUrl.Trim(), UriKind.Absolute, out var uri))
        {
            throw new ArgumentException(
                $"Base URL '{baseUrl}' is not a valid absolute URI.", nameof(baseUrl));
        }

        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
        {
            throw new ArgumentException(
                $"Base URL '{baseUrl}' must use http or https.", nameof(baseUrl));
        }

        // Loopback (localhost / 127.0.0.1 / ::1) is allowed over plain HTTP for local backends; every
        // other host must use HTTPS so tokens are never sent over the wire in the clear.
        if (uri.Scheme == Uri.UriSchemeHttp && !uri.IsLoopback)
        {
            throw new ArgumentException(
                $"Base URL '{baseUrl}' must use https for non-loopback hosts.", nameof(baseUrl));
        }

        // Collapse any number of trailing slashes to exactly one so the base is a stable prefix for
        // relative request paths regardless of how the user typed it.
        var authority = uri.GetLeftPart(UriPartial.Authority);
        var path = uri.AbsolutePath.Trim('/');
        var normalized = path.Length == 0 ? $"{authority}/" : $"{authority}/{path}/";
        return new Uri(normalized, UriKind.Absolute);
    }
}
