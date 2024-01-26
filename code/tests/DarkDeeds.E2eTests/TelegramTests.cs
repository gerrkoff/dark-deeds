using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DarkDeeds.E2eTests.Base;
using DarkDeeds.E2eTests.Common;
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

            driver.GetTaskByTextInNoDateSection(task);
        });
    }

    private async Task<int> GetTestChatId(string userId)
    {
        using var client = CreateHttpClient();
        var url = $"api/test/GetTestChatIdForUser?userId={userId}";
        var result = await client.PostAsync(url, null!);
        result.EnsureSuccessStatusCode();
        var content = await result.Content.ReadAsStringAsync();
        return int.Parse(content);
    }

    private async Task SendCommand(string text, int chatId)
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
                    first_name = "",
                    last_name = "",
                    username = ""
                },
                date = 0,
                chat = new
                {
                    id = chatId,
                    type = "",
                    title = "",
                    first_name = "",
                    last_name = "",
                    username = "",
                },
                text,
            }
        };
        var serialized = JsonSerializer.Serialize(payload, JsonOptions.I);
        var content = new StringContent(serialized, Encoding.UTF8, MediaTypeNames.Application.Json);
        var result = await client.PostAsync("/api/tlgm/bot/bot", content);

        result.EnsureSuccessStatusCode();
    }
}
