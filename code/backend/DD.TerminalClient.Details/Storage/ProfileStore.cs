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

    private static readonly Dictionary<string, string> SeededProfiles =
        new(StringComparer.Ordinal)
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
        var names = new HashSet<string>(StringComparer.Ordinal);

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
            profile = TerminalProfile.Create(name, seededUrl);
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
