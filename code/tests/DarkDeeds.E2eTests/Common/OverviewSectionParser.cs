using System.Globalization;
using System.Text;
using OpenQA.Selenium;

namespace DarkDeeds.E2eTests.Common;

public class OverviewSectionParser(IWebElement section)
{
    private StringBuilder _query = new(".");

    public OverviewSectionParser FindBlock(int order)
    {
        _query.Append(CultureInfo.InvariantCulture, $"//div{XpathHelper.ClassContains("days-block")}[{order}]");
        return this;
    }

    public OverviewSectionParser FindDay(int order)
    {
        _query.Append(CultureInfo.InvariantCulture, $"//div{XpathHelper.ClassContains("days-block-item")}[{order}]");
        return this;
    }

    public OverviewSectionParser FindHeaderDate()
    {
        _query.Append(CultureInfo.InvariantCulture, $"//span{XpathHelper.ClassContains("day-card-header-date")}");
        return this;
    }

    public OverviewSectionParser FindTask(string text)
    {
        _query.Append(CultureInfo.InvariantCulture, $"//{XpathHelper.TaskWithText(text)}");
        return this;
    }

    public int CountExpiredDays()
    {
        var expiredDayXpath = $"//div{XpathHelper.ClassContains("day-card-expired")}";
        return section.FindElements(By.XPath(expiredDayXpath)).Count;
    }

    public IWebElement GetElement()
    {
        var element = section.FindElement(By.XPath(_query.ToString()));
        _query = new StringBuilder(".");
        return element;
    }
}
