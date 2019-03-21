using System;
using System.Net;
using System.Net.Http;
using DarkDeeds.BotIntegration.Dto;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.Common.Exceptions;
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

        public void SendUnknownCommand(int userChatId) => SendMessage(new SendMessageDto
        {
            ChatId = userChatId,
            Text = "Unknown command"
        });

        public void SendText(int userChatId, string text) => SendMessage(new SendMessageDto
        {
            ChatId = userChatId,
            Text = text
        });

        private async void SendMessage(SendMessageDto message)
        {
            var client = new HttpClient();
            string messageSerialized = JsonConvert.SerializeObject(message);
            HttpContent content = new StringContent(messageSerialized);
            HttpResponseMessage response = await client.PostAsync(new Uri(_apiSendMessage), content);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new ServiceException("Bot integration failed. Response: " +
                                           await response.Content.ReadAsStringAsync());
        }
    }
}