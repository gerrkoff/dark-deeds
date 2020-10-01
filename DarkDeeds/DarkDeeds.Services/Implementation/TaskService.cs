using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DarkDeeds.Common.Extensions;
using DarkDeeds.Entities.Enums;
using DarkDeeds.Entities.Models;
using DarkDeeds.Infrastructure.Interfaces.Data;
using DarkDeeds.Models;
using DarkDeeds.Models.Data;
using DarkDeeds.Services.Interface;
using Microsoft.Extensions.Logging;

namespace DarkDeeds.Services.Implementation
{
    public class TaskService : ITaskService
    {
        private readonly IRepository<TaskEntity> _tasksRepository;
        private readonly ILogger<TaskService> _logger;
        private readonly IPermissionsService _permissionsService;
        
        public TaskService(IRepository<TaskEntity> tasksRepository, ILogger<TaskService> logger, IPermissionsService permissionsService)
        {
            _tasksRepository = tasksRepository;
            _logger = logger;
            _permissionsService = permissionsService;
        }
        
        public async Task<IEnumerable<TaskDto>> LoadActualTasksAsync(string userId, DateTime from)
        {
            IQueryable<TaskEntity> tasks = _tasksRepository.GetAll()
                .Where(x => string.Equals(x.UserId, userId))
                .Where(x =>
                    !x.IsCompleted && x.Type != TaskTypeEnum.Additional ||
                    !x.Date.HasValue ||
                    x.Date >= from);
            
            return (await tasks.ProjectTo<TaskDto>().ToListSafeAsync()).ToUtcDate();
        }

        public async Task<IEnumerable<TaskDto>> LoadTasksByDateAsync(string userId, DateTime from, DateTime to)
        {
            IQueryable<TaskEntity> tasks = _tasksRepository.GetAll()
                .Where(x => string.Equals(x.UserId, userId))
                .Where(x => x.Date.HasValue)
                .Where(x => x.Date >= from && x.Date < to);

            return (await tasks.ProjectTo<TaskDto>().ToListSafeAsync()).ToUtcDate();
        }

        public async Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks, string userId)
        {
            await _permissionsService.CheckIfUserCanEditEntitiesAsync(
                tasks.Cast<IDtoWithId>().ToList(),
                _tasksRepository,
                userId,
                "Task");

            int[] taskIds = tasks.Select(x => x.Id).Where(x => x > 0).ToArray();
            
            Dictionary<int, TaskEntity> existingTasks = await _tasksRepository.GetAll()
                .Where(x => taskIds.Contains(x.Id))
                .ToDictionarySafeAsync(x => x.Id, x => x);

            var savedTasks = new List<TaskDto>();
            foreach (var task in tasks)
            {
                TaskDto savedTask = await SaveTaskAsync(existingTasks, task, userId);
                if (savedTask != null)
                    savedTasks.Add(savedTask);
            }

            return savedTasks;
        }

        private async Task<TaskDto> SaveTaskAsync(Dictionary<int, TaskEntity> existingTasks, TaskDto taskToSave, string userId)
        {   
            TaskEntity entity;
            if (taskToSave.Deleted || taskToSave.ClientId > 0)
            {
                if (!existingTasks.ContainsKey(taskToSave.Id))
                {
                    _logger.LogWarning($"Tried to update non existing task. TaskId: {taskToSave.Id}");
                    return null;
                }

                if (existingTasks[taskToSave.Id].Version != taskToSave.Version)
                    return ConvertTaskToDto(existingTasks[taskToSave.Id], taskToSave.ClientId);
            }

            // delete
            if (taskToSave.Deleted)
            {   
                entity = existingTasks[taskToSave.Id];
                await _tasksRepository.DeleteAsync(entity);
                return taskToSave;
            }
            // create
            if (taskToSave.ClientId <= 0)
            {
                entity = Mapper.Map<TaskEntity>(taskToSave);
                entity.Id = 0;
                entity.UserId = userId;
                await _tasksRepository.SaveAsync(entity);
            }
            // update
            else
            {
                entity = existingTasks[taskToSave.Id];
                entity = Mapper.Map(taskToSave, entity);
                entity.Version++;
                await _tasksRepository.SaveAsync(entity);
            }

            return ConvertTaskToDto(entity, taskToSave.ClientId);
        }

        private TaskDto ConvertTaskToDto(TaskEntity entity, int clientId)
        {
            var dto = Mapper.Map<TaskDto>(entity);
            dto.ClientId = clientId;
            return dto;
        }
    }
}