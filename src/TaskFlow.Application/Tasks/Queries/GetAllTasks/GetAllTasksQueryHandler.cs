using MediatR;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Tasks.Queries.GetAllTasks;

public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, IReadOnlyList<TaskItem>>
{
    private readonly ITaskRepository _repository;
    public GetAllTasksQueryHandler(ITaskRepository repository)
    {
        _repository = repository;
    }
    public async Task<IReadOnlyList<TaskItem>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}
