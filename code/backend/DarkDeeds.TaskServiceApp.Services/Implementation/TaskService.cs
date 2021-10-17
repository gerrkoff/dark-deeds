using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DarkDeeds.TaskServiceApp.EfCoreExtensions;
using DarkDeeds.TaskServiceApp.Entities.Enums;
using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using DarkDeeds.TaskServiceApp.Infrastructure.Services;
using DarkDeeds.TaskServiceApp.Infrastructure.Services.Dto;
using DarkDeeds.TaskServiceApp.Models.Dto;
using DarkDeeds.TaskServiceApp.Models.Extensions;
using DarkDeeds.TaskServiceApp.Services.Interface;
using Microsoft.Extensions.Logging;

namespace DarkDeeds.TaskServiceApp.Services.Implementation
{
    public class TaskService : ITaskService
    {
        private readonly IRepository<TaskEntity> _tasksRepository;
        private readonly ILogger<TaskService> _logger;
        private readonly IMapper _mapper;
        private readonly INotifierService _notifierService;
        
        public TaskService(IRepository<TaskEntity> tasksRepository, ILogger<TaskService> logger, IMapper mapper, INotifierService notifierService)
        {
            _tasksRepository = tasksRepository;
            _logger = logger;
            _mapper = mapper;
            _notifierService = notifierService;
        }
        
        public async Task<IEnumerable<TaskDto>> LoadActualTasksAsync(string userId, DateTime from)
        {
            IQueryable<TaskEntity> tasks = _tasksRepository.GetAll()
                .Where(x => string.Equals(x.UserId, userId))
                .Where(x =>
                    !x.IsCompleted && x.Type != TaskTypeEnum.Additional ||
                    !x.Date.HasValue ||
                    x.Date >= from);
            
            return (await _mapper.ProjectTo<TaskDto>(tasks).ToListSafeAsync()).ToUtcDate();
        }

        public async Task<IEnumerable<TaskDto>> LoadTasksByDateAsync(string userId, DateTime from, DateTime to)
        {
            IQueryable<TaskEntity> tasks = _tasksRepository.GetAll()
                .Where(x => string.Equals(x.UserId, userId))
                .Where(x => x.Date.HasValue)
                .Where(x => x.Date >= from && x.Date < to);

            return (await _mapper.ProjectTo<TaskDto>(tasks).ToListSafeAsync()).ToUtcDate();
        }

        public async Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks, string userId)
        {
            var savedTasks = new List<TaskDto>();

            foreach (var taskDto in tasks)
            {
                var savedTask = await SaveTaskByUidAsync(taskDto, userId);
                if (savedTask != null)
                    savedTasks.Add(savedTask);
            }

            await _notifierService.TaskUpdated(new TaskUpdatedDto
            {
                Tasks = savedTasks,
                UserId = userId,
            });

            return savedTasks;
        }

        private async Task<TaskDto> SaveTaskByUidAsync(TaskDto taskToSave, string userId)
        {
            var entity = await _tasksRepository.GetAll()
                .FirstOrDefaultSafeAsync(x => string.Equals(x.Uid, taskToSave.Uid));

            if (entity != null && !string.Equals(entity.UserId, userId))
            {
                _logger.LogWarning($"Tried to update foreign task. TaskUid: {taskToSave.Uid} User: {userId}");
                return null;
            }

            if (taskToSave.Deleted)
            {
                if (entity == null)
                    _logger.LogWarning($"Tried to delete non existing task. TaskUid: {taskToSave.Uid}");
                else
                    await _tasksRepository.DeleteAsync(entity);
                taskToSave.Version++;
                return taskToSave;
            }

            if (entity == null)
            {
                entity = _mapper.Map<TaskEntity>(taskToSave);
                entity.Id = 0;
                entity.UserId = userId;
                entity.Version = 1;
                await _tasksRepository.SaveAsync(entity);
            }
            else if (entity.Version == taskToSave.Version)
            {
                var id = entity.Id;
                entity = _mapper.Map(taskToSave, entity);
                entity.Version++;
                entity.Id = id;
                await _tasksRepository.SaveAsync(entity);
            }
            
            return _mapper.Map<TaskDto>(entity);
        }
    }
}