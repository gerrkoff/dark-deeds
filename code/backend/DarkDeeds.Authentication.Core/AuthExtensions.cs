using System;
using System.Security.Claims;
using DarkDeeds.Authentication.Core.Models;

namespace DarkDeeds.Authentication.Core
{
    public static class AuthExtensions
    {
        public static AuthToken ToAuthToken(this ClaimsPrincipal principal)
        {
            var identity = (ClaimsIdentity) principal.Identity;
            
            var user = new AuthToken
            {
                Username = identity.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value,
                UserId = identity.FindFirst(ClaimTypes.Sid).Value,
                DisplayName = identity.FindFirst(ClaimTypes.GivenName).Value
            };

            string expiration = identity.FindFirst("exp")?.Value;
            if (!string.IsNullOrWhiteSpace(expiration))
                user.Expires = UnixTimeStampToDateTime(double.Parse(expiration));
            
            return user;
        }
        
        private static DateTime UnixTimeStampToDateTime( double unixTimeStamp )
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}