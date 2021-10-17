using System;
using System.Threading.Tasks;

namespace DarkDeeds.Communication.Services.Interface
{
    interface IServiceDiscovery
    {
        Task<Uri> GetService(string name);
    }
}