using DarkDeeds.TelegramClientApp.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.TelegramClientApp.Data.Context
{
    public class DarkDeedsTelegramClientContext : IdentityDbContext<UserEntity>
    {
        public DarkDeedsTelegramClientContext(DbContextOptions<DarkDeedsTelegramClientContext> options) : base(options)
        {
        }
    }
}