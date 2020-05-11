using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Xunit;
using Xunit.Abstractions;

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
            driver.WaitUntilRecurrenceAppeared(recurrenceTask);
            driver.HideToasts();
        }
        
        public static void DeleteRecurrence(this RemoteWebDriver driver, string recurrenceTask, ITestOutputHelper _output)
        {
            _output.WriteLine("___!!1___");
            driver.GetDeleteRecurrenceButton(recurrenceTask).Click();
            _output.WriteLine("___!!2___");
            driver.GetModalConfirmButton().Click();
            _output.WriteLine("___!!3___");
            driver.GetSaveRecurrencesButton().Click();
            _output.WriteLine("___!!4___");
            driver.WaitUntilRecurrencesSkeletonDisappeared();
            _output.WriteLine("___!!5___");
            driver.HideToasts();
            _output.WriteLine("___!!6___");
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

        public static void HideToasts(this RemoteWebDriver driver, ITestOutputHelper _output = null)
        {
            _output?.WriteLine("___%%1___");
            for (int i = 0; i < 5; i++)
            {
                _output?.WriteLine("___%%2___");
                if (!driver.ToastExists())
                {
                    _output?.WriteLine("___%%3___");
                    return;
                }
                    
                _output?.WriteLine("___%%4___");
                try
                {
                    driver.GetToast().Click();
                }
                catch (Exception e)
                {
                    _output?.WriteLine("___%%511___" + e.GetType().ToString());
                    throw;
                }

                _output?.WriteLine("___%%5___");
                Thread.Sleep(1000);
                _output?.WriteLine("___%%6___");
            }

            Assert.True(false, "Too many toasts");
        } 
    }
}
