using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using DD.TerminalClient.Domain.Abstractions;
using DD.TerminalClient.Domain.Profiles;

namespace DD.TerminalClient.Details.Storage;

// Seeds the built-in production/test/local profiles and persists user-created profiles, one JSON file
// per profile inside that profile's own config directory. Names are validated and base URIs
// normalized by TerminalProfile.Create, so a resolved profile is always safe to turn into storage
// paths. Seeded names are reserved and cannot be shadowed by a saved profile.
public sealed class ProfileStore(ApplicationPathProvider paths) : IProfileStore
{
    private const string ProfilesFolderName = "profiles";
    private const string ProfileFileName = "profile.json";

    // Seeded names are reserved and matched case-insensitively: a profile-name segment maps directly to a
    // filesystem directory, and on a case-insensitive volume (the default on macOS, a primary target)
    // "Production" and "production" are the same directory. A case-only variant must therefore resolve to
    // the same built-in profile and be refused as a custom name, so a custom profile can never alias a
    // seeded profile's token/state directory while pointing at a different server.
    private static readonly Dictionary<string, string> SeededProfiles =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["production"] = "https://dark-deeds.com/",
            ["test"] = "https://test.dark-deeds.com/",
            ["local"] = "http://localhost:5000/",
        };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };

    public IReadOnlyList<TerminalProfile> List()
    {
        var profiles = new List<TerminalProfile>();
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var (name, baseUrl) in SeededProfiles)
        {
            profiles.Add(TerminalProfile.Create(name, baseUrl));
            names.Add(name);
        }

        var profilesRoot = Path.Combine(paths.ConfigRoot, ProfilesFolderName);
        if (Directory.Exists(profilesRoot))
        {
            foreach (var directory in Directory.EnumerateDirectories(profilesRoot))
            {
                var name = Path.GetFileName(directory);
                if (names.Contains(name))
                {
                    continue;
                }

                var profile = ReadCustomProfile(name);
                if (profile is not null)
                {
                    profiles.Add(profile);
                    names.Add(name);
                }
            }
        }

        return profiles;
    }

    public bool TryResolve(string name, [NotNullWhen(true)] out TerminalProfile? profile)
    {
        if (SeededProfiles.TryGetValue(name, out var seededUrl))
        {
            // Resolve to the canonical seeded name (the built-in key) rather than whatever casing the
            // caller passed, so every case variant of a built-in profile shares the one storage directory
            // instead of spawning an alias beside it.
            var canonical = SeededProfiles.Keys.First(
                key => string.Equals(key, name, StringComparison.OrdinalIgnoreCase));
            profile = TerminalProfile.Create(canonical, seededUrl);
            return true;
        }

        profile = ReadCustomProfile(name);
        return profile is not null;
    }

    public TerminalProfile Save(string name, string baseUrl)
    {
        var profile = TerminalProfile.Create(name, baseUrl);

        if (SeededProfiles.ContainsKey(profile.Name))
        {
            throw new ArgumentException(
                $"Profile name '{profile.Name}' is reserved by a built-in profile.", nameof(name));
        }

        var directory = paths.GetProfileConfigDirectory(profile.Name);
        Directory.CreateDirectory(directory);

        var document = new PersistedProfile(profile.BaseUri.AbsoluteUri);
        File.WriteAllText(
            Path.Combine(directory, ProfileFileName), JsonSerializer.Serialize(document, JsonOptions));

        return profile;
    }

    private TerminalProfile? ReadCustomProfile(string name)
    {
        string directory;
        try
        {
            directory = paths.GetProfileConfigDirectory(name);
        }
        catch (ArgumentException)
        {
            return null;
        }

        var file = Path.Combine(directory, ProfileFileName);
        if (!File.Exists(file))
        {
            return null;
        }

        PersistedProfile? document;
        try
        {
            document = JsonSerializer.Deserialize<PersistedProfile>(File.ReadAllText(file));
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException(
                $"Profile '{name}' is stored in an unreadable format at '{file}'.", exception);
        }

        if (document is null || string.IsNullOrWhiteSpace(document.BaseUrl))
        {
            throw new InvalidOperationException(
                $"Profile '{name}' is missing a base URL at '{file}'.");
        }

        return TerminalProfile.Create(name, document.BaseUrl);
    }

    private sealed record PersistedProfile(string BaseUrl);
}
