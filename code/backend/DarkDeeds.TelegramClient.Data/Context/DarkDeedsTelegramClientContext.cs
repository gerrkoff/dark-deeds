using DarkDeeds.TelegramClient.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.TelegramClient.Data.Context
{
    public class DarkDeedsTelegramClientContext : IdentityDbContext<UserEntity>
    {
        public DarkDeedsTelegramClientContext(DbContextOptions<DarkDeedsTelegramClientContext> options) : base(options)
        {
        }
    }
}