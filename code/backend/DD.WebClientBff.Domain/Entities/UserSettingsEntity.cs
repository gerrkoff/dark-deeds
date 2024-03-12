namespace DD.WebClientBff.Domain.Entities;

public class UserSettingsEntity
{
    public string UserId { get; set; } = string.Empty;

    public bool ShowCompleted { get; set; }
}
