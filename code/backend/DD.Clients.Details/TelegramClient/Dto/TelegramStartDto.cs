using System.Diagnostics.CodeAnalysis;

namespace DD.Clients.Details.TelegramClient.Dto;

public class TelegramStartDto
{
    [SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "This is a DTO and we need to use a string to represent the URL.")]
    public string Url { get; set; } = string.Empty;
}
