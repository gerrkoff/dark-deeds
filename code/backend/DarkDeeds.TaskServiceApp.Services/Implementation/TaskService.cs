using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Infrastructure.Data.EntityRepository;
using DarkDeeds.TaskServiceApp.Infrastructure.Services;
using DarkDeeds.TaskServiceApp.Infrastructure.Services.Dto;
using DarkDeeds.TaskServiceApp.Models.Dto;
using DarkDeeds.TaskServiceApp.Models.Extensions;
using DarkDeeds.TaskServiceApp.Services.Interface;
using DarkDeeds.TaskServiceApp.Services.Specifications;
using Microsoft.Extensions.Logging;

namespace DarkDeeds.TaskServiceApp.Services.Implementation
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _tasksRepository;
        private readonly ILogger<TaskService> _logger;
        private readonly IMapper _mapper;
        private readonly INotifierService _notifierService;
        private readonly ISpecificationFactory _specFactory;
        
        public TaskService(ITaskRepository tasksRepository, ILogger<TaskService> logger, IMapper mapper, INotifierService notifierService, ISpecificationFactory specFactory)
        {
            _tasksRepository = tasksRepository;
            _logger = logger;
            _mapper = mapper;
            _notifierService = notifierService;
            _specFactory = specFactory;
        }
        
        public async Task<IEnumerable<TaskDto>> LoadActualTasksAsync(string userId, DateTime from)
        {
            var spec = _specFactory.New<ITaskSpecification, TaskEntity>()
                .FilterUserOwned(userId)
                .FilterActual(from);

            var tasks = await _tasksRepository.GetBySpecAsync(spec);

            return _mapper.Map<IList<TaskDto>>(tasks).ToUtcDate();
        }

        public async Task<IEnumerable<TaskDto>> LoadTasksByDateAsync(string userId, DateTime from, DateTime to)
        {
            var spec = _specFactory.New<ITaskSpecification, TaskEntity>()
                .FilterUserOwned(userId)
                .FilterDateInterval(from, to);
            
            var tasks = await _tasksRepository.GetBySpecAsync(spec);

            return _mapper.Map<IList<TaskDto>>(tasks).ToUtcDate();
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

            if (savedTasks.Count > 0)
                await _notifierService.TaskUpdated(new TaskUpdatedDto
                {
                    Tasks = savedTasks,
                    UserId = userId,
                });

            return savedTasks;
        }

        private async Task<TaskDto> SaveTaskByUidAsync(TaskDto taskToSave, string userId)
        {   
            var entity = await _tasksRepository.GetByIdAsync(taskToSave.Uid);

            if (entity != null && !string.Equals(entity.UserId, userId))
            {
                _logger.LogWarning($"Tried to update foreign task. TaskUid: {taskToSave.Uid} User: {userId}");
                return null;
            }

            if (taskToSave.Deleted)
            {
                var success = await _tasksRepository.DeleteAsync(taskToSave.Uid);
                if (!success)
                    _logger.LogWarning($"Tried to delete non existing task. TaskUid: {taskToSave.Uid}");

                taskToSave.Version++;
                return taskToSave;
            }

            if (entity == null)
            {
                entity = _mapper.Map<TaskEntity>(taskToSave);
                entity.UserId = userId;
                entity.Version = 1;
                await _tasksRepository.UpsertAsync(entity);
            }
            else
            {
                entity = _mapper.Map(taskToSave, entity);
                var (success, _) = await _tasksRepository.TryUpdateVersionAsync(entity);
                if (!success)
                    return null;
            }
            
            return _mapper.Map<TaskDto>(entity);
        }
    }
}