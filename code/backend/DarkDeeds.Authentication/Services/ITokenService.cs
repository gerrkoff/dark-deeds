using DarkDeeds.Authentication.Models;

namespace DarkDeeds.Authentication.Services
{
    public interface ITokenService
    {
        string Serialize(AuthToken authToken);
        AuthToken Deserialize(string serializedToken);
    }
}
