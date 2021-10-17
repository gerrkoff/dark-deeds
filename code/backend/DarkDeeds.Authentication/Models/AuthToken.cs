using System;

namespace DarkDeeds.Authentication.Models
{
    public class AuthToken
    {
        public string UserId { get; set; }
        public string Username { get; set; }
		public string DisplayName { get; set; }
        public DateTime? Expires { get; set; }
    }
}