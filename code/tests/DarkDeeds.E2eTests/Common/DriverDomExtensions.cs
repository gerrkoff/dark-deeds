using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace DarkDeeds.E2eTests.Common;

public static class DriverDomExtensions
{
    // fixed
    public static IWebElement GetUsernameInput(this RemoteWebDriver driver)
    {
        return driver.GetElement("//*[@data-test-id='form-signin']//input[@id='username']");
    }

    // fixed
    public static IWebElement GetPasswordInput(this RemoteWebDriver driver)
    {
        return driver.GetElement("//*[@data-test-id='form-signin']//input[@id='password']");
    }

    // fixed
    public static IWebElement GetSignInButton(this RemoteWebDriver driver)
    {
        return driver.GetElement("//*[@data-test-id='form-signin']//button[@type='submit']");
    }

    public static IWebElement GetSignOutButton(this RemoteWebDriver driver)
    {
        return driver.GetElement("//*[@data-test-id='signout-button']");
    }

    public static IWebElement GetAddTaskButton(this RemoteWebDriver driver)
    {
        return driver.GetElement("//*[@data-test-id='add-task-button']");
    }

    public static IWebElement GetEditTaskInput(this RemoteWebDriver driver)
    {
        return driver.GetElement("//*[@data-test-id='edit-task-input']/input");
    }

    public static IWebElement GetSaveTaskButton(this RemoteWebDriver driver)
    {
        return driver.GetElement("//*[@data-test-id='save-task-button']");
    }

    public static IWebElement GetAddTaskToDayButton(this RemoteWebDriver driver)
    {
        return driver.GetElement("//*[@data-test-id='add-task-to-day-button']");
    }

    public static IWebElement GetDeleteTaskButton(this RemoteWebDriver driver)
    {
        return driver.GetElement("//*[@data-test-id='delete-task-button']");
    }

    public static IWebElement GetModalConfirmButton(this RemoteWebDriver driver)
    {
        return driver.GetElement("//*[@data-test-id='modal-confirm-button']");
    }

    public static IWebElement GetTaskByTextInNoDateSection(this RemoteWebDriver driver, string text)
    {
        return driver.GetElement($"//div[@id='no-date-card']//{XpathHelper.TaskWithText(text)}");
    }

    public static IWebElement GetCurrentSection(this RemoteWebDriver driver)
    {
        return driver.GetElement("//*[@data-test-id='current-days-block-component']");
    }

    public static IWebElement GetNavLink(this RemoteWebDriver driver, string link)
    {
        return driver.GetElement($"//*[@data-test-id='nav-{link}']");
    }

    public static IWebElement GetAddRecurrenceButton(this RemoteWebDriver driver)
    {
        return driver.GetElement($"//*[@data-test-id='add-recurrence-button']{XpathHelper.NotContainsAttr("disabled")}");
    }

    public static IWebElement GetSaveRecurrencesButton(this RemoteWebDriver driver)
    {
        return driver.GetElement($"//*[@data-test-id='save-recurrences-button']{XpathHelper.NotContainsAttr("disabled")}");
    }

    public static IWebElement GetCreateRecurrencesButton(this RemoteWebDriver driver)
    {
        return driver.GetElement($"//*[@data-test-id='create-recurrences-button']{XpathHelper.NotContainsAttr("disabled")}");
    }

    public static IWebElement GetCreateRecurrenceFormTaskInput(this RemoteWebDriver driver)
    {
        return driver.GetElement($"//{XpathHelper.CreateRecurrenceForm()}//input[1]");
    }

    public static IWebElement GetCreateRecurrenceFormWeekday(this RemoteWebDriver driver)
    {
        return driver.GetElement($"//{XpathHelper.CreateRecurrenceFormWeekdays()}");
    }

    public static IWebElement GetCreateRecurrenceFormWeekdayOption(this RemoteWebDriver driver, int optionIndex)
    {
        return driver.GetElement($"//{XpathHelper.CreateRecurrenceFormWeekdays()}//*[@role='option'][{optionIndex}]");
    }

    public static IWebElement GetDeleteRecurrenceButton(this RemoteWebDriver driver, string recurrenceText)
    {
        return driver.GetElement($"//{XpathHelper.RecurrenceList()}//{XpathHelper.RecurrenceItem()}" +
                          $"[//*{XpathHelper.TextContains(recurrenceText)}]" +
                          $"//{XpathHelper.RecurrenceItemButton("top")}");
    }

    public static IWebElement GetToast(this RemoteWebDriver driver, string text = "")
    {
        return driver.GetElement(XpathHelper.Toast(text));
    }

    // fixed
    public static void WaitUntilLoginComponentDisappeared(this RemoteWebDriver driver)
    {
        driver.WaitUntilDisappeared("//*[@data-test-id='form-signin']");
    }

    // fixed
    public static void WaitUntilNavbarComponentAppeared(this RemoteWebDriver driver)
    {
        driver.WaitUntilAppeared($"//*{XpathHelper.ClassContains("navbar")}");
    }

    public static void WaitUntilSavingIndicatorDisappeared(this RemoteWebDriver driver)
    {
        driver.WaitUntilDisappeared($"//{XpathHelper.SavingIndicator()}");
    }

    public static void WaitUntilSavingIndicatorAppeared(this RemoteWebDriver driver)
    {
        driver.WaitUntilAppeared($"//{XpathHelper.SavingIndicator()}");
    }

    public static void WaitUntilRecurrencesSkeletonDisappeared(this RemoteWebDriver driver)
    {
        driver.WaitUntilDisappeared("//*[@data-test-id='recurrences-skeleton']");
    }

    public static void WaitUntilToastAppeared(this RemoteWebDriver driver, string text = "")
    {
        driver.WaitUntilAppeared(XpathHelper.Toast(text));
    }

    public static void WaitUntilToastDisappeared(this RemoteWebDriver driver, string text = "")
    {
        driver.WaitUntilDisappeared(XpathHelper.Toast(text));
    }

    public static void WaitUntilTaskDisappeared(this RemoteWebDriver driver, string text)
    {
        driver.WaitUntilDisappeared($"//{XpathHelper.TaskWithText(text)}");
    }

    public static void WaitUntilRecurrenceAppeared(this RemoteWebDriver driver, string text)
    {
        driver.WaitUntilAppeared($"//{XpathHelper.RecurrenceList()}//{XpathHelper.RecurrenceItem()}//*{XpathHelper.TextContains(text)}");
    }

    public static bool ToastExists(this RemoteWebDriver driver, string text = "")
    {
        return driver.ElementExists(XpathHelper.Toast(text));
    }
}
