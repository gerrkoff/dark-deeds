namespace DarkDeeds.E2eTests.Components;

public class RecurrencesPage() : X(string.Empty)
{
    public Button AddRecurrenceButton()
    {
        return new Button($"{this}//*[@data-test-id='btn-add-recurrence']");
    }

    public Button SaveRecurrenceButton()
    {
        return new Button($"{this}//*[@data-test-id='btn-save-recurrences']");
    }
}
