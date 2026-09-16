using System.Text.Json.Serialization;
using DD.TerminalClient.Domain.State;

namespace DD.TerminalClient.Storage;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(PersistedTerminalState))]
internal sealed partial class TerminalStateJsonContext : JsonSerializerContext;
