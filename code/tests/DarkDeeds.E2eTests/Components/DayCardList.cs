using DarkDeeds.E2eTests.Common;

namespace DarkDeeds.E2eTests.Components;

public class DayCardList(string path) : X(path)
{
    public DayCardList TaskByText(string text)
    {
        Query.Append($"//span{XpathHelper.TextContains(text)}");
        return this;
    }
}
