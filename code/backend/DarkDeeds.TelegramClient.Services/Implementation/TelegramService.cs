using System;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.Backend.Entities.Entity;
using DarkDeeds.TelegramClient.Services.Exceptions;
using DarkDeeds.TelegramClient.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.TelegramClient.Services.Implementation
{
    class TelegramService : ITelegramService
    {
        private readonly UserManager<UserEntity> _userManager;

        public TelegramService(UserManager<UserEntity> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> GenerateKey(string userId, int timeAdjustment)
        {
            UserEntity user = await _userManager.FindByIdAsync(userId);
            user.TelegramChatKey = Guid.NewGuid().ToString();
            user.TelegramTimeAdjustment = timeAdjustment;
            IdentityResult result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new ServiceException("Error while generating telegram chat key");

            return user.TelegramChatKey;
        }

        public async Task UpdateChatId(string userChatKey, int chatId)
        {
            var users = await _userManager.Users
                .Where(x => x.TelegramChatKey == userChatKey || x.TelegramChatId == chatId)
                .ToListAsync();

            foreach (UserEntity user in users)
            {
                user.TelegramChatId = user.TelegramChatKey == userChatKey ? chatId : 0;
                
                IdentityResult result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                    throw new ServiceException("Error while updating telegram chat id");
            }
        }

        public async Task<string> GetUserId(int chatId)
        {
            UserEntity user = await _userManager.Users.SingleAsync(x => x.TelegramChatId == chatId);
            return user.Id;
        }
        
        public async Task<int> GetUserTimeAdjustment(int chatId)
        {
            UserEntity user = await _userManager.Users.SingleAsync(x => x.TelegramChatId == chatId);
            return user.TelegramTimeAdjustment;
        }
    }
}