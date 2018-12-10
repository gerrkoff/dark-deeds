using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DarkDeeds.Common.Settings;
using DarkDeeds.Data.Entity;
using DarkDeeds.Services.Interface;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DarkDeeds.Services.Implementation
{
    public class TokenService : ITokenService
    {
        private readonly AuthSettings _authSettings;

        public TokenService(IOptions<AuthSettings> authSettings)
        {
            _authSettings = authSettings.Value;
        }

        public string GetToken(UserEntity user, DateTime? expires = null)
        {
            ClaimsIdentity identity = GetIdentity(user);

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: _authSettings.Issuer,
                audience: _authSettings.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: // TODO: research is needed
                    // expires ??
                    now.Add(TimeSpan.FromMinutes(_authSettings.Lifetime)),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_authSettings.Key)), SecurityAlgorithms.HmacSha256));
            
            string encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }
        
        private ClaimsIdentity GetIdentity(UserEntity user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                new Claim(ClaimTypes.Sid, user.Id),
				new Claim(ClaimTypes.GivenName, user.DisplayName)
			};

            ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token",
                    ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
    }
}