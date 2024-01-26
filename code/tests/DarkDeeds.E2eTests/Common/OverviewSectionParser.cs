using System.Text;
using OpenQA.Selenium;

namespace DarkDeeds.E2eTests.Common;

public class OverviewSectionParser(IWebElement section)
{
    private StringBuilder _query = new(".");

    public OverviewSectionParser FindBlock(int order)
    {
        _query.Append($"//div{Xpath.ClassContains("days-block")}[{order}]");
        return this;
    }

    public OverviewSectionParser FindDay(int order)
    {
        _query.Append($"//div{Xpath.ClassContains("days-block-item")}[{order}]");
        return this;
    }

    public OverviewSectionParser FindHeader()
    {
        _query.Append($"//span{Xpath.ClassContains("day-card-header")}");
        return this;
    }

    public OverviewSectionParser FindTask(string text)
    {
        _query.Append($"//{Xpath.TaskWithText(text)}");
        return this;
    }

    public int CountExpiredDays()
    {
        string expiredDayXpath = $"//div{Xpath.ClassContains("day-card-expired")}";
        return section.FindElements(By.XPath(expiredDayXpath)).Count;
    }

    public IWebElement GetElement()
    {
        var element = section.FindElement(By.XPath(_query.ToString()));
        _query = new StringBuilder(".");
        return element;
    }
}
