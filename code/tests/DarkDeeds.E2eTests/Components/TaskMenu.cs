using DarkDeeds.E2eTests.Common;

namespace DarkDeeds.E2eTests.Components;

public class TaskMenu() : X($"//div{XpathHelper.ClassContains("list-group")}")
{
    public TaskMenu DeleteButton()
    {
        Query.Append("//button[@data-test-id='btn-delete-item']");
        return this;
    }
}
