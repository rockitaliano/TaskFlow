using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Tasks.Commands.CreateTask;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskRepository _repository;
        private readonly IMediator _mediator;
        public TasksController(IMediator mediator, ITaskRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateTaskCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var task = await _repository.GetByIdAsync(id, cancellationToken);
            if (task is null)
                return NotFound();

            return Ok(task);
        }
    }
}
