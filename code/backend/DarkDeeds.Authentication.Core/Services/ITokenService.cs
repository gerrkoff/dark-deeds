using DarkDeeds.Authentication.Core.Models;

namespace DarkDeeds.Authentication.Core.Services
{
    public interface ITokenService
    {
        string Serialize(AuthToken authToken);
        AuthToken Deserialize(string serializedToken);
    }
}
