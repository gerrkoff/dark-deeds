using DarkDeeds.Authentication.Core;
using Microsoft.AspNetCore.SignalR;

namespace DarkDeeds.WebClientBffApp.Web.Hubs
{
    public class HubUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User.ToAuthToken().UserId;
        }
    }
}