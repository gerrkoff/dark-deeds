using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace DarkDeeds.Api.Hubs
{
    public class TaskHub : Hub
    {
        public async Task HelloWorld(string message)
        {
            await Clients.All.SendAsync("helloWorld", message);
        }
    }
}