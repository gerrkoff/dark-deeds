using DD.TerminalClient.Details.Storage;
using Xunit;

namespace DD.Tests.Unit.TerminalClient;

// Covers profile seeding, user-created profile persistence, per-profile path isolation, the test/
// self-test root override, profile-name validation, the loopback-only HTTP exception, and base-URI
// normalization. Every store is rooted in a throwaway override directory cleaned up on Dispose.
public sealed class ProfileStoreTests : IDisposable
{
    private readonly List<string> _tempRoots = [];

    [Theory]
    [InlineData("production", "https://dark-deeds.com/")]
    [InlineData("test", "https://test.dark-deeds.com/")]
    [InlineData("local", "http://localhost:5000/")]
    public void TryResolve_SeededProfile_ReturnsExactBaseUri(string name, string expectedUrl)
    {
        var store = CreateStore();

        var resolved = store.TryResolve(name, out var profile);

        Assert.True(resolved);
        Assert.Equal(name, profile!.Name);
        Assert.Equal(expectedUrl, profile.BaseUri.AbsoluteUri);
    }

    [Fact]
    public void List_WithoutCustomProfiles_ReturnsOnlySeededProfiles()
    {
        var store = CreateStore();

        var names = store.List().Select(profile => profile.Name).ToList();

        Assert.Equal(3, names.Count);
        Assert.Contains("production", names);
        Assert.Contains("test", names);
        Assert.Contains("local", names);
    }

    [Fact]
    public void TryResolve_UnknownProfile_ReturnsFalse()
    {
        var store = CreateStore();

        Assert.False(store.TryResolve("does-not-exist", out var profile));
        Assert.Null(profile);
    }

    [Fact]
    public void Save_ThenResolveOnNewStore_ReturnsPersistedCustomProfile()
    {
        var paths = CreatePaths();
        var store = new ProfileStore(paths);

        var saved = store.Save("work", "https://tasks.example.com");
        Assert.Equal("https://tasks.example.com/", saved.BaseUri.AbsoluteUri);

        var reloaded = new ProfileStore(paths);
        Assert.True(reloaded.TryResolve("work", out var profile));
        Assert.Equal("https://tasks.example.com/", profile.BaseUri.AbsoluteUri);
        Assert.Contains(reloaded.List(), listed => listed.Name == "work");
    }

    [Theory]
    [InlineData("production")]
    [InlineData("test")]
    [InlineData("local")]
    public void Save_SeededName_IsRejected(string name)
    {
        var store = CreateStore();

        Assert.Throws<ArgumentException>(() => store.Save(name, "https://example.com/"));
    }

    [Theory]
    [InlineData("Production")]
    [InlineData("PRODUCTION")]
    [InlineData("Local")]
    public void Save_SeededNameCaseVariant_IsRejected(string name)
    {
        // A profile-name segment maps directly to a filesystem directory; on a case-insensitive volume
        // (the default on macOS) a case variant of a seeded name would alias the built-in profile's token
        // and state, so it must be refused just like the exact seeded name.
        var store = CreateStore();

        Assert.Throws<ArgumentException>(() => store.Save(name, "https://example.com/"));
    }

    [Theory]
    [InlineData("Production", "production", "https://dark-deeds.com/")]
    [InlineData("LOCAL", "local", "http://localhost:5000/")]
    public void TryResolve_SeededNameCaseVariant_ResolvesToCanonicalProfile(
        string requested, string canonical, string expectedUrl)
    {
        var store = CreateStore();

        Assert.True(store.TryResolve(requested, out var profile));
        Assert.Equal(canonical, profile.Name);
        Assert.Equal(expectedUrl, profile.BaseUri.AbsoluteUri);
    }

    [Fact]
    public void ProfileDirectories_AreIsolatedPerProfileUnderRoots()
    {
        var paths = CreatePaths();

        var configAlpha = paths.GetProfileConfigDirectory("alpha");
        var configBeta = paths.GetProfileConfigDirectory("beta");
        var stateAlpha = paths.GetProfileStateDirectory("alpha");
        var stateBeta = paths.GetProfileStateDirectory("beta");

        Assert.NotEqual(configAlpha, configBeta);
        Assert.NotEqual(stateAlpha, stateBeta);
        Assert.NotEqual(configAlpha, stateAlpha);
        Assert.StartsWith(paths.ConfigRoot, configAlpha, StringComparison.Ordinal);
        Assert.StartsWith(paths.StateRoot, stateAlpha, StringComparison.Ordinal);
    }

