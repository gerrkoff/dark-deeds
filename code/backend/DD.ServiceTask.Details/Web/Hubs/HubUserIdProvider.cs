using DD.Shared.Auth;
using Microsoft.AspNetCore.SignalR;

namespace DD.ServiceTask.Details.Web.Hubs;

public class HubUserIdProvider(IAuthTokenConverter authTokenConverter) : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return authTokenConverter.FromPrincipal(connection.User).UserId;
    }
}
