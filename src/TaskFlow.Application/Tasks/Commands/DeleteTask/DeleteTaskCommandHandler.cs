using MediatR;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Tasks.Commands.DeleteTask
{
    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, bool>
    {
        private readonly ITaskRepository _repository;
        public DeleteTaskCommandHandler(ITaskRepository repository)
        {
            _repository = repository;
        }
        public async Task<bool> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (task is null)
                return false;

            await _repository.DeleteAsync(task.Id, cancellationToken);
            return true;
        }
    }
}
