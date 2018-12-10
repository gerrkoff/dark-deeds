using System;
using System.Security.Claims;
using DarkDeeds.Services.Entity;

namespace DarkDeeds.Api.Controllers.Base
{
    public class BaseUserController : BaseController
    {
        protected CurrentUser GetUser()
        {
            var identity = (ClaimsIdentity) User.Identity;
            var user = new CurrentUser
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
        
        private DateTime UnixTimeStampToDateTime( double unixTimeStamp )
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}