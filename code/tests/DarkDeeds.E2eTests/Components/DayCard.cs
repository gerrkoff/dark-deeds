namespace DarkDeeds.E2eTests.Components;

public class DayCard(string path) : X(path)
{
    public DayCardList List()
    {
        return new DayCardList($"{this}//ul");
    }

    public DayCard DateHeader()
    {
        Query.Append("//*[@data-test-id='header-date']");
        return this;
    }
}
