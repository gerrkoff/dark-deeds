using DD.Shared.Data.Abstractions;

namespace DD.WebClientBff.Domain.Entities;

public class SettingsEntity : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public bool ShowCompleted { get; set; }
}
