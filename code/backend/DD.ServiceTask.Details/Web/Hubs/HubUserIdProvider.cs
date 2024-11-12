using DD.Shared.Details.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace DD.ServiceTask.Details.Web.Hubs;

public class HubUserIdProvider(IClaimsService claimsService) : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return claimsService.ToToken(connection.User.Claims).UserId;
    }
}
