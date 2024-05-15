using DD.MobileClient.Domain.Entities;

namespace DD.MobileClient.Domain.Infrastructure;

public interface IMobileUserRepository
{
    Task<MobileUserEntity?> GetByMobileKeyAsync(string mobileKey);

    Task<MobileUserEntity?> GetByIdAsync(string userId);
}
