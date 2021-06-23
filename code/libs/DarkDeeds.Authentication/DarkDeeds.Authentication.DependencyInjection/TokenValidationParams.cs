using System;
using System.Text;
using DarkDeeds.Authentication.Models;
using Microsoft.IdentityModel.Tokens;

namespace DarkDeeds.Authentication.DependencyInjection
{
    public static class TokenValidationParams
    {
        public static TokenValidationParameters Get(AuthSettings settings) => new()
        {
            ValidateIssuer = true,
            ValidIssuer = settings.Issuer,
            ValidateAudience = true,
            ValidAudience = settings.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.Key)),
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    }
}