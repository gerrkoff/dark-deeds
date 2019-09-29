using System.Net.Http;
using DarkDeeds.E2eTests.Extensions;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace DarkDeeds.E2eTests
{
    public class SmokeTests : BaseTest
    {
        private readonly ITestOutputHelper _output;

        public SmokeTests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public async void GetBuildVersionTest()
        {
            var httpClient = new HttpClient();
            var url = $"{Url}/api/build-info";
            var result = await httpClient.GetStringAsync(url);
            var version = (string) JObject.Parse(result)["version"];
            _output.WriteLine(version);
        }

        [Fact]
        public void SignInTest()
        {
            Test(driver =>
            {
                driver.SignIn(Username, Password);
            });
        }
        
        [Fact]
        public void CreateNoDateTest()
        {
            string taskText = RandomizeText("some long & strange name for task");
            Test(driver =>
            {
                driver.SignIn(Username, Password);
                driver.WaitUntillUserLoaded();

                driver.GetAddTaskButton().Click();
                driver.GetEditTaskInput().SendKeys(taskText);
                driver.GetSaveTaskButton().Click();
                driver.GetTaskByTextInNoDateSection(taskText);
                driver.WaitUntillSavingFinished();
            });
        }
    }
}