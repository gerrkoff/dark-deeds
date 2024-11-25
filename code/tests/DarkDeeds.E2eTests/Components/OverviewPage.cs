namespace DarkDeeds.E2eTests.Components;

public class OverviewPage() : X(string.Empty)
{
    public DayCardList NoDateSection()
    {
        return new DayCardList($"{this}//*[@data-test-id='card-no-date']//ul");
    }

    public DayCardsSection CurrentSection()
    {
        return new DayCardsSection($"{this}//*[@data-test-id='section-current']");
    }

    public Button AddTaskButton()
    {
        return new Button($"{this}//*[@data-test-id='btn-add-task']");
    }
}
