using DarkDeeds.E2eTests.Common;

namespace DarkDeeds.E2eTests.Components;

public class EditRecurrenceModal() : X($"//*{XpathHelper.ClassContains("modal-dialog")}")
{
    public EditRecurrenceModal TaskInput()
    {
        Query.Append("//input[@id='taskInput']");
        return this;
    }

    public EditRecurrenceModal WeekdaysInputOption(int optionIndex)
    {
        Query.Append($"//select[@id='weekdaysInput']//option[{optionIndex}]");
        return this;
    }

    public Button SubmitButton()
    {
        return new Button($"{this}//button[@type='submit']");
    }
}
