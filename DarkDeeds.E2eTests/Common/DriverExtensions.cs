using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace DarkDeeds.E2eTests.Common
{
    public static class DriverExtensions
    {
        public static void SignIn(this RemoteWebDriver driver, string username, string password)
        {
            driver.GetUsernameInput().SendKeys(username);
            driver.GetPasswordInput().SendKeys(password);
            driver.GetSignInButton().Click();
            driver.WaitUntilDisappeared("//*[@data-test-id='loginComponent']");
        }

        public static void WaitUntillUserLoaded(this RemoteWebDriver driver)
        {
            driver.WaitUntilAppeared("//*[@data-test-id='overviewComponent']");
        }
        
        public static void WaitUntillSavingFinished(this RemoteWebDriver driver)
        {
            string xpath = "//*[@data-test-id='savingIndicator']";
            driver.WaitUntilAppeared(xpath);
            driver.WaitUntilDisappeared(xpath);
        }

        public static void DeleteTask(this RemoteWebDriver driver, IWebElement taskElement)
        {
            taskElement.Click();
            driver.GetDeleteTaskButton().Click();
            driver.GetModalConfirmButton().Click();
        }
        
        public static void CreateTaskViaDayHeader(this RemoteWebDriver driver, IWebElement dayHeaderElement, string task)
        {
            dayHeaderElement.Click();
            driver.GetAddTaskToDayButton().Click();
            driver.GetEditTaskInput().SendKeys(task);
            driver.GetSaveTaskButton().Click();
        }

        public static void CreateTaskViaAddButton(this RemoteWebDriver driver, string task)
        {
            driver.GetAddTaskButton().Click();
            driver.GetEditTaskInput().SendKeys(task);
            driver.GetSaveTaskButton().Click();
        }
    }
}
