using DarkDeeds.E2eTests.Common;

namespace DarkDeeds.E2eTests.Components;

public class EditTaskModal() : X($"//*{XpathHelper.ClassContains("modal-dialog")}")
{
    public EditTaskModal Input()
    {
        Query.Append("//input[@id='taskInput']");
        return this;
    }

    public EditTaskModal SubmitButton()
    {
        Query.Append("//button[@type='submit']");
        return this;
    }
}
