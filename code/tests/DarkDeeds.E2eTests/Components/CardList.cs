using DarkDeeds.E2eTests.Common;

namespace DarkDeeds.E2eTests.Components;

public class CardList(string initPath) : X(initPath)
{
    public CardList TaskByText(string text)
    {
        Query.Append($"//span{XpathHelper.TextContains(text)}");
        return this;
    }
}
