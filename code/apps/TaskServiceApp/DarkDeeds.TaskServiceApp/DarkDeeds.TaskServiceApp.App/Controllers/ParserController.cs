using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DarkDeeds.Communication.Amqp;
using DarkDeeds.TaskServiceApp.Communication;
using DarkDeeds.TaskServiceApp.Communication.Publishers;
using DarkDeeds.TaskServiceApp.Models.Dto;
using DarkDeeds.TaskServiceApp.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.TaskServiceApp.App.Controllers
{
    // TODO: remove all controllers
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ParserController : ControllerBase
    {
        private readonly ITaskParserService _taskParserService;
        private readonly ITaskUpdatedPublisher _publisher;

        public ParserController(ITaskParserService taskParserService, ITaskUpdatedPublisher publisher)
        {
            _taskParserService = taskParserService;
            _publisher = publisher;
        }


        [HttpGet]
        public TaskDto Get([Required] string text)
        {
            return _taskParserService.ParseTask(text);
        }
        
        [HttpGet(nameof(Print))]
        public IEnumerable<string> Print([FromBody] ICollection<TaskDto> tasks)
        {
            return _taskParserService.PrintTasks(tasks);
        }
        
        [HttpGet(nameof(Test))]
        public string Test(string value)
        {
            _publisher.Send(new[] {new TaskDto {Title = value}});
            return value;
        }
    }
}