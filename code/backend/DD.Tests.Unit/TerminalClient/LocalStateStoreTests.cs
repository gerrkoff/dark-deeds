using DD.TerminalClient.Details.Logging;
using DD.TerminalClient.Details.Storage;
using DD.TerminalClient.Domain.Models;
using DD.TerminalClient.Domain.State;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DD.Tests.Unit.TerminalClient;

// Covers the terminal client's local persistence: atomic schema-versioned state (first load, round
// trip, atomic-replace failure, malformed/unsupported/legacy schemas with outbox-preserving
// migration), the separate owner-only token file, and the bounded, secret-redacting file logger.
// Every store is rooted in a throwaway override directory cleaned up on Dispose.
public sealed class LocalStateStoreTests : IDisposable
{
    private const string Profile = "local";

    private readonly List<string> _tempRoots = [];

    [Fact]
    public void Load_WhenNoStateFile_ReturnsNull()
    {
        var paths = CreatePaths();

        var loaded = new LocalStateStore(paths, Profile).Load();

        Assert.Null(loaded);
    }

    [Fact]
    public void Save_ThenLoadOnNewStore_RoundTripsAllFields()
    {
        var paths = CreatePaths();

        new LocalStateStore(paths, Profile).Save(SampleState());
        var loaded = new LocalStateStore(paths, Profile).Load();

        Assert.NotNull(loaded);
        Assert.Equal("alice", loaded!.DataOwner);
        Assert.True(loaded.ShowCompleted);
        Assert.Equal(PersistedTerminalState.CurrentSchemaVersion, loaded.SchemaVersion);
        Assert.Equal(SampleState().CachedTasks, loaded.CachedTasks);
        Assert.Equal(SampleState().Outbox, loaded.Outbox);
    }

    [Fact]
    public void Save_StampsCurrentSchemaVersionOnDisk()
    {
        var paths = CreatePaths();

        new LocalStateStore(paths, Profile).Save(SampleState() with { SchemaVersion = 0 });

        var raw = File.ReadAllText(StateFilePath(paths));
        Assert.Contains(
            $"\"SchemaVersion\": {PersistedTerminalState.CurrentSchemaVersion}",
            raw,
            StringComparison.Ordinal);
    }

    [Fact]
    public void Save_TightensStateFileToOwnerOnly()
    {
        if (OperatingSystem.IsWindows())
        {
            return;
        }

        var paths = CreatePaths();

        new LocalStateStore(paths, Profile).Save(SampleState());

        Assert.Equal(
            UnixFileMode.UserRead | UnixFileMode.UserWrite,
            File.GetUnixFileMode(StateFilePath(paths)));
    }

    [Fact]
    public void Save_WhenTempPathBlocked_PreservesPreviousFile()
    {
        var paths = CreatePaths();
        var store = new LocalStateStore(paths, Profile);
        store.Save(SampleState());

        // A directory sitting at the temp path makes the temp write fail before the atomic replace,
        // so the previous good file must remain untouched and loadable.
        Directory.CreateDirectory(StateFilePath(paths) + ".tmp");

        var failure = Record.Exception(() => store.Save(SampleState() with { DataOwner = "changed" }));

        Assert.NotNull(failure);
        var reloaded = new LocalStateStore(paths, Profile).Load();
        Assert.NotNull(reloaded);
        Assert.Equal("alice", reloaded.DataOwner);
    }

    [Fact]
    public void Load_MalformedJson_ThrowsBlockingAndRetainsFile()
    {
        var paths = CreatePaths();
        WriteRawState(paths, "{ this is not valid json ");

        Assert.Throws<TerminalStateException>(() => new LocalStateStore(paths, Profile).Load());
        Assert.True(File.Exists(StateFilePath(paths)));
    }

    [Fact]
    public void Load_UnsupportedNewerSchema_ThrowsBlockingAndRetainsFile()
    {
        var paths = CreatePaths();
        WriteRawState(paths, "{ \"SchemaVersion\": 999, \"CachedTasks\": [], \"Outbox\": [] }");

        var exception =
            Assert.Throws<TerminalStateException>(() => new LocalStateStore(paths, Profile).Load());

        Assert.Contains("newer client", exception.Message, StringComparison.Ordinal);
        Assert.True(File.Exists(StateFilePath(paths)));
    }

    [Fact]
    public void Load_CurrentSchema_LoadsWithoutMigration()
    {
        var paths = CreatePaths();
        var json =
            $"{{ \"SchemaVersion\": {PersistedTerminalState.CurrentSchemaVersion}, " +
            "\"DataOwner\": \"carol\", \"CachedTasks\": [], \"Outbox\": [], \"ShowCompleted\": false }";
        WriteRawState(paths, json);

        var loaded = new LocalStateStore(paths, Profile).Load();

        Assert.NotNull(loaded);
        Assert.Equal("carol", loaded.DataOwner);
        Assert.Empty(loaded.Outbox);
    }

