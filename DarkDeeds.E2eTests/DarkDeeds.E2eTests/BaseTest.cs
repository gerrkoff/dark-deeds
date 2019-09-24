using System.IO;
using DarkDeeds.E2eTests.Helpers;
using Newtonsoft.Json;

namespace DarkDeeds.E2eTests
{
    public class BaseTest
    {
        protected static readonly string Url;
        protected static readonly string Username;
        protected static readonly string Password;
        
        static BaseTest()
        {
            string settingsSerialized = File.ReadAllText("settings.json");
            var settings = JsonConvert.DeserializeObject<Settings>(settingsSerialized);
            Url = settings.Url;
            Username = settings.Username;
            Password = settings.Password;
        }
    }
}