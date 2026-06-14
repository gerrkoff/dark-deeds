using DarkDeeds.E2eTests.Models;

namespace DarkDeeds.E2eTests.Backend;

public static class BackendVerifier
{
    public static async Task WaitUntilHasTaskAsync(TestUserDto user, string title)
    {
        var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(30);
        while (DateTime.UtcNow < deadline)
        {
            var titles = await BackendApi.GetTaskTitlesAsync(user);
            if (titles.Contains(title))
                return;

            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        throw new InvalidOperationException($"Backend did not receive task '{title}' within the timeout");
    }
}
