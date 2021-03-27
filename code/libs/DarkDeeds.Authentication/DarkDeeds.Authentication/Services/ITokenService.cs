using DarkDeeds.Authentication.Models;

namespace DarkDeeds.Authentication.Services
{
    public interface ITokenService
    {
        string GetToken(CurrentUser user);
        CurrentUser GetUser(string token);
    }
}
