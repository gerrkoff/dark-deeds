using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace DarkDeeds.E2eTests.Extensions
{
    public static class DriverelementExtensions
    {
        private static string ClassContains(string className) => $"[contains(concat(' ', @class, ' '), ' {className} ')]";
        private static string TextContains(string text) => $"[text()='{text}']";
        private static string TaskWithText(string text) => $"span{ClassContains("task-item")}{TextContains(text)}";


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


        public static IWebElement GetTaskByTextInNoDateSection(this RemoteWebDriver driver, string text) =>
            driver.GetElement($"//div[@id='no-date-card']//{TaskWithText(text)}");
    }
}
