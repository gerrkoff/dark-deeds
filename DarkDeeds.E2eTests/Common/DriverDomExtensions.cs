using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace DarkDeeds.E2eTests.Common
{
    public static class DriverDomExtensions
    {
        public static IWebElement GetUsernameInput(this RemoteWebDriver driver) =>
            driver.GetElement("//*[@data-test-id='username-input']/input");
        
        public static IWebElement GetPasswordInput(this RemoteWebDriver driver) =>
            driver.GetElement("//*[@data-test-id='password-input']/input");
        
        public static IWebElement GetSignInButton(this RemoteWebDriver driver) =>
            driver.GetElement("//*[@data-test-id='signin-button']");
        
        public static IWebElement GetAddTaskButton(this RemoteWebDriver driver) =>
            driver.GetElement("//*[@data-test-id='add-task-button']");
        
        public static IWebElement GetEditTaskInput(this RemoteWebDriver driver) =>
            driver.GetElement("//*[@data-test-id='edit-task-input']/input");
        
        public static IWebElement GetSaveTaskButton(this RemoteWebDriver driver) =>
            driver.GetElement("//*[@data-test-id='save-task-button']");
        
        public static IWebElement GetAddTaskToDayButton(this RemoteWebDriver driver) =>
            driver.GetElement("//*[@data-test-id='add-task-to-day-button']");
        
        public static IWebElement GetDeleteTaskButton(this RemoteWebDriver driver) =>
            driver.GetElement("//*[@data-test-id='delete-task-button']");
        
        public static IWebElement GetModalConfirmButton(this RemoteWebDriver driver) =>
            driver.GetElement("//*[@data-test-id='modal-confirm-button']");

        public static IWebElement GetTaskByTextInNoDateSection(this RemoteWebDriver driver, string text) =>
            driver.GetElement($"//div[@id='no-date-card']//{Xpath.TaskWithText(text)}");
        
        public static IWebElement GetCurrentSection(this RemoteWebDriver driver) =>
            driver.GetElement("//*[@data-test-id='current-days-block-component']");

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

        public static IWebElement GetDeleteRecurrenceButton(this RemoteWebDriver driver, string recurrenceText) =>
            driver.GetElement($"//{Xpath.RecurrenceList()}//{Xpath.RecurrenceItem()}" +
                              $"[//*{Xpath.TextContains(recurrenceText)}]" +
                              $"//{Xpath.RecurrenceItemButton("top")}"
            );

        public static IWebElement GetToast(this RemoteWebDriver driver, string text = null) =>
            driver.GetElement(Xpath.Toast(text));

        
        public static void WaitUntilLoginComponentDisappeared(this RemoteWebDriver driver) =>
            driver.WaitUntilDisappeared("//*[@data-test-id='login-component']");
        
        public static void WaitUntilOverviewComponentAppeared(this RemoteWebDriver driver) =>
            driver.WaitUntilAppeared("//*[@data-test-id='overview-component']");
        
        public static void WaitUntilSavingIndicatorDisappeared(this RemoteWebDriver driver) =>
            driver.WaitUntilDisappeared($"//{Xpath.SavingIndicator()}");
        
        public static void WaitUntilSavingIndicatorAppeared(this RemoteWebDriver driver) =>
            driver.WaitUntilAppeared($"//{Xpath.SavingIndicator()}");
        
        public static void WaitUntilRecurrencesSkeletonDisappeared(this RemoteWebDriver driver) =>
            driver.WaitUntilDisappeared("//*[@data-test-id='recurrences-skeleton']");

        public static void WaitUntilToastAppeared(this RemoteWebDriver driver, string text = null)
            => driver.WaitUntilAppeared(Xpath.Toast(text));
        
        public static void WaitUntilToastDisappeared(this RemoteWebDriver driver, string text = null)
            => driver.WaitUntilDisappeared(Xpath.Toast(text));

        public static void WaitUntilRecurrenceAppeared(this RemoteWebDriver driver, string text) =>
            driver.WaitUntilAppeared(
                $"//{Xpath.RecurrenceList()}//{Xpath.RecurrenceItem()}//*{Xpath.TextContains(text)}");


        public static bool ToastExists(this RemoteWebDriver driver, string text = null) =>
            driver.ElementExists(Xpath.Toast(text));
    }
}
