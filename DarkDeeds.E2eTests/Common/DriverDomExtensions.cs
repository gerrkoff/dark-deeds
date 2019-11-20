using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace DarkDeeds.E2eTests.Common
{
    public static class DriverDomExtensions
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

        public static IWebElement GetNavLink(this RemoteWebDriver driver, string link) =>
            driver.GetElement($"//*[@data-test-id='nav-{link}']");
        
        public static IWebElement GetAddRecurrenceButton(this RemoteWebDriver driver) =>
            driver.GetElement($"//*[@data-test-id='add-recurrence-button']");
        
        public static IWebElement GetSaveRecurrencesButton(this RemoteWebDriver driver) =>
            driver.GetElement($"//*[@data-test-id='save-recurrences-button']");
        
        public static IWebElement GetCreateRecurrencesButton(this RemoteWebDriver driver) =>
            driver.GetElement($"//*[@data-test-id='create-recurrences-button']");
        
        public static IWebElement GetCreateRecurrenceFormTaskInput(this RemoteWebDriver driver) =>
            driver.GetElement($"//{Xpath.CreateRecurrenceForm()}//input[1]");
        
        public static IWebElement GetCreateRecurrenceFormWeekday(this RemoteWebDriver driver) =>
            driver.GetElement($"//{Xpath.CreateRecurrenceFormWeekdays()}");
        
        public static IWebElement GetCreateRecurrenceFormWeekdayOption(this RemoteWebDriver driver, int optionIndex) =>
            driver.GetElement($"//{Xpath.CreateRecurrenceFormWeekdays()}//*[@role='option'][{optionIndex}]");

        
        public static void WaitUntilLoginComponentDisappeared(this RemoteWebDriver driver) =>
            driver.WaitUntilDisappeared("//*[@data-test-id='loginComponent']");
        
        public static void WaitUntilOverviewComponentAppeared(this RemoteWebDriver driver) =>
            driver.WaitUntilAppeared("//*[@data-test-id='overviewComponent']");
        
        public static void WaitUntilSavingIndicatorDisappeared(this RemoteWebDriver driver) =>
            driver.WaitUntilDisappeared($"//{Xpath.SavingIndicator()}");
        
        public static void WaitUntilSavingIndicatorAppeared(this RemoteWebDriver driver) =>
            driver.WaitUntilAppeared($"//{Xpath.SavingIndicator()}");
        
        public static void WaitUntilRecurrencesSkeletonDisappeared(this RemoteWebDriver driver) =>
            driver.WaitUntilDisappeared("//*[@data-test-id='recurrences-skeleton']");

        public static void WaitUntilToastAppeared(this RemoteWebDriver driver, string text) =>
            driver.WaitUntilAppeared(
                $"//*{Xpath.ClassContains("Toastify__toast-container")}" +
                $"//*{Xpath.ClassContains("Toastify__toast--success")}" +
                $"//*{Xpath.ClassContains("Toastify__toast-body")}{Xpath.TextContains(text)}"
            );
        
        public static void WaitUntilRecurrenceAppeared(this RemoteWebDriver driver, string text) =>
            driver.WaitUntilAppeared(
                $"//*{Xpath.ClassContains("recurrences-view-recurrence-list")}" +
                $"//*{Xpath.ClassContains("recurrences-view-recurrence-item")}" +
                $"//*{Xpath.TextContains(text)}"
            );
    }
}
