using System;
using DarkDeeds.Data.Entity;

namespace DarkDeeds.Services.Interface
{
    public interface ITokenService
    {
        string GetToken(UserEntity user, DateTime? expires = null);
    }
}