    [Fact]
    public void Load_LegacySchemaZero_MigratesAndPreservesOutbox()
    {
        var paths = CreatePaths();

        // A pre-versioning document: no SchemaVersion field, outbox under the old "PendingTasks" name.
        const string legacy = """
            {
              "DataOwner": "bob",
              "ShowCompleted": true,
              "CachedTasks": [],
              "PendingTasks": [
                { "Uid": "outbox-legacy", "Title": "Unsaved edit", "Order": 0, "Version": 4 }
              ]
            }
            """;
        WriteRawState(paths, legacy);

        var loaded = new LocalStateStore(paths, Profile).Load();

        Assert.NotNull(loaded);
        Assert.Equal(PersistedTerminalState.CurrentSchemaVersion, loaded.SchemaVersion);
        Assert.Equal("bob", loaded.DataOwner);
        Assert.True(loaded.ShowCompleted);
        var outboxItem = Assert.Single(loaded.Outbox);
        Assert.Equal("outbox-legacy", outboxItem.Uid);
        Assert.Equal(4, outboxItem.Version);
    }

    [Fact]
    public void Load_ExplicitNullOutbox_ThrowsBlockingAndRetainsFile()
    {
        var paths = CreatePaths();

        // A schema-valid document can still carry an explicit null outbox; treating it as an empty
        // outbox would silently drop unsaved edits, so it must be a blocking, file-preserving error.
        var json =
            $"{{ \"SchemaVersion\": {PersistedTerminalState.CurrentSchemaVersion}, " +
            "\"DataOwner\": \"alice\", \"CachedTasks\": [], \"Outbox\": null }";
        WriteRawState(paths, json);

        Assert.Throws<TerminalStateException>(() => new LocalStateStore(paths, Profile).Load());
        Assert.True(File.Exists(StateFilePath(paths)));
    }

    [Fact]
    public void Load_ExplicitNullCachedTasks_ThrowsBlockingAndRetainsFile()
    {
        var paths = CreatePaths();
        var json =
            $"{{ \"SchemaVersion\": {PersistedTerminalState.CurrentSchemaVersion}, " +
            "\"DataOwner\": \"alice\", \"CachedTasks\": null, \"Outbox\": [] }";
        WriteRawState(paths, json);

        Assert.Throws<TerminalStateException>(() => new LocalStateStore(paths, Profile).Load());
        Assert.True(File.Exists(StateFilePath(paths)));
    }

    [Fact]
    public void Load_WhenFileExistsButUnreadable_ThrowsBlockingAndRetainsFile()
    {
        if (OperatingSystem.IsWindows())
        {
            return;
        }

        var paths = CreatePaths();
        new LocalStateStore(paths, Profile).Save(SampleState());
        var path = StateFilePath(paths);
        File.SetUnixFileMode(path, UnixFileMode.None);

        try
        {
            // A privileged (root) host bypasses file permissions, so the read would still succeed there;
            // skip the assertion in that case rather than fail spuriously.
            if (CanRead(path))
            {
                return;
            }

            // A file that exists but cannot be read must not be treated as "no state" (which would let a
            // later save overwrite it): it surfaces as the same blocking, file-preserving error.
            Assert.Throws<TerminalStateException>(() => new LocalStateStore(paths, Profile).Load());
            Assert.True(File.Exists(path));
        }
        finally
        {
            File.SetUnixFileMode(path, UnixFileMode.UserRead | UnixFileMode.UserWrite);
        }
    }

    [Fact]
    public void Token_SaveThenLoad_RoundTripsAndClearRemoves()
    {
        var paths = CreatePaths();
        var store = new TokenStore(paths, Profile);

        Assert.Null(store.Load());

        store.Save("header.payload.signature");
        Assert.Equal("header.payload.signature", new TokenStore(paths, Profile).Load());

        store.Clear();
        Assert.Null(store.Load());
    }

    [Fact]
    public void Token_Save_TightensFileToOwnerOnly()
    {
        if (OperatingSystem.IsWindows())
        {
            return;
        }

        var paths = CreatePaths();

        new TokenStore(paths, Profile).Save("abc.def.ghi");

        Assert.Equal(
            UnixFileMode.UserRead | UnixFileMode.UserWrite,
            File.GetUnixFileMode(TokenFilePath(paths)));
    }

