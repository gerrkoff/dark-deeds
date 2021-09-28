using DarkDeeds.Authentication;
using Microsoft.AspNetCore.SignalR;

namespace DarkDeeds.WebClientBffApp.App.Hubs
{
    public class HubUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User.ToAuthToken().UserId;
        }
    }
}