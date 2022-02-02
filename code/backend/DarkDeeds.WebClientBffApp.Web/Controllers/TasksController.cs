﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;
using DarkDeeds.WebClientBffApp.UseCases.Handlers.Tasks.LoadActual;
using DarkDeeds.WebClientBffApp.UseCases.Handlers.Tasks.Save;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.WebClientBffApp.Web.Controllers
{
    public class TasksController : BaseController
    {
        private readonly IMediator _mediator;

        public TasksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public Task<IEnumerable<TaskDto>> Get([Required] DateTime from)
        {
            return _mediator.Send(new LoadActualRequestModel(from));
        }
        
        // TODO: use separate dto?
        [HttpPost]
        public Task<IEnumerable<TaskDto>> Post([FromBody] ICollection<TaskDto> tasks)
        {
            return _mediator.Send(new SaveRequestModel(tasks));
        }
    }
}