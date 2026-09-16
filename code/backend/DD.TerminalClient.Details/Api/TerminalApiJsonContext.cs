using System.Text.Json;
using System.Text.Json.Serialization;
using DD.Shared.Details.Abstractions.Dto;

namespace DD.TerminalClient.Details.Api;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(SignInRequest))]
[JsonSerializable(typeof(SignInResponse))]
[JsonSerializable(typeof(List<TaskDto>))]
internal sealed partial class TerminalApiJsonContext : JsonSerializerContext;
