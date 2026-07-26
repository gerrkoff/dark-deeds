using System.Text.Json;
using System.Text.Json.Nodes;
using DD.TerminalClient.Domain.Abstractions;
using DD.TerminalClient.Domain.State;

namespace DD.TerminalClient.Details.Storage;

// Persists one profile's local state as a single JSON file, written atomically and owner-only. Load
// runs the incoming document through an explicit, ordered migration pipeline before deserializing, so
// older layouts upgrade forward without ever dropping the durable outbox. Malformed JSON and
// newer-than-supported schemas are never silently discarded: the file is retained and a blocking
// TerminalStateException is thrown for the caller to surface.
public sealed class LocalStateStore(ApplicationPathProvider paths, string profileName) : ILocalStateStore
{
    private const string StateFileName = "state.json";
    private const string SchemaVersionProperty = "SchemaVersion";
    private const string LegacyOutboxProperty = "PendingTasks";
    private const string OutboxProperty = "Outbox";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };

    // Ordered upgrade steps keyed by the schema they migrate away from; each step must preserve the
    // outbox. Schema 0 = pre-versioning documents (no SchemaVersion field) whose outbox lived under
    // the earlier "PendingTasks" name.
    private static readonly StateMigration[] Migrations =
    [
        new StateMigration(0, MigrateSchema0ToSchema1),
    ];

    private readonly string _stateFilePath =
        Path.Combine(paths.GetProfileStateDirectory(profileName), StateFileName);

    public PersistedTerminalState? Load()
    {
        if (!File.Exists(_stateFilePath))
        {
            return null;
        }

        return Deserialize(File.ReadAllText(_stateFilePath), _stateFilePath);
    }

    public void Save(PersistedTerminalState state)
    {
        var stamped = state with { SchemaVersion = PersistedTerminalState.CurrentSchemaVersion };
        var bytes = JsonSerializer.SerializeToUtf8Bytes(stamped, JsonOptions);
        AtomicFileWriter.WriteAllBytes(_stateFilePath, bytes);
    }

    private static PersistedTerminalState Deserialize(string json, string path)
    {
        var root = ParseObject(json, path);
        var version = ReadSchemaVersion(root, path);

        if (version > PersistedTerminalState.CurrentSchemaVersion)
        {
            throw Blocking(
                path, $"it was written by a newer client (schema {version}); update the terminal client");
        }

        root = Migrate(root, version, path);

        try
        {
            return root.Deserialize<PersistedTerminalState>(JsonOptions)
                ?? throw Blocking(path, "it is empty");
        }
        catch (JsonException exception)
        {
            throw Blocking(path, "its contents do not match the expected layout", exception);
        }
    }

    private static JsonObject Migrate(JsonObject root, int version, string path)
    {
        while (version < PersistedTerminalState.CurrentSchemaVersion)
        {
            var migration = Array.Find(Migrations, candidate => candidate.FromVersion == version)
                ?? throw Blocking(path, $"no upgrade path exists from schema {version}");
            root = migration.Upgrade(root);
            version++;
            root[SchemaVersionProperty] = version;
        }

        return root;
    }

    private static JsonObject MigrateSchema0ToSchema1(JsonObject legacy)
    {
        // The outbox is the one thing an upgrade must never drop: carry the legacy "PendingTasks"
        // array over to its schema-1 "Outbox" name with its contents untouched.
        if (legacy[OutboxProperty] is null && legacy[LegacyOutboxProperty] is JsonNode pending)
        {
            legacy[OutboxProperty] = pending.DeepClone();
        }

        legacy.Remove(LegacyOutboxProperty);
        return legacy;
    }

    private static JsonObject ParseObject(string json, string path)
    {
        try
        {
            return JsonNode.Parse(json) as JsonObject
                ?? throw Blocking(path, "it is not a JSON object");
        }
        catch (JsonException exception)
        {
            throw Blocking(path, "it is not valid JSON", exception);
        }
    }

    private static int ReadSchemaVersion(JsonObject root, string path)
    {
        // An absent version marks a pre-versioning (schema 0) document.
        if (!root.TryGetPropertyValue(SchemaVersionProperty, out var node) || node is null)
        {
            return 0;
        }

        try
        {
            return node.GetValue<int>();
        }
        catch (Exception exception) when (exception is InvalidOperationException or FormatException)
        {
            throw Blocking(path, "its schema version is not a number", exception);
        }
    }

    private static TerminalStateException Blocking(string path, string reason, Exception? inner = null)
    {
        var message =
            $"Local state at '{path}' cannot be loaded because {reason}. " +
            "Fix or remove the file, then restart the terminal client.";
        return inner is null
            ? new TerminalStateException(message)
            : new TerminalStateException(message, inner);
    }

    private sealed record StateMigration(int FromVersion, Func<JsonObject, JsonObject> Upgrade);
}
