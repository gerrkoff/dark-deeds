using System.Threading.Tasks;

namespace DarkDeeds.Authentication.Core.Services
{
    public interface IAuthTokenProvider
    {
        Task<string> GetToken();
    }
}