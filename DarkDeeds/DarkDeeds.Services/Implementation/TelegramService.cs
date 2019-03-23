using System;
using System.Threading.Tasks;
using DarkDeeds.Common.Exceptions;
using DarkDeeds.Data.Entity;
using DarkDeeds.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.Services.Implementation
{
    public class TelegramService : ITelegramService
    {
        private readonly UserManager<UserEntity> _userManager;

        public TelegramService(UserManager<UserEntity> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> GenerateKey(string userId)
        {
            UserEntity user = await _userManager.FindByIdAsync(userId);
            user.TelegramChatKey = Guid.NewGuid().ToString();
            IdentityResult result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new ServiceException("Error while generating telegram chat key");

            return user.TelegramChatKey;
        }

        public async Task UpdateChatId(string userChatKey, int chatId)
        {
            UserEntity user = await _userManager.Users.SingleAsync(x => x.TelegramChatKey == userChatKey);
            user.TelegramChatId = chatId;
            IdentityResult result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new ServiceException("Error while updating telegram chat id");
        }

        public async Task<string> GetUserId(int chatId)
        {
            UserEntity user = await _userManager.Users.SingleAsync(x => x.TelegramChatId == chatId);
            return user.Id;
        }
    }
}