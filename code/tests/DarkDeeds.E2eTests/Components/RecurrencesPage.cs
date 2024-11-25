namespace DarkDeeds.E2eTests.Components;

public class RecurrencesPage() : X(string.Empty)
{
    public Button AddRecurrenceButton()
    {
        return new Button($"{this}//*[@data-test-id='btn-add-recurrence']");
    }

    public Button SaveRecurrencesButton()
    {
        return new Button($"{this}//*[@data-test-id='btn-save-recurrences']");
    }

    public Button CreateRecurrencesButton()
    {
        return new Button($"{this}//*[@data-test-id='btn-create-recurrences']");
    }
}
