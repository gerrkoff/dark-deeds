using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Dto;
using DarkDeeds.Models;

namespace DarkDeeds.BotIntegration.Interface
{
    public interface IBotProcessMessageService
    {
        Task ProcessMessageAsync(UpdateDto update);
    }
}