using DarkDeeds.E2eTests.Common;

namespace DarkDeeds.E2eTests.Components;

public class DayCardsSection(string path) : X(path)
{
    public DayCardsSection Block(int order)
    {
        Query.Append($"//div{XpathHelper.ClassContains("row")}[{order}]");
        return this;
    }

    public DayCard Day(int order)
    {
        return new DayCard(
            $"{this}//div{XpathHelper.ClassContains("col-sm")}[{order}]//*{XpathHelper.ClassContains("card")}");
    }

    public DayCardsSection Expired()
    {
        Query.Append($"//*{XpathHelper.ClassContains("border-0")}");
        return this;
    }
}
