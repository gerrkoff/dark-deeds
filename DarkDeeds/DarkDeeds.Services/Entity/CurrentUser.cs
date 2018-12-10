using System;

namespace DarkDeeds.Services.Entity
{
    public class CurrentUser
    {
        public string UserId { get; set; }
        public string Username { get; set; }
		public string DisplayName { get; set; }

        public DateTime? Expires { get; set; }
    }
}