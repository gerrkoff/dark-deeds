using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DarkDeeds.Authentication.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DarkDeeds.Authentication.Services
{
    public class TokenService : ITokenService
    {
        private readonly AuthSettings _authSettings;

        public TokenService(IOptions<AuthSettings> authSettings)
        {
            _authSettings = authSettings.Value;
        }

        public string Serialize(AuthToken authToken)
        {
            ClaimsIdentity identity = ToIdentity(authToken);

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                _authSettings.Issuer,
                _authSettings.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(_authSettings.Lifetime)),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_authSettings.Key)), SecurityAlgorithms.HmacSha256));
            
            string encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        public AuthToken Deserialize(string serializedToken)
        {
            var validationParams = TokenValidationParams.Get(_authSettings);

            var claims = new JwtSecurityTokenHandler().ValidateToken(serializedToken, validationParams, out SecurityToken _);

            return claims.ToAuthToken();
        }

        private ClaimsIdentity ToIdentity(AuthToken user)
        {
            var claims = new List<Claim>
            {
                new(ClaimsIdentity.DefaultNameClaimType, user.Username),
                new(ClaimTypes.Sid, user.UserId),
				new(ClaimTypes.GivenName, user.DisplayName)
			};

            ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token",
                    ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
    }
}