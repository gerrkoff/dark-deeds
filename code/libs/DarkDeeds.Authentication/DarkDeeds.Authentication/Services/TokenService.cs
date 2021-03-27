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

        public string GetToken(CurrentUser user)
        {
            ClaimsIdentity identity = ToIdentity(user);

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: _authSettings.Issuer,
                audience: _authSettings.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(_authSettings.Lifetime)),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_authSettings.Key)), SecurityAlgorithms.HmacSha256));
            
            string encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        public CurrentUser GetUser(string token)
        {
            var validationParams = TokenValidationParams.Get(_authSettings);

            var claims = new JwtSecurityTokenHandler().ValidateToken(token, validationParams, out SecurityToken _);

            return FromIdentity(claims);
        }

        private ClaimsIdentity ToIdentity(CurrentUser user)
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
        
        private CurrentUser FromIdentity(ClaimsPrincipal principal)
        {
            var identity = (ClaimsIdentity) principal.Identity;
            if (identity == null)
                return new CurrentUser();

            string expiration = identity.FindFirst("exp")?.Value;
            var user = new CurrentUser
            {
                Username = identity.FindFirst(ClaimsIdentity.DefaultNameClaimType)?.Value,
                UserId = identity.FindFirst(ClaimTypes.Sid)?.Value,
                DisplayName = identity.FindFirst(ClaimTypes.GivenName)?.Value,
                Expires = string.IsNullOrWhiteSpace(expiration)
                    ? null
                    : UnixTimeStampToDateTime(double.Parse(expiration))
            };
            
            static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
            {
                var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
                return dtDateTime;
            }
            
            return user;
        }
    }
}