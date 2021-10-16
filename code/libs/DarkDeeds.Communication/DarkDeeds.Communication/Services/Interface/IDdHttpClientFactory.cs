using System.Net.Http;
using System.Threading.Tasks;

namespace DarkDeeds.Communication.Services.Interface
{
    public interface IDdHttpClientFactory
    {
        Task<HttpClient> Create(string appName);
    }
}