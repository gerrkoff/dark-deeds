using System.Text.Json.Serialization;

namespace DD.TerminalClient.Details.Storage;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(PersistedProfile))]
internal sealed partial class TerminalProfileJsonContext : JsonSerializerContext;
