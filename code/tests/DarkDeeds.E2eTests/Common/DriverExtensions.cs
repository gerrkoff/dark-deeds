using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Xunit;

namespace DarkDeeds.E2eTests.Common;

public static class DriverExtensions
{
    public static void SignIn(this RemoteWebDriver driver, string username, string password)
    {
        driver.GetUsernameInput().SendKeys(username);
        driver.GetPasswordInput().SendKeys(password);
        driver.GetSignInButton().Click();
        driver.WaitUntilLoginComponentDisappeared();
    }

    public static void WaitUntilUserLoaded(this RemoteWebDriver driver)
    {
        driver.WaitUntilOverviewComponentAppeared();
    }

    public static void WaitUntilSavingFinished(this RemoteWebDriver driver)
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

    public static void NavigateToSettings(this RemoteWebDriver driver)
    {
        driver.GetNavLink("/settings").Click();
    }

    public static void CreateRecurrence(this RemoteWebDriver driver, string recurrenceTask)
    {
        driver.GetAddRecurrenceButton().Click();
        driver.GetCreateRecurrenceFormTaskInput().SendKeys(recurrenceTask);
        driver.GetCreateRecurrenceFormWeekday().Click();
        driver.GetCreateRecurrenceFormWeekdayOption(7).Click();
        driver.GetSaveRecurrencesButton().Click();
        driver.WaitUntilRecurrenceAppeared(recurrenceTask);
        driver.HideToasts();
    }

    public static void DeleteRecurrence(this RemoteWebDriver driver, string recurrenceTask)
    {
        driver.GetDeleteRecurrenceButton(recurrenceTask).Click();
        driver.GetModalConfirmButton().Click();
        driver.GetSaveRecurrencesButton().Click();
        driver.WaitUntilRecurrencesSkeletonDisappeared();
        driver.HideToasts();
    }

    public static void WaitUntilRecurrencesLoaded(this RemoteWebDriver driver)
    {
        driver.WaitUntilRecurrencesSkeletonDisappeared();
    }

    public static void CreateTaskRecurrences(this RemoteWebDriver driver, int expectedTaskRecurrencesCount)
    {
        driver.GetCreateRecurrencesButton().Click();
        driver.WaitUntilToastAppeared($"{expectedTaskRecurrencesCount} recurrences were created");
        driver.HideToasts();
    }

    public static void SwitchToTab(this RemoteWebDriver driver, int tabIndex)
    {
        driver.SwitchTo().Window(driver.WindowHandles[tabIndex]);
    }

    public static void OpenNewTab(this RemoteWebDriver driver, Uri url)
    {
        driver.CreateTab();
        driver.SwitchTo().Window(driver.WindowHandles.Last());
        driver.Navigate().GoToUrl(url);
        driver.WaitUntilUserLoaded();
    }

    private static void HideToasts(this RemoteWebDriver driver)
    {
        for (var i = 0; i < 5; i++)
        {
            if (!driver.ToastExists())
                return;

            try
            {
                driver.GetToast().Click();
            }
            catch (ElementNotInteractableException)
            {
                return;
            }

            Thread.Sleep(1000);
        }

        Assert.True(false, "Too many toasts");
    }
}
