using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DD.ServiceTask.Details.Web.Hubs;

[Authorize]
public class TaskHub : Hub;
