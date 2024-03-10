namespace DD.Shared.Data;

public interface IMigrationBody
{
    Task UpAsync(CancellationToken cancellationToken);
}
