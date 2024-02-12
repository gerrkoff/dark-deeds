using DD.ServiceAuth.Domain.Entities;
using DD.TelegramClient.Domain.Entities;
using DD.WebClientBff.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DD.Shared.Data.Context;

internal sealed class BackendDbContext(DbContextOptions<BackendDbContext> options) : IdentityDbContext<UserEntity>(options)
{
    public DbSet<SettingsEntity> Settings { get; init; } = null!;

    public DbSet<TelegramUserEntity> TelegramUsers { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserEntity>()
            .HasOne<SettingsEntity>()
            .WithOne()
            .HasForeignKey<SettingsEntity>(x => x.UserId);

        modelBuilder.Entity<UserEntity>()
            .HasOne<TelegramUserEntity>()
            .WithOne()
            .HasForeignKey<TelegramUserEntity>(x => x.UserId);
    }
}
