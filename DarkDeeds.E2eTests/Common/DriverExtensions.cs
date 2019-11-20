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
            driver.WaitUntilLoginComponentDisappeared();
        }

        public static void WaitUntillUserLoaded(this RemoteWebDriver driver)
        {
            driver.WaitUntilOverviewComponentAppeared();
        }
        
        public static void WaitUntillSavingFinished(this RemoteWebDriver driver)
        {
            driver.WaitUntilSavingIndicatorAppeared();
            driver.WaitUntilSavingIndicatorDisappeared();
        }

        public static void DeleteTask(this RemoteWebDriver driver, IWebElement taskElement)
        {
            driver.ScrollToElement(taskElement);
            taskElement.Click();
            driver.GetDeleteTaskButton().Click();
            driver.GetModalConfirmButton().Click();
        }
        
        public static void CreateTaskViaDayHeader(this RemoteWebDriver driver, IWebElement dayHeaderElement, string task)
        {
            driver.ScrollToElement(dayHeaderElement);
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
        
        public static void NavigateToOverview(this RemoteWebDriver driver)
        {
            driver.GetNavLink("/").Click();
        }
        
        public static void NavigateToRecurrences(this RemoteWebDriver driver)
        {
            driver.GetNavLink("/recurrences").Click();
        }
        
        public static void CreateRecurrence(this RemoteWebDriver driver, string recurrenceTask)
        {
            driver.GetAddRecurrenceButton().Click();
            driver.GetCreateRecurrenceFormTaskInput().SendKeys(recurrenceTask);
            driver.GetCreateRecurrenceFormWeekday().Click();
            driver.GetCreateRecurrenceFormWeekdayOption(7).Click();
            driver.GetSaveRecurrencesButton().Click();
        }

        public static void WaitUntilRecurrencesLoaded(this RemoteWebDriver driver)
        {
            driver.WaitUntilRecurrencesSkeletonDisappeared();
        }
        
        public static void CreateTaskRecurrences(this RemoteWebDriver driver, int expectedTaskRecurrencesCount)
        {
            driver.GetCreateRecurrencesButton().Click();
            driver.WaitUntilToastAppeared($"{expectedTaskRecurrencesCount} recurrences were created");
        }
    }
}
