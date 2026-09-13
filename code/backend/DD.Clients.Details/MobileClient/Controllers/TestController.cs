using DD.Clients.Details.MobileClient.Data;
using DD.MobileClient.Domain.Entities;
using DD.Shared.Details.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DD.Clients.Details.MobileClient.Controllers;

public class TestController(MobileUserRepository repository) : BaseControllerTest
{
    [HttpPost(nameof(CreateMobileUserMapping))]
    public Task CreateMobileUserMapping(string userId, string mobileKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentException.ThrowIfNullOrWhiteSpace(mobileKey);

        return repository.UpsertAsync(new MobileUserEntity
        {
            UserId = userId,
            MobileKey = mobileKey,
        });
    }
}
