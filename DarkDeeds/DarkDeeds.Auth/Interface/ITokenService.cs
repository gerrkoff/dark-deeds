using System;
using DarkDeeds.Entities.Models;

namespace DarkDeeds.Auth.Interface
{
    public interface ITokenService
    {
        string GetToken(UserEntity user, DateTime? expires = null);
    }
}