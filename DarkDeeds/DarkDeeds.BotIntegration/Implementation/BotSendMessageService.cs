using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Dto;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.Models.Exceptions;
using Newtonsoft.Json;

namespace DarkDeeds.BotIntegration.Implementation
{
    public class BotSendMessageService : IBotSendMessageService
    {
        private readonly string _apiSendMessage = "https://api.telegram.org/bot{0}/sendMessage";

        public BotSendMessageService(string botToken)
        {
            _apiSendMessage = string.Format(_apiSendMessage, botToken);
        }

        public Task SendUnknownCommandAsync(int userChatId) => SendMessage(new SendMessageDto
        {
            ChatId = userChatId,
            Text = "Unknown command"
        });

        public Task SendTextAsync(int userChatId, string text) => SendMessage(new SendMessageDto
        {
            ChatId = userChatId,
            Text = text
        });

        public Task SendFailedAsync(int userChatId) => SendMessage(new SendMessageDto
        {
            ChatId = userChatId,
            Text = "Failed"
        });

        protected virtual async Task SendMessage(SendMessageDto message)
        {
            var client = new HttpClient();
            string messageSerialized = JsonConvert.SerializeObject(message);
            HttpContent content = new StringContent(messageSerialized, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(new Uri(_apiSendMessage), content);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new ServiceException("Bot integration failed. Response: " +
                                           await response.Content.ReadAsStringAsync());
        }
    }
    
    public class BotSendMessageDebugService : BotSendMessageService
    {
        public BotSendMessageDebugService(string botToken) : base(botToken)
        {
        }

        protected override Task SendMessage(SendMessageDto message)
        {
            Debug.WriteLine("");
            Debug.WriteLine("_________________");
            Debug.WriteLine($"Chat Id: {message.ChatId}");
            Debug.WriteLine("Text:");
            Debug.WriteLine(message.Text);
            Debug.WriteLine("_________________");
            Debug.WriteLine("");
            return Task.CompletedTask;
        }
    }
}