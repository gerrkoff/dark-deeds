namespace DarkDeeds.Auth.Settings
{
    public class AuthSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
        public int Lifetime { get; set; }

//		public AuthGoogleSettings AuthGoogleSettings { get; set; }
//		public AuthTwitterSettings AuthTwitterSettings { get; set; }
//		public AuthFacebookSettings AuthFacebookSettings { get; set; }
//		public AuthVkSettings AuthVkSettings { get; set; }
	}

//	public class AuthGoogleSettings
//	{
//		public string ClientId { get; set; }
//		public string ClientSecret { get; set; }
//	}
//
//	public class AuthTwitterSettings
//	{
//		public string ConsumerKey { get; set; }
//		public string ConsumerSecret { get; set; }
//	}
//
//	public class AuthFacebookSettings
//	{
//		public string AppId { get; set; }
//		public string AppSecret { get; set; }
//	}
//
//	public class AuthVkSettings
//	{
//		public string ClientId { get; set; }
//		public string ClientSecret { get; set; }
//	}
}