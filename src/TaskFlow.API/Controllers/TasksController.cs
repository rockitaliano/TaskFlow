using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Tasks.Commands.CompleteTask;
using TaskFlow.Application.Tasks.Commands.CreateTask;
using TaskFlow.Application.Tasks.Commands.DeleteTask;
using TaskFlow.Application.Tasks.Queries.GetAllTasks;
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
        public async Task<IActionResult> Create([FromBody] CreateTaskCommand command)
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

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllTasksQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpPost("{id:guid}/complete")]
        public async Task<IActionResult> Complete(Guid id, CancellationToken cancellationToken)
        {
            var tasks = await _mediator.Send(new CompleteTaskCommand(id), cancellationToken);
            if (!tasks)
                return NotFound();

            return NoContent(); //204- sucesso sem corpo
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteTaskById(Guid id, CancellationToken cancellationToken)
        {
            var task = await _mediator.Send(new DeleteTaskCommand(id), cancellationToken);
            if (!task)
                return NotFound();

            return NoContent();
        }
    }
}
