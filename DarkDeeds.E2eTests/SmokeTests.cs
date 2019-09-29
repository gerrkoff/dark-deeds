using DarkDeeds.E2eTests.Extensions;
using Xunit;

namespace DarkDeeds.E2eTests
{
    public class SmokeTests : BaseTest
    {
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