    [Fact]
    public void Logger_Redacts_JwtBearerAndPasswordSecrets()
    {
        var paths = CreatePaths();
        using var provider = new TerminalFileLoggerProvider(paths, Profile, TimeProvider.System);
        var logger = provider.CreateLogger("Auth");

        const string secret =
            "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJ0ZXN0In0.SIGVALUE " +
            "Authorization Bearer opaque-secret-123 " +
            "{\"password\":\"hunter2\"}";
        WriteLog(logger, secret);

        var contents = File.ReadAllText(LogFilePath(paths));
        Assert.DoesNotContain("eyJhbGciOiJIUzI1NiJ9", contents, StringComparison.Ordinal);
        Assert.DoesNotContain("opaque-secret-123", contents, StringComparison.Ordinal);
        Assert.DoesNotContain("hunter2", contents, StringComparison.Ordinal);
        Assert.Contains("***redacted***", contents, StringComparison.Ordinal);
    }

    [Fact]
    public void Logger_RollsOver_WhenSizeCapExceeded()
    {
        var paths = CreatePaths();
        using var provider =
            new TerminalFileLoggerProvider(paths, Profile, TimeProvider.System, maxBytes: 256);
        var logger = provider.CreateLogger("Cap");

        for (var index = 0; index < 50; index++)
        {
            WriteLog(logger, $"log line number {index} with some padding to grow the file");
        }

        Assert.True(File.Exists(LogFilePath(paths)));
        Assert.True(File.Exists(LogFilePath(paths) + ".1"));
        Assert.True(new FileInfo(LogFilePath(paths)).Length <= 256);
    }

    [Fact]
    public void Logger_WhenWriteFails_DoesNotThrow()
    {
        var paths = CreatePaths();
        using var provider = new TerminalFileLoggerProvider(paths, Profile, TimeProvider.System);
        var logger = provider.CreateLogger("Cap");

        // A directory sitting exactly at the log file path makes every append fail. An ILogger must never
        // surface a disk failure into its caller (which could break the hub reconnect loop or shutdown),
        // so the write is swallowed rather than thrown.
        Directory.CreateDirectory(LogFilePath(paths));

        var failure = Record.Exception(() => WriteLog(logger, "should never throw"));

        Assert.Null(failure);
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

    private static void WriteLog(ILogger logger, string message)
    {
        // Call the low-level ILogger.Log with an identity formatter so the exact message reaches the
        // file logger unchanged (the LoggerExtensions helpers are barred by the CA1848 analyzer).
        logger.Log(LogLevel.Information, new EventId(0), message, null, static (state, _) => state);
    }

    private static PersistedTerminalState SampleState()
    {
        return new PersistedTerminalState
        {
            DataOwner = "alice",
            ShowCompleted = true,
            CachedTasks =
            [
                new TerminalTask
                {
                    Uid = "cached-1",
                    Title = "Weekly review",
                    Date = new DateOnly(2026, 7, 20),
                    Time = 9 * 60,
                    Order = 2,
                    Completed = false,
                    Deleted = false,
                    Type = TerminalTaskType.Weekly,
                    IsProbable = true,
                    Version = 7,
                },
            ],
            Outbox =
            [
                new TerminalTask
                {
                    Uid = "outbox-1",
                    Title = "Unsaved edit",
                    Date = null,
                    Time = null,
                    Order = 0,
                    Completed = true,
                    Deleted = false,
                    Type = TerminalTaskType.Simple,
                    IsProbable = false,
                    Version = 3,
                },
            ],
        };
    }

    private ApplicationPathProvider CreatePaths()
    {
        return new ApplicationPathProvider(NewTempRoot());
    }

    private string NewTempRoot()
    {
        var root = Path.Combine(
            Path.GetTempPath(), "dd-terminal-state-tests", Guid.NewGuid().ToString("N"));
        _tempRoots.Add(root);
        return root;
    }

    private static string StateFilePath(ApplicationPathProvider paths)
    {
        return Path.Combine(paths.GetProfileStateDirectory(Profile), "state.json");
    }

    private static string TokenFilePath(ApplicationPathProvider paths)
    {
        return Path.Combine(paths.GetProfileStateDirectory(Profile), "token.jwt");
    }

    private static string LogFilePath(ApplicationPathProvider paths)
    {
        return Path.Combine(paths.GetProfileStateDirectory(Profile), "logs", "terminal.log");
    }

    private static void WriteRawState(ApplicationPathProvider paths, string content)
    {
        var path = StateFilePath(paths);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
    }

    private static bool CanRead(string path)
    {
        try
        {
            _ = File.ReadAllText(path);
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        catch (IOException)
        {
            return false;
        }
    }
}
