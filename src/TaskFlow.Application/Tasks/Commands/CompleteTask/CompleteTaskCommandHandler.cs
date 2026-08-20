using MediatR;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Tasks.Commands.CompleteTask;

public class CompleteTaskCommandHandler : IRequestHandler<CompleteTaskCommand, bool>
{
    private readonly ITaskRepository _repository;
    public CompleteTaskCommandHandler(ITaskRepository repository)
    {
        _repository = repository;
    }
    public async Task<bool> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (task is null)
            return false;

        task.Complete(); // essa linha é o Domain fazendo o trabalho

        await _repository.UpdateAsync(task, cancellationToken);
        return true;
    }
}