    [Fact]
    public void ApplicationPathProvider_WithRootOverride_PlacesConfigAndStateUnderOverride()
    {
        var root = NewTempRoot();

        var paths = new ApplicationPathProvider(root);

        var fullRoot = Path.GetFullPath(root);
        Assert.StartsWith(fullRoot, paths.ConfigRoot, StringComparison.Ordinal);
        Assert.StartsWith(fullRoot, paths.StateRoot, StringComparison.Ordinal);
        Assert.NotEqual(paths.ConfigRoot, paths.StateRoot);
    }

    [Fact]
    public void ApplicationPathProvider_WithoutOverride_UsesDataDirectoryBesideExecutable()
    {
        var paths = new ApplicationPathProvider();
        var expectedRoot = Path.Combine(AppContext.BaseDirectory, "data");

        Assert.Equal(expectedRoot, paths.ConfigRoot);
        Assert.Equal(expectedRoot, paths.StateRoot);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(".")]
    [InlineData("..")]
    [InlineData("../escape")]
    [InlineData("nested/name")]
    [InlineData("nested\\name")]
    [InlineData("bad\0name")]
    public void Save_InvalidProfileName_IsRejected(string name)
    {
        var store = CreateStore();

        Assert.Throws<ArgumentException>(() => store.Save(name, "https://example.com/"));
    }

    [Theory]
    [InlineData("http://localhost:5000/")]
    [InlineData("http://127.0.0.1:8080/")]
    [InlineData("http://[::1]:5000/")]
    public void Save_LoopbackHttp_IsAllowed(string url)
    {
        var store = CreateStore();

        var profile = store.Save("loopback", url);

        Assert.Equal(Uri.UriSchemeHttp, profile.BaseUri.Scheme);
        Assert.True(profile.BaseUri.IsLoopback);
    }

    [Theory]
    [InlineData("http://dark-deeds.com/")]
    [InlineData("http://example.com:8080/")]
    public void Save_RemoteHttp_IsRejected(string url)
    {
        var store = CreateStore();

        Assert.Throws<ArgumentException>(() => store.Save("remote", url));
    }

    [Theory]
    [InlineData("not-a-uri")]
    [InlineData("ftp://example.com/")]
    public void Save_NonHttpBaseUrl_IsRejected(string url)
    {
        var store = CreateStore();

        Assert.Throws<ArgumentException>(() => store.Save("scheme", url));
    }

    [Theory]
    [InlineData("https://example.com", "https://example.com/")]
    [InlineData("https://example.com/", "https://example.com/")]
    [InlineData("https://example.com//", "https://example.com/")]
    [InlineData("https://example.com/api", "https://example.com/api/")]
    [InlineData("https://example.com/api/", "https://example.com/api/")]
    [InlineData("  https://example.com  ", "https://example.com/")]
    public void Save_NormalizesBaseUriToSingleTrailingSlash(string input, string expected)
    {
        var store = CreateStore();

        var profile = store.Save("normalize", input);

        Assert.Equal(expected, profile.BaseUri.AbsoluteUri);
    }

    public void Dispose()
    {
        foreach (var root in _tempRoots)
        {
            try
            {
                if (Directory.Exists(root))
                {
                    Directory.Delete(root, recursive: true);
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }

    private ProfileStore CreateStore()
    {
        return new ProfileStore(CreatePaths());
    }

    private ApplicationPathProvider CreatePaths()
    {
        return new ApplicationPathProvider(NewTempRoot());
    }

    private string NewTempRoot()
    {
        var root = Path.Combine(
            Path.GetTempPath(), "dd-terminal-profile-tests", Guid.NewGuid().ToString("N"));
        _tempRoots.Add(root);
        return root;
    }
}
