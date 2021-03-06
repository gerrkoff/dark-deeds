﻿using System;

namespace DarkDeeds.Auth.Models
{
    public class CurrentUser
    {
        public string UserId { get; set; }
        public string Username { get; set; }
		public string DisplayName { get; set; }

        public DateTime? Expires { get; set; }
    }
}