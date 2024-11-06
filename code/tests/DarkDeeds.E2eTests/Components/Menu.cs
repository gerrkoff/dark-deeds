using DarkDeeds.E2eTests.Common;

namespace DarkDeeds.E2eTests.Components;

public class Menu() : X($"//div{XpathHelper.ClassContains("list-group")}")
{
    public Menu AddButton()
    {
        Query.Append("//button[@data-test-id='btn-add-item']");
        return this;
    }

    public Menu DeleteButton()
    {
        Query.Append("//button[@data-test-id='btn-delete-item']");
        return this;
    }
}
