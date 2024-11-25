using DarkDeeds.E2eTests.Components;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace DarkDeeds.E2eTests.Common;

public static class DriverExtensions
{
    public static void SignIn(this RemoteWebDriver driver, string username, string password)
    {
        driver.GetElement(X.SignInForm().UsernameInput()).SendKeys(username);
        driver.GetElement(X.SignInForm().PasswordInput()).SendKeys(password);
        driver.GetElement(X.SignInForm().SubmitButton()).Click();
        driver.WaitUntilDisappeared(X.SignInForm());
    }

    public static void WaitUntilUserLoaded(this RemoteWebDriver driver)
    {
        driver.WaitUntilAppeared(X.Navbar());
    }

    public static void WaitUntilSavingFinished(this RemoteWebDriver driver)
    {
        driver.WaitUntilAppeared(X.Root().SavingStatus());
        driver.WaitUntilDisappeared(X.Root().SavingStatus());
    }

    public static void DeleteTask(this RemoteWebDriver driver, IWebElement taskElement)
    {
        driver.ScrollToElement(taskElement);
        taskElement.Click();
        driver.GetElement(X.TaskMenu().DeleteButton()).Click();
        driver.GetElement(X.TaskMenu().DeleteButton()).Click();
    }

    public static void CreateTaskViaDayHeader(this RemoteWebDriver driver, IWebElement dayHeaderElement, string task)
    {
        driver.ScrollToElement(dayHeaderElement);
        dayHeaderElement.Click();
        driver.GetElement(X.TaskMenu().AddButton()).Click();
        driver.GetElement(X.EditTaskModal().Input()).SendKeys(task);
        driver.GetElement(X.EditTaskModal().SubmitButton()).Click();
    }

    public static void CreateTaskViaAddButton(this RemoteWebDriver driver, string task)
    {
        driver.GetElement(X.OverviewPage().AddTaskButton()).Click();
        driver.GetElement(X.EditTaskModal().Input()).SendKeys(task);
        driver.GetElement(X.EditTaskModal().SubmitButton()).Click();
    }

    public static void NavigateToOverview(this RemoteWebDriver driver)
    {
        driver.GetElement(X.Navbar().Overview()).Click();
    }

    public static void NavigateToRecurrences(this RemoteWebDriver driver)
    {
        driver.GetElement(X.Navbar().Recurrences()).Click();
        driver.WaitUntilAppeared(X.Root().Loader());
        driver.WaitUntilDisappeared(X.Root().Loader());
    }

    public static void SignOut(this RemoteWebDriver driver)
    {
        driver.GetElement(X.Navbar().Settings()).Click();
        driver.GetElement(X.SettingsPage().SignOutButton()).Click();
    }

    public static void CreateRecurrence(this RemoteWebDriver driver, string recurrenceTask)
    {
        driver.GetElement(X.RecurrencesPage().AddRecurrenceButton()).Click();
        driver.GetElement(X.EditRecurrenceModal().TaskInput()).SendKeys(recurrenceTask);
        driver.GetElement(X.EditRecurrenceModal().WeekdaysInputOption(7)).Click();
        driver.GetElement(X.EditRecurrenceModal().SubmitButton()).Click();
        driver.WaitUntilDisappeared(X.EditTaskModal());
        driver.GetElement(X.RecurrencesPage().SaveRecurrencesButton()).Click();
        driver.WaitUntilAppeared(X.RecurrencesPage().SaveRecurrencesButton().Loader());
        driver.WaitUntilDisappeared(X.RecurrencesPage().SaveRecurrencesButton().Loader());
    }

    public static void CreateTaskRecurrences(this RemoteWebDriver driver)
    {
        driver.GetElement(X.RecurrencesPage().CreateRecurrencesButton()).Click();
        driver.WaitUntilAppeared(X.RecurrencesPage().CreateRecurrencesButton().Loader());
        driver.WaitUntilDisappeared(X.RecurrencesPage().CreateRecurrencesButton().Loader());
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
}
