using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.Models;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.Services.Implementation
{
    public class TaskService : ITaskService
    {
        private static int Count = 1000000;
        
        public async Task<IEnumerable<TaskDto>> LoadTasksAsync()
        {
            var list = new List<TaskDto>
            {
                new TaskDto {Id = 1, Title = "Test 1", Order = 0, DateTime = DateTime.Today.AddHours(10), ClientId = 1, WithTime = true},
                new TaskDto {Id = 2, Title = "Test 2", Order = 1, DateTime = DateTime.Today, ClientId = 2},
                new TaskDto {Id = 3, Title = "Test 3", Order = 0, DateTime = DateTime.Today.AddHours(5), ClientId = 3, WithTime = true},
                new TaskDto {Id = 4, Title = "Test 4", Order = 2, DateTime = DateTime.Today, ClientId = 4},
                new TaskDto {Id = 5, Title = "Test 5", Order = 3, DateTime = DateTime.Today, ClientId = 5},
            };
            return await Task.FromResult(list);
            //return await Task.FromResult(GenTasks());
        }

        public async Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks)
        {
            foreach (var taskDto in tasks)
            {
                if (taskDto.ClientId < 0) taskDto.Id = Count++;
            }
            return await Task.FromResult(tasks);
        }

        
        // TODO: remove
        #region GENTASKS
        
        private static readonly Random Rand = new Random();
        private static readonly DateTime Monday = CurrentMonday();
        private static readonly string[] TodoSamples = {
            "Some todo",
            "Some todo some todo some todo some todo some todo",
            "Some very long todo some very long todo some very long todo some very long todo some very long todo some very long todo some very long todo"
        };
        
        private static ICollection<TaskDto> GenTasks()
        {
            var list = new List<TaskDto>();
            
            // CURRENT
            for (int i = 1; i <= 14; i++)
            {
                for (int j = 0; j < Rand.Next(1, 9); j++)
                {
                    list.Add(GenTask(i * 10 + j, Rand.Next(13)));
                }
            }

            // NO DATE
            for (int i = 1; i <= Rand.Next(1, 9); i++)
            {
                list.Add(GenTask(i, null));
            }

            // FUTURE
            for (int i = 1; i <= 5; i++)
            {
                for (int j = 0; j < Rand.Next(1, 9); j++)
                {
                    list.Add(GenTask(i * 1000 + j, Rand.Next(30) + 14));
                }
            }

            // EXPIRED
            for (int i = 1; i <= 3; i++)
            {
                for (int j = 0; j < Rand.Next(1, 9); j++)
                {
                    list.Add(GenTask(i * 10000 + j, 0 - Rand.Next(1, 9)));
                }
            }

            list.Sort((x, y) => x.Id.CompareTo(y.Id));
            int order = 1;
            list.ForEach(x => x.Order = order++);

            return list;
        }

        private static DateTime CurrentMonday()
        {
            DateTime today = DateTime.Today;
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            return today.AddDays(-1 * diff).Date; 
        }
        
        private static TaskDto GenTask(int id, int? dayDiff)
        {
            var task = new TaskDto
            {
                Id = id,
                Title = $"{id} {TodoSamples[Rand.Next(2)]}",
                ClientId = id,
                Completed = Rand.Next(3) == 2
            };

            if (dayDiff.HasValue)
                task.DateTime = Monday.AddDays(dayDiff.Value).AddHours(Rand.Next(23)).AddMinutes(Rand.Next(59));

            return task;
        }
        
        #endregion
    }
}