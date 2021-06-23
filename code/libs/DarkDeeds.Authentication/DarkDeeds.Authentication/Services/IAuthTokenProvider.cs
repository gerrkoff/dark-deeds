using System.Threading.Tasks;

namespace DarkDeeds.Authentication.Services
{
    public interface IAuthTokenProvider
    {
        Task<string> GetToken();
    }
}