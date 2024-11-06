namespace DarkDeeds.E2eTests.Components;

public class OverviewPage() : X(string.Empty)
{
    public DayCardList NoDateSection()
    {
        return new DayCardList("//*[@data-test-id='card-no-date']//ul");
    }

    public DayCardsSection CurrentSection()
    {
        return new DayCardsSection("//*[@data-test-id='section-current']");
    }

    public OverviewPage AddTaskButton()
    {
        Query.Append("//*[@data-test-id='btn-add-task']");
        return this;
    }
}
