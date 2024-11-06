using System.Globalization;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using DarkDeeds.E2eTests.Base;
using DarkDeeds.E2eTests.Common;
using DarkDeeds.E2eTests.Components;
using DarkDeeds.E2eTests.Models;
using Xunit;

namespace DarkDeeds.E2eTests;

public class TelegramTests : BaseTest
{
    [Fact]
    public Task CreateTaskCheckUpdateShowTodoTest()
    {
        return Test(async driver =>
        {
            var testUser = await CreateUserAndLogin(driver);

            var testChatId = await GetTestChatId(testUser.UserId);

            var task = RandomizeText("task");
            await SendCommand(task, testChatId);

            driver.GetElement(X.GetNoDateList().GetTaskByText(task));
        });
    }

    private static async Task<int> GetTestChatId(string userId)
    {
        using var client = CreateHttpClient();
        var url = new Uri($"api/test/GetTestChatIdForUser?userId={userId}", UriKind.Relative);
        var result = await client.PostAsync(url, null!);
        result.EnsureSuccessStatusCode();
        var content = await result.Content.ReadAsStringAsync();
        return int.Parse(content, CultureInfo.InvariantCulture);
    }

    private static async Task SendCommand(string text, int chatId)
    {
        using var client = CreateHttpClient();
        var payload = new
        {
            update_id = 0,
            message = new
            {
                message_id = 0,
                from = new
                {
                    id = 0,
                    first_name = string.Empty,
                    last_name = string.Empty,
                    username = string.Empty,
                },
                date = 0,
                chat = new
                {
                    id = chatId,
                    type = string.Empty,
                    title = string.Empty,
                    first_name = string.Empty,
                    last_name = string.Empty,
                    username = string.Empty,
                },
                text,
            },
        };
        var serialized = JsonSerializer.Serialize(payload, JsonOptions.I);
        using var content = new StringContent(serialized, Encoding.UTF8, MediaTypeNames.Application.Json);
        var result = await client.PostAsync(new Uri("api/tlgm/bot/bot", UriKind.Relative), content);

        result.EnsureSuccessStatusCode();
    }
}
