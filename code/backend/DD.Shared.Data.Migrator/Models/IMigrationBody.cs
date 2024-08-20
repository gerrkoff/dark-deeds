namespace DD.Shared.Data.Migrator.Models;

public interface IMigrationBody
{
    Task UpAsync(CancellationToken cancellationToken);
}
