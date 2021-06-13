using System;
using System.Threading.Tasks;

namespace DarkDeeds.Communication.Services.Interface
{
    internal interface IServiceDiscovery
    {
        Task<Uri> GetService(string name);
    }
}