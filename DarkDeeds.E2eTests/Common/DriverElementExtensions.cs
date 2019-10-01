using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace DarkDeeds.E2eTests.Common
{
    public static class DriverElementExtensions
    {
        public static IWebElement GetUsernameInput(this RemoteWebDriver driver) =>
            driver.GetElement("//*[@data-test-id='usernameInput']/input");
        
        public static IWebElement GetPasswordInput(this RemoteWebDriver driver) =>
            driver.GetElement("//*[@data-test-id='passwordInput']/input");
        
        public static IWebElement GetSignInButton(this RemoteWebDriver driver) =>
            driver.GetElement("//*[@data-test-id='signinButton']");
        
        public static IWebElement GetAddTaskButton(this RemoteWebDriver driver) =>
            driver.GetElement("//*[@data-test-id='addTaskButton']");
        
        public static IWebElement GetEditTaskInput(this RemoteWebDriver driver) =>
            driver.GetElement("//*[@data-test-id='editTaskInput']/input");
        
        public static IWebElement GetSaveTaskButton(this RemoteWebDriver driver) =>
            driver.GetElement("//*[@data-test-id='saveTaskButton']");
        
        public static IWebElement GetAddTaskToDayButton(this RemoteWebDriver driver) =>
            driver.GetElement("//*[@data-test-id='addTaskToDayButton']");
        
        public static IWebElement GetDeleteTaskButton(this RemoteWebDriver driver) =>
            driver.GetElement("//*[@data-test-id='deleteTaskButton']");
        
        public static IWebElement GetModalConfirmButton(this RemoteWebDriver driver) =>
            driver.GetElement("//*[@data-test-id='modalConfirmButton']");


        public static IWebElement GetTaskByTextInNoDateSection(this RemoteWebDriver driver, string text) =>
            driver.GetElement($"//div[@id='no-date-card']//{Xpath.TaskWithText(text)}");
        
        public static IWebElement GetCurrentSection(this RemoteWebDriver driver) =>
            driver.GetElement("//*[@data-test-id='currentDaysBlockComponent']");


        
    }
}
